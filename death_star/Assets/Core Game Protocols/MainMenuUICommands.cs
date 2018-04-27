using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class UnsavedEdit
{
    public string[] id;
    public string v;

    public UnsavedEdit(string[] ids, string value)
    {
        id = ids;
        v = value;
    }
}

//Delegates to create UI here?
//Delegates are here? Hmm...
public class MainMenuUICommands : MonoBehaviour
{
    List<UnsavedEdit> edits;
    public Font font;
    Text instance;
    string[] layers;

    DelegateIterator[] inIt; //InputFiend initialisers

    public void InitialiseFieldForUse(MonoBehaviour[] target, string[] id) //Makes fields here usable by this script. Auto saves it to the data.
    {
        for (int k = 0; k < target.Length; k++)
            if (target[k] is Selectable)
                Iterator.ReturnObject<DelegateIterator>(inIt, target[k].GetType().Name).d.Invoke(new object[] { target[k], id });
    }

    void InitialiseInIt() //Contains MethodRunners to help prepare field for use.
    {
        inIt = new DelegateIterator[]
        {
            new DelegateIterator("InputField",new DH((p) => {
                InputField field = p[0] as InputField;
                string[] id = p[1] as string[];

                
                field.onEndEdit.AddListener((s) => {
                    edits.Add(new UnsavedEdit(id,s));
                    Debug.LogFormat("ID: {0}, String: {1}",id,s);
                });

            })),
            new DelegateIterator("Dropdown",new DH((p) => {
                Dropdown field = p[0] as Dropdown;
                string[] id = p[1] as string[];

                field.onValueChanged.AddListener((s) => { Debug.Log(s); });
            }))
        };
    }

    void Start()
    {
        edits = new List<UnsavedEdit>();
        InitialiseInIt();

        instance = Spawner.GetCType<Text>(UIDrawer.i.CreateScriptedObject(new MonoBehaviour[] { UIDrawer.i.CreateComponent<Text>() }, new DelegateInfo[] { new DelegateInfo("UIPosition", new object[] { new Vector3(0.5f, 0.9f) }) }));

        UIDrawer.i.Remove(UIDrawer.i.CreateScriptedObject(new MonoBehaviour[] { UIDrawer.i.CreateComponent<Text>() }));
        ScriptableObject[] buttons = new ScriptableObject[LoadedData.gIPEI.Length];

        for (int i = 0; i < LoadedData.uL.Length; i++)
        {
            ScriptableObject inst = Iterator.ReturnObject<ObjectCreationTemplate>(UIDrawer.i.tplts, "ButtonMaker").d();

            string name = LoadedData.uL[i].GetType().Name;
            Debug.Log(i + " " + name + LoadedData.uL.Length);
            Spawner.GetCType<Button>(inst).onClick.AddListener(new DH(ChangeEditableUITemplate, new object[] { i }).Invoke);
            buttons[i] = inst;
        }

        PatternControl.i.Pattern_Args(buttons,
            new object[][] {
            new object[] { Patterns.GROUP_PATTERN, "Secondary", GroupArgs.ADD_PARAMETER_OBJECTS,GroupArgs.PARENT_PARAMETER_OBJECTS},
            new object[] { Patterns.VECTOR_PATTERN, UIDrawer.UINormalisedPosition(new Vector3(0.1f, 0.85f)), new Vector3(-10f,-25f) }
        });

        PlayerInput.i.AddNewInput(KeyCode.S, new DH(Save), 0); 
    }

    public void Save(object[] p)
    {
        for (int i = 0; i < edits.Count; i++)
            XMLHelper.i.SetNode(edits[i].id, edits[i].v);
        

        edits = new List<UnsavedEdit>();
    }

    public void ChangeEditableUITemplate(object[] p)
    {
        int arg0 = (int)p[0];
        instance.text = LoadedData.uL[arg0].GetType().Name;
        MethodInfo returnedMethod = LoadedData.uL[arg0].GetMainMethod();
        ParameterInfo[] fields = new ParameterInfo[0];

        if (returnedMethod != null)
            fields = returnedMethod.GetParameters();

        ScriptableObject[] fieldNames = new ScriptableObject[fields.Length];
        ScriptableObject[] field = new ScriptableObject[fields.Length];

        UIDrawer.i.ModifyLayer(0);

        for (int i = 0; i < fields.Length; i++)
        {
            ScriptableObject tinst = UIDrawer.i.CreateScriptedObject(new MonoBehaviour[] { UIDrawer.i.CreateComponent<Text>() });
            Spawner.GetCType<Text>(tinst).text = fields[i].Name;
            fieldNames[i] = tinst;

            ScriptableObject iinst = Iterator.ReturnObject<ObjectCreationTemplate>(UIDrawer.i.tplts, "InputFieldMaker").d();
            field[i] = iinst;
            InitialiseFieldForUse(iinst.scripts,new string[] { LoadedData.uL[arg0].GetType().Name, fields[i].Name });
        }

        PatternControl.i.Pattern_Args(fieldNames,
            new object[][] {
            new object[] { Patterns.GROUP_PATTERN, "FieldNames", GroupArgs.ADD_PARAMETER_OBJECTS,GroupArgs.PARENT_ALL_CURRENT_OBJECTS },
            new object[] { Patterns.VECTOR_PATTERN, UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.85f)), new Vector3(0,-25f) }
        });

        PatternControl.i.Pattern_Args(field,
            new object[][] {
            new object[] { Patterns.GROUP_PATTERN, "FieldNames", GroupArgs.ADD_PARAMETER_OBJECTS,GroupArgs.PARENT_ALL_CURRENT_OBJECTS },
            new object[] { Patterns.VECTOR_PATTERN, UIDrawer.UINormalisedPosition(new Vector3(0.6f, 0.85f)), new Vector3(0,-25f) }
        });

        UIDrawer.i.ModifyLayer(0,"FieldNames");
    }

    public void CreateIMenu<T>(string groupName)
    {

    }
}
