using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPopulationList<T> {

    public List<T> l;

    public AutoPopulationList() {
        l = new List<T>();
    }

    public void ModifyElementAt(int index, T element) {
        int diff = index - l.Count;

        for(int i = 0; i <= diff; i++) {
            l.Add(default(T));
            Debug.Log(l.Count);
        }

        Debug.Log(l.Count);
        l[index] = element;
    }
}
