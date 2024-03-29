﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class UpdateAbilityDataEncoder : NetworkMessageEncoder {

    /*public class PackedNodeData<T> : PackedNodeData {
        public T value;

        public PackedNodeData(int a, int v, T val) {
            ability = a;
            var = v;
            value = val;
        }

        public override void UpdateCentral(AbilityCentralThreadPool centralInst) {

            //Debug.Log("Input recieved!!!");

            if(ability > -1) {
                int nTID = centralInst.GetNode(ability).GetNodeThreadId();

                if(nTID > -1) {
                    //Debug.Log("Input integrated.");
                    centralInst.UpdateVariableValue<T>(ability, var, value);
                    centralInst.UpdateVariableData<T>(nTID, var);
                }
            }
        }
    }

    public class PackedNodeData {
        public int ability;
        public int var;

        public virtual void UpdateCentral(AbilityCentralThreadPool centralInst) {

        }
    }*/

    //List<AbilityCentralThreadPool> playerGeneratedAbilities;

    public override void CalibrateEncoder(int id) {
        //playerGeneratedAbilities = new List<AbilityCentralThreadPool>();
        base.CalibrateEncoder(id);
    }

    public void SendVariableManifest(AbilityCentralThreadPool inst, AbilityNodeNetworkData[] manifest) {

        byte[] playerId = BitConverter.GetBytes(inst.ReturnPlayerCasted());
        byte[] centralId = BitConverter.GetBytes(inst.ReturnCentralId());

        //Debug.LogFormat("Sending out Data for {0},{1}", inst.ReturnPlayerCasted(), inst.ReturnCentralId());
        byte[] manifestData = PrepareVariableManifest(manifest);

        bytesToSend = new byte[8 + manifestData.Length];

        Buffer.BlockCopy(playerId, 0, bytesToSend, 0, 4);
        Buffer.BlockCopy(centralId, 0, bytesToSend, 4, 4);
        Buffer.BlockCopy(manifestData, 0, bytesToSend, 8, manifestData.Length);
        SendEncodedMessages();
    }

    public static byte[] PrepareVariableManifest(AbilityNodeNetworkData[] manifest) {

        if(manifest == null || manifest.Length == 0)
            return new byte[0];

        List<byte> byteData = new List<byte>();

        for(int i = 0; i < manifest.Length; i++) {

            int argType = -1;
            byte[] vBytes = new byte[0];

            //Debug.Log("Datatype: " + manifest[i].dataType);
            //if(manifest[i].additionalData != null)
            //Debug.Log("Loop ID: " + BitConverter.ToInt32(manifest[i].additionalData, 0));

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

            if(manifest[i].dataType == typeof(Vector3)) {
                argType = 5;

                List<byte> fBData = new List<byte>();
                Vector3 vData = (manifest[i] as AbilityNodeNetworkData<Vector3>).value;

                fBData.AddRange(BitConverter.GetBytes(vData.x));
                fBData.AddRange(BitConverter.GetBytes(vData.y));

                vBytes = fBData.ToArray();
            }

            if(manifest[i].dataType == typeof(bool)) {
                argType = 6;
                vBytes = BitConverter.GetBytes((manifest[i] as AbilityNodeNetworkData<bool>).value);
            }

            if(argType > -1) {
                byteData.AddRange(BitConverter.GetBytes(manifest[i].nodeId));
                byteData.AddRange(BitConverter.GetBytes(manifest[i].varId));
                byteData.AddRange(BitConverter.GetBytes(argType));
                byteData.AddRange(BitConverter.GetBytes(manifest[i].variableSetCount));
                byteData.AddRange(BitConverter.GetBytes(vBytes.Length));
                byteData.AddRange(vBytes);

                /*if(manifest[i].additionalData != null) {
                    byteData.AddRange(BitConverter.GetBytes(manifest[i].additionalData.Length));
                    byteData.AddRange(manifest[i].additionalData);
                } else
                    byteData.AddRange(BitConverter.GetBytes(0));*/
            }
        }

        return byteData.ToArray();
    }

    public override void MessageRecievedCallback() {

        if(targetId == ClientProgram.clientId)
            return;

        if(!AbilitiesManager.playerLoadedInLobby) {
            AbilitiesManager.pendingData.Add(bytesRecieved);
            return;
        }

        ParseMessage(bytesRecieved);
    }

    public void ParseMessage(byte[] bytesToParse) {
        int playerId = BitConverter.ToInt32(bytesToParse, 0);
        int centralId = BitConverter.ToInt32(bytesToParse, 4);

        List<AbilityNodeNetworkData> cCData = new List<AbilityNodeNetworkData>();
        AbilityCentralThreadPool centralInst = AbilitiesManager.aData[playerId].playerSpawnedCentrals.GetElementAt(centralId);

        //Debug.LogFormat("Node data for {0}/{1}: ", playerId, centralId);
        foreach(AbilityNodeNetworkData parsedData in ParseManifest(bytesToParse, 8)) {

            cCData.Add(parsedData);

            if(centralInst == null) {
                if(cCData.Count == 2) {
                    centralInst = new AbilityCentralThreadPool(playerId);

                    int pId = (cCData[0] as AbilityNodeNetworkData<int>).value;
                    string aId = (cCData[1] as AbilityNodeNetworkData<string>).value;
                    AbilitiesManager.aData[pId].abilties[aId].CreateAbility(centralInst, playerId, centralId);
                }

                //Debug.Log("Continued");
                continue;
            }

            //Debug.Log("Data recieved!");
            centralInst.AddPendingData(parsedData);           
        }

        if(ClientProgram.clientId == ClientProgram.hostId)
            centralInst.AddVariableNetworkData(cCData.ToArray());
        
        centralInst.StartThreads(0);
    }

    public IEnumerable<AbilityNodeNetworkData> ParseManifest(byte[] bytesToParse, int offset = 0) {

        int i = offset;
        //Debug.Log(bytesRecieved.Length);

        while(i < bytesToParse.Length) {

            int ability = BitConverter.ToInt32(bytesToParse, i);
            int var = BitConverter.ToInt32(bytesToParse, i + 4);
            int argType = BitConverter.ToInt32(bytesToParse, i + 8);
            int setCount = BitConverter.ToInt32(bytesToParse, i + 12);
            int valueLen = BitConverter.ToInt32(bytesToParse, i + 16);

            //Debug.Log(setCount);

            switch(argType) {

                case 0: //int                    
                    int iData = BitConverter.ToInt32(bytesToParse, i + 20);
                    yield return new AbilityNodeNetworkData<int>(ability, var, iData, setCount);
                    break;

                case 1: //float                    
                    float fData = BitConverter.ToSingle(bytesToParse, i + 20);
                    yield return new AbilityNodeNetworkData<float>(ability, var, fData, setCount);
                    break;

                case 2: //string
                    string sData = Encoding.Default.GetString(bytesToParse, i + 20, valueLen);
                    yield return new AbilityNodeNetworkData<string>(ability, var, sData, setCount);
                    break;

                case 3: //int[]
                    int[] iArray = new int[valueLen / 4];

                    for(int j = 0; j < iArray.Length; j++)
                        iArray[j] = BitConverter.ToInt32(bytesToParse, i + 20 + (j * 4));

                    yield return new AbilityNodeNetworkData<int[]>(ability, var, iArray, setCount);
                    break;

                case 4: //float[]
                    float[] fArray = new float[valueLen / 4];

                    for(int j = 0; j < fArray.Length; j++)
                        fArray[j] = BitConverter.ToSingle(bytesToParse, i + 20 + (j * 4));

                    yield return new AbilityNodeNetworkData<float[]>(ability, var, fArray, setCount);
                    break;

                case 5: //vector3
                    Vector3 v = new Vector3(BitConverter.ToSingle(bytesToParse, i + 20), BitConverter.ToSingle(bytesToParse, i + 24));
                    yield return new AbilityNodeNetworkData<Vector3>(ability, var, v, setCount);
                    break;

                case 6: //bool                    
                    bool bData = BitConverter.ToBoolean(bytesToParse, i + 20);
                    yield return new AbilityNodeNetworkData<bool>(ability, var, bData, setCount);
                    break;
            }

            //Debug.Log(i);
            i += 20 + valueLen;
        }
    }
}
