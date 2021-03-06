using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Linq;

public class LoadFromXML : MonoBehaviour
{

    public TextAsset XMLFile;
    public bool ShowDebugs;
    GameObject /*GO, child_gameobj,*/ gc;
    XmlNode ProjectNode;
    GameObject MainNode;

    string Translations = "Translations" , VertexIndices = "VertexIndices", Vertices = "Vertices";

    // Use this for initialization
    void Start()
    {

        if (XMLFile)
        {
            //Debug.Log(" Found the file");
            string data = XMLFile.text;
            CreateHierarchyFromXML(data);
        }
        else
            if (ShowDebugs) Debug.Log("File not found!"); return;


    }

    public void CreateHierarchyFromXML(string xmldata)
    {
        XmlDocument xmldoc = new XmlDocument();

        xmldoc.LoadXml(xmldata);

        XmlNodeList xmlnodes = xmldoc.ChildNodes;

        foreach (XmlNode xn in xmlnodes)
        {
            if (xn.Name != "xml" && xn.Attributes.GetNamedItem("Type").Value == "IfcProject")
            {
                ProjectNode = xn;
                string name = xn.Attributes.GetNamedItem("Name").Value;
            }

        }
        GameObject gameobj = CreateGO(ProjectNode);

    }

    public GameObject CreateGO(XmlNode node)
    {
        if (node.Attributes.GetNamedItem("CreateGameObject").Value == "True")
        {
            GameObject GO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GO.name = node.Attributes.GetNamedItem("Name").Value + "[" + node.Attributes.GetNamedItem("ID").Value + "]";


            if (node.HasChildNodes)
            {
                foreach (XmlNode cn in node.ChildNodes)

                {
                    
                    if (cn.Name.Equals(Translations))
                    {
                        getandsetTranslations( cn, GO);
                        
                    }

                    else if (cn.Name.Equals(VertexIndices))
                    {
                        getVertexIndices(cn, node);
                        
                    }

                    else if (cn.Name.Equals(Vertices))
                    {
                        getVertices(cn, node);
                        
                    }

                    else if (node.Attributes.GetNamedItem("CreateGameObject").Value == "True")
                    {
                        GameObject child_gameobj = CreateGO(cn);
                        if (child_gameobj != null)
                        {
                            child_gameobj.transform.parent = GO.transform;
                        }
                    }

                }
            }

            return GO;
        }
        else
            return null;
    }

    public void getandsetTranslations(XmlNode childnode, GameObject G_O)
    {
        if (childnode.HasChildNodes)
        {
            Vector3 position = new Vector3((float)XmlConvert.ToDouble(childnode.ChildNodes[0].InnerText), (float)XmlConvert.ToDouble(childnode.ChildNodes[2].InnerText), (float)XmlConvert.ToDouble(childnode.ChildNodes[1].InnerText));
            if (ShowDebugs) Debug.Log("Position beingset: " + position);
            G_O.transform.position = position;
        }
        else
            { if (ShowDebugs) Debug.Log("NO Translation Info Found!!"); }
    }

    public int[] getVertexIndices(XmlNode childnode, XmlNode ParentNode)
    {
        if (childnode.HasChildNodes)
        {
            int[] triangles = new int[XmlConvert.ToInt32(childnode.Attributes.GetNamedItem("CountOfVertexIndices").Value)];
            string[] vertsfortris = new string[XmlConvert.ToInt32(childnode.Attributes.GetNamedItem("CountOfVertexIndices").Value)];
            foreach (XmlNode c in childnode.ChildNodes)
            {
                if (c.Name.Equals("V_Indices"))
                {
                    string indices_v = c.InnerText;
                    if (ShowDebugs) Debug.Log("====================================================================================== \n" + "The vertex_Indices for " + "'" + ParentNode.Name + "' " + "are: [" + indices_v + "]");
                    string[] ind_v = indices_v.Split(',');
                    for (int i = 0; i < ind_v.Length; i++)
                    {
                        triangles[i] = XmlConvert.ToInt32(ind_v[i]);
                        vertsfortris[i] = (ind_v[i]);

                    }
                    if (ShowDebugs) Debug.Log("triangles array for : " + "'" + ParentNode.Name + "' " + string.Format("[{0}]", string.Join(" ", vertsfortris)) + "\n ========================================" + (ind_v.Length) + "=============================================");
                }

            }
            return triangles;
        }
        else
        {
            if (ShowDebugs) Debug.Log("NO VertexIndices Info Found!!");
            return null;
        }
    }

    public Vector3[] getVertices(XmlNode childnode, XmlNode ParentNode)
    {
        if (childnode.HasChildNodes)
        {
            Vector3[] vertices = new Vector3[XmlConvert.ToInt32(childnode.Attributes.GetNamedItem("CountOfVertices").Value)];
            //Debug.Log("No  of vertices in "+ node.Name + ": "+ cn.ChildNodes.Count);
            for (int i = 0; i < childnode.ChildNodes.Count; i++)
            {
                if (childnode.ChildNodes[i].HasChildNodes)
                {
                    vertices[i].x = (float)XmlConvert.ToDouble(childnode.ChildNodes[i].ChildNodes[0].InnerText);
                    vertices[i].y = (float)XmlConvert.ToDouble(childnode.ChildNodes[i].ChildNodes[1].InnerText);
                    vertices[i].z = (float)XmlConvert.ToDouble(childnode.ChildNodes[i].ChildNodes[2].InnerText);
                    if (ShowDebugs) Debug.Log(ParentNode.Attributes.GetNamedItem("ID").Value.ToString() + "_" + childnode.ChildNodes[i].Name + "_" + childnode.ChildNodes[i].Attributes.GetNamedItem("ID").Value + "_" + childnode.ChildNodes[i].ChildNodes[0].Name + "= " + childnode.ChildNodes[i].ChildNodes[0].InnerText + "|vs| " + vertices[i].x);
                    if (ShowDebugs) Debug.Log(ParentNode.Attributes.GetNamedItem("ID").Value.ToString() + "_" + childnode.ChildNodes[i].Name + "_" + childnode.ChildNodes[i].Attributes.GetNamedItem("ID").Value + "_" + childnode.ChildNodes[i].ChildNodes[1].Name + "= " + childnode.ChildNodes[i].ChildNodes[1].InnerText + "|vs| " + vertices[i].y);
                    if (ShowDebugs) Debug.Log(ParentNode.Attributes.GetNamedItem("ID").Value.ToString() + "_" + childnode.ChildNodes[i].Name + "_" + childnode.ChildNodes[i].Attributes.GetNamedItem("ID").Value + "_" + childnode.ChildNodes[i].ChildNodes[2].Name + "= " + childnode.ChildNodes[i].ChildNodes[2].InnerText + "|vs| " + vertices[i].z);

                }
                else
                { if (ShowDebugs) Debug.Log("Couldn't find the Vertex info"); }
            }
            /*********************************************************************************/
            if (ShowDebugs) Debug.Log("No  of vertices in " + ParentNode.Name + ": " + vertices.Length + "\n-------------------------------------------------------------");
            foreach (Vector3 v in vertices)
            {
                if (ShowDebugs) Debug.Log(ParentNode.Name + "_" + v);
            }
            /*********************************************************************************/
            return vertices;
        }
        else
        {
            if (ShowDebugs) Debug.Log("NO Vertices Info Found!!");
            return null;
        }
    }
}

