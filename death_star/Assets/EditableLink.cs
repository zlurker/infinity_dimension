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
        int a = Iterator.ReturnKey<EditableLinkObjects>(EditableLinkInstance.links.l.ToArray(), id, (p) => { return p.n; });

        if(a == -1) 
            a = EditableLinkInstance.links.Add(new EditableLinkObjects(id));
        
        return a;
    }

    public static void RelinkLoadedData(SavedData[] target) {
        EditableLinkInstance.links = new EnhancedList<EditableLinkObjects>();

        for(int i = 0; i < target.Length; i++) {
            int groupId;
            for (int j=0; j < target[i].connectedInt.Count; j++) {
                groupId = GetGroupByID(target[i].connectedInt[j].ToString());
                EditableLinkInstance.links.l[groupId].linkedData.Add(target[i]);
            }

            for(int j = 0; j < target[i].fields.Count; j++) {
                RuntimeParameters<EditableLinkInstance> instance = target[i].fields[j] as RuntimeParameters<EditableLinkInstance>;

                if(instance != null) {
                    groupId = GetGroupByID(instance.v.linkIdStr);
                    EditableLinkInstance.links.l[groupId].l = instance.v;
                }
            }
        }

        for(int i = 0; i < EditableLinkInstance.links.l.Count; i++) {
            int cI = -1;

            if(EditableLinkInstance.links.l[i].l != null) {
                EditableLinkInstance.links.l[i].l.linkId = i;
                EditableLinkInstance.links.l[i].l.linkIdStr = i.ToString();
                cI = i;

                for(int j = 0; j < EditableLinkInstance.links.l[i].linkedData.Count; j++)
                    EditableLinkInstance.links.l[i].linkedData[j].connectedInt.Add(cI);
            }
        }
    }
}

public class EditableLinkInstance {
    public static EnhancedList<EditableLinkObjects> links = new EnhancedList<EditableLinkObjects>();
    public int linkId;
    public string linkIdStr;

    public EditableLinkInstance() {
    }

    public EditableLinkInstance(SavedData[] startingObjects) {
        linkId = links.Add(new EditableLinkObjects(this));
        linkIdStr = linkId.ToString();
        
        for(int i = 0; i < startingObjects.Length; i++)
            LinkObject(startingObjects[i]);
    }

    public SavedData[] GetLinkedObjects() {
        return links.l[linkId].linkedData.ToArray();
    }

    public void LinkObject(SavedData target) {
        target.connectedInt.Add(linkId);
        links.l[linkId].linkedData.Add(target);
    }
}
