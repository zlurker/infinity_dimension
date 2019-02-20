using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableLinkObjects {
    public List<SavedData> linkedData;

    public EditableLinkObjects() {
        linkedData = new List<SavedData>();
    }

    public EditableLinkObjects(SavedData[] startingObjects) {
        linkedData = new List<SavedData>(startingObjects);
    }
}

public class EditableLinkInstance {
    public static List<EditableLinkObjects> links = new List<EditableLinkObjects>();
    public int linkId;

    public EditableLinkInstance() {
        linkId = links.Count;
        links.Add(new EditableLinkObjects());
    }

    public EditableLinkInstance(EditableLinkObjects defaultValue) {
        linkId = links.Count;
        links.Add(defaultValue);
    }


}
