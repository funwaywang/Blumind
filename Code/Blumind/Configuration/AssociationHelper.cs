using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Blumind.Core;
using Blumind.Model.Documents;
using Microsoft.Win32;

namespace Blumind.Configuration
{
    static class AssociationHelper
    {
        const string ARG_REG_BMD = "-regbmd";
        const string ARG_UNREG_BMD = "-unregbmd";

        const string DOC_TYPE_BMD_EXT = Document.Extension;
        const string DOC_TYPE_BMD_NAME = "Blumind.MindMap";
        const string DOC_TYPE_BMD_DESC = "Blumind Mind Map Document";

        public static bool TestBmdAssociation()
        {
            return TestAssociation(DOC_TYPE_BMD_EXT, DOC_TYPE_BMD_NAME, Application.ExecutablePath);
        }

        public static bool TestAssociation(string extension, string docTypeName, string appName)
        {
            string docTypeName2 = null;
            using (var extensionKey = Registry.ClassesRoot.OpenSubKey(extension))
            {
                if (extensionKey == null)
                    return false;
                docTypeName2 = extensionKey.GetValue("") as string;
                extensionKey.Close();
            }
            if (string.IsNullOrEmpty(docTypeName2) || !StringComparer.OrdinalIgnoreCase.Equals(docTypeName2, docTypeName))
                return false;

            //
            using (var docType = Registry.ClassesRoot.OpenSubKey(docTypeName2))
            {
                if (docType == null)
                    return false;

                // 如果图标设置为当前程序, 则清空它
                var icon = docType.GetSubKeyDefaultValue("DefaultIcon") as string;
                if (icon == null || !ST.IsSameFile(GetFilePathFromRegStr(icon), appName))
                    return false;

                using (var shell = docType.OpenSubKey("Shell"))
                {
                    if (shell == null)
                        return false;

                    var openWith = shell.GetSubKeyDefaultValue(@"Open\Command") as string;
                    if (!ST.IsSameFile(GetFilePathFromRegStr(openWith), appName))
                        return false;
                    //var editWith = shell.GetSubKeyDefaultValue(@"Edit\Command") as string;
                    //if (!ST.IsSameFile(editWith, appName))
                    //    return false;
                    shell.Close();
                }
                docType.Close();
            }

            return true;
        }

        public static void RegisterDocType(bool regist)
        {
            string filename = Application.ExecutablePath;
            if (File.Exists(filename))
            {
                Process process = new Process();
                process.StartInfo.FileName = filename;
                if (!regist)
                    process.StartInfo.Arguments = ARG_UNREG_BMD;
                else
                    process.StartInfo.Arguments = ARG_REG_BMD;
                process.StartInfo.Verb = "runas"; // 提升权限为 administrators
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
            }
        }

        public static void SetAssociation(string extension, string docTypeName, string docTypeDescription, string appName, string docIcon)
        {
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException("Extension");
            if (string.IsNullOrEmpty(appName))
                throw new ArgumentException("AppName");

            using (var extensionKey = Registry.ClassesRoot.CreateSubKey(extension))
            {
                extensionKey.SetValue("", docTypeName);
                extensionKey.Close();
            }

            using (var docType = Registry.ClassesRoot.CreateSubKey(docTypeName))
            {
                docType.SetValue("", docTypeDescription);

                if (string.IsNullOrEmpty(docIcon))
                    docIcon = string.Format("\"{0}\",0", appName);
                docType.CreateSubKey("DefaultIcon").SetValue("", docIcon);

                using (var shell = docType.CreateSubKey("Shell"))
                {
                    shell.CreateSubKey("Open").CreateSubKey("Command").SetValue("", "\"" + appName + "\"" + " \"%1\"");
                    //shell.CreateSubKey("Edit").CreateSubKey("Command").SetValue("", "\"" + appName + "\"" + " \"%1\"");
                    shell.Close();
                }
                docType.Close();
            }
        }

        public static void RemoveAssociation(string extension, string docTypeName, string appName)
        {
            string docTypeName2 = Registry.ClassesRoot.GetSubKeyDefaultValue(extension) as string;
            if (StringComparer.OrdinalIgnoreCase.Equals(docTypeName2, docTypeName))
            {
                Registry.ClassesRoot.DeleteSubKeyTree(extension);
            }

            //
            if (Registry.ClassesRoot.ContainsSubKey(docTypeName))
            {
                Registry.ClassesRoot.DeleteSubKeyTree(docTypeName);
            }
        }

        static string GetFilePathFromRegStr(string regStr)
        {
            if (string.IsNullOrEmpty(regStr))
                return regStr;

            var regex = new Regex("^(\"?)(.[^,\"%]*)\\1([ |,]*)");
            var m = regex.Match(regStr);
            if (m != null && m.Success)
                return m.Groups[2].Value;

            return null;
        }

        public static bool PreProcessApplicationArgs(string[] args)
        {
            if (args != null)
            {
                if (args.Contains(ARG_REG_BMD, StringComparer.OrdinalIgnoreCase))
                {
                    var icon = string.Format("\"{0}\",1", Application.ExecutablePath);
                    SetAssociation(DOC_TYPE_BMD_EXT, DOC_TYPE_BMD_NAME, DOC_TYPE_BMD_DESC, Application.ExecutablePath, icon);
                    return true;
                }
                else if (args.Contains(ARG_UNREG_BMD, StringComparer.OrdinalIgnoreCase))
                {
                    RemoveAssociation(DOC_TYPE_BMD_EXT, DOC_TYPE_BMD_NAME, Application.ExecutablePath);
                    return true;
                }
            }

            return false;
        }
    }
}
