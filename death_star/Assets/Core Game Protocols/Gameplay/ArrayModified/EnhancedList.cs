using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//A enhanced list that will keep track of free spaces while not changing indexes of current existing elements
public class EnhancedList<T> {

    public List<T> l; //list
    protected HashSet<int> iNS; //internalNumberingSystem

    public EnhancedList() {
        l = new List<T>();
        iNS = new HashSet<int>();
    }

    public EnhancedList(T[] presetElements) {
        l = new List<T>(presetElements);
        iNS = new HashSet<int>();
    }

    public int Add(T element) {
        int index = -1;

        if(iNS.Count > 0) {
            index = iNS.First<int>();
            l[index] = element;
        } else {
            index = l.Count;
            l.Add(element);
        }

        return index;
    }

    public void Remove(int index) {
        l[index] = default(T);

        if(!iNS.Contains(index))
            iNS.Add(index);
    }

    public int GetActiveElementsLength() {
        return l.Count - iNS.Count;
    }

    public int[] ReturnActiveElementIndex() {
        List<int> all = new List<int>();

        for(int i = 0; i < l.Count; i++)
            all.Add(i);

        int[] insArray = iNS.ToArray();

        for(int i = 0; i < insArray.Length; i++)
            all.Remove(insArray[i]);

        return all.ToArray();
    }

    public T[] ReturnActiveElements() {
        int[] eI = ReturnActiveElementIndex();
        T[] aE = new T[eI.Length];

        for(int i = 0; i < eI.Length; i++)
            aE[i] = l[eI[i]];

        return aE;
    }

    public int[] ReturnINS() {
        return iNS.ToArray();
    }
}
