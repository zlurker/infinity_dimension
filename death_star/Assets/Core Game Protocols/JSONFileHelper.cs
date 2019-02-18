using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;



public class JSONFileHelper : MonoBehaviour
{
    //----------------------SAMPLE FILE OUTPUT----------------------------//
    // 

    public void JSONFileWriter(SavedData saveFile) {
        StringBuilder fileWriter = new StringBuilder();
        
        for (int i=0; i < saveFile.fields.Count; i++) {
            //fileWriter.Append();
        }

    }

}
