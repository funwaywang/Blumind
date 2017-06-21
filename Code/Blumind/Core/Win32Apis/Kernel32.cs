using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Blumind.Controls.OS
{
    public static class Kernel32
    {
        const string DllName = "Kernel32";

        [DllImport(DllName)]
        public static extern Int32 GetLastError();

//        [DllImport(DllName)]
//        public static extern int WideCharToMultiByte(uint CodePage,            // code page
//  uint dwFlags,            // performance and mapping flags
//  char lpWideCharStr,    // wide-character string
//  int cchWideChar,          // number of chars in string.
//  StringBuilder lpMultiByteStr,     // buffer for new string
//  int cbMultiByte,          // size of buffer
//  LPCSTR lpDefaultChar,     // default for unmappable chars
//  LPBOOL lpUsedDefaultChar  // set when default char used
//);
        [DllImport(DllName)]
        public static extern bool Beep(uint dwFreq, uint dwDuration);

        [DllImport(DllName)]
        public static extern UIntPtr GlobalSize(IntPtr hMem);

        [DllImport(DllName)]
        public static extern IntPtr GlobalLock(IntPtr hMem);
    }
}
