using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variable {
    //Variable details
    public RuntimeParameters field;

    //Addressed to [ get(0)/set(1) enum, subclass, variable]
    public int[][] links;

    public Variable() {
    }

    public Variable(RuntimeParameters f, int[][] ls) {
        field = f;
        links = ls;
    }

    public Variable(RuntimeParameters f) {
        field = f;
        links = new int[0][];
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
                for(int k = 0; k < target[i].var[j].links.Length; k++) {

                    Debug.Log(i + "\n" + target[i].var[j].links[k][0]);
                    connected.ModifyElementAt(target[i].var[j].links[k][0], true);
                }

        for(int i = 0; i < connected.l.Count; i++)
            if(!connected.l[i])
                rootClasses.Add(i);

        return rootClasses.ToArray();
    }

    public static int[] ReturnNodeBranchData(AbilityDataSubclass[] target) {
        int[] bData = new int[target.Length];

        for(int i = 0; i < target.Length; i++)
            for(int j = 0; j < target[i].var.Length; j++)
                bData[i] += target[i].var[j].links.Length;


        return bData;
    }

    public static int CalculateSpecialisedNodeThreads(AbilityDataSubclass[] target, int[] nextId, Dictionary<int, int> mappedValues) {

        int total = 0;

        for(int i = 0; i < nextId.Length; i++) {

            int totalLinks = 0;

            for(int j = 0; j < target[nextId[i]].var.Length; j++) {
                int[] followingIds = new int[target[nextId[i]].var[j].links.Length];
                totalLinks += followingIds.Length;

                for(int k = 0; k < target[nextId[i]].var[j].links.Length; k++)
                    followingIds[k] = target[nextId[i]].var[j].links[k][0];

                int nextNodeValues = 0;

                if(followingIds.Length > 0) {
                    nextNodeValues = CalculateSpecialisedNodeThreads(target, followingIds, mappedValues);
                    total += nextNodeValues;
                }

                if(target[nextId[i]].classType == typeof(ThreadSplitter)) {
                    if(!mappedValues.ContainsKey(nextId[i])) {
                        mappedValues.Add(nextId[i], nextNodeValues);
                    }
                }
            }

            if(totalLinks == 0) {
                total += 1;
            }
        }

        return total;
    }
}

//Contains list of functions/data structures to assist with the UI side of ability data.
//Refer to notebook if unsure, Line & windows
public class UIAbilityData {
    public EnhancedList<AbilityDataSubclass> subclasses;
    public EnhancedList<int[]> linkAddresses;

    public float[][] loadedWindowsLocation;

    public UIAbilityData() {
        subclasses = new EnhancedList<AbilityDataSubclass>();
        linkAddresses = new EnhancedList<int[]>();
    }


    public UIAbilityData(AbilityDataSubclass[] elements, float[][] windowsLocation) {
        loadedWindowsLocation = windowsLocation;

        subclasses = new EnhancedList<AbilityDataSubclass>(elements);
        linkAddresses = new EnhancedList<int[]>();

        for(int i = 0; i < elements.Length; i++)
            for(int j = 0; j < elements[i].var.Length; j++) {
                for(int k = 0; k < elements[i].var[j].links.Length; k++) {
                    int[] link = elements[i].var[j].links[k];
                    linkAddresses.Add(new int[] { i, j, link[0], link[1] });
                }
            }
    }

    public AbilityDataSubclass[] RelinkSubclass() {

        int[] globalAddress = new int[subclasses.l.Count];
        int[] active = subclasses.ReturnActiveElementIndex();
        int[] activeConnections = linkAddresses.ReturnActiveElementIndex();

        AbilityDataSubclass[] relinkedClasses = new AbilityDataSubclass[active.Length];

        //Floods global address with negative values first.
        for(int i = 0; i < globalAddress.Length; i++)
            globalAddress[i] = -1;

        //Fills global address up with valid classes.
        for(int i = 0; i < active.Length; i++)
            globalAddress[active[i]] = i;

        List<int[]>[][] vLinks = new List<int[]>[active.Length][];

        for(int i = 0; i < active.Length; i++) {
            relinkedClasses[i] = subclasses.l[active[i]];
            int varLength = relinkedClasses[i].var.Length;
            vLinks[i] = new List<int[]>[varLength];

            for(int j = 0; j < varLength; j++)
                vLinks[i][j] = new List<int[]>();
        }

        // Replaces the values correctly.
        for(int i = 0; i < activeConnections.Length; i++) {
            Debug.LogFormat("Start node id changed from {0} to {1}", linkAddresses.l[activeConnections[i]][0], globalAddress[linkAddresses.l[activeConnections[i]][0]]);
            Debug.LogFormat("End node id changed from {0} to {1}", linkAddresses.l[activeConnections[i]][2], globalAddress[linkAddresses.l[activeConnections[i]][2]]);
            int modS = globalAddress[linkAddresses.l[activeConnections[i]][0]];
            int modR = globalAddress[linkAddresses.l[activeConnections[i]][2]];

            int[] linkTarget = new int[] { modR, linkAddresses.l[activeConnections[i]][3] };

            vLinks[modS][linkAddresses.l[activeConnections[i]][1]].Add(linkTarget);
        }

        for(int i = 0; i < vLinks.Length; i++)
            for(int j = 0; j < vLinks[i].Length; j++)
                relinkedClasses[i].var[j].links = vLinks[i][j].ToArray();

        return relinkedClasses;
    }
}