using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
public class XMLTesting : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ReadPath();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void ReadPath() {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load("Ability.xml");
        XmlNode titleNode = xmlDoc.SelectSingleNode("/savefile/ability");

        if (titleNode != null)
            Debug.Log(titleNode.Attributes[0].Value);
      
        else
            Debug.Log("Empty"); 

        Debug.Log(titleNode.ChildNodes[0].InnerText);
    }

    void Read() {
        XmlReader xmlReader = XmlReader.Create("http://rss.cnn.com/rss/edition_world.rss");


        while (xmlReader.Read()) { //Code for reading XML
            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "Cube")) {
                if (xmlReader.HasAttributes)
                    Debug.Log(xmlReader.GetAttribute("currency") + ": " + xmlReader.GetAttribute("rate"));
            }
        }
    }

    void Write() {
        XmlWriter xmlWriter = XmlWriter.Create("test.xml");

        xmlWriter.WriteStartDocument();
        xmlWriter.WriteStartElement("users");

        xmlWriter.WriteStartElement("user");
        xmlWriter.WriteAttributeString("age", "42");
        xmlWriter.WriteString("John Doe");
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("user");
        xmlWriter.WriteAttributeString("age", "39");
        xmlWriter.WriteString("Jane Doe");

        xmlWriter.WriteEndDocument();
        xmlWriter.Close();
    }
}
