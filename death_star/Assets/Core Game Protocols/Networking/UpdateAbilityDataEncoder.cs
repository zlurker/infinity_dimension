using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class UpdateAbilityDataEncoder : NetworkMessageEncoder {

    public void SendUpdatedNodeData<T>(int central, int instId, int ability, int var,int vType, T value) {

        byte[] vBytes = new byte[0];
        int argType = -1;

        if(value is int) {
            vBytes = BitConverter.GetBytes((int)(object)value);
            argType = 0;
        }

        if(value is float) {
            vBytes = BitConverter.GetBytes((float)(object)value);
            argType = 1;
        }

        if(value is string) {
            vBytes = Encoding.UTF8.GetBytes((string)(object)value);
            argType = 2;
        }

        if(argType > -1)
            CompileMessage(central, instId, ability, var, vBytes, vType,argType);
    }

    public void SendUpdatedNodeData<T>(int central, int instId, int ability, int var,int vType, T[] value) {
        Debug.Log("(Update)Time start" + Time.realtimeSinceStartup);
        byte[] vBytes = new byte[0];
        int argType = -1;

        if(typeof(T) == typeof(int)) {
            vBytes = new byte[4 * value.Length];
            argType = 3;
        }

        if(typeof(T) == typeof(float)) {
            vBytes = new byte[4 * value.Length];
            argType = 4;
        }

        if(argType > -1) {
            for(int i = 0; i < value.Length; i++) {

                byte[] data = new byte[0];

                switch(argType) {
                    case 3:
                        data = BitConverter.GetBytes((int)(object)value[i]);
                        break;
                    case 4:
                        data = BitConverter.GetBytes((float)(object)value[i]);
                        break;
                }
                Buffer.BlockCopy(data, 0, vBytes, i * data.Length, data.Length);
            }

            CompileMessage(central, instId, ability, var, vBytes, vType,argType);
        }
    }

    public void CompileMessage(int central, int instId, int ability, int var, byte[] vBytes,int vType, int argType) {
        bytesToSend = new byte[24 + vBytes.Length];

        byte[] cBytes = BitConverter.GetBytes(central);
        byte[] iBytes = BitConverter.GetBytes(instId);
        byte[] aBytes = BitConverter.GetBytes(ability);
        byte[] vaBytes = BitConverter.GetBytes(var);
        byte[] argBytes = BitConverter.GetBytes(argType);
        byte[] vTypeBytes = BitConverter.GetBytes(vType);

        Buffer.BlockCopy(cBytes, 0, bytesToSend, 0, 4);
        Buffer.BlockCopy(iBytes, 0, bytesToSend, 4, 4);
        Buffer.BlockCopy(aBytes, 0, bytesToSend, 8, 4);
        Buffer.BlockCopy(vaBytes, 0, bytesToSend, 12, 4);
        Buffer.BlockCopy(argBytes, 0, bytesToSend, 16, 4);
        Buffer.BlockCopy(vTypeBytes, 0, bytesToSend, 20, 4);
        Buffer.BlockCopy(vBytes, 0, bytesToSend, 24, vBytes.Length);

        SendEncodedMessages();
    }

    public override void MessageRecievedCallback() {
        int central = BitConverter.ToInt32(bytesRecieved, 0);
        int instId = BitConverter.ToInt32(bytesRecieved, 4);

        // Checks if inst id given was updated.
        if(NetworkObjectTracker.inst.CheckIfInstIdMatches(central, instId)) {

            int ability = BitConverter.ToInt32(bytesRecieved, 8);
            int var = BitConverter.ToInt32(bytesRecieved, 12);
            int argType = BitConverter.ToInt32(bytesRecieved, 16);
            VariableTypes vtype = (VariableTypes) BitConverter.ToInt32(bytesRecieved, 20);

            AbilityCentralThreadPool centralInst = NetworkObjectTracker.inst.ReturnNetworkObject(central) as AbilityCentralThreadPool;
            int abilityNodes = centralInst.GetAbilityNodeId();
            int nTID = AbilityTreeNode.globalList.l[abilityNodes].abiNodes[ability].GetNodeThreadId();

            if(nTID > -1)
                switch(argType) {

                    case 0: //int                    
                        int iData = BitConverter.ToInt32(bytesRecieved, 24);
                        AbilityCentralThreadPool.globalCentralList.l[central].NodeVariableCallback<int>(nTID,var,iData);
                        break;

                    case 1: //float                    
                        float fData = BitConverter.ToSingle(bytesRecieved, 24);
                        AbilityCentralThreadPool.globalCentralList.l[central].NodeVariableCallback<float>(nTID, var, fData);
                        break;

                    case 2: //string
                        string sData = Encoding.Default.GetString(bytesRecieved, 24, bytesRecieved.Length - 24);
                        AbilityCentralThreadPool.globalCentralList.l[central].NodeVariableCallback<string>(nTID, var, sData);
                        break;

                    case 3: //int[]
                        int[] iArray = ProcessArrayable<int>();
                        AbilityCentralThreadPool.globalCentralList.l[central].NodeVariableCallback<int[]>(nTID, var, iArray);
                        break;

                    case 4: //float[]
                        float[] fArray = ProcessArrayable<float>();
                        Debug.Log("Working");
                        AbilityCentralThreadPool.globalCentralList.l[central].NodeVariableCallback<float[]>(nTID, var, fArray);
                        break;
                }
        }

        Debug.Log("(Update)Time end" + Time.realtimeSinceStartup);
    }

    public T[] ProcessArrayable<T>() {

        int bytesPerElement = 0;
        int argType = -1;

        if(typeof(T) == typeof(int) || typeof(T) == typeof(int[])) {
            bytesPerElement = 4;
            argType = 0;
        }

        if(typeof(T) == typeof(float) || typeof(T) == typeof(float[])) {
            bytesPerElement = 4;
            argType = 1;
        }

        if(argType > -1) {
            T[] cArray = new T[(bytesRecieved.Length - 24) / bytesPerElement];

            for(int i = 0; i < cArray.Length; i++) {
                switch(argType) {
                    case 0:
                        cArray[i] = (T)(object)BitConverter.ToInt32(bytesRecieved, 24 + (i * bytesPerElement));
                        break;
                    case 1:
                        cArray[i] = (T)(object)BitConverter.ToSingle(bytesRecieved, 24 + (i * bytesPerElement));
                        break;
                }
            }

            return cArray;
        }
        return null;
    }
}
