using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VariableAction {
    GET, SET
}

public class Variable {
    public RuntimeParameters field;
    public int[][] links; //Addressed to [ subclass, variable, get(0)/set(1) enum]

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
    public float[] wL;

    public AbilityDataSubclass() {

    }

    public AbilityDataSubclass(Type t) {
        IPlayerEditable[] interfaces = (Iterator.ReturnObject<IPlayerEditable>(LoadedData.lI) as InterfaceLoader).ReturnLoadedInterfaces() as IPlayerEditable[];
        IPlayerEditable selectedInterface = Iterator.ReturnObject(interfaces, t, (p) => {
            return p.GetType();
        });

        classType = t;

        RuntimeParameters[] fields = selectedInterface.GetRuntimeParameters();
        var = new Variable[fields.Length];

        for(int i = 0; i < var.Length; i++)
            var[i] = new Variable(fields[i]);

        wL = new float[2];
    }
}

public class UIAbilityData { //Refer to notebook if unsure, Line & windows
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
                EnhancedList<int[]> dynaLink = new EnhancedList<int[]>(elements[i].var[j].links);
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

    public void SaveLinkEdits() {

        int[] aE = subclasses.ReturnActiveElementIndex();

        for(int i = 0; i < aE.Length; i++)
            for(int j = 0; j < subclasses.l[aE[i]].var.Length; j++) {
                subclasses.l[aE[i]].var[j].links = linksEdit.GetElementAt(aE[i])[j].ReturnActiveElements();
            }
    }

    public AbilityDataSubclass[] RelinkSubclass() { //Amend this

        SaveLinkEdits();

        int[] pathId = new int[subclasses.l.Count];
        int[] emptyElements = subclasses.ReturnINS();
        AbilityDataSubclass[] activeInstances = new AbilityDataSubclass[subclasses.l.Count - emptyElements.Length];

        for(int i = 0; i < emptyElements.Length; i++)
            pathId[emptyElements[i]] = -1;

        int actualInt = 0;

        for(int i = 0; i < pathId.Length; i++)
            if(pathId[i] != -1) {
                pathId[i] = actualInt;
                activeInstances[actualInt] = subclasses.l[i];
                actualInt++;
            }

        for(int i = 0; i < activeInstances.Length; i++)
            for(int j = 0; j < activeInstances[i].var.Length; j++)
                for(int k = 0; k < activeInstances[i].var[j].links.Length; k++)
                    activeInstances[i].var[j].links[k][0] = pathId[activeInstances[i].var[j].links[k][0]];

        return activeInstances;
    }
}
