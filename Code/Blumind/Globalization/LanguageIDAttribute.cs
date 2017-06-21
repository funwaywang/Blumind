using System;
using System.Collections.Generic;
using System.Text;

namespace Blumind.Globalization
{
    public class LanguageIDAttribute : Attribute
    {
        private string _LanguageID;

        public LanguageIDAttribute(string languageID)
        {
            LanguageID = languageID;
        }

        public string LanguageID
        {
            get { return _LanguageID; }
            set { _LanguageID = value; }
        }

        public string GetText()
        {
            if (LanguageManage.Current != null)
                return LanguageManage.Current.GetText(LanguageID);
            else
                return LanguageID;
        }
    }
}
