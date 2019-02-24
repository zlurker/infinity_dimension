using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using System.IO;

public class SavedDataCommit {

    public List<SavedDataCommit> members;
    public string n;
    public int vI;
    public int cI;
    public string sO;

    public SavedDataCommit(string name, string soValue, int variableIndex) {
        members = new List<SavedDataCommit>();
        n = name;
        sO = soValue;
        vI = variableIndex;
    }

    public SavedDataCommit(string name, int connectedInt) {
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
            SavedDataCommit commitHeader = new SavedDataCommit(target[i].classType.Name, target[i].connectedInt);
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
            instance.connectedInt = cI;

            for(int i = 0; i < instance.fields.Count; i++) {
                int j = Iterator.ReturnKey(members.ToArray(), instance.fields[i].n, (t) => { return t.n; });
                //Debug.Log(selectedInterface.GetType().Name);
                if(j > -1)
                    instance.fields[i] = VariableTypeIndex.ReturnRuntimeType(members[j].vI, members[j].sO);
            }
        }

        return instance;
    }
}


public class SavedData {
    public List<RuntimeParameters> fields;
    public int connectedInt;
    public Type classType;

    public SavedData(Type t) {
        IPlayerEditable[] interfaces = (Iterator.ReturnObject<IPlayerEditable>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as IPlayerEditable[];
        IPlayerEditable selectedInterface = Iterator.ReturnObject(interfaces, t, (p) => {
            return p.GetType();
        });

        classType = t;
        fields = new List<RuntimeParameters>(selectedInterface.GetRuntimeParameters());
        connectedInt = -1;
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
    public static List<SavedData> savedData;

    SavedData testGP;
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
    int windowsCount;

    //UICalibration
    UICalibrator[] uiCalibration;

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

        uiCalibration = new UICalibrator[]
        {
            new UICalibrator<InputField>((t,p)=>{
                Debug.Log(t.contentType);
                string outputIndex = "";
                for (int i=0; i < p.Length; i++)
                    outputIndex += p[i].ToString();

                switch (t.contentType){
                        case InputField.ContentType.Standard:
                            t.text = (savedData[p[0]].fields[p[1]] as RuntimeParameters<string>).v;//SavedData.GetData<string>(savedData,p);
                            break;

                        case InputField.ContentType.IntegerNumber:
                            t.text = (savedData[p[0]].fields[p[1]] as RuntimeParameters<int>).v.ToString();
                            break;

                        case InputField.ContentType.DecimalNumber:
                            t.text = (savedData[p[0]].fields[p[1]] as RuntimeParameters<float>).v.ToString();
                            break;
                }

                t.onValueChanged.AddListener((s) => {
                    switch (t.contentType){
                        case InputField.ContentType.Standard:
                            (savedData[p[0]].fields[p[1]] as RuntimeParameters<string>).v = s;
                            break;

                        case InputField.ContentType.IntegerNumber:
                            (savedData[p[0]].fields[p[1]] as RuntimeParameters<int>).v = int.Parse(s);
                            break;

                        case InputField.ContentType.DecimalNumber:
                            (savedData[p[0]].fields[p[1]] as RuntimeParameters<float>).v = float.Parse(s);
                            break;
                }
            });})
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

        SavedData[] prevData = SavedData.CreateLoadFile(path);
        savedData = new List<SavedData>(prevData);
        SpawnUIFromData(prevData);

        Singleton.GetSingleton<PatternControl>().ModifyGroup("Test", new object[] { Singleton.GetSingleton<UIDrawer>().CustomiseBaseObject(), Singleton.GetSingleton<Spawner>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<Spawner>().CreateComponent<LinearLayout>() }) });

        ScriptableObject[] buttons = new ScriptableObject[interfaces.Length];
        for(int i = 0; i < interfaces.Length; i++) {
            ScriptableObject buttonTest = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Button>() });
            Iterator.ReturnObject<DelegateIterator>(inIt, "WindowSpawner").d.Invoke(new object[] { buttonTest, i });

            Singleton.GetSingleton<PatternControl>().ModifyGroup("Test", new object[] { buttonTest });
        }

        Singleton.GetSingleton<PatternControl>().GetGroup("Test").gP.position = UIDrawer.UINormalisedPosition(new Vector3(0.1f, 0.9f));
        windowSpawner = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Image>(), Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>() });
        windowSpawner.gameObject.SetActive(false);

        ScriptableObject saveButton = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Button>() });
        Spawner.GetCType<Button>(saveButton).onClick.AddListener(() => {
            FileSaver.SaveFile(new string[] { "DataFiles", "TestFile" }, JsonConvert.SerializeObject(SavedDataCommit.ConvertToCommit(savedData.ToArray())));
            Debug.Log(JsonConvert.SerializeObject(savedData));
        });

        Spawner.GetCType<Text>(saveButton).text = "Save JSON";
        saveButton.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.1f));
    }

    public void SpawnUIFromData(SavedData[] data) {
        Debug.Log("Called to spawn UI");
        for(int i = 0; i < data.Length; i++)
            CreateWindow(data[i]);
    }

    public void WindowSpawnState(int index) {
        Spawner.GetCType<Text>(windowSpawner).text = index.ToString();
        dataIndex = index;

        windowSpawner.gameObject.SetActive(true);
        windowSpawnMode = true;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(windowSpawnMode) {
            windowSpawnMode = false;
            windowSpawner.gameObject.SetActive(false);

            SavedData newClass = new SavedData(interfaces[dataIndex].GetType());

            SavedData[] allWindows = GetDefaultWindows(new SavedData[] { newClass });
            for(int i = 0; i < allWindows.Length; i++) {
                savedData.Add(allWindows[i]);

                ScriptableObject window = CreateWindow(allWindows[i]);
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


    public ScriptableObject CreateWindow(SavedData runtimePara) {
        string windowNo = windowsCount.ToString();
        int currWindow = windowsCount;
        windowsCount++;

        ScriptableObject window = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<WindowsScript>() });
        ScriptableObject lL = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<LinearLayout>() });

        Singleton.GetSingleton<PatternControl>().ModifyGroup(windowNo, new object[] { window, lL });
        Spawner.GetCType<LinearLayout>(lL).Add(window);

        for(int i = 0; i < runtimePara.fields.Count; i++) {
            string generatedString = windowNo + " - " + i.ToString();
            ScriptableObject elementName = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>() });
            ScriptableObject align = Singleton.GetSingleton<Spawner>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<LinearLayout>() });
            ScriptableObject element = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<InputField>() });
            Debug.Log(runtimePara.fields[i].n);
            Spawner.GetCType<Text>(elementName).text = runtimePara.fields[i].n;
            Spawner.GetCType<Text>(elementName).color = Color.white;
            Spawner.GetCType<LinearLayout>(align).o = LinearLayout.Orientation.X;

            Spawner.GetCType<InputField>(element).contentType = InputField.ContentType.Custom;

            if(runtimePara.fields[i].t == typeof(string))
                Spawner.GetCType<InputField>(element).contentType = InputField.ContentType.Standard;

            if(runtimePara.fields[i].t == typeof(int))
                Spawner.GetCType<InputField>(element).contentType = InputField.ContentType.IntegerNumber;

            if(runtimePara.fields[i].t == typeof(float))
                Spawner.GetCType<InputField>(element).contentType = InputField.ContentType.DecimalNumber;

            Singleton.GetSingleton<PatternControl>().ModifyGroup(generatedString, new object[] { Singleton.GetSingleton<UIDrawer>().CustomiseBaseObject(), align, elementName, element });
            Singleton.GetSingleton<PatternControl>().ModifyGroup(windowNo, new object[] { Singleton.GetSingleton<PatternControl>().GetGroup(generatedString) });

            UICalibrator(element, new int[] { currWindow, i });
        }

        return window;
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
