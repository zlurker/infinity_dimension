﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using System.IO;

public class WindowsData {
    public SavedData lD;
    public EditableWindow eW;
    public int wN;

    public WindowsData(SavedData linkedData) {
        lD = linkedData;
    }

    public void RemoveWindows() {
        eW.window.gameObject.SetActive(false);
    }
}

public class EditableWindow {

    public ScriptableObject window;
    public LinearLayout lL;
    public ScriptableObject windowsConnector;
    public ScriptableObject windowsDeleter;
    public LineManager lineManager;

    public EditableWindow() {
        window = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<WindowsScript>() });
        lL = Spawner.GetCType<LinearLayout>(Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<LinearLayout>() }));
        windowsConnector = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(Button) });
        windowsDeleter = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(Button) });
        lineManager = new LineManager();

        Spawner.GetCType<Image>(windowsConnector).color = Color.black;
        Spawner.GetCType<Image>(windowsDeleter).color = Color.red;

        Spawner.GetCType<Text>(windowsConnector).text = "";
        Spawner.GetCType<Text>(windowsDeleter).text = "";

        windowsConnector.transform.SetParent(window.transform);
        windowsDeleter.transform.SetParent(window.transform);
        lL.transform.SetParent(window.transform);

        windowsConnector.transform.position = UIDrawer.UINormalisedPosition(window.transform as RectTransform, new Vector2(0.15f, 0.5f));
        windowsDeleter.transform.position = UIDrawer.UINormalisedPosition(window.transform as RectTransform, new Vector2(0.85f, 0.5f));
        lL.transform.position = UIDrawer.UINormalisedPosition(window.transform as RectTransform, new Vector2(0f, -1));

        UIDrawer.ChangeUISize(windowsConnector, new Vector2(20, 20));
        UIDrawer.ChangeUISize(windowsDeleter, new Vector2(20, 20));
    }
}

public class SavedDataCommit {

    public List<SavedDataCommit> members;
    public string n;
    public int vI;
    public int[] cI;
    public string sO;

    public SavedDataCommit(string name, string soValue, int variableIndex) {
        members = new List<SavedDataCommit>();
        n = name;
        sO = soValue;
        vI = variableIndex;
    }

    public SavedDataCommit(string name, int[] connectedInt) {
        members = new List<SavedDataCommit>();
        cI = connectedInt;
        n = name;
    }

    public SavedDataCommit() {
        members = new List<SavedDataCommit>();
    }

    public static SavedDataCommit[] ConvertToCommit(SavedData[] target) {
        List<SavedDataCommit> commit = new List<SavedDataCommit>();

        for(int i = 0; i < target.Length; i++) {
            SavedDataCommit commitHeader = new SavedDataCommit(target[i].classType.Name, target[i].connectedInt.ToArray());
            commit.Add(commitHeader);
            //RuntimeParameters<SavedData> inst = target.fields[i] as RuntimeParameters<SavedData>;

            for(int j = 0; j < target[i].fields.Count; j++)
                commitHeader.members.Add(new SavedDataCommit(target[i].fields[j].n, target[i].fields[j].GetSerializedObject(), target[i].fields[j].vI));
        }

        return commit.ToArray();
    }

    public SavedData CreateSavedData() {
        IPlayerEditable[] interfaces = (Iterator.ReturnObject<IPlayerEditable>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as IPlayerEditable[];
        IPlayerEditable selectedInterface = Iterator.ReturnObject<IPlayerEditable>(interfaces, n, (p) => {
            return p.GetType().Name;
        });

        SavedData instance = null;

        if(selectedInterface != null) {
            instance = new SavedData(selectedInterface.GetType());
            instance.connectedInt = new List<int>(cI);

            for(int i = 0; i < instance.fields.Count; i++) {
                int j = Iterator.ReturnKey(members.ToArray(), instance.fields[i].n, (t) => { return t.n; });

                if(j > -1)
                    instance.fields[i] = VariableTypeIndex.ReturnRuntimeType(members[j].vI, members[j].sO);
            }
        }

        return instance;
    }
}

public class SavedData {
    public List<RuntimeParameters> fields;
    public List<int> connectedInt;
    public Type classType;

    public SavedData(Type t) {
        IPlayerEditable[] interfaces = (Iterator.ReturnObject<IPlayerEditable>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as IPlayerEditable[];
        IPlayerEditable selectedInterface = Iterator.ReturnObject(interfaces, t, (p) => {
            return p.GetType();
        });

        classType = t;
        fields = new List<RuntimeParameters>(selectedInterface.GetRuntimeParameters());
        connectedInt = new List<int>();
    }

    public static SavedData[] LoadData(string filePath) {
        List<SavedData> instance = new List<SavedData>();
        SavedDataCommit[] textData;

        using(StreamReader reader = new StreamReader(filePath)) {
            string text = reader.ReadToEnd();
            textData = JsonConvert.DeserializeObject<SavedDataCommit[]>(text);
            reader.Close();
        }

        if(textData != null) {
            for(int i = 0; i < textData.Length; i++) {
                SavedData inst = textData[i].CreateSavedData();
                if(inst != null)
                    instance.Add(inst);
            }
        }

        return instance.ToArray();
    }

    public static SavedData[] CreateLoadFile(string filepath) {
        SavedData[] prevData = LoadData(filepath);
        StartupLinkerHelper.RelinkLoadedData(prevData);
        return prevData;
    }
}

public class UICalibrator<T> : UICalibrator where T : Component {
    public Action<T, int[]> calibrationDeleg;

    public UICalibrator(Action<T, int[]> deleg) {
        calibrationDeleg = deleg;
        t = typeof(T);
    }

    public override void RunCalibrator(MonoBehaviour script, int[] parameters) {
        calibrationDeleg(script as T, parameters);
    }
}

public class UICalibrator : Iterator {
    public virtual void RunCalibrator(MonoBehaviour script, int[] parameters) {

    }
}

public class MainMenuUICommands : MonoBehaviour, IPointerDownHandler {

    public class ButtonData {
        public Transform root;
        public int[] p;
        public bool active;
    }

    public class SourceData {
        public Transform src;
        public int taggedId;

        public SourceData(Transform source, int tagged) {
            src = source;
            taggedId = tagged;
        }
    }

    public static EnhancedList<WindowsData> savedData;
    public static AutoPopulationList<SourceData> srcData;

    public ScriptableObject mainClassSelection;

    public Font font;
    Text instance;

    //Load test data
    public string path;

    //General data
    public static IPlayerEditable[] interfaces;

    //Windows system essentials
    ScriptableObject windowSpawner;
    bool windowSpawnMode;
    int dataIndex;

    //UICalibration
    UICalibrator[] uiCalibration;

    //Line Drawing System
    Camera cam;
    ButtonData bD;

    string[] layers;

    DelegateIterator[] inIt; //InputFiend initialisers

    public void InitialiseFieldForUse(MonoBehaviour[] target, string[] id) //Makes fields here usable by this script. Auto saves it to the data.
    {
        for(int k = 0; k < target.Length; k++)
            if(target[k] is Selectable)
                Iterator.ReturnObject<DelegateIterator>(inIt, target[k].GetType().Name).d.Invoke(new object[] { target[k], id });
    }

    void InitialiseInIt() //Contains MethodRunners to help prepare field for use.
    {
        interfaces = (Iterator.ReturnObject<IPlayerEditable>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as IPlayerEditable[];
        bD = new ButtonData();

        uiCalibration = new UICalibrator[]
        {
            new UICalibrator<InputField>((t,p)=>{
                string outputIndex = "";
                for (int i=0; i < p.Length; i++)
                    outputIndex += p[i].ToString();

                switch (t.contentType){
                        case InputField.ContentType.Standard:
                            t.text = (savedData.l[p[0]].lD.fields[p[1]] as RuntimeParameters<string>).v;//SavedData.GetData<string>(savedData,p);
                            break;

                        case InputField.ContentType.IntegerNumber:
                            t.text = (savedData.l[p[0]].lD.fields[p[1]] as RuntimeParameters<int>).v.ToString();
                            break;

                        case InputField.ContentType.DecimalNumber:
                            t.text = (savedData.l[p[0]].lD.fields[p[1]] as RuntimeParameters<float>).v.ToString();
                            break;
                }

                t.onValueChanged.AddListener((s) => {
                    switch (t.contentType){
                        case InputField.ContentType.Standard:
                            (savedData.l[p[0]].lD.fields[p[1]] as RuntimeParameters<string>).v = s;
                            break;

                        case InputField.ContentType.IntegerNumber:
                            (savedData.l[p[0]].lD.fields[p[1]] as RuntimeParameters<int>).v = int.Parse(s);
                            break;

                        case InputField.ContentType.DecimalNumber:
                            (savedData.l[p[0]].lD.fields[p[1]] as RuntimeParameters<float>).v = float.Parse(s);
                            break;
                }
            });}), new UICalibrator<Button>((t,p)=>{
                t.onClick.AddListener(()=>{
                    bD.active = true;
                    bD.root = t.transform;
                    bD.p = p;
                });
            })
        };

        inIt = new DelegateIterator[]
        {
            new DelegateIterator("WindowSpawner", new DH((p) =>
            {
                Button button = Spawner.GetCType<Button>(p[0] as ScriptableObject);
                int id = (int) p[1];

                Spawner.GetCType<Text>(p[0] as ScriptableObject).text = id.ToString();

                button.onClick.AddListener(()=>{ WindowSpawnState(id); });
            }))
        };
    }

    void Start() {
        InitialiseInIt();

        path = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GetBaseLevelPath(new string[] { "TestFile101" });

        SavedData[] prevData = SavedData.CreateLoadFile(path);
        Debug.Log(path);
        savedData = new EnhancedList<WindowsData>();
        srcData = new AutoPopulationList<SourceData>();

        for(int i = 0; i < prevData.Length; i++) {
            WindowsData inst = new WindowsData(prevData[i]);
            inst.wN = savedData.Add(inst);
        }

        SpawnUIFromData();
        GenerateLines();

        mainClassSelection = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(LinearLayout) });
        //Singleton.GetSingleton<PatternControl>().ModifyGroup("Test", new object[] { Singleton.GetSingleton<UIDrawer>().CustomiseBaseObject(), Singleton.GetSingleton<Spawner>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<Spawner>().CreateComponent<LinearLayout>() }) });

        ScriptableObject[] buttons = new ScriptableObject[interfaces.Length];
        for(int i = 0; i < interfaces.Length; i++) {
            ScriptableObject buttonTest = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Button>() });
            Iterator.ReturnObject<DelegateIterator>(inIt, "WindowSpawner").d.Invoke(new object[] { buttonTest, i });

            Spawner.GetCType<LinearLayout>(mainClassSelection).Add(buttonTest.transform as RectTransform);
        }

        mainClassSelection.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.1f, 0.9f));
        windowSpawner = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Image>(), Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>() });
        windowSpawner.gameObject.SetActive(false);

        ScriptableObject saveButton = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Button>() });
        Spawner.GetCType<Button>(saveButton).onClick.AddListener(() => {
            SavedData[] data;

            List<SavedData> sDL = new List<SavedData>();
            for(int i = 0; i < savedData.l.Count; i++) {
                if(savedData.l[i].lD != null)
                    sDL.Add(savedData.l[i].lD);
            }

            data = sDL.ToArray();
            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { "TestFile101" }, JsonConvert.SerializeObject(SavedDataCommit.ConvertToCommit(data)));
        });

        Spawner.GetCType<Text>(saveButton).text = "Save JSON";
        saveButton.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.1f));
    }

    public void SpawnUIFromData() {
        for(int i = 0; i < savedData.l.Count; i++)
            CreateWindow(savedData.l[i]);
    }

    public void GenerateLines() {

        for(int i = 0; i < savedData.l.Count; i++) {
            for (int j=0;j< savedData.l[i].lD.connectedInt.Count; j++) {

                EditableWindow currGroup = savedData.l[i].eW;
                LineData lineData = new LineData(srcData.l[savedData.l[i].lD.connectedInt[j]].src, currGroup.windowsConnector.transform);

                savedData.l[srcData.l[savedData.l[i].lD.connectedInt[j]].taggedId].eW.lineManager.lineData.Add(lineData);
                savedData.l[srcData.l[savedData.l[i].lD.connectedInt[j]].taggedId].eW.lineManager.lineData.Add(lineData);
                currGroup.lineManager.lineData.Add(lineData);
                Debug.Log("Data added");
                currGroup.lineManager.UpdateLines();

                /*
                ScriptableObject line = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(Line) });
                UIDrawer.ChangeUISize(line, typeof(Image), new Vector2(10, 500));
                Spawner.GetCType<Image>(line).rectTransform.pivot = new Vector2(0.5f, 0);
                //line.transform.position = bD.root.position;
                Spawner.GetCType<Line>(line).target = linkedGroup.transforms[0].transform;
                Spawner.GetCType<Line>(line).lineRoot = currGroup.gE[0].transform;
                Spawner.GetCType<Line>(line).EstablishJoint();

                */
                //Singleton.GetSingleton<PatternControl>().ModifyGroup("lineStart" + savedData.l[i].lD.connectedInt.ToString(), new object[] { line });
                //Singleton.GetSingleton<PatternControl>().ModifyGroup("lineEnd" + i.ToString(), new object[] { line });
            }
        }
    }

    public void WindowSpawnState(int index) {
        Spawner.GetCType<Text>(windowSpawner).text = index.ToString();
        dataIndex = index;

        windowSpawner.gameObject.SetActive(true);
        windowSpawnMode = true;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(!cam)
            cam = eventData.pressEventCamera;

        if(windowSpawnMode) {
            windowSpawnMode = false;
            windowSpawner.gameObject.SetActive(false);

            SavedData newClass = new SavedData(interfaces[dataIndex].GetType());

            SavedData[] allWindows = GetDefaultWindows(new SavedData[] { newClass });
            for(int i = 0; i < allWindows.Length; i++) {
                WindowsData inst = new WindowsData(allWindows[i]);
                inst.wN = savedData.Add(inst);
                ScriptableObject window = CreateWindow(inst);
                Vector2 cursorPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.root as RectTransform, eventData.position, eventData.pressEventCamera, out cursorPos);
                window.transform.localPosition = cursorPos;
            }
        }
    }

    public SavedData[] GetDefaultWindows(SavedData[] root) {
        List<SavedData> additionalWindows = new List<SavedData>();

        for(int j = 0; j < root.Length; j++) {
            additionalWindows.Add(root[j]);

            for(int i = 0; i < root[j].fields.Count; i++) {
                RuntimeParameters<EditableLinkInstance> link = root[j].fields[i] as RuntimeParameters<EditableLinkInstance>;

                if(link != null)
                    if(link.v != null) {
                        SavedData[] returnedWindows = GetDefaultWindows(link.v.GetLinkedObjects());

                        for(int k = 0; k < returnedWindows.Length; k++)
                            additionalWindows.Add(returnedWindows[k]);
                    }
            }
        }
        return additionalWindows.ToArray();
    }


    public ScriptableObject CreateWindow(WindowsData runtimePara) {

        EditableWindow editWindow = new EditableWindow();
        editWindow.window.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.5f));
        runtimePara.eW = editWindow;

        Spawner.GetCType<WindowsScript>(editWindow.window).AddEvent(editWindow.lineManager);

        Spawner.GetCType<Button>(editWindow.windowsConnector).onClick.AddListener(() => { //Generates line when clicked on
            if(bD.active) {
                bD.active = false;
                LineData lineData = new LineData(savedData.l[bD.p[0]].eW.window.transform, editWindow.windowsConnector.transform);
                editWindow.lineManager.lineData.Add(lineData);
                editWindow.lineManager.UpdateLines();
                (savedData.l[bD.p[0]].lD.fields[bD.p[1]] as RuntimeParameters<EditableLinkInstance>).v.LinkObject(savedData.l[runtimePara.wN].lD);
            }
        });

        Spawner.GetCType<Button>(editWindow.windowsDeleter).onClick.AddListener(() => { //Deletes windows when clicked on
            runtimePara.RemoveWindows();
        });


        for(int i = 0; i < runtimePara.lD.fields.Count; i++) {
            ScriptableObject elementName = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>() });
            ScriptableObject element = ReturnElementField(runtimePara.lD.fields[i], runtimePara);
            Spawner.GetCType<Text>(elementName).text = runtimePara.lD.fields[i].n;
            Spawner.GetCType<Text>(elementName).color = Color.white;

            ScriptableObject align = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<LinearLayout>() });

            Spawner.GetCType<LinearLayout>(align).o = LinearLayout.Orientation.X;
            Spawner.GetCType<LinearLayout>(align).Add(elementName.transform as RectTransform);
            Spawner.GetCType<LinearLayout>(align).Add(element.transform as RectTransform);
            (align.transform as RectTransform).sizeDelta = (Spawner.GetCType<LinearLayout>(align).transform as RectTransform).sizeDelta;
            editWindow.lL.Add(align.transform as RectTransform);

            UICalibrator(element, new int[] { runtimePara.wN, i });
        }

        return editWindow.window;
    }

    public ScriptableObject ReturnElementField(RuntimeParameters variable, WindowsData window) {
        ScriptableObject element = null;

        if(variable.t == typeof(string)) {
            element = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(InputField) });
            Spawner.GetCType<InputField>(element).contentType = InputField.ContentType.Standard;
        }

        if(variable.t == typeof(int)) {
            element = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(InputField) });
            Spawner.GetCType<InputField>(element).contentType = InputField.ContentType.IntegerNumber;
        }

        if(variable.t == typeof(float)) {
            element = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(InputField) });
            Spawner.GetCType<InputField>(element).contentType = InputField.ContentType.DecimalNumber;
        }

        if(variable.t == typeof(EditableLinkInstance)) {
            element = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(Button) });
            srcData.ModifyElementAt((variable as RuntimeParameters<EditableLinkInstance>).v.linkId, new SourceData(element.transform, window.wN));
            //(variable as RuntimeParameters<EditableLinkInstance>).v.src = element.transform;
            //ScriptableObject lineHolder = Singleton.GetSingleton<Spawner>().CustomiseBaseObject();
            //lineHolder.transform.parent = element.transform;
            //Singleton.GetSingleton<PatternControl>().ModifyGroup(generatedString, new object[] { lineHolder.transform });
            //window.AddEvent(new LineUpdater(generatedString));
        }

        return element;
    }

    public void UICalibrator(ScriptableObject target, int[] id) {
        for(int i = 0; i < target.scripts.Length; i++) {
            UICalibrator uiC = Iterator.ReturnObject(uiCalibration, target.scripts[i].GetType()) as UICalibrator;
            if(uiC != null)
                uiC.RunCalibrator(target.scripts[i], id);
        }
    }

    void Update() {
        if(windowSpawnMode)
            windowSpawner.transform.position = Input.mousePosition;
    }
}