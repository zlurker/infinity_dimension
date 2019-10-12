using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using System.IO;

public class WindowsData {
    public int id;
    public EditableWindow eW;

    public WindowsData(int dataId, EditableWindow editableWindow) {
        id = dataId;
        eW = editableWindow;
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

    //public static EnhancedList<WindowsData> savedData;
    public static AutoPopulationList<SourceData> srcData;
    public static AbilityData abilityData;
    public EnhancedList<WindowsData> windows;

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

    AbilityDescription abilityDescription;

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
                        
                            t.text = (abilityData.subclasses.l[p[0]].var[p[1]].field as RuntimeParameters<string>).v;//SavedData.GetData<string>(savedData,p);
                            break;

                        case InputField.ContentType.IntegerNumber:
                            t.text = (abilityData.subclasses.l[p[0]].var[p[1]].field as RuntimeParameters<int>).v.ToString();
                            break;

                        case InputField.ContentType.DecimalNumber:
                            t.text = (abilityData.subclasses.l[p[0]].var[p[1]].field as RuntimeParameters<float>).v.ToString();
                            break;
                }

                t.onValueChanged.AddListener((s) => {
                    switch (t.contentType){
                        case InputField.ContentType.Standard:
                            (abilityData.subclasses.l[p[0]].var[p[1]].field as RuntimeParameters<string>).v = s;
                            break;

                        case InputField.ContentType.IntegerNumber:
                            (abilityData.subclasses.l[p[0]].var[p[1]].field as RuntimeParameters<int>).v = int.Parse(s);
                            break;

                        case InputField.ContentType.DecimalNumber:
                            (abilityData.subclasses.l[p[0]].var[p[1]].field as RuntimeParameters<float>).v = float.Parse(s);
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

        AutoPopulationList<string> test = new AutoPopulationList<string>();
        
        string cData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 0);

        if(cData != "")
            abilityData = new AbilityData(JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(cData)));
        else
            abilityData = new AbilityData();

        Debug.Log(abilityData);

        srcData = new AutoPopulationList<SourceData>();
        windows = new EnhancedList<WindowsData>();

        SpawnUIFromData();
        //GenerateLines();

        mainClassSelection = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(LinearLayout) });

        ScriptableObject name = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(InputField) });
        name.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.9f));

        string data = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 1);
        abilityDescription = JsonConvert.DeserializeObject<AbilityDescription>(data);
        Spawner.GetCType<InputField>(name).text = abilityDescription.n;

        Spawner.GetCType<InputField>(name).onValueChanged.AddListener((s) => {
            abilityDescription.n = s;
        });

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

            int[] aEle = windows.ReturnActiveElements();
            

            for (int i =0; i < aEle.Length; i++) {

                abilityData.subclasses.l[windows.l[aEle[i]].id].wL[0] =  windows.l[aEle[i]].eW.window.transform.localPosition.x;
                abilityData.subclasses.l[windows.l[aEle[i]].id].wL[1] = windows.l[aEle[i]].eW.window.transform.localPosition.y;
            }


            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 0, JsonConvert.SerializeObject(JSONFileConvertor.ConvertToStandard(abilityData.RelinkSubclass())));
            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 1, JsonConvert.SerializeObject(abilityDescription));
        });

        Spawner.GetCType<Text>(saveButton).text = "Save JSON";
        saveButton.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.1f));
    }

    public void SpawnUIFromData() {
        for(int i = 0; i < abilityData.subclasses.l.Count; i++) {
            Vector2 loc = new Vector2(abilityData.subclasses.l[i].wL[0], abilityData.subclasses.l[i].wL[1]);
            CreateWindow(i,loc);
        }
    }

    /*public void GenerateLines() {

        for(int i = 0; i < savedData.l.Count; i++) {
            for(int j = 0; j < savedData.l[i].lD.connectedInt.Count; j++) {

                EditableWindow currGroup = savedData.l[i].eW;
                Debug.Log(savedData.l[i].lD.connectedInt[j]);
                LineData lineData = new LineData(srcData.l[savedData.l[i].lD.connectedInt[j]].src, currGroup.windowsConnector.transform);

                savedData.l[srcData.l[savedData.l[i].lD.connectedInt[j]].taggedId].eW.lineManager.lineData.Add(lineData);
                //savedData.l[srcData.l[savedData.l[i].lD.connectedInt[j]].taggedId].eW.lineManager.lineData.Add(lineData);
                currGroup.lineManager.lineData.Add(lineData);
                currGroup.lineManager.UpdateLines();

                /*
                ScriptableObject line = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(Line) });
                UIDrawer.ChangeUISize(line, typeof(Image), new Vector2(10, 500));
                Spawner.GetCType<Image>(line).rectTransform.pivot = new Vector2(0.5f, 0);
                //line.transform.position = bD.root.position;
                Spawner.GetCType<Line>(line).target = linkedGroup.transforms[0].transform;
                Spawner.GetCType<Line>(line).lineRoot = currGroup.gE[0].transform;
                Spawner.GetCType<Line>(line).EstablishJoint();

                
                //Singleton.GetSingleton<PatternControl>().ModifyGroup("lineStart" + savedData.l[i].lD.connectedInt.ToString(), new object[] { line });
                //Singleton.GetSingleton<PatternControl>().ModifyGroup("lineEnd" + i.ToString(), new object[] { line });
            }
        }
    }*/

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

            int id = abilityData.subclasses.Add(new AbilityDataSubclass(interfaces[dataIndex].GetType()));

            Vector2 cursorPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.root as RectTransform, eventData.position, eventData.pressEventCamera, out cursorPos);
            CreateWindow(id, cursorPos);

            /*SavedData newClass = new SavedData();

            for(int i = 0; i < allWindows.Length; i++) {
                WindowsData inst = new WindowsData(allWindows[i]);
                inst.wN = savedData.Add(inst);
                ScriptableObject window = CreateWindow(inst, new Vector3());
                
                window.transform.localPosition = cursorPos;
            }*/
        }
    }

    public ScriptableObject CreateWindow(int id,Vector3 location) {

        EditableWindow editWindow = new EditableWindow();
        editWindow.window.transform.localPosition = location;
        int windowsId = windows.Add(new WindowsData(id, editWindow));

        //Spawner.GetCType<WindowsScript>(editWindow.window).AddEvent(editWindow.lineManager);

        /*Spawner.GetCType<Button>(editWindow.windowsConnector).onClick.AddListener(() => { //Generates line when clicked on
            if(bD.active) {
                bD.active = false;
                LineData lineData = new LineData(savedData.l[bD.p[0]].eW.window.transform, editWindow.windowsConnector.transform);
                editWindow.lineManager.lineData.Add(lineData);
                editWindow.lineManager.UpdateLines();
                (savedData.l[bD.p[0]].lD.fields[bD.p[1]] as RuntimeParameters<EditableLinkInstance>).v.LinkObject(savedData.l[runtimePara.wN].lD);
            }
        });*/

        Spawner.GetCType<Button>(editWindow.windowsDeleter).onClick.AddListener(() => { //Deletes windows when clicked on
            //savedData.Remove(runtimePara.wN);
            //runtimePara.RemoveWindows();
        });


        for(int i = 0; i < abilityData.subclasses.l[id].var.Length; i++) {
            ScriptableObject elementName = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>() });
            ScriptableObject element = ReturnElementField(abilityData.subclasses.l[id].var[i].field);
            Spawner.GetCType<Text>(elementName).text = abilityData.subclasses.l[id].var[i].field.n;
            Spawner.GetCType<Text>(elementName).color = Color.white;

            ScriptableObject align = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<LinearLayout>() });

            Spawner.GetCType<LinearLayout>(align).o = LinearLayout.Orientation.X;
            Spawner.GetCType<LinearLayout>(align).Add(elementName.transform as RectTransform);
            Spawner.GetCType<LinearLayout>(align).Add(element.transform as RectTransform);
            (align.transform as RectTransform).sizeDelta = (Spawner.GetCType<LinearLayout>(align).transform as RectTransform).sizeDelta;
            editWindow.lL.Add(align.transform as RectTransform);

            UICalibrator(element, new int[] { id, i });
        }

        return editWindow.window;
    }

    public ScriptableObject ReturnElementField(RuntimeParameters variable) {
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