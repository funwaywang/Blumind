using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Blumind.Core;

namespace Blumind.Canvas.Svg
{
    class SvgDocument
    {
        XmlDocument Dom;
        int _Width;
        int _Height;
        string _Title;
        Color _BackColor;
        Color _ForeColor;

        public SvgDocument()
        {
            Width = 800;
            Height = 600;

            InitializeDocument();
        }

        public SvgDocument(int width, int height)
        {
            Width = width;
            Height = height;

            InitializeDocument();
        }

        public string Version
        {
            get { return "1.1"; }
        }

        [DefaultValue(800)]
        public int Width
        {
            get { return _Width; }
            set 
            {
                if (_Width != value)
                {
                    _Width = value;
                    OnWidthChanged();
                }
            }
        }

        [DefaultValue(600)]
        public int Height
        {
            get { return _Height; }
            set 
            {
                if (_Height != value)
                {
                    _Height = value;
                    OnHeightChanged();
                }
            }
        }

        public Color BackColor
        {
            get { return _BackColor; }
            set 
            {
                if (_BackColor != value)
                {
                    _BackColor = value;
                    OnBackColorChanged();
                }
            }
        }

        public Color ForeColor
        {
            get { return _ForeColor; }
            set 
            {
                if (_ForeColor != value)
                {
                    _ForeColor = value;
                    OnForeColorChanged();
                }
            }
        }

        [DefaultValue(null)]
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    OnTitleChanged();
                }
            }
        }

        [Browsable(false)]
        public XmlElement DocumentElement
        {
            get { return Dom.DocumentElement; }
        }

        public XmlElement Defines { get; private set; }

        public XmlElement Styles { get; private set; }

        public XmlElement Canvas { get; private set; }

        void OnWidthChanged()
        {
            if(Dom != null)
                Dom.DocumentElement.SetAttribute("width", Width.ToString());
        }

        void OnHeightChanged()
        {
            if (Dom != null)
                Dom.DocumentElement.SetAttribute("height", Height.ToString());
        }

        void OnTitleChanged()
        {
            if (Dom != null)
                ST.WriteTextNode(Dom.DocumentElement, "title", Title);
        }

        void OnBackColorChanged()
        {
        }

        void OnForeColorChanged()
        {
        }

        void InitializeDocument()
        {
            XmlDocument dom = new XmlDocument();
            dom.XmlResolver = null;
            dom.CreateDocumentFragment();

            // declaration
            XmlDeclaration declaration = dom.CreateXmlDeclaration("1.0", "UTF-8", "no");
            dom.AppendChild(declaration);

            // root
            XmlElement svg = dom.CreateElement("svg");
            dom.AppendChild(svg);
            svg.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
            svg.SetAttribute("xmlns:xlink", "http://www.w3.org/1999/xlink");
            svg.SetAttribute("version", Version);
            //svg.SetAttribute("creator", ProductInfo.Title);
            //svg.SetAttribute("creator_version", ProductInfo.Version);
            svg.SetAttribute("width", Width.ToString());
            svg.SetAttribute("height", Height.ToString());
            svg.SetAttribute("viewBox", string.Format("{0} {1} {2} {3}", 0, 0, Width, Height));

            // comment
            var comment = dom.CreateComment("Create by Blumind, you can download it free from http://blumind.org ");
            svg.AppendChild(comment);

            // title
            var titleElement = dom.CreateElement("title");
            titleElement.InnerText = Title;
            svg.AppendChild(titleElement);

            // defs
            Defines = dom.CreateElement("defs");
            svg.AppendChild(Defines);

            // style
            Styles = dom.CreateElement("style");
            Defines.AppendChild(Styles);

            // canvas
            Canvas = dom.CreateElement("g");
            Canvas.SetAttribute("id", "canvas");
            svg.AppendChild(Canvas);

            Dom = dom;
        }

        public void Save(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException();

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                Save(stream);
            }
        }

        public void Save(FileStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();

            OnBeforeSave();
            Dom.Save(stream);
        }

        protected virtual void OnBeforeSave()
        {
            BuildDocumentStyle();
        }

        void BuildDocumentStyle()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("svg {");
            if (!BackColor.IsEmpty)
                sb.AppendFormat("background-color:{0};", ST.ToWebColor(BackColor));
            if (!ForeColor.IsEmpty)
                sb.AppendFormat("color:{0}", ST.ToWebColor(ForeColor));
            sb.AppendLine("}");

            //var style_data = Dom.CreateCDataSection(sb.ToString());
            ///Styles.AppendChild(style_data);
            Styles.InnerText = sb.ToString();
        }

        public XmlElement CreateElement(string name)
        {
            return Dom.CreateElement(name);
        }

        public XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
        {
            return Dom.CreateAttribute(prefix, localName, namespaceURI);
        }
    }
}
