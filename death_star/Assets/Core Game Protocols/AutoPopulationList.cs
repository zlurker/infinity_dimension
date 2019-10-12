using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPopulationList<T> {

    public List<T> l;
    Stack<int> iNS; //internalNumberingSystem

    public AutoPopulationList() {
        l = new List<T>();
        iNS = new Stack<int>();
    }

    public void ModifyElementAt(int index, T element) {
        Resize(index);
        l.Add(element);
    }

    public T GetElementAt(int index) {
        Resize(index);
        return l[index];
    }

    public int AddElement(T element) {
        int index = 0;

        if(iNS.Count > 0) {
            index = iNS.Pop();
            //Debug.LogFormat("Popped index {0}", index);
            l[index] = element;
        } else {
            index = l.Count;
            l.Add(element);
        }

        return index;
    }

    public void Resize(int index) {
        int diff = index - l.Count;

        for(int i = 0; i < diff; i++) {
            l.Add(default(T));
            iNS.Push(l.Count -1);
        }
    }
}
