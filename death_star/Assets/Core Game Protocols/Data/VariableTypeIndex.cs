using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class VariableTypeIndex {

    public class RuntimeParameterConversion<T> : RuntimeParameterConversion {

        public RuntimeParameterConversion() {
            rPT = typeof(T);
        }

        public override RuntimeParameters ReturnRuntimeType(string sO) {
            return JsonConvert.DeserializeObject<RuntimeParameters<T>>(sO);
        }
    }

    public class RuntimeParameterConversion {
        public Type rPT;

        public virtual RuntimeParameters ReturnRuntimeType(string sO) {
            return null;
        }
    }

    public static RuntimeParameterConversion[] convertors = new RuntimeParameterConversion[] {
        new RuntimeParameterConversion<string>(), new RuntimeParameterConversion<float>(), new RuntimeParameterConversion<int>()
    };

    public static int ReturnVariableIndex(Type type) {

        if(type == typeof(string))
            return 0;

        if(type == typeof(float))
            return 1;

        if(type == typeof(int))
            return 2;

        return -1;
    }
}
