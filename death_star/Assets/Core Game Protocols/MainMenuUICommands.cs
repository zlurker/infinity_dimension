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
    public string sO;

    public SavedDataCommit(SavedData target, string name) {
        members = new List<SavedDataCommit>();
        n = name;
        ConvertToCommit(target);
    }

    public SavedDataCommit(string name, string soValue, int variableIndex) {
        members = new List<SavedDataCommit>();
        n = name;
        sO = soValue;
        vI = variableIndex;

    }

    public SavedDataCommit() {
        members = new List<SavedDataCommit>();
    }

    public void ConvertToCommit(SavedData target) {
        for(int i = 0; i < target.fields.Count; i++) {
            RuntimeParameters<SavedData> inst = target.fields[i] as RuntimeParameters<SavedData>;

            if(inst != null)
                members.Add(new SavedDataCommit(inst.v, inst.n));
            else
                members.Add(new SavedDataCommit(target.fields[i].n, target.fields[i].GetSerializedObject(), target.fields[i].vI));
        }
    }

    public SavedData CreateSavedData() {
        SavedData instance = new SavedData();
        RuntimeParameters[] validatedData = new RuntimeParameters[0];

        int a = Iterator.ReturnKey<IPlayerEditable>(MainMenuUICommands.interfaces, n, (t) => { return t.GetType().Name; });

        //Debug.Log(instance.fields.Count);
        if(a > -1)
            instance.fields = new List<RuntimeParameters>(MainMenuUICommands.interfaces[a].GetRuntimeParameters());
        else
            instance.fields = new List<RuntimeParameters>(new RuntimeParameters[members.Count]);

        for(int i = 0; i < members.Count; i++) {

            if(members[i].members.Count > 0)
                instance.fields[i] = new RuntimeParameters<SavedData>(members[i].n, members[i].CreateSavedData());
            else {
 
                int b = Iterator.ReturnKey<RuntimeParameters>(instance.fields.ToArray(), members[i].n, (t) => { return t.n; });

                if(b > -1)
                    instance.fields[b] = VariableTypeIndex.ReturnRuntimeType(members[i].vI, members[i].sO);
            }

        }
        return instance;
    }
}


public class SavedData {
    public List<RuntimeParameters> fields;

    public SavedData(RuntimeParameters[] runtimeParameters) {
        fields = new List<RuntimeParameters>();

        for(int i = 0; i < runtimeParameters.Length; i++)
            fields.Add(runtimeParameters[i]);
    }

    public SavedData() {
        fields = new List<RuntimeParameters>();
    }

    public static SavedData LoadData(string filePath) {
        SavedData instance = null;
        SavedDataCommit textData;

        using(StreamReader reader = new StreamReader(filePath)) {
            string text = reader.ReadToEnd();
            textData = JsonConvert.DeserializeObject<SavedDataCommit>(text);
            reader.Close();
        }
        //Debug.Log(textData.members.Count);
        //Debug.LogError(JsonConvert.SerializeObject(textData));
        instance = textData.CreateSavedData();

        return instance;

    }

    public static T GetData<T>(SavedData s, int[] pathToData) {
        Debug.Log(GetDataPath(s, pathToData).fields[pathToData[pathToData.Length - 1]].n);
        return (GetDataPath(s, pathToData).fields[pathToData[pathToData.Length - 1]] as RuntimeParameters<T>).v;
    }

    public static void SetData<T>(SavedData s, int[] pathToData, T value, string name = "-") {
        Iterator inst = GetDataPath(s, pathToData).fields[pathToData[pathToData.Length - 1]];
        RuntimeParameters<T> paraInst = inst as RuntimeParameters<T>;

        if(paraInst == null)
            paraInst = new RuntimeParameters<T>(name, value);
        else
            paraInst.v = value;

        GetDataPath(s, pathToData).fields[pathToData[pathToData.Length - 1]] = paraInst;
        Debug.Log(MainMenuUICommands.savedData.fields.Count);
        //Debug.Log((GetDataPath(s, pathToData).fields[pathToData[pathToData.Length - 1]] as RuntimeParameters<T>).n);
    }

    public static SavedData GetDataPath(SavedData s, int[] id) {
        SavedData root = s;

        for(int i = 0; i < id.Length; i++) {
            int d = id[i] - root.fields.Count;

            for(int j = 0; j <= d; j++)
                root.fields.Add(new RuntimeParameters());

            if(id.Length - i > 1) {
                if(root.fields[id[i]] as RuntimeParameters<SavedData> == null)
                    root.fields[id[i]] = new RuntimeParameters<SavedData>("Header", new SavedData());
                root = (root.fields[id[i]] as RuntimeParameters<SavedData>).v;
            }
            //Debug.Log(root.fields.Count); lol;,./.ro9ieioede
        }
        //Debug.Log(root.fields.Count);
        return root;
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
    public static SavedData savedData;

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

                    Debug.Log(outputIndex);

                switch (t.contentType){
                        case InputField.ContentType.Standard:
                            t.text = SavedData.GetData<string>(savedData,p);
                            break;

                        case InputField.ContentType.IntegerNumber:

                            t.text = SavedData.GetData<int>(savedData,p).ToString();

                            Debug.Log("Unable to get");
                            break;

                        case InputField.ContentType.DecimalNumber:
                            t.text = SavedData.GetData<float>(savedData,p).ToString();
                            break;
                }


                t.onValueChanged.AddListener((s) => {
                    switch (t.contentType){
                        case InputField.ContentType.Standard:
                            SavedData.SetData<string>(savedData,p,s);
                            break;

                        case InputField.ContentType.IntegerNumber:
                            SavedData.SetData<int>(savedData,p,int.Parse(s));
                            break;

                        case InputField.ContentType.DecimalNumber:
                            SavedData.SetData<float>(savedData,p,float.Parse(s));
                            break;
                }

                Debug.Log(JsonConvert.SerializeObject(savedData));
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

        savedData = SavedData.LoadData(path);

        if(savedData == null)
            savedData = new SavedData(new RuntimeParameters[0]);

        SpawnUIFromData(savedData);

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
            FileSaver.SaveFile(new string[] { "DataFiles", "TestFile" }, JsonConvert.SerializeObject(new SavedDataCommit(savedData, "-")));
            Debug.Log(JsonConvert.SerializeObject(savedData));
        });

        Spawner.GetCType<Text>(saveButton).text = "Save JSON";
        saveButton.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.1f));
    }

    public void SpawnUIFromData(SavedData data) {
        for(int i = 0; i < data.fields.Count; i++) {
            int a = Iterator.ReturnKey<IPlayerEditable>(interfaces, (data.fields[i] as RuntimeParameters<SavedData>).n,
                (t) => { return t.GetType().Name; });

            if(a > -1)
                CreateWindow(a);
        }
    }

    /*public void ChangeEditableUITemplate(object[] p)
    {
        int arg0 = (int)p[0];
        IPlayerEditable[] instances = (Iterator.ReturnObject<IPlayerEditable>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as IPlayerEditable[];
        Debug.Log(instances.Length);
        instance.text = instances[arg0].GetType().Name;
        MethodInfo returnedMethod = instances[arg0].GetMainMethod();
        ParameterInfo[] fields = new ParameterInfo[0];

        if (returnedMethod != null)
            fields = returnedMethod.GetParameters();

        ScriptableObject[] fieldNames = new ScriptableObject[fields.Length];
        ScriptableObject[] field = new ScriptableObject[fields.Length];

        Singleton.GetSingleton<UIDrawer>().ModifyLayer(0);

        for (int i = 0; i < fields.Length; i++)
        {
            ScriptableObject tinst = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>() });
            Spawner.GetCType<Text>(tinst).text = fields[i].Name;
            fieldNames[i] = tinst;

            ScriptableObject iinst = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<InputField>() });
            field[i] = iinst;
            InitialiseFieldForUse(iinst.scripts, new string[] { instances[arg0].GetType().Name, fields[i].Name });
        }

        /*PatternControl.i.Pattern_Args(fieldNames,
            new object[][] {
            new object[] { Patterns.GROUP_PATTERN, "FieldNames", GroupArgs.ADD_PARAMETER_OBJECTS,GroupArgs.PARENT_ALL_CURRENT_OBJECTS },
            new object[] { Patterns.VECTOR_PATTERN, UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.85f)), new Vector3(0,-25f) }
        });

        PatternControl.i.Pattern_Args(field,
            new object[][] {
            new object[] { Patterns.GROUP_PATTERN, "FieldNames", GroupArgs.ADD_PARAMETER_OBJECTS,GroupArgs.PARENT_ALL_CURRENT_OBJECTS },
            new object[] { Patterns.VECTOR_PATTERN, UIDrawer.UINormalisedPosition(new Vector3(0.6f, 0.85f)), new Vector3(0,-25f) }
        });

        Singleton.GetSingleton<UIDrawer>().ModifyLayer(0, "FieldNames");
    }*/

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

            RuntimeParameters[] runtimePara = interfaces[dataIndex].GetRuntimeParameters();
            SavedData newClass = new SavedData();

            savedData.fields.Add(new RuntimeParameters<SavedData>(interfaces[dataIndex].GetType().Name, newClass));

            Debug.Log(runtimePara.Length);
            for(int i = 0; i < runtimePara.Length; i++)
                newClass.fields.Add(runtimePara[i]);

            ScriptableObject window = CreateWindow(dataIndex);
            Vector2 cursorPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.root as RectTransform, eventData.position, eventData.pressEventCamera, out cursorPos);
            window.transform.localPosition = cursorPos;
        }
    }

    public ScriptableObject CreateWindow(int targetInterface) {
        string windowNo = windowsCount.ToString();

        Iterator[] runtimePara = interfaces[dataIndex].GetRuntimeParameters();

        ScriptableObject window = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<WindowsScript>() });
        ScriptableObject lL = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<LinearLayout>() });

        Singleton.GetSingleton<PatternControl>().ModifyGroup(windowNo, new object[] { window, lL });
        Spawner.GetCType<LinearLayout>(lL).Add(window);

        Debug.Log("Runtime Para Length + " + runtimePara.Length);
        for(int i = 0; i < runtimePara.Length; i++) {
            string generatedString = windowsCount.ToString() + " - " + i.ToString();
            ScriptableObject elementName = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>() });
            ScriptableObject align = Singleton.GetSingleton<Spawner>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<LinearLayout>() });
            ScriptableObject element = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<InputField>() });

            Spawner.GetCType<Text>(elementName).text = runtimePara[i].n;
            Spawner.GetCType<Text>(elementName).color = Color.white;
            Spawner.GetCType<LinearLayout>(align).o = LinearLayout.Orientation.X;

            if(runtimePara[i].t == typeof(int))
                Spawner.GetCType<InputField>(element).contentType = InputField.ContentType.IntegerNumber;

            if(runtimePara[i].t == typeof(float))
                Spawner.GetCType<InputField>(element).contentType = InputField.ContentType.DecimalNumber;


            Singleton.GetSingleton<PatternControl>().ModifyGroup(generatedString, new object[] { Singleton.GetSingleton<UIDrawer>().CustomiseBaseObject(), align, elementName, element });
            Singleton.GetSingleton<PatternControl>().ModifyGroup(windowNo, new object[] { Singleton.GetSingleton<PatternControl>().GetGroup(generatedString) });


            UICalibrator(element, new int[] { windowsCount, i });
            Debug.Log(windowsCount + " " + i);
            Debug.Log(i);
        }

        windowsCount++;
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
