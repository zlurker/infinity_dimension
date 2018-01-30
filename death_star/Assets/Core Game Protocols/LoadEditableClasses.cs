using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

public class LoadEditableClasses : MonoBehaviour {

    void Start() {
        GlobalData.uL = new List<IPlayerEditable>();

        Type type = typeof(IPlayerEditable);
        Type[] types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p)).ToArray();

        for (int i = 0; i < types.Length; i++) {
            ConstructorInfo info = types[i].GetConstructor(new Type[0]);

            if (info != null)
                GlobalData.uL.Add(info.Invoke(new object[0]) as IPlayerEditable);
        }

        for (int i =0; i < GlobalData.uL.Count; i++) {
            GlobalData.uL[i].LoadUI();
        }
    }
}
