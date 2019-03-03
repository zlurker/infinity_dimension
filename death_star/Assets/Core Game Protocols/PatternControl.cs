using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAddOn
{
    void Add(object target);
    void LinkedGroup(Group linkedGroup);
}

public class AddOnData : Iterator
{
    public IAddOn i;

    public AddOnData(IAddOn interf)
    {
        i = interf;
        t = interf.GetType();
    }
}

/*public class GroupElement : Iterator
{

    public List<ScriptableObject> gE; //groupElements
    public Transform gP; //groupParent

    public GroupElement(string name)
    {
        n = name;
        gE = new List<ScriptableObject>();
    }

    public GroupElement(string name, ScriptableObject[] items)
    {
        n = name;
        gE = new List<ScriptableObject>();
        AddItem(items);
    }

    public GroupElement(string name, ScriptableObject item)
    {
        n = name;
        gE = new List<ScriptableObject>();
        AddItem(item);
    }

    public void ResetGroupElements()
    {
        for (int i = 0; i < gE.Count; i++)
            UIDrawer.i.Remove(gE[i]);

        gE = new List<ScriptableObject>();
    }

    public void AddItem(ScriptableObject item)
    {
        gE.Add(item);
    }

    public void AddItem(ScriptableObject[] items)
    {
        for (int i = 0; i < items.Length; i++)
            gE.Add(items[i]);
    }
}*/

public class Group : Iterator
{
    public List<ScriptableObject> gE; //groupElements
    public List<Transform> transforms;
    public List<Group> g;//groups
    public List<AddOnData> aO;

    public Transform gP; //groupParent

    public Group()
    {
        ResetGroup();
        //gP = new GameObject("Group").transform;
    }

    public void ResetGroup()
    {       
        gE = new List<ScriptableObject>();
        transforms = new List<Transform>();
        g = new List<Group>();
        aO = new List<AddOnData>();
        n = "";
        gP = null;
        //ChangeRoot(Singleton.GetSingleton<Spawner>());
    }

    public void AddItem(ScriptableObject scriptableObject)
    {
        gE.Add(scriptableObject);
        Root(scriptableObject.transform);
        //scriptableObject.transform.parent = gP.transform;
    }

    public void AddItem(IAddOn addOn)
    {
        aO.Add(new AddOnData(addOn));
    }

    public void AddItem(Group group)
    {
        g.Add(group);
        Root(group.gP);
        //group.gP.transform.parent = gP.transform;
    }

    public void AddItem(Transform transform) {
        transforms.Add(transform);
        Root(transform);
    }

    void Root(Transform target)
    {
        if (!gP)
            gP = target;
        else
            target.parent = gP;
    }

    /*public void ChangeRoot(Spawner inst)
    {
        gP = inst.CustomiseBaseObject();

        for (int i = 0; i <g.Count; i++)
            g[i].gP.transform.parent = gP.transform;

        for (int i = 0; i < gE.Count; i++)
            gE[i].transform.parent = gP.transform;      
    }

    public void ChangeRoot(ScriptableObject inst)
    {
        gP = inst;

        for (int i = 0; i < g.Count; i++)
            g[i].gP.transform.parent = gP.transform;

        for (int i = 0; i < gE.Count; i++)
            gE[i].transform.parent = gP.transform;
    }*/
}

public enum Patterns
{
    VECTOR_PATTERN, GROUP_PATTERN
}

public enum GroupMode
{
    NONE,UI
}

public enum GroupArgs
{
    NONE, ADD_PARAMETER_OBJECTS, REMOVE_PARAMETER_OBJECTS, REMOVE_ALL_CURRENT_OBJECTS, REMOVE_GROUP
}

public class PatternControl : MonoBehaviour, ISingleton
{
    //List<GroupElement> g; //groups 
    List<Group> groups;
    Pool<Group> groupSpawner;

    public void RunOnCreated()
    {
        //g = new List<GroupElement>();
        groups = new List<Group>();
        groupSpawner = new Pool<Group>(CreateNewGroup, null);
    }

    public void RunOnStart()
    {

    }

    public object CreateNewGroup(object p)
    {
        return new Group();
    }

    public Group GetGroup(string name)
    {
        Group instance = Iterator.ReturnObject<Group>(groups.ToArray(), name);

        if(instance == null) {
            Singleton.GetSingleton<PatternControl>().ModifyGroup(name, new object[0]);
            instance = Iterator.ReturnObject<Group>(groups.ToArray(), name);
        }

        return Iterator.ReturnObject<Group>(groups.ToArray(), name);
    }

    public void ModifyGroup(string groupName, object[] objects, GroupArgs commands = GroupArgs.ADD_PARAMETER_OBJECTS)
    {
        Group target = Iterator.ReturnObject<Group>(groups.ToArray(), groupName);

        if (target == null)
        {
            target = groupSpawner.Retrieve();
            target.n = groupName;

            groups.Add(target);
        }

        switch (commands)
        {
            case GroupArgs.ADD_PARAMETER_OBJECTS:
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] is Group)
                    {
                        target.AddItem(objects[i] as Group);

                        for (int j = 0; j < target.aO.Count; j++)
                            target.aO[j].i.Add(objects[i] as Group);
                    }

                    if (objects[i] is ScriptableObject)
                    {
                        ScriptableObject sO = objects[i] as ScriptableObject;
                        target.AddItem(sO);

                        for (int j = 0; j < sO.scripts.Length; j++) //checks which is IAddOn
                            if (sO.scripts[j] is IAddOn)
                            {
                                IAddOn iAOinst = sO.scripts[j] as IAddOn;
                                target.AddItem(iAOinst);
                                iAOinst.LinkedGroup(target);
                            }

                        for (int j = 0; j < target.aO.Count; j++)
                            target.aO[j].i.Add(objects[i] as ScriptableObject);
                    }

                    if (objects[i] is Transform)
                        target.AddItem(objects[i] as Transform);
                }
                break;
        }
    }

    /*public object[] Pattern_Args(ScriptableObject[] objects, object[][] arg_values)
    {

        List<object> returnItems = new List<object>();

        for (int i = 0; i < arg_values.Length; i++)

            switch ((Patterns)arg_values[i][0])
            {

                case Patterns.VECTOR_PATTERN:
                    Vector3 s_P = (Vector3)arg_values[i][1];
                    Vector3 v_P = (Vector3)arg_values[i][2];

                    //Debug.Log(objects.Length);
                    for (int j = 0; j < objects.Length; j++)
                    {
                        //Debug.Log(objects[j]);
                        objects[j].transform.position = s_P + (v_P * j);
                    }
                    break;

                case Patterns.GROUP_PATTERN:
                    string gN = (string)arg_values[i][1];
                    int index = Iterator.ReturnKey(g.ToArray(), gN);

                    if (index == -1)
                    {
                        g.Add(new GroupElement(gN));
                        index = g.Count - 1;
                    }
                    
                    for (int j = 2; j < arg_values[i].Length; j++)

                        switch ((GroupArgs)arg_values[i][j])
                        {
                            case GroupArgs.ADD_PARAMETER_OBJECTS:
                                g[index].AddItem(objects);
                                break;

                            case GroupArgs.REMOVE_PARAMETER_OBJECTS: //Removal args
                                for (int l = 0; l < objects.Length; l++)
                                    g[index].gE.Remove(objects[l]);
                                break;

                            case GroupArgs.REMOVE_ALL_CURRENT_OBJECTS:
                                g[index].ResetGroupElements();
                                break;

                            case GroupArgs.PARENT_PARAMETER_OBJECTS: //Parent args
                                if (!g[index].gP)
                                    //Creates a GroupParent
                                    g[index].gP = new GameObject(gN).transform;

                                //Makes it a child to the first objects parent, if any
                                if (objects.Length > 0)
                                    if (objects[0].transform.parent != null)
                                        g[index].gP.transform.parent = objects[0].transform.parent;

                                for (int l = 0; l < objects.Length; l++)
                                    objects[l].transform.SetParent(g[index].gP);
                                break;

                            case GroupArgs.PARENT_ALL_CURRENT_OBJECTS:
                                if (!g[index].gP) //Creates a GroupParent
                                    g[index].gP = new GameObject(gN).transform;

                                //Debug.LogWarning("Parent is " + g[index].gP);
                                //Makes it a child to the first objects parent, if any
                                if (g[index].gE.Count > 0)
                                    if (g[index].gE[0].transform.parent != null)
                                        g[index].gP.transform.parent = g[index].gE[0].transform.parent;

                                for (int l = 0; l < g[index].gE.Count; l++)
                                    g[index].gE[l].transform.SetParent(g[index].gP);
                                break;

                            case GroupArgs.REMOVE_GROUP:
                                g[index].ResetGroupElements();
                                g.Remove(g[index]);
                                break;

                            case GroupArgs.GET_GROUP:
                                returnItems.Add(g[index]);
                                break;
                        }

                    break;
            }

        return returnItems.ToArray();
    }*/
}
