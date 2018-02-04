using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public enum OperationType {
    Attribute, InnerText
}

public class XMLHelper : MonoBehaviour {

    string[][] test;
    string l;
    // XmlDocument xmlDoc;
    // Use this for initialization
    void Start() {

        WriteNode(new int[] { 1, 3, 10, 20 }, "/Fuck/My/Asshole/Hard", "INNER_TEXT,ATTRIBUTES", new string[][] { new string[] { "I LOVE SEX" }, new string[] { "senpainame", "FUCK ME SENSELESS" } });
        GetNodes("Asshole", "/savefile/Fuck/My");

    }

    /*public XmlDocument GetCurrentDoc() {
        if (xmlDoc == null) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Ability.xml");
        }

        return xmlDoc;
    }*/

    public XmlNode[] GetNodes(string elementType, string xPath) {

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load("Ability.xml");
        XmlNodeList baseNode = xmlDoc.SelectNodes(xPath);
        List<XmlNode> list = new List<XmlNode>();

        for (int i = 0; i < baseNode.Count; i++) {
            Loop(elementType, baseNode[i], list);
        }

        return list.ToArray();    
    }

    void Loop(string elementType, XmlNode node, List<XmlNode> list) {

        if (string.Equals(node.Name, elementType))
            list.Add(node);

        for (int i = 0; i < node.ChildNodes.Count; i++)
            Loop(elementType, node.ChildNodes[i],list);
    }

    public void WriteNode(int[] path, string generalPath, string actions, string[][] args) {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load("Ability.xml");

        XmlNode baseNode = xmlDoc.DocumentElement;
        string[] paths = generalPath.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < path.Length; i++) {

            for (int j = baseNode.ChildNodes.Count; j < path[i] + 1; j++) {
                XmlNode tempNode = xmlDoc.CreateElement(paths[i]);
                baseNode.AppendChild(tempNode);
            }

            baseNode = baseNode.ChildNodes[path[i]];
        }

        string[] action = actions.Split(',');

        for (int i = 0; i < action.Length; i++)
            switch (action[i]) {
                case "INNER_TEXT":
                    baseNode.InnerText = args[i][0];
                    break;

                case "ATTRIBUTES":
                    (baseNode as XmlElement).SetAttribute(args[i][0], args[i][1]);
                    break;
            }

        xmlDoc.Save("Ability.xml");
    }

    /*string[][] ReturnData(string path) {
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.Load("Ability.xml");
    XmlNodeList nodes = xmlDoc.SelectNodes(path);

    string[][] results = new string[nodes.Count][];

    for (int i = 0; i < nodes.Count; i++) {
        results[i] = new string[nodes[i].ChildNodes.Count + 1];
        results[i][0] = nodes[i].Attributes[0].Value;

        for (int j = 1; j < results[i].Length; j++)
            results[i][j] = nodes[i].ChildNodes[j - 1].Attributes[0].Value + "," + nodes[i].ChildNodes[j - 1].InnerText;
    }

    return results;
}*/

    /*XmlNode ReturnNode(string path) {

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load("Ability.xml");
        XmlNode node;

        Debug.Log(path);
        node = xmlDoc.SelectSingleNode(path);

        if (node != null) //returns node if path already has element.
            return node;

        Debug.Log("No node exists");
        //if not, find out which part of the elements it does not have. Start building it up.
        node = xmlDoc.DocumentElement;
        string[] paths = path.Split('/');
        path = "";

        for (int i = 0; i < paths.Length; i++) {
            path += "/" + paths[i];
            XmlNode tempNode = xmlDoc.SelectSingleNode(path);

            if (tempNode == null) {

                string[] attriPath = paths[i].Split(new char[] { '@', '=', '[', ']', '\'' }, System.StringSplitOptions.RemoveEmptyEntries);
                tempNode = xmlDoc.CreateElement(attriPath[0]);

                if (attriPath.Length > 1)  //Checks for attribute
                    (tempNode as XmlElement).SetAttribute(attriPath[1], attriPath[2]);

                node.AppendChild(tempNode);
            }
            node = tempNode;
        }

        xmlDoc.Save("Ability.xml");
        return node;
    }*/
}
