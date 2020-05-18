using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using System.Linq;

public enum ActionType {
    RECIEVE, SEND
}

public enum MouseMode {
    NONE, CREATE_NODE, EDIT_CONN, REMOVE_CONN
}

public interface ILineHandler {
    void UpdateLines(int[] id);
}

public class EditableWindow : WindowsWrapper {

    //Main linear layout.
    public LinearLayout lL;

    //Variable linear layout.
    public SpawnerOutput[] variables;

    public List<int> linesRelated;
    public ILineHandler link;

    public override void OnSpawn() {
        base.OnSpawn();

        lL = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(LinearLayout)).script as LinearLayout;
        linesRelated = new List<int>();

        lL.transform.SetParent(content.transform);
        lL.transform.localPosition = new Vector3();
    }

    public override void OnDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
        link.UpdateLines(linesRelated.ToArray());
    }
}

public class MainMenuUICommands : MonoBehaviour, IPointerDownHandler, ILineHandler {

    public UIAbilityData abilityData;
    public AutoPopulationList<EditableWindow> abilityWindows;
    public AutoPopulationList<LineData> lineData;
    public WindowsWrapper additionalOptions;

    public SpawnerOutput mainClassSelection;
    public SpawnerOutput classSelectionScrollRect;

    public Font font;
    Text instance;

    SpawnerOutput windowSpawner;

    DropdownWrapper dW;
    int[] inputValues;

    MouseMode mMode;
    LinkMode lMode;
    Type selectedType;


    //Line Drawing System
    Camera cam;

    AbilityInfo abilityDescription;

    int[] prevPath;

    void Start() {

        string cData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility }, 0);
        string wData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility }, 2);

        if(cData != "")
            abilityData = new UIAbilityData(LoadedData.GetSingleton<JSONFileConvertor>().ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(cData)), JsonConvert.DeserializeObject<float[][]>(wData));
        else
            abilityData = new UIAbilityData();


        abilityWindows = new AutoPopulationList<EditableWindow>();
        lineData = new AutoPopulationList<LineData>();

        SpawnUIFromData();

        mainClassSelection = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(LinearLayout));

        SpawnerOutput name = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(InputFieldWrapper));
        name.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.9f));

        InputField castedName = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<InputField>(name);

        string data = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility }, 1);
        abilityDescription = JsonConvert.DeserializeObject<AbilityInfo>(data);
        castedName.text = abilityDescription.n;

        castedName.onValueChanged.AddListener((s) => {
            abilityDescription.n = s;
        });

        SpawnerOutput[] buttons = new SpawnerOutput[LoadedData.loadedNodeInstance.Count];


        foreach(KeyValuePair<Type, AbilityTreeNode> entry in LoadedData.loadedNodeInstance) {
            SpawnerOutput button = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));

            Button butInst = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(button);

            // Need another way to get elements within spawner output...
            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(button, "Text").text = entry.Key.Name;

            butInst.onClick.AddListener(() => {
                selectedType = entry.Key;
                windowSpawner.script.gameObject.SetActive(true);
                mMode = MouseMode.CREATE_NODE;
            });

            (mainClassSelection.script as LinearLayout).Add(butInst.transform as RectTransform);

        }

        classSelectionScrollRect = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ScrollRectWrapper));
        (classSelectionScrollRect.script as ScrollRectWrapper).ChangeScrollRectSize(new Vector2(100, 600));


        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<UIMule>(classSelectionScrollRect, "Content").GetRectTransform().sizeDelta = (mainClassSelection.script.transform as RectTransform).sizeDelta;
        classSelectionScrollRect.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.1f, 0.9f));

        mainClassSelection.script.transform.SetParent(LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<UIMule>(classSelectionScrollRect, "Content").transform);
        mainClassSelection.script.transform.localPosition = new Vector2(-(mainClassSelection.script.transform as RectTransform).sizeDelta.x / 2, 0);


        windowSpawner = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(Image));
        windowSpawner.script.gameObject.SetActive(false);

        SpawnerOutput optLL = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(LinearLayout));

        SpawnerOutput normConnButt = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(normConnButt, "Text").text = "Normal Conection";
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Image>(normConnButt, "Image").color = Color.green;

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(normConnButt).onClick.AddListener(() => {
            mMode = MouseMode.EDIT_CONN;
            lMode = LinkMode.NORMAL;
        });

        SpawnerOutput sigConnButt = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(sigConnButt, "Text").text = "Signal Conection";
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Image>(sigConnButt, "Image").color = Color.red;

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(sigConnButt).onClick.AddListener(() => {
            mMode = MouseMode.EDIT_CONN;
            lMode = LinkMode.SIGNAL;
        });

        SpawnerOutput rmConnButt = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(rmConnButt, "Text").text = "Remove Conection";

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(rmConnButt).onClick.AddListener(() => {
            mMode = MouseMode.REMOVE_CONN;
        });

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<LinearLayout>(optLL).Add(normConnButt.script.transform as RectTransform);
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<LinearLayout>(optLL).Add(sigConnButt.script.transform as RectTransform);
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<LinearLayout>(optLL).Add(rmConnButt.script.transform as RectTransform);

        optLL.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.9f, 0.9f));

        SpawnerOutput saveButton = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(saveButton).onClick.AddListener(() => {

            int[] aEle = abilityData.subclasses.ReturnActiveElementIndex();

            AbilityDataSubclass[] cAD = abilityData.RelinkSubclass();

            string[] imgDependencies = AbilityDataSubclass.GetImageDependencies(cAD);

            // Gets all window locations.
            float[][] windowLocations = new float[cAD.Length][];

            for(int i = 0; i < windowLocations.Length; i++) {
                windowLocations[i] = new float[2];

                windowLocations[i][0] = abilityWindows.l[aEle[i]].transform.position.x;
                windowLocations[i][1] = abilityWindows.l[aEle[i]].transform.position.y;
            }

            FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility }, 0, JsonConvert.SerializeObject(LoadedData.GetSingleton<JSONFileConvertor>().ConvertToStandard(cAD)));
            FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility }, 1, JsonConvert.SerializeObject(abilityDescription));
            FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility }, 2, JsonConvert.SerializeObject(windowLocations));
            FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility }, 3, JsonConvert.SerializeObject(imgDependencies));
        });

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(saveButton, "Text").text = "Save JSON";
        saveButton.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.1f));


        // Creates dropdown for input.
        dW = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper)).script as DropdownWrapper;

        dW.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.75f, 0.9f));
        List<Dropdown.OptionData> inputs = new List<Dropdown.OptionData>();

        string[] inputNames = Enum.GetNames(typeof(KeyCode));
        int inputIndex = 0;

        inputValues = (int[])Enum.GetValues(typeof(KeyCode));

        for(int i = 0; i < inputNames.Length; i++) {
            inputs.Add(new Dropdown.OptionData(inputNames[i]));

            if(abilityDescription.kC == inputValues[i])
                inputIndex = i;
        }

        dW.dropdown.AddOptions(inputs);
        dW.dropdown.value = inputIndex;

        dW.dropdown.onValueChanged.AddListener((v) => {
            abilityDescription.kC = inputValues[v];
        });

        // Creates dropdown for startnode.
        DropdownWrapper sNDW = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(DropdownWrapper)).script as DropdownWrapper;

        sNDW.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.65f, 0.9f));
        List<Dropdown.OptionData> sNDWinputs = new List<Dropdown.OptionData>();

        sNDWinputs.Add(new Dropdown.OptionData("ThreadSplitter"));
        sNDWinputs.Add(new Dropdown.OptionData("Repeater"));
        sNDW.dropdown.AddOptions(sNDWinputs);

        sNDW.dropdown.value = abilityDescription.sNT;

        sNDW.dropdown.onValueChanged.AddListener((v) => {
            abilityDescription.sNT = v;
        });

        

        ButtonWrapper addOptionsButton = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper)).script as ButtonWrapper;
        addOptionsButton.button.onClick.AddListener(() => CreateWindowForAdditionalOptions());

        addOptionsButton.text.text = "Additional Options";
        addOptionsButton.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.1f,0.1f));
    }

    public void CreateWindowForAdditionalOptions() {
        if(additionalOptions == null) {
            additionalOptions = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(WindowsWrapper)).script as WindowsWrapper;
            additionalOptions.ChangeWindowsContentSize(new Vector2(400, 500));
            additionalOptions.windowsText.text = "Additional Options";

            LinearLayout mainAddOptionsLL = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(LinearLayout)).script as LinearLayout;
            mainAddOptionsLL.transform.SetParent(additionalOptions.content.transform);
            mainAddOptionsLL.transform.localPosition = new Vector3();
        } else
            additionalOptions.gameObject.SetActive(true);
    }


    public void SpawnUIFromData() {

        //Creates windows UI from data. 
        for(int i = 0; i < abilityData.subclasses.l.Count; i++) {
            Vector2 loc = new Vector2(abilityData.loadedWindowsLocation[i][0], abilityData.loadedWindowsLocation[i][1]);
            CreateWindow(i, loc);
        }

        for(int i = 0; i < abilityData.linkAddresses.l.Count; i++) {
            prevPath = new int[] { abilityData.linkAddresses.l[i][0], abilityData.linkAddresses.l[i][1] };
            CreateLinkage(new int[] { abilityData.linkAddresses.l[i][2], abilityData.linkAddresses.l[i][3], abilityData.linkAddresses.l[i][4] }, i);
        }
    }

    public void OnPointerDown(PointerEventData eventData) {

        if(mMode == MouseMode.CREATE_NODE) {
            if(!cam)
                cam = eventData.pressEventCamera;

            windowSpawner.script.gameObject.SetActive(false);

            int id = abilityData.subclasses.Add(new AbilityDataSubclass(selectedType));

            Vector3 cursorPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.root as RectTransform, eventData.position, eventData.pressEventCamera, out cursorPos);
            CreateWindow(id, cursorPos);

            mMode = MouseMode.NONE;
        }
    }

    public void CreateWindow(int id, Vector3 location) {

        EditableWindow editWindow = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(EditableWindow)).script as EditableWindow;
        editWindow.link = this;

        //Runs deletion delegate.
        Button del = editWindow.deleteButton.button;

        del.onClick.AddListener(() => {
            //Handles UI deletion.
            editWindow.gameObject.SetActive(false);

            for(int i = editWindow.linesRelated.Count - 1; i >= 0; i--) {
                //lineData.l[editWindow.linesRelated[i]].line.gameObject.SetActive(false);
                //abilityData.linkAddresses.Remove(editWindow.linesRelated[i]);
                RemoveLine(editWindow.linesRelated[i]);
            }

            abilityData.subclasses.Remove(id);
        });

        editWindow.transform.position = location;
        abilityWindows.ModifyElementAt(id, editWindow);

        //Initialises variable linear layouts.
        editWindow.variables = new SpawnerOutput[abilityData.subclasses.l[id].var.Length];

        Vector2 varGraphicsDimensions = new Vector2(0, 0);

        for(int i = 0; i < abilityData.subclasses.l[id].var.Length; i++) {

            if(LoadedData.GetVariableType(abilityData.subclasses.l[id].classType, i, VariableTypes.HIDDEN))
                continue;

            SpawnerOutput[] var = CreateVariableField(id, i);

            SpawnerOutput get = CreateVariableButtons(ActionType.RECIEVE, new int[] { id, i });
            SpawnerOutput set = CreateVariableButtons(ActionType.SEND, new int[] { id, i });

            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Image>(get, "Image").color = Color.red;
            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Image>(set, "Image").color = Color.green;

            SpawnerOutput align = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(LinearLayout));

            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<LinearLayout>(align).o = LinearLayout.Orientation.X;
            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<LinearLayout>(align).Add(get.script.transform as RectTransform);

            for(int j = 0; j < var.Length; j++)
                LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<LinearLayout>(align).Add(var[j].script.transform as RectTransform);

            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<LinearLayout>(align).Add(set.script.transform as RectTransform);

            //(align.script.transform as RectTransform).sizeDelta = (align.script.transform as RectTransform).sizeDelta;
            editWindow.lL.Add(align.script.transform as RectTransform);
            editWindow.variables[i] = align;

            varGraphicsDimensions.y += (align.script.transform as RectTransform).sizeDelta.y;

            if((align.script.transform as RectTransform).sizeDelta.x > varGraphicsDimensions.x)
                varGraphicsDimensions.x = (align.script.transform as RectTransform).sizeDelta.x;
        }

        editWindow.ChangeWindowsContentSize(varGraphicsDimensions);
        editWindow.windowsText.text = abilityData.subclasses.l[id].classType.Name;
    }

    SpawnerOutput CreateVariableButtons(ActionType aT, int[] id) {

        SpawnerOutput linkageButton = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(linkageButton, "Text").text = "";
        UIDrawer.ChangeUISize(linkageButton, new Vector2(20, 20));

        switch(aT) {
            case ActionType.RECIEVE:
                LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(linkageButton).onClick.AddListener(() => {
                    CreateLinkage(id);
                });
                break;

            case ActionType.SEND:
                LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(linkageButton).onClick.AddListener(() => {
                    prevPath = id;
                });
                break;
        }

        return linkageButton;
    }

    void CreateLinkage(int[] id, int connectionId = -1) {
        if(prevPath.Length > 0) {

            if(connectionId == -1) {

                int linkType = LoadedData.GetVariableType(abilityData.subclasses.l[prevPath[0]].classType, prevPath[1], VariableTypes.SIGNAL_ONLY) ? 1 : 0;
                connectionId = abilityData.linkAddresses.Add(new int[] { prevPath[0], prevPath[1], id[0], id[1], linkType });
            }

            //Debug.LogFormat("ConnectionID assigned {0}. For {1} and {2}", connectionId, prevPath[0], id[0]);

            // Make sure both ends will feedback if window was dragged.
            abilityWindows.l[prevPath[0]].linesRelated.Add(connectionId);
            abilityWindows.l[id[0]].linesRelated.Add(connectionId);

            Transform[] points = new Transform[2];

            int lastObj = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<LinearLayout>(abilityWindows.l[prevPath[0]].variables[prevPath[1]]).objects.Count - 1;
            points[0] = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<LinearLayout>(abilityWindows.l[prevPath[0]].variables[prevPath[1]]).objects[lastObj];
            points[1] = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<LinearLayout>(abilityWindows.l[id[0]].variables[id[1]]).objects[0];

            CreateLines(points, connectionId);
            // Removes the prev path. 
            prevPath = new int[0];
        }
    }

    void CreateLines(Transform[] points, int id) {
        // Creates the graphical strings.
        SpawnerOutput lGraphic = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Image>(lGraphic, "Image").rectTransform.pivot = new Vector2(0.5f, 0);
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(lGraphic, "Text").text = "";

        // Adds event for changing of button colors
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(lGraphic).onClick.AddListener(() => {
            switch(mMode) {
                case MouseMode.EDIT_CONN:
                    int[] linkData = abilityData.linkAddresses.l[id];

                    if(!LoadedData.GetVariableType(abilityData.subclasses.l[linkData[0]].classType, linkData[1], VariableTypes.PERMENANT_TYPE)) {
                        linkData[4] = (int)lMode;
                        UpdateLineColor(id);
                        mMode = MouseMode.NONE;
                    }
                    break;

                case MouseMode.REMOVE_CONN:
                    RemoveLine(id);
                    mMode = MouseMode.NONE;
                    break;
            }
        });

        LineData line = new LineData(points[0], points[1], lGraphic);
        lineData.ModifyElementAt(id, line);
        UpdateLineColor(id);
        UpdateLines(id);
    }

    void UpdateLineColor(int id) {
        switch((LinkMode)abilityData.linkAddresses.l[id][4]) {
            case LinkMode.NORMAL:
                LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Image>(lineData.GetElementAt(id).l, "Image").color = Color.green;
                break;

            case LinkMode.SIGNAL:
                LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Image>(lineData.GetElementAt(id).l, "Image").color = Color.red;
                break;
        }
    }

    void RemoveLine(int id) {
        lineData.l[id].l.script.gameObject.SetActive(false);

        // Removes the linkage from the other to prevent the other window closing linkage.
        abilityWindows.l[abilityData.linkAddresses.l[id][0]].linesRelated.Remove(id);
        abilityWindows.l[abilityData.linkAddresses.l[id][2]].linesRelated.Remove(id);

        abilityData.linkAddresses.Remove(id);
    }

    SpawnerOutput[] CreateVariableField(int id, int varId) {
        SpawnerOutput elementName = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper));

        SpawnerOutput element = LoadedData.loadedNodeInstance[abilityData.subclasses.l[id].classType].ReturnCustomUI(varId, abilityData.subclasses.l[id].var[varId].field);

        if(element == null)
            element = ReturnElementField(id, varId);

        Text eName = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(elementName);

        eName.text = abilityData.subclasses.l[id].var[varId].field.n;
        eName.color = Color.white;

        if(element != null)
            return new SpawnerOutput[] { elementName, element };


        return new SpawnerOutput[] { elementName };
    }

    SpawnerOutput ReturnElementField(int id, int varId) {
        SpawnerOutput element = null;

        RuntimeParameters variable = abilityData.subclasses.l[id].var[varId].field;

        if(variable.t == typeof(string)) {
            element = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(InputFieldWrapper));

            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<InputField>(element).text = (variable as RuntimeParameters<string>).v;

            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<InputField>(element).onValueChanged.AddListener((s) => {
                (abilityData.subclasses.l[id].var[varId].field as RuntimeParameters<string>).v = s;
            });
        }

        if(variable.t == typeof(int)) {
            element = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(InputFieldWrapper));

            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<InputField>(element).text = (variable as RuntimeParameters<int>).v.ToString();

            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<InputField>(element).onValueChanged.AddListener((s) => {
                int i = 0;

                if(int.TryParse(s, out i))
                    (abilityData.subclasses.l[id].var[varId].field as RuntimeParameters<int>).v = i;
            });
        }

        if(variable.t == typeof(float)) {
            element = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(InputFieldWrapper));
            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<InputField>(element).text = (variable as RuntimeParameters<float>).v.ToString();

            LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<InputField>(element).onValueChanged.AddListener((s) => {
                float f = 0;

                if(float.TryParse(s, out f))
                    (abilityData.subclasses.l[id].var[varId].field as RuntimeParameters<float>).v = f;
            });
        }

        if(variable.t == typeof(bool)) {
            element = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ToggleWrapper));
            Toggle t = LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Toggle>(element);
            RuntimeParameters<bool> b = variable as RuntimeParameters<bool>;

            t.isOn = b.v;

            t.onValueChanged.AddListener((value) => {
                b.v = value;
            });
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
        if(mMode == MouseMode.CREATE_NODE)
            windowSpawner.script.transform.position = Input.mousePosition;
    }

    public void UpdateLines(params int[] id) {
        for(int i = 0; i < id.Length; i++)
            if(lineData.l[id[i]].l != null) {
                lineData.l[id[i]].l.script.transform.position = lineData.l[id[i]].s.position;
                Vector2 d = lineData.l[id[i]].e.position - lineData.l[id[i]].s.position;
                LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Image>(lineData.l[id[i]].l, "Image").rectTransform.sizeDelta = new Vector2(10f, d.magnitude);
                lineData.l[id[i]].l.script.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Math.CalculateAngle(d)));
            }
    }
}