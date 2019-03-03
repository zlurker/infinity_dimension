using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableLinkObjects {
    public List<SavedData> linkedData;
    public EditableLinkInstance l;
    public string n;

    public EditableLinkObjects(EditableLinkInstance linker) {
        linkedData = new List<SavedData>();
        l = linker;
    }

    public EditableLinkObjects(string name) {
        linkedData = new List<SavedData>();
        n = name;
    }

    public EditableLinkObjects(string name, SavedData[] startingObjects) {
        linkedData = new List<SavedData>(startingObjects);
        n = name;
    }
}

public class StartupLinkerHelper {
    public static int GetGroupByID(string id) {
        int a = Iterator.ReturnKey<EditableLinkObjects>(EditableLinkInstance.links.ToArray(), id, (p) => { return p.n; });

        if (a == -1) {
            a = EditableLinkInstance.links.Count;
            EditableLinkInstance.links.Add(new EditableLinkObjects(id));
            Debug.Log("Creation of new object " + id);
        }

        return a;
    }

    public static void RelinkLoadedData(SavedData[] target) {
        EditableLinkInstance.links = new List<EditableLinkObjects>();

        for(int i = 0; i < target.Length; i++) {
            int groupId;
            if(target[i].connectedInt > -1) {
                groupId = GetGroupByID(target[i].connectedInt.ToString());
                EditableLinkInstance.links[groupId].linkedData.Add(target[i]);
                Debug.Log("Exception: " + target[i].connectedInt);
            } 
                

            for(int j = 0; j < target[i].fields.Count; j++) {
                RuntimeParameters<EditableLinkInstance> instance = target[i].fields[j] as RuntimeParameters<EditableLinkInstance>;

                if(instance != null) {
                    groupId = GetGroupByID(instance.v.linkIdStr);
                    Debug.Log(instance.v.linkIdStr);
                    EditableLinkInstance.links[groupId].l = instance.v;
                }
            }
        }
        
        for(int i = 0; i < EditableLinkInstance.links.Count; i++) {
            Debug.Log(EditableLinkInstance.links[i]);
            int cI = -1;
            Debug.Log(cI);
            if(EditableLinkInstance.links[i].l != null) {
                EditableLinkInstance.links[i].l.linkId = i;
                EditableLinkInstance.links[i].l.linkIdStr = i.ToString();
                cI = i;
            }

            for(int j = 0; j < EditableLinkInstance.links[i].linkedData.Count; j++)
                EditableLinkInstance.links[i].linkedData[j].connectedInt = cI;
        }
    }
}

public class EditableLinkInstance {
    public static List<EditableLinkObjects> links = new List<EditableLinkObjects>();
    public int linkId;
    public string linkIdStr;

    public EditableLinkInstance() {
        Debug.Log("Called" + links.Count);
    }

    public EditableLinkInstance(SavedData[] startingObjects) {
        Debug.Log("Called" + links.Count);
        linkId = links.Count;
        linkIdStr = linkId.ToString();
        links.Add(new EditableLinkObjects(this));

        for(int i = 0; i < startingObjects.Length; i++)
            LinkObject(startingObjects[i]);
    }

    public SavedData[] GetLinkedObjects() {
        return links[linkId].linkedData.ToArray();
    }

    public void LinkObject(SavedData target) {
        target.connectedInt = linkId;
        links[linkId].linkedData.Add(target);
    }

    

}
