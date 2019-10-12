using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class VariableTypeIndex {

	public static int ReturnVariableIndex(Type type) {
        if(type == typeof(string))
            return 0;

        if(type == typeof(float))
            return 1;

        if(type == typeof(int))
            return 2;

        return -1;
    }

    public static RuntimeParameters ReturnRuntimeType(int index,string sO) {
        RuntimeParameters inst = null;

        switch(index) {
            case 0:
                inst = JsonConvert.DeserializeObject<RuntimeParameters<string>>(sO);
                break;
            case 1:
                inst = JsonConvert.DeserializeObject<RuntimeParameters<float>>(sO);
                break;
            case 2:
                inst = JsonConvert.DeserializeObject<RuntimeParameters<int>>(sO);
                break; 
        }
        return inst;
    }
}
