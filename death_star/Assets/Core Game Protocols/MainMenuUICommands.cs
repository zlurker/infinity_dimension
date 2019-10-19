using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using System.IO;

public interface ILineHandler {
    void UpdateLines(int[] id);
}

public class EditableWindow : WindowsScript {

    public LinearLayout lL;
    public ScriptableObject windowsDeleter;
    public List<int> linesRelated;
    public ILineHandler link;

    public void InitialiseWindow() {
        lL = Spawner.GetCType<LinearLayout>(Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<LinearLayout>() }));
        windowsDeleter = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(Button) });
        linesRelated = new List<int>();

        Spawner.GetCType<Image>(windowsDeleter).color = Color.red;
        Spawner.GetCType<Text>(windowsDeleter).text = "";

        windowsDeleter.transform.SetParent(transform);
        lL.transform.SetParent(transform);

        windowsDeleter.transform.position = UIDrawer.UINormalisedPosition(transform as RectTransform, new Vector2(0.85f, 0.5f));
        lL.transform.position = UIDrawer.UINormalisedPosition(transform as RectTransform, new Vector2(0f, 0.1f));

        UIDrawer.ChangeUISize(windowsDeleter, new Vector2(20, 20));
    }

    public override void OnDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
        link.UpdateLines(linesRelated.ToArray());
    }
}

public class MainMenuUICommands : MonoBehaviour, IPointerDownHandler, ILineHandler {

    public class LinkageHandler { //To handle linkage work.
        public VariableAction vA;
        public int[] path;
        public bool alt;
        public Transform target;

        public LinkageHandler() {
            alt = false;
        }
    }

    public UIAbilityData abilityData;
    //public EnhancedList<WindowsData> windows;
    public AutoPopulationList<EditableWindow> abilityWindows;
    public EnhancedList<LineData> lineData;

    public ScriptableObject mainClassSelection;

    public Font font;
    Text instance;

    IPlayerEditable[] interfaces;

    ScriptableObject windowSpawner;
    bool windowSpawnMode;
    int dataIndex;

    LinkageHandler lH;
    //Line Drawing System
    Camera cam;

    AbilityDescription abilityDescription;

    void Start() {

        interfaces = (Iterator.ReturnObject<IPlayerEditable>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as IPlayerEditable[];

        lH = new LinkageHandler();

        string cData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 0);

        if(cData != "")
            abilityData = new UIAbilityData(JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(cData)));
        else
            abilityData = new UIAbilityData();

        //windows = new EnhancedList<WindowsData>();
        abilityWindows = new AutoPopulationList<EditableWindow>();
        lineData = new EnhancedList<LineData>();

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

            Button button = Spawner.GetCType<Button>(buttonTest);

            Spawner.GetCType<Text>(buttonTest).text = i.ToString();

            int eleId = i;
            button.onClick.AddListener(() => { WindowSpawnState(eleId); });
            Spawner.GetCType<LinearLayout>(mainClassSelection).Add(buttonTest.transform as RectTransform);
        }

        mainClassSelection.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.1f, 0.9f));
        windowSpawner = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Image>(), Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>() });
        windowSpawner.gameObject.SetActive(false);

        ScriptableObject saveButton = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Button>() });

        Spawner.GetCType<Button>(saveButton).onClick.AddListener(() => {

            int[] aEle = abilityData.subclasses.ReturnActiveElementIndex();


            for(int i = 0; i < aEle.Length; i++) {
                abilityData.subclasses.l[aEle[i]].wL[0] = abilityWindows.l[aEle[i]].transform.parent.position.x;
                abilityData.subclasses.l[aEle[i]].wL[1] = abilityWindows.l[aEle[i]].transform.parent.position.y;
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
            CreateWindow(i, loc);
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

            int id = abilityData.Add(new AbilityDataSubclass(interfaces[dataIndex].GetType()));

            Vector2 cursorPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.root as RectTransform, eventData.position, eventData.pressEventCamera, out cursorPos);
            CreateWindow(id, cursorPos);
        }
    }

    public void CreateWindow(int id, Vector3 location) {

        EditableWindow editWindow = Spawner.GetCType<EditableWindow>(Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<EditableWindow>() }));
        editWindow.InitialiseWindow();
        editWindow.link = this;
        editWindow.transform.parent.position = location;
        abilityWindows.ModifyElementAt(id, editWindow);

        Spawner.GetCType<Button>(editWindow.windowsDeleter).onClick.AddListener(() => { //Deletes windows when clicked on
            //savedData.Remove(runtimePara.wN);
            //runtimePara.RemoveWindows();
        });


        for(int i = 0; i < abilityData.subclasses.l[id].var.Length; i++) {
            ScriptableObject[] var = CreateVariableField(id, i);

            ScriptableObject get = CreateVariableLinkage(VariableAction.GET, new int[] { id, i });
            ScriptableObject set = CreateVariableLinkage(VariableAction.SET, new int[] { id, i });

            Spawner.GetCType<Image>(get).color = Color.red;
            Spawner.GetCType<Image>(set).color = Color.green;

            ScriptableObject align = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<LinearLayout>() });
            Spawner.GetCType<LinearLayout>(align).o = LinearLayout.Orientation.X;

            Spawner.GetCType<LinearLayout>(align).Add(get.transform as RectTransform);

            for(int j = 0; j < var.Length; j++)
                Spawner.GetCType<LinearLayout>(align).Add(var[j].transform as RectTransform);

            Spawner.GetCType<LinearLayout>(align).Add(set.transform as RectTransform);

            (align.transform as RectTransform).sizeDelta = (Spawner.GetCType<LinearLayout>(align).transform as RectTransform).sizeDelta;
            editWindow.lL.Add(align.transform as RectTransform);
        }
    }

    ScriptableObject CreateVariableLinkage(VariableAction variableAction, int[] path) {

        ScriptableObject linkageButton = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(Button) });

        Spawner.GetCType<Image>(linkageButton).color = Color.black;
        Spawner.GetCType<Text>(linkageButton).text = "";
        UIDrawer.ChangeUISize(linkageButton, new Vector2(20, 20));

        Spawner.GetCType<Button>(linkageButton).onClick.AddListener(() => {
            switch(lH.alt) {
                case false:

                    //Registers everything that was on first click, so we may be able to perform
                    //actions on second click.
                    lH.vA = variableAction;
                    lH.path = path;
                    lH.target = linkageButton.transform;
                    lH.alt = true;
                    break;

                case true:

                    //Adds current selected path to previously selected path.
                    int[] currPath = new int[] { path[0], path[1], (int)variableAction };
                    abilityData.linksEdit.l[lH.path[0]][lH.path[1]].Add(currPath);
                    lH.alt = false;

                    //Creates the graphical strings.
                    LineData line = new LineData(linkageButton.transform, lH.target);
                    int lineId = lineData.Add(line);

                    //Make sure both ends will feedback if window was dragged.
                    abilityWindows.l[path[0]].linesRelated.Add(lineId);
                    abilityWindows.l[lH.path[0]].linesRelated.Add(lineId);
                    UpdateLines(new int[] { lineId });

                    //To add break link, Reverse flow option.

                    break;
            }
        });

        return linkageButton;
    }

    ScriptableObject[] CreateVariableField(int id, int varId) {
        ScriptableObject elementName = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>() });
        ScriptableObject element = ReturnElementField(abilityData.subclasses.l[id].var[varId].field);
        Spawner.GetCType<Text>(elementName).text = abilityData.subclasses.l[id].var[varId].field.n;
        Spawner.GetCType<Text>(elementName).color = Color.white;
        TextfieldCalibrator(Spawner.GetCType<InputField>(element), new int[] { id, varId });

        return new ScriptableObject[] { elementName, element };
    }

    ScriptableObject ReturnElementField(RuntimeParameters variable) {
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

    public void TextfieldCalibrator(InputField t, int[] p) {
        switch(t.contentType) {
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
            switch(t.contentType) {
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
        });
    }

    void Update() {
        if(windowSpawnMode)
            windowSpawner.transform.position = Input.mousePosition;
    }

    public void UpdateLines(int[] id) {
        for(int i = 0; i < id.Length; i++)
            if(lineData.l[id[i]].line != null) {
                lineData.l[id[i]].line.transform.position = lineData.l[id[i]].s.position;
                Vector2 d = lineData.l[id[i]].e.position - lineData.l[id[i]].s.position;
                Spawner.GetCType<Image>(lineData.l[id[i]].line).rectTransform.sizeDelta = new Vector2(10f, d.magnitude);
                lineData.l[id[i]].line.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Math.CalculateAngle(d)));
            }
    }
}