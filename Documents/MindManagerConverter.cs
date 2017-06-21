public class MindManagerConverter
{
    // Fields
    private Dictionary<string, string> ids = new Dictionary<string, string>();
    private Map map = new Map();
    private readonly string unzipPath = (Path.GetTempPath() + "MindManager/");

    // Methods
    private string AddNodeToMap(XmlNode node, string nodeName)
    {
        if (node.ParentNode.Name == "OneTopic")
        {
            this.map.ChangeNodeName("$MAIN", nodeName);
            return "$MAIN";
        }
        return this.map.AddNode(nodeName).Id;
    }

    public Map Convert(string url)
    {
        Directory.CreateDirectory(this.unzipPath);
        string path = this.unzipPath + Path.GetFileName(url);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        File.Copy(url, path);
        this.Unzip(path);
        if (File.Exists(this.unzipPath + "Document.xml"))
        {
            XmlDocument document = new XmlDocument();
            document.Load(this.unzipPath + "Document.xml");
            document.InnerXml = document.InnerXml.Replace("ap:", "");
            this.ParseNode(document.DocumentElement, null);
            Directory.Delete(this.unzipPath, true);
        }
        return this.map;
    }

    private static string GetNodeName(XmlNode node)
    {
        XmlNode node2 = node.SelectSingleNode("Text/@PlainText");
        string str = string.Empty;
        if (node2 != null)
        {
            str = node2.Value;
        }
        if (!string.IsNullOrEmpty(str))
        {
            return str;
        }
        if (node.ParentNode.Name == "OneTopic")
        {
            return "Central Topic";
        }
        if (node.ParentNode.Name == "FloatingTopics")
        {
            return "Floating Topic";
        }
        return "Sub Topic";
    }

    private void ParseNode(XmlNode node, string parentId)
    {
        string str = null;
        if (node.Name == "Topic")
        {
            if ((node.ParentNode.Name == "FloatingTopics") && (parentId != null))
            {
                return;
            }
            str = this.AddNodeToMap(node, GetNodeName(node));
            if (node.Attributes["OId"] != null)
            {
                this.ids.Add(node.Attributes["OId"].Value, str);
            }
            if (parentId != null)
            {
                this.map.AddLink(parentId, str);
            }
            this.SetSpace(str, node, parentId);
            this.SetTextColor(str, node);
            this.SetFillColor(str, node, parentId);
            this.SetImage(str, node);
            this.SetNote(str, node);
        }
        else if (node.Name == "SubTopics")
        {
            str = parentId;
        }
        else if ((node.Name == "FloatingTopics") && (parentId != "$MAIN"))
        {
            str = parentId;
        }
        else if (node.Name == "Relationship")
        {
            XmlNode node2 = node.SelectSingleNode("ConnectionGroup[@Index='0']//ObjectReference/@OIdRef");
            XmlNode node3 = node.SelectSingleNode("ConnectionGroup[@Index='1']//ObjectReference/@OIdRef");
            if ((((node2 != null) && (node3 != null)) && this.ids.ContainsKey(node2.Value)) && this.ids.ContainsKey(node3.Value))
            {
                this.map.AddLink(this.ids[node2.Value], this.ids[node3.Value]);
            }
        }
        foreach (XmlNode node4 in node.ChildNodes)
        {
            this.ParseNode(node4, str);
        }
    }

    private void SetFillColor(string id, XmlNode node, string parentId)
    {
        string fillColor = string.Empty;
        XmlNode node2 = node.SelectSingleNode("Color/@FillColor");
        if (node2 != null)
        {
            fillColor = "#" + node2.Value;
        }
        else if ((parentId == "$MAIN") || (parentId == null))
        {
            fillColor = Style.GetRandomColor().ToString();
        }
        else if (id != "$MAIN")
        {
            fillColor = this.map.GetNodeById(parentId).NodeStyle.FillColor;
        }
        if (!string.IsNullOrEmpty(fillColor))
        {
            this.map.ChangeNodeFillColor(id, fillColor);
        }
    }

    private void SetImage(string id, XmlNode node)
    {
        XmlNode node2 = node.SelectSingleNode("OneImage/Image/ImageData/node()");
        if (node2 != null)
        {
            string str = node2.InnerText.Substring(node2.InnerText.IndexOf("://") + 3);
            if (File.Exists(this.unzipPath + str))
            {
                string path = this.unzipPath + str;
                FileStream stream = File.OpenRead(path);
                stream.Seek(0L, SeekOrigin.Current);
                this.map.SetNodeResource(id, stream, Path.GetExtension(path));
                stream.Close();
                if (this.map.GetNodeById(id).Name == "Floating Topic")
                {
                    this.map.ChangeNodeName(id, "Image");
                }
            }
        }
    }

    private void SetNote(string id, XmlNode node)
    {
        XmlNode node2 = node.SelectSingleNode("NotesGroup/NotesXhtmlData");
        if (!((node2 == null) || string.IsNullOrEmpty(node2.InnerXml)))
        {
            this.map.ChangeNodeNote(id, node2.InnerXml);
        }
    }

    private void SetSpace(string id, XmlNode node, string parentId)
    {
        string name = string.Empty;
        if (node.SelectSingleNode("OneBoundary") != null)
        {
            name = this.map.GetNodeById(id).Name;
        }
        else
        {
            MapData.NodesRow nodeById = this.map.GetNodeById(parentId);
            if (!((nodeById == null) || nodeById.IsSpaceEmpty))
            {
                name = nodeById.SpaceName;
            }
        }
        if (!string.IsNullOrEmpty(name))
        {
            this.map.ChangeNodeSpace(id, name);
        }
    }

    private void SetTextColor(string id, XmlNode node)
    {
        XmlNode node2 = node.SelectSingleNode("Text/Font/@Color");
        if (node2 != null)
        {
            this.map.ChangeNodeTextColor(id, "#" + node2.Value);
        }
    }

    private void Unzip(string url)
    {
        ZipEntry entry;
        ZipInputStream stream = new ZipInputStream(File.OpenRead(url));
        while ((entry = stream.GetNextEntry()) != null)
        {
            bool flag;
            string directoryName = Path.GetDirectoryName(entry.Name);
            string fileName = Path.GetFileName(entry.Name);
            Directory.CreateDirectory(this.unzipPath + directoryName);
            if (!(fileName != string.Empty))
            {
                goto Label_00BA;
            }
            FileStream stream2 = File.Create(this.unzipPath + entry.Name);
            int count = 0x800;
            byte[] buffer = new byte[count];
            goto Label_00AC;
        Label_007C:
            count = stream.Read(buffer, 0, buffer.Length);
            if (count <= 0)
            {
                goto Label_00B1;
            }
            stream2.Write(buffer, 0, count);
        Label_00AC:
            flag = true;
            goto Label_007C;
        Label_00B1:
            stream2.Close();
        Label_00BA:;
        }
        stream.Close();
    }
}


