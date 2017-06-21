using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Win32;

namespace Blumind.Core
{
    static class RegistryKeyExtensions
    {
        public static object GetSubKeyDefaultValue(this RegistryKey regKey, string subKeyName)
        {
            return GetSubKeyValue(regKey, subKeyName, "");
        }

        public static object GetSubKeyValue(this RegistryKey regKey, string subKeyName, string itemName)
        {
            if (regKey == null)
                throw new NullReferenceException();

            object result = null;
            using (var subKey = regKey.OpenSubKey(subKeyName))
            {
                if (subKey != null)
                {
                    result = subKey.GetValue(itemName);
                }
                subKey.Close();
            }

            return result;
        }

        public static void SetSubKeyDefaultValue(this RegistryKey regKey, string subKeyName, object value)
        {
            SetSubKeyValue(regKey, subKeyName, "", value);
        }

        public static void SetSubKeyValue(this RegistryKey regKey, string subKeyName, string itemName, object value)
        {
            if (regKey == null)
                throw new NullReferenceException();

            using (var subKey = regKey.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.SetValue))
            {
                if (subKey != null)
                {
                    subKey.SetValue(itemName, value);
                    subKey.Close();
                }
            }
        }

        public static void TryDeleteSubKey(this RegistryKey regKey, string subKeyName)
        {
            TryDeleteSubKey(regKey, subKeyName);
        }

        public static void TryDeleteSubKey(this RegistryKey regKey, string subKeyName, Predicate<RegistryKey> deleteTest)
        {
            if (regKey == null)
                throw new NullReferenceException();

            bool willDel = false;
            using (var subKey = regKey.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
            {
                if (subKey != null)
                {
                    if (deleteTest == null || deleteTest(subKey))
                        willDel = true;
                }
            }

            if (willDel)
                regKey.DeleteSubKeyTree(subKeyName);
        }

        public static bool ContainsSubKey(this RegistryKey regKey, string subKeyName)
        {
            if (regKey == null)
                throw new NullReferenceException();

            using (var subKey = regKey.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey))
            {
                return subKey != null;
            }
        }
    }
}
