using System;
using System.Collections.Generic;
using System.Xml;
using Blumind.Core;
using System.Collections;
using Blumind.Globalization;

namespace Blumind.Model
{
    class Thanks
    {
        public class ThankItem : IComparable
        {
            public string Name { get; set; }
            public string Jobs { get; set; }
            public string Email { get; set; }

            #region IComparable 成员

            public int CompareTo(object obj)
            {
                if (obj is ThankItem)
                    return String.Compare(Name, ((ThankItem)obj).Name, true);
                else
                    throw new ArgumentNullException();
            }

            #endregion
        }

        public static ThankItem[] GetList()
        {
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(Properties.Resources.thanks);
            List<ThankItem> list = new List<ThankItem>();
            Hashtable peoples = new Hashtable(StringComparer.OrdinalIgnoreCase);

            foreach (var l in LanguageManage.Languages)
            {
                foreach (var a in l.Authors)
                {
                    if (string.IsNullOrEmpty(a.Name) || peoples.ContainsKey(a.Name))
                        continue;
                    ThankItem ti = new ThankItem();
                    ti.Name = a.Name;
                    ti.Email = a.Email;
                    ti.Jobs = string.Format("Translation:{0}", l.Name);
                    list.Add(ti);
                    peoples.Add(ti.Name, null);
                }
            }

            foreach (XmlElement node in dom.DocumentElement.SelectNodes("people"))
            {
                ThankItem item = new ThankItem();
                item.Name = node.GetAttribute("name");
                item.Jobs = node.GetAttribute("jobs");
                item.Email = node.GetAttribute("email");
                if (!peoples.ContainsKey(item.Name))
                {
                    peoples.Add(item.Name, null);
                    list.Add(item);
                }
            }

            list.Sort();
            return list.ToArray();
        }
    }
}
