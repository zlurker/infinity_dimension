﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

public enum ActionType {
    RECIEVE, SEND
}

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

    public UIAbilityData abilityData;
    public AutoPopulationList<EditableWindow> abilityWindows;
    public AutoPopulationList<LineData> lineData;

    public ScriptableObject mainClassSelection;

    public Font font;
    Text instance;

    AbilityTreeNode[] interfaces;

    ScriptableObject windowSpawner;
    bool windowSpawnMode;
    int dataIndex;

    //Line Drawing System
    Camera cam;

    AbilityDescription abilityDescription;

    int[] prevPath;

    void Start() {

        interfaces = (Iterator.ReturnObject<AbilityTreeNode>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as AbilityTreeNode[];

        //lH = new LinkageHandler();

        string cData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 0);
        string wData = Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 2);

        if(cData != "")
            abilityData = new UIAbilityData(JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(cData)), JsonConvert.DeserializeObject<float[][]>(wData));
        else
            abilityData = new UIAbilityData();


        abilityWindows = new AutoPopulationList<EditableWindow>();
        lineData = new AutoPopulationList<LineData>();

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

            Spawner.GetCType<Text>(buttonTest).text = interfaces[i].GetType().Name;

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

            AbilityDataSubclass[] cAD = abilityData.RelinkSubclass();
            int[] rootClasses = AbilityDataSubclass.ReturnFirstClasses(cAD);
            //int[] endNodeData = AbilityDataSubclass.ReturnNodeEndData(cAD);
            //int[][] getEndData = AbilityDataSubclass.ReturnGetEndNode(cAD, rootClasses);
            int[] nBranchData = AbilityDataSubclass.ReturnNodeBranchData(cAD);

            Dictionary<int, int> specialisedNodeThreadCount = new Dictionary<int, int>();
            AbilityDataSubclass.CalculateSpecialisedNodeThreads(cAD, rootClasses, specialisedNodeThreadCount);

            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 0, JsonConvert.SerializeObject(JSONFileConvertor.ConvertToStandard(cAD)));
            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 1, JsonConvert.SerializeObject(abilityDescription));
            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 3, JsonConvert.SerializeObject(rootClasses));
            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 4, JsonConvert.SerializeObject(nBranchData));
            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 5, JsonConvert.SerializeObject(specialisedNodeThreadCount));
            // Gets all window locations.
            float[][] windowLocations = new float[cAD.Length][];

            for(int i = 0; i < windowLocations.Length; i++) {
                windowLocations[i] = new float[2];

                windowLocations[i][0] = abilityWindows.l[aEle[i]].transform.parent.position.x;
                windowLocations[i][1] = abilityWindows.l[aEle[i]].transform.parent.position.y;
            }

            Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 2, JsonConvert.SerializeObject(windowLocations));

            //Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 4, JsonConvert.SerializeObject(AbilityDataSubclass.ReturnGetterAndSetters(cAD)));
        });

        Spawner.GetCType<Text>(saveButton).text = "Save JSON";
        saveButton.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.1f));
    }

    public void SpawnUIFromData() {

        //Creates windows UI from data. 
        for(int i = 0; i < abilityData.subclasses.l.Count; i++) {
            Vector2 loc = new Vector2(abilityData.loadedWindowsLocation[i][0], abilityData.loadedWindowsLocation[i][1]);
            CreateWindow(i, loc);
        }

        for(int i = 0; i < abilityData.linkAddresses.l.Count; i++) {
            prevPath = new int[] { abilityData.linkAddresses.l[i][0], abilityData.linkAddresses.l[i][1] };
            CreateLinkage(new int[] { abilityData.linkAddresses.l[i][2], abilityData.linkAddresses.l[i][3] },i);
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

            int id = abilityData.subclasses.Add(new AbilityDataSubclass(interfaces[dataIndex].GetType()));

            
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

            for(int i = editWindow.linesRelated.Count - 1; i >= 0; i--) {
                //lineData.l[editWindow.linesRelated[i]].line.gameObject.SetActive(false);
                //abilityData.linkAddresses.Remove(editWindow.linesRelated[i]);

                int relatedLine = editWindow.linesRelated[i];
                lineData.l[relatedLine].line.gameObject.SetActive(false);

                // Removes the linkage from the other to prevent the other window closing linkage.
                abilityWindows.l[abilityData.linkAddresses.l[relatedLine][0]].linesRelated.Remove(relatedLine);
                abilityWindows.l[abilityData.linkAddresses.l[relatedLine][2]].linesRelated.Remove(relatedLine);

                abilityData.linkAddresses.Remove(relatedLine);
            }

            //Handles UI Data deletion.
            abilityData.subclasses.Remove(id);
            //abilityData.linksEdit.ModifyElementAt(id, null);
            //abilityData.ResetTunnelEnd(id);
        });

        editWindow.transform.parent.position = location;
        abilityWindows.ModifyElementAt(id, editWindow);

        //Initialises variable linear layouts.
        editWindow.variables = new LinearLayout[abilityData.subclasses.l[id].var.Length];

        for(int i = 0; i < abilityData.subclasses.l[id].var.Length; i++) {
            ScriptableObject[] var = CreateVariableField(id, i);

            ScriptableObject get = CreateVariableButtons(ActionType.RECIEVE, new int[] { id, i });
            ScriptableObject set = CreateVariableButtons(ActionType.SEND, new int[] { id, i });

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

    ScriptableObject CreateVariableButtons(ActionType aT, int[] id) {

        ScriptableObject linkageButton = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new Type[] { typeof(Button) });

        Spawner.GetCType<Image>(linkageButton).color = Color.black;
        Spawner.GetCType<Text>(linkageButton).text = "";
        UIDrawer.ChangeUISize(linkageButton, new Vector2(20, 20));

        switch(aT) {
            case ActionType.RECIEVE:
                Spawner.GetCType<Button>(linkageButton).onClick.AddListener(() => {
                    // Checks if there's a prev path.
                    if(prevPath.Length > 0) {

                        int connectionId = abilityData.linkAddresses.Add(new int[] { prevPath[0], prevPath[1], id[0], id[1] });

                        Debug.LogFormat("ConnectionID assigned {0}. For {1} and {2}", connectionId, prevPath[0], id[0]);
                        // Make sure both ends will feedback if window was dragged.
                        abilityWindows.l[prevPath[0]].linesRelated.Add(connectionId);
                        abilityWindows.l[id[0]].linesRelated.Add(connectionId);

                        Transform[] points = new Transform[2];

                        int lastObj = abilityWindows.l[prevPath[0]].variables[prevPath[1]].objects.Count - 1;
                        points[0] = abilityWindows.l[prevPath[0]].variables[prevPath[1]].objects[lastObj];

                        points[1] = abilityWindows.l[id[0]].variables[id[1]].objects[0];

                        CreateLines(points, connectionId);
                        // Removes the prev path. 
                        prevPath = new int[0];
                    }
                });
                break;

            case ActionType.SEND:
                Spawner.GetCType<Button>(linkageButton).onClick.AddListener(() => {
                    prevPath = id;
                });
                break;
        }

        return linkageButton;
    }

    void CreateLinkage(int[] id, int connectionId = -1) {
        if(prevPath.Length > 0) {

            if(connectionId == -1)
                connectionId = abilityData.linkAddresses.Add(new int[] { prevPath[0], prevPath[1], id[0], id[1] });

            Debug.LogFormat("ConnectionID assigned {0}. For {1} and {2}", connectionId, prevPath[0], id[0]);
            // Make sure both ends will feedback if window was dragged.
            abilityWindows.l[prevPath[0]].linesRelated.Add(connectionId);
            abilityWindows.l[id[0]].linesRelated.Add(connectionId);

            Transform[] points = new Transform[2];

            int lastObj = abilityWindows.l[prevPath[0]].variables[prevPath[1]].objects.Count - 1;
            points[0] = abilityWindows.l[prevPath[0]].variables[prevPath[1]].objects[lastObj];

            points[1] = abilityWindows.l[id[0]].variables[id[1]].objects[0];

            CreateLines(points, connectionId);
            // Removes the prev path. 
            prevPath = new int[0];
        }
    }

    void CreateLines(Transform[] points, int id) {
        // Creates the graphical strings.
        LineData line = new LineData(points[0], points[1]);
        lineData.ModifyElementAt(id, line);
        UpdateLines(new int[] { id });
    }

    ScriptableObject[] CreateVariableField(int id, int varId) {
        ScriptableObject elementName = Singleton.GetSingleton<UIDrawer>().CreateScriptedObject(new MonoBehaviour[][] { Singleton.GetSingleton<UIDrawer>().CreateComponent<Text>() });
        ScriptableObject element = ReturnElementField(abilityData.subclasses.l[id].var[varId].field);
        Spawner.GetCType<Text>(elementName).text = abilityData.subclasses.l[id].var[varId].field.n;
        Spawner.GetCType<Text>(elementName).color = Color.white;

        if(element != null) {
            TextfieldCalibrator(Spawner.GetCType<InputField>(element), new int[] { id, varId });
            return new ScriptableObject[] { elementName, element };
        }

        return new ScriptableObject[] { elementName };
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

                //Debug.Log("Line ID Rendered: " + id[i]);
            }
    }
}