using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace Blumind.Controls
{
    static class UIColorThemeManage
    {
        public const string ColorThemeFileExtension = ".xml";
        static List<UIColorTheme> AllThemes = new List<UIColorTheme>();

        public static UIColorTheme Default
        {
            get
            {
                return new UIColorTheme("Default",
                    Color.Black,
                    Color.White,
                    Color.FromArgb(0x63, 0x9A, 0x05),
                    Color.FromArgb(0xB7, 0xC8, 0xE2),
                    Color.FromArgb(0xE6, 0xED, 0xF6));
            }
        }

        public static UIColorTheme System
        {
            get
            {
                return new UIColorTheme(
                    "System",
                    SystemColors.ControlDarkDark,
                    SystemColors.Window,
                    SystemColors.ActiveCaption,
                    SystemColors.Control,
                    SystemColors.ControlLight);
            }
        }

        public static UIColorTheme GetNamedTheme(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                foreach (var t in All)
                    if (StringComparer.OrdinalIgnoreCase.Equals(t.Name, name))
                        return t;
            }

            return null;
        }

        public static void Initialize()
        {
            AllThemes.Clear();

            string path = Blumind.Configuration.ProgramEnvironment.UIThemesDirectory;
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, string.Format("*{0}", ColorThemeFileExtension));
                foreach (var f in files)
                {
                    var fn = Path.GetFileNameWithoutExtension(f);
                    if (StringComparer.OrdinalIgnoreCase.Equals("_Template", fn))    // 跳过模版
                        continue;

                    var t = UIColorTheme.Load(f);
                    if (t != null)
                        AllThemes.Add(t);
                }
            }
        }

        public static void SaveAll(string path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (System.Exception ex)
                {
                    Program.MainForm.ShowMessage(ex);
                    return;
                }
            }

            foreach (var ct in All)
            {
                if (string.IsNullOrEmpty(ct.Name))
                    continue;
                var filename = Path.Combine(path, string.Format("{0}{1}", ct.Name, ColorThemeFileExtension));
                try
                {
                    ct.Save(filename);
                }
                catch (System.Exception ex)
                {
                    Program.MainForm.ShowMessage(ex);
                }
            }
        }

        public static IEnumerable<UIColorTheme> All
        {
            get
            {
                foreach (var t in AllThemes)
                    yield return t;
            }
        }
    }
}
