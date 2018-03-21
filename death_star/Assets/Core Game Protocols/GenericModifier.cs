using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenericModifier<T> {

    //I just need one delegate to pass values to here.
    public static void ModifyValue(ref T variable, object value) {
        //ref int test;       
        variable = (T) value;
    }

    /*static ref int ReturnByReference() { C# 7.0 not working yet.
        int[] arr = { 1 };
        ref int x = ref arr[0];
        return ref x;
    }*/
    
}
