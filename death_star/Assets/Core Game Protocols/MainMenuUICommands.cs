using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

public interface ILineHandler {
    void UpdateLines(int[] id);
}

public class EditableWindow : WindowsScript {

    //Main linear layout.
    public LinearLayout lL;

    //Variable linear layout.
    public LinearLayout[] variables;

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

    //To handle linkage work.
    public class LinkageHandler {
        public VariableAction vA;
        public int[] path;
        public bool alt;
        public bool createDataLinkage;
        public Transform target;

        public LinkageHandler() {
            alt = false;
            createDataLinkage = false;
        }
    }

    public UIAbilityData abilityData;
    public AutoPopulationList<EditableWindow> abilityWindows;
    public EnhancedList<LineData> lineData;

    public ScriptableObject mainClassSelection;

    public Font font;
    Text instance;

    AbilityTreeNode[] interfaces;

    ScriptableObject windowSpawner;
    bool windowSpawnMode;
    int dataIndex;

    LinkageHandler lH;
    //Line Drawing System
    Camera cam;

    AbilityDescription abilityDescription;

    void Start() {

        interfaces = (Iterator.ReturnObject<AbilityTreeNode>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as AbilityTreeNode[];

        lH = new LinkageHandler();

        string cData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 0);

        if(cData != "")
            abilityData = new UIAbilityData(JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(cData)));
        else
            abilityData = new UIAbilityData();

        abilityWindows = new AutoPopulationList<EditableWindow>();
        lineData = new EnhancedList<LineData>();

        SpawnUIFromData();

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

            AbilityDataSubclass[] cAD = abilityData.RelinkSubclass();

            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 0, JsonConvert.SerializeObject(JSONFileConvertor.ConvertToStandard(cAD)));
            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 1, JsonConvert.SerializeObject(abilityDescription));

            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 3, JsonConvert.SerializeObject(AbilityDataSubclass.ReturnFirstClasses(cAD)));
        });

        Spawner.GetCType<Text>(saveButton).text = "Save JSON";
        saveButton.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.1f));
    }

    public void SpawnUIFromData() {

        //Creates windows UI from data. 
        for(int i = 0; i < abilityData.subclasses.l.Count; i++) {
            Vector2 loc = new Vector2(abilityData.subclasses.l[i].wL[0], abilityData.subclasses.l[i].wL[1]);
            CreateWindow(i, loc);
        }

        //Creates linkage from data.
        for(int i = 0; i < abilityData.subclasses.l.Count; i++)
            for(int j = 0; j < abilityData.subclasses.l[i].var.Length; j++)
                for(int k = 0; k < abilityData.subclasses.l[i].var[j].links.Length; k++)
                    CreateVariableLinkage(new int[] { i, j }, abilityData.subclasses.l[i].var[j].links[k]);

        lH.createDataLinkage = true;
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

            Vector3 cursorPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.root as RectTransform, eventData.position, eventData.pressEventCamera, out cursorPos);
            CreateWindow(id, cursorPos);
        }
    }

    public void CreateWindow(int id, Vector3 location) {

        EditableWindow editWindow = Spawner.GetCType<EditableWindow>(Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<EditableWindow>() }));
        editWindow.InitialiseWindow();
        editWindow.link = this;

        //Runs deletion delegate.
        Button del = Spawner.GetCType<Button>(editWindow.windowsDeleter);

        del.onClick.AddListener(() => {
            //Handles UI deletion.
            editWindow.gameObject.SetActive(false);

            for(int i = 0; i < editWindow.linesRelated.Count; i++)
                lineData.l[editWindow.linesRelated[i]].line.gameObject.SetActive(false);

            //Handles UI Data deletion.
            abilityData.subclasses.Remove(id);
        });

        editWindow.transform.parent.position = location;
        abilityWindows.ModifyElementAt(id, editWindow);

        Spawner.GetCType<Button>(editWindow.windowsDeleter).onClick.AddListener(() => { //Deletes windows when clicked on
            //savedData.Remove(runtimePara.wN);
            //runtimePara.RemoveWindows();
        });

        //Initialises variable linear layouts.
        editWindow.variables = new LinearLayout[abilityData.subclasses.l[id].var.Length];

        for(int i = 0; i < abilityData.subclasses.l[id].var.Length; i++) {
            ScriptableObject[] var = CreateVariableField(id, i);

            ScriptableObject get = CreateVariableLinkerUI(VariableAction.GET, new int[] { id, i });
            ScriptableObject set = CreateVariableLinkerUI(VariableAction.SET, new int[] { id, i });

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
            editWindow.variables[i] = Spawner.GetCType<LinearLayout>(align);
        }
    }

    ScriptableObject CreateVariableLinkerUI(VariableAction variableAction, int[] path) {

        ScriptableObject linkageButton = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(Button) });

        Spawner.GetCType<Image>(linkageButton).color = Color.black;
        Spawner.GetCType<Text>(linkageButton).text = "";
        UIDrawer.ChangeUISize(linkageButton, new Vector2(20, 20));

        Spawner.GetCType<Button>(linkageButton).onClick.AddListener(() => {
            switch(lH.alt) {
                case false:

                    /*Registers everything that was on first click, so we may be able to perform
                    actions on second click.*/
                    lH.vA = variableAction;
                    lH.path = path;

                    //Completes the loop.
                    lH.alt = true;
                    break;

                case true:

                    //Register previous path.
                    int[] prevPath = new int[] { lH.path[0], lH.path[1] };

                    /*Adds current selected path to previously selected path, with VariableAction to know what
                    action to take with this linkage.*/
                    int[] currPath = new int[] { path[0], path[1], (int)lH.vA };

                    //Creates linkage
                    CreateVariableLinkage(prevPath, currPath);

                    //Completes the loop.
                    lH.alt = false;

                    break;
            }
        });

        return linkageButton;
    }

    void CreateVariableLinkage(int[] prevPath, int[] currPath) {

        //Handles the data linkage.
        if(lH.createDataLinkage)
            abilityData.linksEdit.l[prevPath[0]][prevPath[1]].Add(currPath);

        //Gets the UI data of both paths.
        Transform[] linePoints = new Transform[2];

        switch((VariableAction)currPath[2]) {

            case VariableAction.GET:

                linePoints[0] = abilityWindows.l[prevPath[0]].variables[prevPath[1]].objects[0];

                int lI0 = abilityWindows.l[currPath[0]].variables[currPath[1]].objects.Count - 1;
                linePoints[1] = abilityWindows.l[currPath[0]].variables[currPath[1]].objects[lI0];
                break;

            case VariableAction.SET:

                int lI1 = abilityWindows.l[prevPath[0]].variables[prevPath[1]].objects.Count - 1;
                linePoints[0] = abilityWindows.l[prevPath[0]].variables[prevPath[1]].objects[lI1];

                linePoints[1] = abilityWindows.l[currPath[0]].variables[currPath[1]].objects[0];
                break;
        }

        //Creates the graphical strings.
        LineData line = new LineData(linePoints[0], linePoints[1]);
        int lineId = lineData.Add(line);

        //Make sure both ends will feedback if window was dragged.
        abilityWindows.l[currPath[0]].linesRelated.Add(lineId);
        abilityWindows.l[prevPath[0]].linesRelated.Add(lineId);
        UpdateLines(new int[] { lineId });
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