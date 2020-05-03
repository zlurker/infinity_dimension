using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPopulationList<T> : EnhancedList<T> {

    public AutoPopulationList() {
        l = new List<T>();
    }

    public AutoPopulationList(int length) {
        l = new List<T>();
        Resize(length - 1);
    }

    public void ModifyElementAt(int index, T element) {
        Resize(index);

        if(iNS.Contains(index))
            l[index] = element;
    }

    public T GetElementAt(int index) {
        Resize(index);
        return l[index];
    }

    public void Resize(int index) {
        int diff = index - l.Count + 1;

        for(int i = 0; i < diff; i++) {

            if(!iNS.Contains(index))
                iNS.Add(i);

            l.Add(default(T));
        }
    }
}
