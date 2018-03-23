using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupElement : Iterator {

    public List<PoolElement> gE; //groupElements
    public Transform gP; //groupParent

    public GroupElement(string name) {
        n = name;
        gE = new List<PoolElement>();
    }

    public GroupElement(string name, PoolElement[] items) {
        n = name;
        gE = new List<PoolElement>();
        AddItem(items);
    }

    public GroupElement(string name, PoolElement item) {
        n = name;
        gE = new List<PoolElement>();
        AddItem(item);
    }

    public void ResetGroupElements() {
        gE = new List<PoolElement>();
    }

    public void AddItem(PoolElement item) {
        gE.Add(item);
    }

    public void AddItem(PoolElement[] items) {
        for (int i = 0; i < items.Length; i++)
            gE.Add(items[i]);
    }
}

public enum Patterns {
    VECTOR_PATTERN, GROUP_PATTERN
}

public enum GroupArgs {
    ADD_PARAMETER_OBJECTS, REMOVE_PARAMETER_OBJECTS, REMOVE_ALL_CURRENT_OBJECTS, PARENT_PARAMETER_OBJECTS, PARENT_ALL_CURRENT_OBJECTS, REMOVE_GROUP, GET_GROUP
}

public class PatternControl : MonoBehaviour,ISingleton {

    List<GroupElement> g; //groups 
    public static PatternControl i;

    public void RunOnCreated() {
        g = new List<GroupElement>();
        i = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RunOnStart() {

    }

    public object ReturnInstance() {
        return this;
    }

    public object[] Pattern_Args(PoolElement[] objects, object[][] arg_values) {

        List<object> returnItems = new List<object>();

        for (int i = 0; i < arg_values.Length; i++)

            switch ((Patterns)arg_values[i][0]) {

                case Patterns.VECTOR_PATTERN:
                    Vector3 s_P = (Vector3)arg_values[i][1];
                    Vector3 v_P = (Vector3)arg_values[i][2];

                    for (int j = 0; j < objects.Length; j++)
                        objects[j].o[0].s.transform.position = s_P + (v_P * j);
                    break;

                case Patterns.GROUP_PATTERN:
                    string gN = (string)arg_values[i][1];
                    int index = Iterator.ReturnKey(g.ToArray(), gN);

                    if (index == -1) {
                        g.Add(new GroupElement(gN));
                        index = g.Count - 1;
                    }


                    for (int j = 2; j < arg_values.Length; j++)

                        switch ((GroupArgs)arg_values[i][j]) {

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
                                    if (objects[0].o[0].s.transform.parent != null)
                                        g[index].gP.transform.parent = objects[0].o[0].s.transform.parent;

                                for (int l = 0; l < objects.Length; l++)
                                    objects[l].o[0].s.transform.parent = g[index].gP;
                                break;

                            case GroupArgs.PARENT_ALL_CURRENT_OBJECTS:
                                if (!g[index].gP) //Creates a GroupParent
                                    g[index].gP = new GameObject(gN).transform;

                                //Makes it a child to the first objects parent, if any
                                if (g[index].gE.Count > 0)
                                    if (g[index].gE[0].o[0].s.transform.parent != null)
                                        g[index].gP.transform.parent = g[index].gE[0].o[0].s.transform.parent;

                                for (int l = 0; l < g[index].gE.Count; l++)
                                    g[index].gE[l].o[0].s.transform.parent = g[index].gP;
                                break;

                            case GroupArgs.REMOVE_GROUP:
                                g.Remove(g[index]);
                                break;

                            case GroupArgs.GET_GROUP:
                                returnItems.Add(g[index]);
                                break;
                        }

                    break;
            }

        return returnItems.ToArray();
    }
}
