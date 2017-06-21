using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Blumind.Configuration;

namespace Blumind.Globalization
{
    static class LanguageManage
    {
        static Language _Current;
        static List<Language> _Languages = new List<Language>();
        public static readonly string NaturalCultureName;

        public static event System.EventHandler CurrentChanged;

        static LanguageManage()
        {
            NaturalCultureName = CultureInfo.CurrentUICulture.Name;
        }

        public static Language Current
        {
            get { return LanguageManage._Current; }
            set 
            {
                if (_Current != value)
                {
                    LanguageManage._Current = value;
                    OnCurrentChanged();
                }
            }
        }

        public static Language[] Languages
        {
            get { return _Languages.ToArray(); }
        }

        private static string LanguagesDirectory
        {
            get { return Path.Combine(Application.StartupPath, "Languages"); }
        }

        public static Language GetLanguage(string id)
        {
            foreach (var lang in _Languages)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(lang.ID, id))
                    return lang;
            }

            foreach (var lang in _Languages)
            {
                if (lang.CompatibleWith(id))
                    return lang;
            }

            return null;
        }

        static void OnCurrentChanged()
        {
            if (CurrentChanged != null)
            {
                CurrentChanged(null, EventArgs.Empty);
            }
        }

        public static void Initialize()
        {
            LoadLanguages();

            var langid = Options.Current.GetString(Blumind.Configuration.OptionNames.Localization.LanguageID);
            if (string.IsNullOrEmpty(langid))
            {
                SetDefaultLanguage();
            }
            else
            {
                ChangeLanguage(langid);
            }
        }

        static void LoadLanguages()
        {
            //D.Message("LoadLanguages...");
            _Languages.Clear();

            string path = Blumind.Configuration.ProgramEnvironment.LanguagesDirectory;
            LoadLanguages(path);

            //AddLanguage(Language.LoadXml(Properties.Resources.ca_ES));
            //AddLanguage(Language.LoadXml(Properties.Resources.cs_CZ));
            //AddLanguage(Language.LoadXml(Properties.Resources.da_DK));
            //AddLanguage(Language.LoadXml(Properties.Resources.de_DE));
            AddLanguage(Language.LoadXml(Properties.Resources.en_US));
            //AddLanguage(Language.LoadXml(Properties.Resources.es_ES));
            //AddLanguage(Language.LoadXml(Properties.Resources.fr_FR));
            //AddLanguage(Language.LoadXml(Properties.Resources.hi_IN));
            //AddLanguage(Language.LoadXml(Properties.Resources.it_IT));
            //AddLanguage(Language.LoadXml(Properties.Resources.ja_JP));
            //AddLanguage(Language.LoadXml(Properties.Resources.ko_KR));
            //AddLanguage(Language.LoadXml(Properties.Resources.mr_IN));
            //AddLanguage(Language.LoadXml(Properties.Resources.mk_MK));
            //AddLanguage(Language.LoadXml(Properties.Resources.nl_NL));
            //AddLanguage(Language.LoadXml(Properties.Resources.pl_PL));
            //AddLanguage(Language.LoadXml(Properties.Resources.ru_RU));
            //AddLanguage(Language.LoadXml(Properties.Resources.sr_CS));
            //AddLanguage(Language.LoadXml(Properties.Resources.sv_SE));
            //AddLanguage(Language.LoadXml(Properties.Resources.tr_TR));
            AddLanguage(Language.LoadXml(Properties.Resources.zh_CHS));
            //AddLanguage(Language.LoadXml(Properties.Resources.zh_CHT));

            LoadLanguages(LanguagesDirectory);
            //D.Message(new string('-', 40));
        }

        private static void LoadLanguages(string directory)
        {
            //if (!Directory.Exists(directory))
            //{
            //    return;
            //}

            //string[]files = Directory.GetFiles(directory, "*.xml");
            //foreach(string filename in files)
            //{
            //    var language = new Language();
            //    if (language.LoadInfo(filename))
            //    {
            //        AddLanguage(language);
            //    }
            //}

            //D.Message("Scan languages in Folder \"{0}\"", directory);
            if (Directory.Exists(directory))
            {
                var files = Directory.GetFiles(directory, "*.xml");
                foreach (var f in files)
                {
                    //if (StringComparer.OrdinalIgnoreCase.Equals(Path.GetFileNameWithoutExtension(f), "cs.CZ"))
                    //    D.Message(string.Format("LoadLanguages() {0}", f));
                    var fn = Path.GetFileNameWithoutExtension(f);
                    if (StringComparer.OrdinalIgnoreCase.Equals("_Template", fn))    // 跳过模版
                        continue;

                    var lang = Language.LoadFile(f);
                    if (lang != null)
                        AddLanguage(lang);
                }
            }
            else
            {
                //D.Message("Folder not exists");
            }
        }

        private static void AddLanguage(Language language)
        {
            if (language != null)
            {
                if (GetLanguage(language.ID) != null)
                    GetLanguage(language.ID).Merge(language);
                else
                    _Languages.Add(language);
            }
        }

        #region Change Language
        public static void SetDefaultLanguage()
        {
            if (!ChangeLanguage(NaturalCultureName))
            {
                ChangeLanguage("en");
            }
        }

        public static bool ChangeLanguage(string id)
        {
            //D.Message(string.Format("ChangeLanguage {0}", id));
            if (string.IsNullOrEmpty(id))
                return false;

            //D.Message(string.Format("GetLanguage {0}", id));
            var language = GetLanguage(id);
            if (language != null)
            {
                //D.Message(string.Format("Load is OK, Lang {0}, Words {1}", language, language.Words.Count));
                ChangeLanguage(language);
                return true;
            }
            else
            {
                //D.Message("Load fail");
                return false;
            }
        }

        public static void ChangeLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException();

            Current = language;
        }

        #endregion

        public static void Apply(Language language, Control control)
        {
        }

        public static string GetText(string name)
        {
            if (Current != null)
                return Current[name];
            else
                return name;
        }

        public static string GetTextWithEllipsis(string name)
        {
            return GetText(name) + "...";
        }
    }
}
