using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Variable {
    //Variable details
    public RuntimeParameters field;
    //
    public bool varUpdated;
    //Addressed to [subclass, variable]
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

public class AbilityBooleanData {
    public bool[][] varsBlocked;

    public bool[][] ReturnNewCopy() {
        bool[][] clone = new bool[varsBlocked.Length][];

        for(int i = 0; i < clone.Length; i++) {
            clone[i] = new bool[varsBlocked[i].Length];

            for(int j = 0; j < clone[i].Length; j++)
                clone[i][j] = varsBlocked[i][j];
        }

        return clone;

    }
}

public class AbilityDataSubclass {
    public Variable[] var;
    public Type classType;

    public AbilityDataSubclass() {
    }

    public AbilityDataSubclass(Type t) {

        classType = t;

        RuntimeParameters[] fields = LoadedData.ReturnNodeVariables(t);
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

    public static int IterateLinks(AbilityDataSubclass[] target, int[] nextId, Dictionary<int, int> mappedValues, AbilityBooleanData bData) {

        int total = 0;

        for(int i = 0; i < nextId.Length; i++) {

            int totalLinks = 0;

            for(int j = 0; j < target[nextId[i]].var.Length; j++) {

                int[] followingIds = new int[target[nextId[i]].var[j].links.Length];
                totalLinks += followingIds.Length;

                for(int k = 0; k < target[nextId[i]].var[j].links.Length; k++) {
                    int[] currLink = target[nextId[i]].var[j].links[k];
                    followingIds[k] = currLink[0];

                    if(target[nextId[i]].var[j].field.t == target[currLink[0]].var[currLink[1]].field.t)
                        bData.varsBlocked[currLink[0]][currLink[1]] = true;
                    //bData.blockedVars++;
                }

                int nextNodeValues = 0;

                if(followingIds.Length > 0)
                    nextNodeValues = IterateLinks(target, followingIds, mappedValues, bData);

                total += nextNodeValues;
            }

            if(target[nextId[i]].classType == typeof(ThreadSplitter)) {
                if(!mappedValues.ContainsKey(nextId[i]))
                    mappedValues.Add(nextId[i], total);

                // Because these threads will be combined in Threadsplitter,
                // We must combine them as 1 so if it is nested it would be counted correctly.
                Debug.Log("Previous Value: " + total);
                total = 1;
                Debug.Log("Current Value: " + total);
            }

            if(totalLinks == 0)
                total = 1;
        }

        return total;
    }

    public static string[] GetImageDependencies(AbilityDataSubclass[] target) {
        HashSet<string> imageDependencies = new HashSet<string>();

        for(int i = 0; i < target.Length; i++)
            for(int j = 0; j < target[i].var.Length; j++)
                if(LoadedData.GetVariableType(target[i].classType, j, VariableTypes.IMAGE_DEPENDENCY)) {

                    RuntimeParameters<string> imagePath = target[i].var[j].field as RuntimeParameters<string>;

                    if(!imageDependencies.Contains(imagePath.v))
                        imageDependencies.Add(imagePath.v);
                }

        string[] sArray = new string[imageDependencies.Count];
        int index = 0;

        foreach(string path in imageDependencies) {
            sArray[index] = path;
            index++;
        }

        return sArray;
        //return imageDependencies.ToArray();
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
            //Debug.LogFormat("Start node id changed from {0} to {1}", linkAddresses.l[activeConnections[i]][0], globalAddress[linkAddresses.l[activeConnections[i]][0]]);
            //Debug.LogFormat("End node id changed from {0} to {1}", linkAddresses.l[activeConnections[i]][2], globalAddress[linkAddresses.l[activeConnections[i]][2]]);
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