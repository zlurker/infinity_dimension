using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPopulationList<T> {

    public List<T> l;

    public AutoPopulationList() {
        l = new List<T>();
    }

    public void ModifyElementAt(int index, T element) {
        Resize(index);

        l[index] = element;
    }

    public T GetElementAt(int index) {
        Resize(index);
        return l[index];

    }

    public void Resize(int index) {
        int diff = index - l.Count;

        for(int i = 0; i <= diff; i++) 
            l.Add(default(T));    
    }
}
