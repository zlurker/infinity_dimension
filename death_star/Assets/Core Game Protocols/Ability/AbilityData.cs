using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VariableAction {
    GET, SET
}

/*public class RecursivePath {

    AbilityDataSubclass[] target;

    public RecursivePath(AbilityDataSubclass[] t) {
        target = t;
    }


    public int[][] RecursivePathing(int[] path) {

        List<int[]> paths = new List<int[]>();
        bool noLinks = true;

        for(int i = 0; i < target[path[path.Length - 1]].var.Length; i++)
            for(int j = 0; j < target[path[path.Length - 1]].var[i].links.Length; j++) {
                for(int k = 0; k < target[path[path.Length - 1]].var[i].links[j].Length; k++) {
                    List<int> cP = new List<int>(path);

                    cP.Add(target[path[path.Length - 1]].var[i].links[j][k][0]);

                    int[][] gatheredPaths = RecursivePathing(cP.ToArray());

                    for(int l = 0; l < gatheredPaths.Length; l++)
                        paths.Add(gatheredPaths[l]);
                }

                if(target[path[path.Length - 1]].var[i].links[0].Length > 0 ||
                    target[path[path.Length - 1]].var[i].links[1].Length > 0)
                    noLinks = false;
            }


        if(noLinks) {
            Debug.Log("P:" + path[path.Length - 1]);
            return new int[][] { path };
        }

        return paths.ToArray();
    }
}*/

public class Variable {
    //Variable details
    public RuntimeParameters field;

    //Addressed to [ get(0)/set(1) enum, subclass, variable]
    public int[][][] links;

    public Variable() {
    }

    public Variable(RuntimeParameters f, int[][][] ls) {
        field = f;
        links = ls;
    }

    public Variable(RuntimeParameters f) {
        field = f;
        links = new int[0][][];
    }
}

public class AbilityDataSubclass {
    public Variable[] var;
    public Type classType;

    public AbilityDataSubclass() {
    }

    public AbilityDataSubclass(Type t) {
        AbilityTreeNode[] interfaces = (Iterator.ReturnObject<AbilityTreeNode>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as AbilityTreeNode[];
        AbilityTreeNode selectedInterface = Iterator.ReturnObject(interfaces, t, (p) => {
            return p.GetType();
        });

        classType = t;

        RuntimeParameters[] fields = selectedInterface.GetRuntimeParameters();
        var = new Variable[fields.Length];

        for(int i = 0; i < var.Length; i++)
            var[i] = new Variable(fields[i]);

    }

    //Returns all root classes.
    public static int[] ReturnFirstClasses(AbilityDataSubclass[] target) {

        List<int> rootClasses = new List<int>();
        AutoPopulationList<bool> connected = new AutoPopulationList<bool>(target.Length);

        for(int i = 0; i < target.Length; i++)
            for(int j = 0; j < target[i].var.Length; j++)
                for(int k = 0; k < target[i].var[j].links.Length; k++)
                    for(int l = 0; l < target[i].var[j].links[k].Length; l++) {

                        Debug.Log(i + "\n" + target[i].var[j].links[k][l][0]);
                        connected.ModifyElementAt(target[i].var[j].links[k][l][0], true);
                    }

        for(int i = 0; i < connected.l.Count; i++)
            if(!connected.l[i])
                rootClasses.Add(i);

        return rootClasses.ToArray();
    }

    public static int[] ReturnNodeEndData(AbilityDataSubclass[] target) {
        int[] paths = new int[target.Length];

        for(int i = 0; i < target.Length; i++) {
            int cummulativeTotal = 0;

            for(int j = 0; j < target[i].var.Length; j++)
                for(int k = 0; k < target[i].var[j].links.Length; k++)
                    cummulativeTotal += target[i].var[j].links[k].Length;

            paths[i] = cummulativeTotal;
        }

        return paths;
    }

    public static int[][] ReturnGetEndNode(AbilityDataSubclass[] target, int[] startId) {

        List<int[]> compiledList = new List<int[]>();

        for (int i =0; i < startId.Length; i++) 
            for(int j = 0; j < target[startId[i]].var.Length; j++) {
                int[][] output = RunNestedVariables(target, new int[] { startId[i], j });

                for(int k = 0; k < output.Length; k++)
                    compiledList.Add(output[k]);
            }

        return compiledList.ToArray();      
    }

    public static int[][] RunNestedVariables(AbilityDataSubclass[] target, int[] id, int[] lastSet = null, int prevAction = 1) {
        List<int[]> sets = new List<int[]>();

        for(int k = 0; k < target[id[0]].var[id[1]].links.Length; k++)
            for(int l = 0; l < target[id[0]].var[id[1]].links[k].Length; l++) {

                if(prevAction == 1)
                    lastSet = new int[] { id[0], id[1] };

                Debug.LogFormat("currnode{0}/var{1}", id[0], id[1]);
                Debug.LogFormat("nextnode{0}/nextvar{1}", target[id[0]].var[id[1]].links[k][l][0], target[id[0]].var[id[1]].links[k][l][1]);

                int[][] setData = RunNestedVariables(target, target[id[0]].var[id[1]].links[k][l], lastSet, k);

                for(int a = 0; a < setData.Length; a++)
                    sets.Add(setData[a]);              
            }

        if(prevAction == 0)
            if(target[id[0]].var[id[1]].links[0].Length == 0)
                sets.Add(new int[] { id[0], id[1], lastSet[0], lastSet[1] });

        return sets.ToArray();
    }

    /*public static int[][] ReturnGetEndNode(AbilityDataSubclass[] target, int[] id, int[] lastSet = null, int prevAction = 1) {

         List<int[]> sets = new List<int[]>();

         for(int i = 0; i < id.Length; i++)
             for(int j = 0; j < target[id[i]].var.Length; j++) {
                 for(int k = 0; k < target[id[i]].var[j].links.Length; k++)
                     for(int l = 0; l < target[id[i]].var[j].links[k].Length; l++) {

                         if(prevAction == 1)
                             lastSet = new int[] { id[i],j };

                         int[][] setData = ReturnGetEndNode(target, new int[] { target[id[i]].var[j].links[k][l][0] }, lastSet, k);

                         for(int a = 0; a < setData.Length; a++)
                             sets.Add(setData[a]);
                     }

                 if(prevAction == 0)
                     if(target[id[i]].var[j].links[0].Length == 0 && target[id[i]].var[j].links[1].Length == 0)
                         sets.Add(new int[] { id[i], j, lastSet[0], lastSet[1] });
             }

         return sets.ToArray();
     }*/

    // Returns all possible ends for every node.
    /*public static int[][] ReturnNodeEndData(AbilityDataSubclass[] target, int[] root) {

        RecursivePath test = new RecursivePath(target);
        List<int[]> paths = new List<int[]>();

        for(int i = 0; i < root.Length; i++) {
            int[][] tp = test.RecursivePathing(new int[] { root[i] });

            for(int j = 0; j < tp.Length; j++)
                paths.Add(tp[j]);
        }

        return paths.ToArray();
    }*/


    /*public static int[][][][] ReturnGetterAndSetters(AbilityDataSubclass[] target) {
        int[][][][] compiled = new int[target.Length][][][];

        for(int i = 0; i < target.Length; i++) {
            compiled[i] = new int[target[i].var.Length][][];

            for(int k = 0; k < target[i].var.Length; k++) {
                compiled[i][k] = new int[2][];

                List<int>[] gs = new List<int>[2];
                gs[0] = new List<int>();
                gs[1] = new List<int>();

                for(int j = 0; j < target[i].var[k].links.Length; j++) 
                    gs[target[i].var[k].links[j][2]].Add(j);

                for(int l = 0; l < gs.Length; l++)
                    compiled[i][k][l] = gs[l].ToArray();


            }
        }

        return compiled;
    }*/
}

//Contains list of functions/data structures to assist with the UI side of ability data.
//Refer to notebook if unsure, Line & windows
public class UIAbilityData {
    public EnhancedList<AbilityDataSubclass> subclasses;
    public AutoPopulationList<EnhancedList<int[]>[]> linksEdit;
    public AutoPopulationList<List<int[]>> linkTunnelEnd;

    public float[][] loadedWindowsLocation;

    public UIAbilityData() {
        subclasses = new EnhancedList<AbilityDataSubclass>();
        linksEdit = new AutoPopulationList<EnhancedList<int[]>[]>();
        linkTunnelEnd = new AutoPopulationList<List<int[]>>();
    }

    public void ResetTunnelEnd(int id) {
        for(int i = 0; i < linkTunnelEnd.l[id].Count; i++)
            for(int j = 0; j < linkTunnelEnd.l[id].Count; j++) {
                int[] path = linkTunnelEnd.l[id][j];

                //Debug.LogFormat("Path : {0} , {1} , {2}", path[0], path[1], path[2]);
                linksEdit.l[path[0]][path[1]].Remove(path[2]);
            }

        linkTunnelEnd.ModifyElementAt(id, new List<int[]>());
    }

    public UIAbilityData(AbilityDataSubclass[] elements, float[][] windowsLocation) {
        loadedWindowsLocation = windowsLocation;

        subclasses = new EnhancedList<AbilityDataSubclass>(elements);
        linksEdit = new AutoPopulationList<EnhancedList<int[]>[]>();
        linkTunnelEnd = new AutoPopulationList<List<int[]>>();

        for(int i = 0; i < elements.Length; i++)
            linkTunnelEnd.ModifyElementAt(i, new List<int[]>());

        for(int i = 0; i < elements.Length; i++) {
            EnhancedList<int[]>[] cList = new EnhancedList<int[]>[elements[i].var.Length];

            for(int j = 0; j < elements[i].var.Length; j++) {
                EnhancedList<int[]> dynaLink = new EnhancedList<int[]>();//new EnhancedList<int[]>(elements[i].var[j].links);

                for(int k = 0; k < elements[i].var[j].links.Length; k++)
                    for(int l = 0; l < elements[i].var[j].links[k].Length; l++) {
                        int id = dynaLink.Add(new int[] { elements[i].var[j].links[k][l][0], elements[i].var[j].links[k][l][1], k });
                        CreateLinkShadow(elements[i].var[j].links[k][l][0], new int[] { i, j, id });
                    }


                cList[j] = dynaLink;
            }

            linksEdit.ModifyElementAt(i, cList);
        }
    }

    public void CreateLink(int[] prevPath, int[] currPath) {
        int dataId = linksEdit.l[prevPath[0]][prevPath[1]].Add(currPath);
        Debug.Log(linkTunnelEnd.l.Count);
        Debug.Log(currPath[0]);

        CreateLinkShadow(currPath[0], new int[] { prevPath[0], prevPath[1], dataId });
    }

    public void CreateLinkShadow(int classAffected, int[] pathRoot) {
        //linkTunnelEnd.l[currPath[0]].Add(new int[] { prevPath[0], prevPath[1], dataId });
        linkTunnelEnd.l[classAffected].Add(pathRoot);

    }

    public int Add(AbilityDataSubclass inst) {
        int instId = subclasses.Add(inst);
        CreateLinkSpaces(instId, inst.var.Length);

        return instId;
    }

    void CreateLinkSpaces(int id, int length) {
        EnhancedList<int[]>[] varLinks = new EnhancedList<int[]>[length];

        for(int i = 0; i < length; i++)
            varLinks[i] = new EnhancedList<int[]>();

        linksEdit.ModifyElementAt(id, varLinks);
        linkTunnelEnd.ModifyElementAt(id, new List<int[]>());
    }

    public AbilityDataSubclass[] RelinkSubclass() {

        int[] globalAddress = new int[subclasses.l.Count];
        int[] active = subclasses.ReturnActiveElementIndex();

        AbilityDataSubclass[] relinkedClasses = new AbilityDataSubclass[active.Length];

        //Floods global address with negative values first.
        for(int i = 0; i < globalAddress.Length; i++)
            globalAddress[i] = -1;

        //Fills global address up with valid classes.
        for(int i = 0; i < active.Length; i++)
            globalAddress[active[i]] = i;

        //Loop to relink classes.
        for(int i = 0; i < active.Length; i++) {
            for(int j = 0; j < subclasses.l[active[i]].var.Length; j++) {

                //Code to check if the link is a active link and not broken.
                List<int[]> linkCheck = new List<int[]>();

                //Gets current ability link values.
                int[][] linkValues = linksEdit.GetElementAt(active[i])[j].ReturnActiveElements();

                for(int k = 0; k < linkValues.Length; k++) {

                    //previousId
                    int oldId = linkValues[k][0];

                    if(globalAddress[oldId] > -1) {

                        //Replaces with new id if current mapped ID is active.
                        linkValues[k][0] = globalAddress[oldId];

                        //Adds active link into array.
                        linkCheck.Add(linkValues[k]);
                    }
                }

                subclasses.l[active[i]].var[j].links = new int[2][][];

                // Prepares get set list.
                List<int[]>[] gser = new List<int[]>[2];
                gser[0] = new List<int[]>();
                gser[1] = new List<int[]>();

                for(int l = 0; l < linkCheck.Count; l++)
                    gser[linkCheck[l][2]].Add(new int[] { linkCheck[l][0], linkCheck[l][1] });

                // Sets array with updated link values.
                for(int l = 0; l < 2; l++) {
                    subclasses.l[active[i]].var[j].links[l] = new int[gser[l].Count][];

                    for(int o = 0; o < gser[l].Count; o++)
                        subclasses.l[active[i]].var[j].links[l][o] = gser[l][o];
                }
            }

            relinkedClasses[i] = subclasses.l[active[i]];
        }

        return relinkedClasses;
    }
}
