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
    public SpawnerOutput[] variables;

    public SpawnerOutput windowsDeleter;
    public List<int> linesRelated;
    public ILineHandler link;

    public void InitialiseWindow() {
        lL = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(LinearLayout)).script as LinearLayout;
        windowsDeleter = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(Button));
        linesRelated = new List<int>();

        UIDrawer.GetTypeInElement<Image>(windowsDeleter).color = Color.red;
        UIDrawer.GetTypeInElement<Text>(windowsDeleter).text = "";

        windowsDeleter.script.transform.SetParent(transform);
        lL.transform.SetParent(transform);

        windowsDeleter.script.transform.position = UIDrawer.UINormalisedPosition(transform as RectTransform, new Vector2(0.85f, 0.5f));
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

    public SpawnerOutput mainClassSelection;

    public Font font;
    Text instance;

    Type[] typeMap;

    SpawnerOutput windowSpawner;
    bool windowSpawnMode;
    int dataIndex;

    //Line Drawing System
    Camera cam;

    AbilityDescription abilityDescription;

    int[] prevPath;

    void Start() {

        //interfaces = (Iterator.ReturnObject<AbilityTreeNode>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as AbilityTreeNode[];

        //lH = new LinkageHandler();

        string cData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 0);
        string wData = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 2);

        if(cData != "")
            abilityData = new UIAbilityData(JSONFileConvertor.ConvertToData(JsonConvert.DeserializeObject<StandardJSONFileFormat[]>(cData)), JsonConvert.DeserializeObject<float[][]>(wData));
        else
            abilityData = new UIAbilityData();


        abilityWindows = new AutoPopulationList<EditableWindow>();
        lineData = new AutoPopulationList<LineData>();

        SpawnUIFromData();

        mainClassSelection = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(LinearLayout));

        SpawnerOutput name = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(InputField));
        name.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.9f));

        InputField castedName = UIDrawer.GetTypeInElement<InputField>(name);

        string data = FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericLoadTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 1);
        abilityDescription = JsonConvert.DeserializeObject<AbilityDescription>(data);
        UIDrawer.GetTypeInElement<InputField>(name).text = abilityDescription.n;

        castedName.onValueChanged.AddListener((s) => {
            abilityDescription.n = s;
        });

        SpawnerOutput[] buttons = new SpawnerOutput[LoadedData.loadedNodeInstance.Count];
        typeMap = new Type[LoadedData.loadedNodeInstance.Count];

        int typeInt = 0;

        foreach(KeyValuePair<Type, AbilityTreeNode> entry in LoadedData.loadedNodeInstance) {
            SpawnerOutput button = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(Button));

            Button butInst = UIDrawer.GetTypeInElement<Button>(button);

            typeMap[typeInt] = entry.Key;

            // Need another way to get elements within spawner output...
            UIDrawer.GetTypeInElement<Text>(button).text = entry.Key.Name;

            butInst.onClick.AddListener(() => { WindowSpawnState(typeInt); });
            (mainClassSelection.script as LinearLayout).Add(butInst.transform as RectTransform);
            typeInt++;
        }

        mainClassSelection.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.1f, 0.9f));
        windowSpawner = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(Image));
        windowSpawner.script.gameObject.SetActive(false);

        SpawnerOutput saveButton = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(Button));

        UIDrawer.GetTypeInElement<Button>(saveButton).onClick.AddListener(() => {

            int[] aEle = abilityData.subclasses.ReturnActiveElementIndex();

            AbilityDataSubclass[] cAD = abilityData.RelinkSubclass();
            int[] rootClasses = AbilityDataSubclass.ReturnFirstClasses(cAD);
            //int[] endNodeData = AbilityDataSubclass.ReturnNodeEndData(cAD);
            //int[][] getEndData = AbilityDataSubclass.ReturnGetEndNode(cAD, rootClasses);
            int[] nBranchData = AbilityDataSubclass.ReturnNodeBranchData(cAD);

            Dictionary<int, int> specialisedNodeThreadCount = new Dictionary<int, int>();
            AbilityDataSubclass.CalculateSpecialisedNodeThreads(cAD, rootClasses, specialisedNodeThreadCount);

            FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 0, JsonConvert.SerializeObject(JSONFileConvertor.ConvertToStandard(cAD)));
            FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 1, JsonConvert.SerializeObject(abilityDescription));
            FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 3, JsonConvert.SerializeObject(rootClasses));
            FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 4, JsonConvert.SerializeObject(nBranchData));
            FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 5, JsonConvert.SerializeObject(specialisedNodeThreadCount));
            
            // Gets all window locations.
            float[][] windowLocations = new float[cAD.Length][];

            for(int i = 0; i < windowLocations.Length; i++) {
                windowLocations[i] = new float[2];

                windowLocations[i][0] = abilityWindows.l[aEle[i]].transform.parent.position.x;
                windowLocations[i][1] = abilityWindows.l[aEle[i]].transform.parent.position.y;
            }

            FileSaver.sFT[FileSaverTypes.PLAYER_GENERATED_DATA].GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 2, JsonConvert.SerializeObject(windowLocations));

            //Iterator.ReturnObject<FileSaveTemplate>(FileSaver.sFT, "Datafile", (s) => { return s.c; }).GenericSaveTrigger(new string[] { AbilityPageScript.selectedAbility.ToString() }, 4, JsonConvert.SerializeObject(AbilityDataSubclass.ReturnGetterAndSetters(cAD)));
        });

        UIDrawer.GetTypeInElement<Text>(saveButton).text = "Save JSON";
        saveButton.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.1f));
    }

    public void SpawnUIFromData() {

        //Creates windows UI from data. 
        for(int i = 0; i < abilityData.subclasses.l.Count; i++) {
            Vector2 loc = new Vector2(abilityData.loadedWindowsLocation[i][0], abilityData.loadedWindowsLocation[i][1]);
            CreateWindow(i, loc);
        }

        for(int i = 0; i < abilityData.linkAddresses.l.Count; i++) {
            prevPath = new int[] { abilityData.linkAddresses.l[i][0], abilityData.linkAddresses.l[i][1] };
            CreateLinkage(new int[] { abilityData.linkAddresses.l[i][2], abilityData.linkAddresses.l[i][3] }, i);
        }
    }

    public void WindowSpawnState(int index) {
        //windowSpawner.GetSupportScript<Text>(SupportObjectID).text = index.ToString();
        dataIndex = index;

        windowSpawner.script.gameObject.SetActive(true);
        windowSpawnMode = true;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(!cam)
            cam = eventData.pressEventCamera;


        if(windowSpawnMode) {
            windowSpawnMode = false;
            windowSpawner.script.gameObject.SetActive(false);

            int id = abilityData.subclasses.Add(new AbilityDataSubclass(typeMap[dataIndex]));


            Vector3 cursorPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.root as RectTransform, eventData.position, eventData.pressEventCamera, out cursorPos);
            CreateWindow(id, cursorPos);
        }
    }

    public void CreateWindow(int id, Vector3 location) {

        EditableWindow editWindow = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(EditableWindow)).script as EditableWindow;
        editWindow.InitialiseWindow();
        editWindow.link = this;

        //Runs deletion delegate.
        Button del = editWindow.windowsDeleter.script as Button;

        del.onClick.AddListener(() => {
            //Handles UI deletion.
            editWindow.gameObject.SetActive(false);

            for(int i = editWindow.linesRelated.Count - 1; i >= 0; i--) {
                //lineData.l[editWindow.linesRelated[i]].line.gameObject.SetActive(false);
                //abilityData.linkAddresses.Remove(editWindow.linesRelated[i]);

                int relatedLine = editWindow.linesRelated[i];
                lineData.l[relatedLine].line.script.gameObject.SetActive(false);

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
        editWindow.variables = new SpawnerOutput[abilityData.subclasses.l[id].var.Length];

        for(int i = 0; i < abilityData.subclasses.l[id].var.Length; i++) {
            SpawnerOutput[] var = CreateVariableField(id, i);

            SpawnerOutput get = CreateVariableButtons(ActionType.RECIEVE, new int[] { id, i });
            SpawnerOutput set = CreateVariableButtons(ActionType.SEND, new int[] { id, i });
            
            UIDrawer.GetTypeInElement<Image>(get).color = Color.red;
            UIDrawer.GetTypeInElement<Image>(set).color = Color.green;

            SpawnerOutput align = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(LinearLayout));

            UIDrawer.GetTypeInElement<LinearLayout>(align).o = LinearLayout.Orientation.X;

            UIDrawer.GetTypeInElement<LinearLayout>(align).Add(get.script.transform as RectTransform);

            for(int j = 0; j < var.Length; j++)
                UIDrawer.GetTypeInElement<LinearLayout>(align).Add(var[j].script.transform as RectTransform);

            UIDrawer.GetTypeInElement<LinearLayout>(align).Add(set.script.transform as RectTransform);

            (align.script.transform as RectTransform).sizeDelta = (align.script.transform as RectTransform).sizeDelta;
            editWindow.lL.Add(align.script.transform as RectTransform);
            editWindow.variables[i] = align;
        }
    }

    SpawnerOutput CreateVariableButtons(ActionType aT, int[] id) {

        SpawnerOutput linkageButton = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(Button));

        UIDrawer.GetTypeInElement<Text>(linkageButton).text = "";
        UIDrawer.ChangeUISize(linkageButton, new Vector2(20, 20));

        switch(aT) {
            case ActionType.RECIEVE:
                UIDrawer.GetTypeInElement<Button>(linkageButton).onClick.AddListener(() => {
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
                UIDrawer.GetTypeInElement<Button>(linkageButton).onClick.AddListener(() => {
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

    SpawnerOutput[] CreateVariableField(int id, int varId) {
        SpawnerOutput elementName = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(Text));
        SpawnerOutput element = ReturnElementField(abilityData.subclasses.l[id].var[varId].field);

        Text eName = elementName.script as Text;

        eName.text = abilityData.subclasses.l[id].var[varId].field.n;
        eName.color = Color.white;

        if(element != null)
            return new SpawnerOutput[] { elementName, element };


        return new SpawnerOutput[] { elementName };
    }

    SpawnerOutput ReturnElementField(RuntimeParameters variable) {
        SpawnerOutput element = null;

        if(variable.t == typeof(string) || variable.t == typeof(int) || variable.t == typeof(float))
            element = LoadedData.GetSingleton<UIDrawer>().CreateUIObject(typeof(InputField));

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
            windowSpawner.script.transform.position = Input.mousePosition;
    }

    public void UpdateLines(int[] id) {
        for(int i = 0; i < id.Length; i++)
            if(lineData.l[id[i]].line != null) {
                lineData.l[id[i]].line.script.transform.position = lineData.l[id[i]].s.position;
                Vector2 d = lineData.l[id[i]].e.position - lineData.l[id[i]].s.position;
                UIDrawer.GetTypeInElement<Image>(lineData.l[id[i]].line).rectTransform.sizeDelta = new Vector2(10f, d.magnitude);
                lineData.l[id[i]].line.script.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Math.CalculateAngle(d)));

                //Debug.Log("Line ID Rendered: " + id[i]);
            }
    }
}