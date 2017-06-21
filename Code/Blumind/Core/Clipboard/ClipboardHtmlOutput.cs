using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Blumind.Core
{
    class ClipboardHtmlOutput
    {
        public Double Version { get; private set; }
        public byte[] InputData { get; private set; }
        //public String Input { get; private set; }
        public string Source { get; private set; }
        //public String Html { get { return Input.Substring(startHTML, (endHTML - startHTML)); } }

        int startHTML;
        int endHTML;
        int startFragment;
        int endFragment;

        public String Html
        {
            get
            {
                return ExtractText(startHTML, Math.Min(endHTML - startHTML, InputData.Length - startHTML));
            }
        }

        public String Fragment
        {
            get
            {
                return ExtractText(startFragment, (endFragment - startFragment));
            }
        }

        string ExtractText(int startPos, int length)
        {
            if (InputData == null
                || startPos >= InputData.Length
                || startPos + length >= InputData.Length)
                return null;

            return Encoding.UTF8.GetString(InputData, startPos, length);
        }

        public static ClipboardHtmlOutput FromClipboard()
        {
            if (!Clipboard.ContainsText(TextDataFormat.Html))
                return null;

            var buffer = ClipboardHelper.GetClipboardData(DataFormats.Html);
            return ParseData(buffer);
        }

        static ClipboardHtmlOutput ParseData(byte[] buffer)
        {
            var html = new ClipboardHtmlOutput();
            html.InputData = buffer;

            string text = Encoding.Default.GetString(buffer);
            //string pattern = @"Version:(?<version>[0-9]+(?:\.[0-9]*)?).+StartHTML:(?<startH>\d*).+EndHTML:(?<endH>\d*).+StartFragment:(?<startF>\d+).+EndFragment:(?<endF>\d*).+SourceURL:(?<source>f|ht{1}tps?://[-a-zA-Z0-9@:%_\+.~#?&//=]+)";
            string pattern = @"Version:(?<version>[0-9]+(?:\.[0-9]*)?)[\s\S]*?StartHTML:(?<startH>\d*)[\s\S]*?EndHTML:(?<endH>\d*)[\s\S]*?StartFragment:(?<startF>\d+)[\s\S]*?EndFragment:(?<endF>\d*)[\s\S]*?";
            var match = Regex.Match(text, pattern, RegexOptions.Singleline);

            if (match.Success)
            {
                try
                {
                    //html.Input = text;
                    html.Version = Double.Parse(match.Groups["version"].Value, CultureInfo.InvariantCulture);
                    html.Source = match.Groups["source"].Value;
                    html.startHTML = int.Parse(match.Groups["startH"].Value);
                    html.endHTML = int.Parse(match.Groups["endH"].Value);
                    html.startFragment = int.Parse(match.Groups["startF"].Value);
                    html.endFragment = int.Parse(match.Groups["endF"].Value);
                }
                catch (Exception)
                {
                    return null;
                }
                return html;
            }
            return null;
        }

        public static string Package(string html)
        {
            const string Header = @"Version:1.0
StartHTML:{000000}
EndHTML:{111111}
StartFragment:{222222}
EndFragment:{333333}
SourceURL:
";
            const string HtmlHead = @"<Html><Head><Title>HTML clipboard</Title></Head><Body><!--StartFragment-->";
            const string HtmlFoot = @"<!--EndFragment--></Body></Html>";

            var content = Encoding.Default.GetBytes(html);
            var bl = content.Length;
            var sh = Header.Length;
            var sf = sh + HtmlHead.Length;
            var ef = sf + bl;
            var eh = sf + HtmlFoot.Length;

            return Header.Replace("{000000}", sh.ToString("D8"))
                .Replace("{111111}", eh.ToString("D8"))
                .Replace("{222222}", sf.ToString("D8"))
                .Replace("{333333}", ef.ToString("D8"))
                + HtmlHead
                + html
                + HtmlFoot;

            //var buffer = new List<byte>();
            //buffer.AddRange(Encoding.Default.GetBytes(header));
            //buffer.AddRange(Encoding.Default.GetBytes(HtmlHead));
            //buffer.AddRange(content);
            //buffer.AddRange(Encoding.Default.GetBytes(HtmlFoot));
            //return buffer.ToArray();
        }
    }
}
