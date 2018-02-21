using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupElement : BaseIterator {

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

public class PatternControl : MonoBehaviour {

    List<GroupElement> g; //groups 
    public static PatternControl i;

    void Awake() {
        g = new List<GroupElement>();
        i = this;
        DontDestroyOnLoad(gameObject);
    }

    public object[] Pattern_Args(PoolElement[] objects, object[][] arg_values) {

        List<object> returnItems = new List<object>();

        for (int i = 0; i < arg_values.Length; i++)

            switch (arg_values[i][0] as string) {

                case "VECTOR_PATTERN":
                    Vector3 s_P = (Vector3)arg_values[i][1];
                    Vector3 v_P = (Vector3)arg_values[i][2];

                    for (int j = 0; j < objects.Length; j++)
                        objects[j].o.transform.position = s_P + (v_P * j);
                    break;

                case "GROUP_PATTERN":
                    string gN = (string)arg_values[i][1];
                    string oP = (string)arg_values[i][2];
                    int index = BaseIteratorFunctions.IterateKey(g.ToArray(), gN);

                    if (index == -1) {
                        g.Add(new GroupElement(gN));
                        index = g.Count - 1;
                    }

                    string[] oP_Augs = oP.Split(',');

                    for (int j = 0; j < oP_Augs.Length; j++)

                        switch (oP_Augs[j]) {

                            case "ADD_PARAMETER_OBJECTS":
                                g[index].AddItem(objects);
                                break;

                            case "REMOVE_PARAMETER_OBJECTS": //Removal args
                                for (int l = 0; l < objects.Length; l++)
                                    g[index].gE.Remove(objects[l]);
                                break;

                            case "REMOVE_ALL_CURRENT_OBJECTS":
                                g[index].ResetGroupElements();
                                break;

                            case "PARENT_PARAMETER_OBJECTS": //Parent args
                                if (!g[index].gP)
                                    //Creates a GroupParent
                                    g[index].gP = Instantiate(new GameObject("parent")).transform;

                                //Makes it a child to the first objects parent, if any
                                if (objects.Length > 0)
                                    if (objects[0].o.transform.parent != null)
                                        g[index].gP.transform.parent = objects[0].o.transform.parent;

                                for (int l = 0; l < objects.Length; l++)
                                    objects[l].o.transform.parent = g[index].gP;
                                break;

                            case "PARENT_ALL_CURRENT_OBJECTS":
                                if (!g[index].gP) //Creates a GroupParent
                                    g[index].gP = Instantiate(new GameObject("parent")).transform;

                                //Makes it a child to the first objects parent, if any
                                if (g[index].gE.Count > 0)
                                    if (g[index].gE[0].o.transform.parent != null)
                                        g[index].gP.transform.parent = g[index].gE[0].o.transform.parent;

                                for (int l = 0; l < g[index].gE.Count; l++)
                                    g[index].gE[l].o.transform.parent = g[index].gP;
                                break;

                            case "REMOVE_GROUP":
                                g.Remove(g[index]);
                                break;

                            case "GET_GROUP":
                                returnItems.Add(g[index]);
                                break;
                        }

                    break;
            }

        return returnItems.ToArray();
    }
}
