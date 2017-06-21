using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Blumind.Core.Win32Apis
{
    class OSHelper
    {
        public static IntPtr IntPtrAlloc<T>(T param)
        {
            IntPtr retval = Marshal.AllocHGlobal(Marshal.SizeOf(param));
            Marshal.StructureToPtr(param, retval, false);
            return (retval);
        }

        public static IntPtr IntPtrAlloc(byte[] param)
        {
            IntPtr retval = Marshal.AllocHGlobal(param.Length);
            Marshal.Copy(param, 0, retval, param.Length);
            return (retval);
        }

        public static T IntPtrToStruct<T>(IntPtr ptr)
            where T : struct
        {
            var obj = Marshal.PtrToStructure(ptr, typeof(T));
            if (obj is T)
                return (T)obj;
            else
                return default(T);
        }

        public static void IntPtrFree(IntPtr preAllocated)
        {
            if (IntPtr.Zero == preAllocated) throw (new Exception("Go Home"));
            Marshal.FreeHGlobal(preAllocated); preAllocated = IntPtr.Zero;
        }

        public static byte[] GetBuffer(IntPtr intPtr, int len)
        {
            byte[] buffer = new byte[len];
            Marshal.Copy(intPtr, buffer, 0, len);
            return buffer;
        }
    }
}
