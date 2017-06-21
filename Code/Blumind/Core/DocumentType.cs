using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Blumind.Controls;
using Blumind.Globalization;
using Blumind.Model.Documents;

namespace Blumind.Core
{
    class DocumentType
    {
        string _Name;
        string _TypeMime;

        public static readonly DocumentType Png;
        public static readonly DocumentType Tiff;
        public static readonly DocumentType Jpeg;
        public static readonly DocumentType Gif;
        public static readonly DocumentType Bmp;
        public static readonly DocumentType Svg;
        public static readonly DocumentType Txt;
        public static readonly DocumentType Csv;
        public static readonly DocumentType FreeMind;
        public static readonly DocumentType Blumind;
        public static readonly DocumentType Pdf;

        static DocumentType()
        {
            Png = new DocumentType("PNG",
                "image/png", 
                new string[] { ".png" },
                "Portable Network Graphics");
            Tiff = new DocumentType("TIFF", 
                "image/tiff", 
                new string[] { ".tiff" },
                "Tagged Image File Format");
            Jpeg = new DocumentType("JPEG",
                "image/jpeg", 
                new string[] { ".jpg", ".jpeg" },
                "Joint Photographic Experts Group");
            Gif = new DocumentType("GIF", 
                "image/gif",
                new string[] { ".gif" },
                "Graphics Interchange Format");
            Bmp = new DocumentType("BMP", 
                "image/bmp", 
                new string[] { ".bmp" });
            Svg = new DocumentType("SVG",
                "image/svg+xml",
                new string[] { ".svg" },
                "Scalable Vector Graphics");
            Txt = new DocumentType("Text",
                "text/plain",
                new string[] { ".txt" },
                "Text File");
            Csv = new DocumentType("CSV",
                "text/csv",
                new string[] { ".csv" });
            FreeMind = new DocumentType("FreeMind", 
                "application/freemind",
                new string[] { ".mm" });
            Blumind = new DocumentType("Blumind",
                "application/blumind",
                new string[] { Document.Extension });
            Pdf = new DocumentType("PDF",
                "application/pdf",
                new string[] { ".pdf" },
                "Portable Document Format");

            //if (Svg.Icon == null)
            //    Svg.Icon = Properties.Resources.svg;

            //if (FreeMind.Icon == null)
            //    FreeMind.Icon = Properties.Resources.freemind_butterfly;
        }

        public DocumentType()
        {
        }

        public DocumentType(string name, string typeMime, IEnumerable<string> extensions, string desc = null)
        {
            Name = name;
            TypeMime = typeMime;
            Extensions = extensions;
            Description = desc;

            Icon = IconExtractor.ExtractSmallIconByExtension(DefaultExtension);
        }

        public string Name
        {
            get { return _Name; }
            private set { _Name = value; }
        }

        public string TypeMime
        {
            get { return _TypeMime; }
            private set { _TypeMime = value; }
        }

        public Image Icon { get; private set; }

        public IEnumerable<string> Extensions { get; private set; }

        public string ExtensionsToString()
        {
            var sb = new StringBuilder();
            foreach (var ext in Extensions)
            {
                if (sb.Length > 0)
                    sb.Append(";");
                sb.Append(ext);
            }

            return sb.ToString();
        }
        
        public string DefaultExtension
        {
            get
            {
                if (!Extensions.IsNullOrEmpty())
                    return Extensions.First();
                else
                    return null;
            }
        }

        public string Description { get; set; }

        public string FileDialogFilter
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var ext in Extensions)
                {
                    if (sb.Length > 0)
                        sb.Append(";");
                    sb.Append("*" + ext);
                }

                return string.Format("{0} ({1})|{1}", LanguageManage.GetText(Name), sb.ToString()); 
            }
        }

        public static DocumentType[] GetDocumentTypes()
        {
            return new DocumentType[] {
                 Png,
                 Tiff,
                 Jpeg,
                 Gif,
                 Bmp,
                 Svg,
                 FreeMind,
                 Txt,
                 Csv,
                 Blumind,
                 Pdf,
            };
        }

        public override string ToString()
        {
            return Name;
        }

        public static DocumentType FindDocumentType(string extension)
        {
            return GetDocumentTypes().Find(dt => dt.Extensions.Exists(dex => StringComparer.OrdinalIgnoreCase.Equals(extension, dex)));
        }

        public static DocumentType[] GetImageTypes()
        {
            return new DocumentType[] {
                 Jpeg,
                 Png,
                 Gif,
                 Bmp,
                 Tiff,
            };
        }

        public static string GetImageTypeFilters()
        {
            var sb = new StringBuilder();
            foreach (var dt in GetImageTypes())
            {
                if (dt.Extensions.IsNullOrEmpty())
                    continue;

                if(sb.Length > 0)
                    sb.Append("|");

                sb.AppendFormat("{0}  ({1})|{2}",
                    Lang._(dt.Name),
                    dt.Extensions.Select(e => "*" + e).JoinString("; "),
                    dt.Extensions.Select(e => "*" + e).JoinString(";"));
            }

            return sb.ToString();
        }

        public static ImageFormat GetImageFormat(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
                throw new ArgumentException();

            switch (fileExtension.ToLower())
            {
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".emf":
                    return ImageFormat.Emf;
                case ".exif":
                    return ImageFormat.Exif;
                case ".gif":
                    return ImageFormat.Gif;
                case ".icon":
                    return ImageFormat.Icon;
                case ".jpeg":
                case ".jpg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
                case ".tiff":
                    return ImageFormat.Tiff;
                case ".wmf":
                    return ImageFormat.Wmf;
                default:
                    return null;
            }
        }
    }
}
