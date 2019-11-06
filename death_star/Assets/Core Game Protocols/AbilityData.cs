﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VariableAction {
    GET, SET
}

public class Variable {
    //Variable details
    public RuntimeParameters field;

    //Addressed to [ subclass, variable, get(0)/set(1) enum]
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
    public float[] wL;

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

        wL = new float[2];
    }

    //Returns all root classes.
    public static int[] ReturnFirstClasses(AbilityDataSubclass[] target) {

        List<int> rootClasses = new List<int>();
        AutoPopulationList<bool> connected = new AutoPopulationList<bool>(target.Length);

        for(int i = 0; i < target.Length; i++)
            for(int j = 0; j < target[i].var.Length; j++)
                for(int k = 0; k < target[i].var[j].links.Length; k++)
                    for(int l = 0; l < target[i].var[j].links[k].Length; l++) 
                        connected.ModifyElementAt(target[i].var[j].links[k][l][0], true);
                    

        for(int i = 0; i < connected.l.Count; i++)
            if(!connected.l[i])
                rootClasses.Add(i);

        return rootClasses.ToArray();
    }

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

    public UIAbilityData() {
        subclasses = new EnhancedList<AbilityDataSubclass>();
        linksEdit = new AutoPopulationList<EnhancedList<int[]>[]>();
    }

    public UIAbilityData(AbilityDataSubclass[] elements) {
        subclasses = new EnhancedList<AbilityDataSubclass>(elements);
        linksEdit = new AutoPopulationList<EnhancedList<int[]>[]>();

        for(int i = 0; i < elements.Length; i++) {
            EnhancedList<int[]>[] cList = new EnhancedList<int[]>[elements[i].var.Length];

            for(int j = 0; j < elements[i].var.Length; j++) {
                EnhancedList<int[]> dynaLink = new EnhancedList<int[]>();//new EnhancedList<int[]>(elements[i].var[j].links);

                for(int k = 0; k < elements[i].var[j].links.Length; k++)
                    for(int l = 0; l < elements[i].var[j].links[k].Length; l++)
                        dynaLink.Add(new int[] { elements[i].var[j].links[k][l][0], elements[i].var[j].links[k][l][1], k });

                cList[j] = dynaLink;
            }

            linksEdit.ModifyElementAt(i, cList);
        }
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
                for(int l =0; l < 2; l++) {
                    subclasses.l[active[i]].var[j].links[l] = new int[gser[l].Count][];

                    for (int o=0; o < gser[l].Count; o++) 
                        subclasses.l[active[i]].var[j].links[l][o] = gser[l][o];                   
                }
            }

            relinkedClasses[i] = subclasses.l[active[i]];
        }

        return relinkedClasses;
    }
}
