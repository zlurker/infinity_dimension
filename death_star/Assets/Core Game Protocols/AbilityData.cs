using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VariableAction {
    GET, SET
}

public class Variable {
    public RuntimeParameters field;
    public List<int[]> links; //Addressed to [ subclass, variable, get(0)/set(1) enum]
    
    public Variable() {
    }

    public Variable(RuntimeParameters f, int[][] ls) {
        field = f;
        links = new List<int[]>(ls);
    }
      
    public Variable(RuntimeParameters f) {
        field = f;
        links = new List<int[]>();
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

public class AbilityData {
    public EnhancedList<AbilityDataSubclass> subclasses;

    public AbilityData() {
        subclasses = new EnhancedList<AbilityDataSubclass>();
    }

    public AbilityData(AbilityDataSubclass[] elements) {
        subclasses = new EnhancedList<AbilityDataSubclass>(elements);
    }

    public AbilityDataSubclass[] RelinkSubclass() { //Amend this

        int[] pathId = new int[subclasses.l.Count];
        int[] emptyElements = subclasses.ReturnINS();
        AbilityDataSubclass[] activeInstances = new AbilityDataSubclass[subclasses.l.Count - emptyElements.Length];

        for(int i = 0; i < emptyElements.Length; i++)
            pathId[emptyElements[i]] = -1;

        int actualInt = 0;
        
        for (int i =0; i < pathId.Length; i++) 
            if(pathId[i] != -1) {
                pathId[i] = actualInt;
                activeInstances[actualInt] = subclasses.l[i];
                actualInt++;
            }

        for(int i = 0; i < activeInstances.Length; i++) 
            for (int j =0; j < activeInstances[i].var.Length; j++)
                for(int k = 0; k < activeInstances[i].var[j].links.Count; k++)
                    activeInstances[i].var[j].links[k][0] = pathId[activeInstances[i].var[j].links[k][0]];
                    
        return activeInstances;
    }
}
