using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;



public class UpdateAbilityDataEncoder : NetworkMessageEncoder {

    public class PackedNodeData<T> : PackedNodeData {
        public T value;

        public PackedNodeData(int a, int v, T val) {
            ability = a;
            var = v;
            value = val;
        }

        public override void UpdateCentral(AbilityCentralThreadPool centralInst) {
            int abilityNodes = centralInst.GetAbilityNodeId();
            Debug.Log("AN: " + abilityNodes);
            Debug.Log("A: " + ability);
            int nTID = AbilityTreeNode.globalList.l[abilityNodes].abiNodes[ability].GetNodeThreadId();

            if(nTID > -1) {
                centralInst.UpdateVariableValue<T>(nTID, var, value);
                centralInst.UpdateVariableData<T>(nTID, var);
            }
        }
    }

    public class PackedNodeData {
        public int ability;
        public int var;

        public virtual void UpdateCentral(AbilityCentralThreadPool centralInst) {

        }
    }

    List<AbilityCentralThreadPool> playerGeneratedAbilities;

    public override void CalibrateEncoder(int id) {
        playerGeneratedAbilities = new List<AbilityCentralThreadPool>();
        base.CalibrateEncoder(id);
    }

    public void SendVariableManifest(AbilityCentralThreadPool inst, AbilityNodeNetworkData[] manifest) {

        if(inst.ReturnNetworkObjectId() == -1) {
            playerGeneratedAbilities.Add(inst);
        }

        byte[] cData = BitConverter.GetBytes(inst.ReturnNetworkObjectId());
        byte[] instData = BitConverter.GetBytes(inst.ReturnInstId());

        byte[] manifestData = PrepareVariableManifest(manifest);

        bytesToSend = new byte[8 + manifestData.Length];

        Buffer.BlockCopy(cData, 0, bytesToSend, 0, 4);
        Buffer.BlockCopy(instData, 0, bytesToSend, 4, 4);
        Buffer.BlockCopy(manifestData, 0, bytesToSend, 8, manifestData.Length);
        SendEncodedMessages();
    }

    public static byte[] PrepareVariableManifest(AbilityNodeNetworkData[] manifest) {

        if(manifest == null || manifest.Length == 0)
            return new byte[0];

        List<byte> byteData = new List<byte>();
        int argType = -1;
        byte[] vBytes = new byte[0];

        for(int i = 0; i < manifest.Length; i++) {

            if(manifest[i].dataType == typeof(int)) {
                argType = 0;
                vBytes = BitConverter.GetBytes((manifest[i] as AbilityNodeNetworkData<int>).value);
            }

            if(manifest[i].dataType == typeof(float)) {
                argType = 1;
                vBytes = BitConverter.GetBytes((manifest[i] as AbilityNodeNetworkData<float>).value);
            }

            if(manifest[i].dataType == typeof(string)) {
                argType = 2;
                vBytes = Encoding.Default.GetBytes((manifest[i] as AbilityNodeNetworkData<string>).value);
            }

            if(manifest[i].dataType == typeof(int[])) {
                argType = 3;

                List<byte> iBData = new List<byte>();
                int[] iData = (manifest[i] as AbilityNodeNetworkData<int[]>).value;

                for(int j = 0; j < iData.Length; j++)
                    iBData.AddRange(BitConverter.GetBytes(iData[j]));

                vBytes = iBData.ToArray();
            }

            if(manifest[i].dataType == typeof(float[])) {
                argType = 4;

                List<byte> fBData = new List<byte>();
                float[] fData = (manifest[i] as AbilityNodeNetworkData<float[]>).value;

                for(int j = 0; j < fData.Length; j++)
                    fBData.AddRange(BitConverter.GetBytes(fData[j]));

                vBytes = fBData.ToArray();
            }

            Debug.Log(argType);

            if(argType > -1) {
                byteData.AddRange(BitConverter.GetBytes(manifest[i].nodeId));
                byteData.AddRange(BitConverter.GetBytes(manifest[i].varId));
                byteData.AddRange(BitConverter.GetBytes(argType));
                byteData.AddRange(BitConverter.GetBytes(vBytes.Length));
                byteData.AddRange(vBytes);
            }
        }

        return byteData.ToArray();
    }

    public override void MessageRecievedCallback() {
        int central = BitConverter.ToInt32(bytesRecieved, 0);
        int instId = BitConverter.ToInt32(bytesRecieved, 4);
        List<PackedNodeData> pND = new List<PackedNodeData>();
        AbilityCentralThreadPool centralInst = null;
        int loopStartId = 0;

        foreach(PackedNodeData parsedData in ParseManifest(bytesRecieved, 8))
            pND.Add(parsedData);

        if(central > -1) {
            if(NetworkObjectTracker.inst.CheckIfInstIdMatches(central, instId))
                centralInst = NetworkObjectTracker.inst.ReturnNetworkObject(central) as AbilityCentralThreadPool;
        } else {

            loopStartId = 1;
            // Handles creation of new centrals.
            if(targetId != ClientProgram.clientId) {              
                centralInst = new AbilityCentralThreadPool(targetId);
                int aId = (pND[0] as PackedNodeData<int>).value;
                AbilitiesManager.aData[targetId].abilties[aId].CreateAbility(centralInst);
            } else {
                NetworkObjectTracker.inst.AddNetworkObject(playerGeneratedAbilities[0]);
                playerGeneratedAbilities[0].RenameAllNodes();
                centralInst = playerGeneratedAbilities[0];
                playerGeneratedAbilities.RemoveAt(0);
            }
        }

        if (centralInst != null)
            if (loopStartId < pND.Count)
                for (int i=loopStartId; i < pND.Count; i++) 
                    pND[i].UpdateCentral(centralInst);       
    }

    public IEnumerable<PackedNodeData> ParseManifest(byte[] bytesRecieved, int offset = 0) {

        int i = offset;

        while(i < bytesRecieved.Length) {

            int ability = BitConverter.ToInt32(bytesRecieved, i);
            int var = BitConverter.ToInt32(bytesRecieved, i + 4);
            int argType = BitConverter.ToInt32(bytesRecieved, i + 8);
            int valueLen = BitConverter.ToInt32(bytesRecieved, i + 12);

            switch(argType) {

                case 0: //int                    
                    int iData = BitConverter.ToInt32(bytesRecieved, i + 16);
                    yield return new PackedNodeData<int>(ability, var, iData);
                    break;

                case 1: //float                    
                    float fData = BitConverter.ToSingle(bytesRecieved, i + 16);
                    yield return new PackedNodeData<float>(ability, var, fData);
                    break;

                case 2: //string
                    string sData = Encoding.Default.GetString(bytesRecieved, i + 16, valueLen);
                    yield return new PackedNodeData<string>(ability, var, sData);
                    break;

                case 3: //int[]
                    int[] iArray = new int[valueLen / 4];

                    for(int j = 0; j < iArray.Length; j++)
                        iArray[j] = BitConverter.ToInt32(bytesRecieved, i + 16 + (j * 4));

                    yield return new PackedNodeData<int[]>(ability, var, iArray);
                    break;

                case 4: //float[]
                    float[] fArray = new float[valueLen / 4];

                    for(int j = 0; j < fArray.Length; j++)
                        fArray[j] = BitConverter.ToSingle(bytesRecieved, i + 16 + (j * 4));

                    yield return new PackedNodeData<float[]>(ability, var, fArray);
                    break;
            }

            i += 16 + valueLen;
        }
    }



    /*public override void MessageRecievedCallback() {
        int central = BitConverter.ToInt32(bytesRecieved, 0);
        int instId = BitConverter.ToInt32(bytesRecieved, 4);

        // Checks if inst id given was updated.
        if(NetworkObjectTracker.inst.CheckIfInstIdMatches(central, instId)) {

            int ability = BitConverter.ToInt32(bytesRecieved, 8);
            int var = BitConverter.ToInt32(bytesRecieved, 12);
            int argType = BitConverter.ToInt32(bytesRecieved, 16);
            VariableTypes vtype = (VariableTypes)BitConverter.ToInt32(bytesRecieved, 20);

            AbilityCentralThreadPool centralInst = NetworkObjectTracker.inst.ReturnNetworkObject(central) as AbilityCentralThreadPool;
            int abilityNodes = centralInst.GetAbilityNodeId();
            int nTID = AbilityTreeNode.globalList.l[abilityNodes].abiNodes[ability].GetNodeThreadId();

            if(nTID > -1)
                switch(argType) {

                    case 0: //int                    
                        int iData = BitConverter.ToInt32(bytesRecieved, 24);
                        centralInst.NodeVariableCallback<int>(nTID, var, iData, vtype);
                        break;

                    case 1: //float                    
                        float fData = BitConverter.ToSingle(bytesRecieved, 24);
                        AbilityCentralThreadPool.globalCentralList.l[central].NodeVariableCallback<float>(nTID, var, fData, vtype);
                        break;

                    case 2: //string
                        string sData = Encoding.Default.GetString(bytesRecieved, 24, bytesRecieved.Length - 24);
                        AbilityCentralThreadPool.globalCentralList.l[central].NodeVariableCallback<string>(nTID, var, sData, vtype);
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

    public static T[] ProcessArrayable<T>() {

        int bytesPerElement = 0;
        int argType = -1;

        if(typeof(T) == typeof(int)) {
            bytesPerElement = 4;
            argType = 0;
        }

        if(typeof(T) == typeof(float)) {
            bytesPerElement = 4;
            argType = 1;
        }

        if(argType > -1) {
            T[] cArray = new T[(bytesRecieved.Length - 28) / bytesPerElement];

            for(int i = 0; i < cArray.Length; i++) {
                switch(argType) {
                    case 0:
                        cArray[i] = (T)(object)BitConverter.ToInt32(bytesRecieved, 28 + (i * bytesPerElement));
                        break;
                    case 1:
                        cArray[i] = (T)(object)BitConverter.ToSingle(bytesRecieved, 28 + (i * bytesPerElement));
                        break;
                }
            }

            return cArray;
        }
        return null;
    }*/
}
