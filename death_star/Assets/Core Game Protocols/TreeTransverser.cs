using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTransverser {

    public int currentTransverse;

    public TreeTransverser() {
        currentTransverse = 1;
        Transverse<int[][][]>(new int[] { 1, 2 });
    }

    public void Transverse(int abilityId, int nodeId, int variableId, int variableAction) {

    }

    public void Transverse<T>(int[] paths,int progress =0) {
        List<int> tracker = new List<int>(paths);

        progress++;


        if(tracker.Count > progress)
            Transverse<T>(tracker.ToArray());
        else {
            // Do transversing here.

        }
    }

}
