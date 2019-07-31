using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Blumind.Canvas;
using Blumind.Canvas.Svg;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Controls.OS;
using Blumind.Core;
using Blumind.Core.Win32Apis;
using Blumind.Dialogs;
using Blumind.Globalization;
using Blumind.Model.Widgets;

namespace Blumind
{
    static class Program
    {
        public const long OPEN_FILES_MESSAGE = 0x0999;

        public static MainForm MainForm { get; private set; }

        public static bool IsRunTime { get; private set; }

        [STAThread]
        static void Main(params string[] args)
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                Console.WriteLine("ya existe la aplicación en ejecución");
                return;
            }
            if (PreProcessApplicationArgs(args))
                return;

            // 如果需要打开文件, 偿试寻找是否有已经存在的应用实例打开
            if (!args.IsNullOrEmpty() && TryOpenByOtherInstance(args))
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IsRunTime = true;
            
            Options.Current.OpitonsChanged += Current_OpitonsChanged;
            Options.Current.Load(args);
            
            //D.Message("ProgramEnvironment.RunMode is {0}", ProgramEnvironment.RunMode);
            //D.Message("ApplicationDataDirectory is {0}", ProgramEnvironment.ApplicationDataDirectory);
            //D.Message(new string('-', 40));

            UIColorThemeManage.Initialize();
            //D.Message("LanguageManage.Initialize");
            LanguageManage.Initialize();
            RecentFilesManage.Default.Initialize();

            Current_OpitonsChanged(null, EventArgs.Empty);
            if (!IsUserAdministrator())
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Assembly.GetEntryAssembly().CodeBase;

                foreach (string arg in args)
                {
                    proc.Arguments += String.Format("\"{0}\" ", arg);
                }

                proc.Verb = "runas";
                
                try
                {
                    Process.Start(proc);
                }
                catch
                {
                    Console.WriteLine("This application requires elevated credentials in order to operate correctly!");
                }
            }
#if DEBUG
            MainForm = new MainForm(args);
            Application.Run(MainForm);
#else
            MainForm = new MainForm(args);
            Application.Run(MainForm);
#endif
            Blumind.Configuration.Options.Current.Save();
        }

        static bool TryOpenByOtherInstance(string[] args)
        {
            var files = args.Where(arg => !arg.StartsWith("-")).ToArray();
            if (files.IsEmpty())
                return false;

            var name = Process.GetCurrentProcess().ProcessName;
            var otherInstances = Process.GetProcessesByName(name)
                .Where(inst => inst != Process.GetCurrentProcess() && inst.MainWindowHandle != IntPtr.Zero)
                .ToArray();
            if (!otherInstances.IsNullOrEmpty())
            {
                var inst = otherInstances.First();
                var data = Encoding.UTF8.GetBytes(files.JoinString(";"));
                var buffer = OSHelper.IntPtrAlloc(data);

                var cds = new COPYDATASTRUCT();
                cds.dwData = new IntPtr(OPEN_FILES_MESSAGE);
                cds.cbData = data.Length;
                cds.lpData = buffer;
                var cbs_buffer = OSHelper.IntPtrAlloc(cds);
                IntPtr result = User32.SendMessage(inst.MainWindowHandle, WinMessages.WM_COPYDATA, IntPtr.Zero, cbs_buffer);
                OSHelper.IntPtrFree(cbs_buffer);
                OSHelper.IntPtrFree(buffer);

                return result != IntPtr.Zero;
            }

            return false;
        }

        static bool PreProcessApplicationArgs(string[] args)
        {
            if (AssociationHelper.PreProcessApplicationArgs(args))
                return true;

            return false;
        }

        static void Current_OpitonsChanged(object sender, EventArgs e)
        {
            UITheme.Default.Colors = UIColorThemeManage.GetNamedTheme(CommonOptions.Appearances.UIThemeName);
            LanguageManage.ChangeLanguage(CommonOptions.Localization.LanguageID);

            //D.Message(string.Format("Test: Options => {0}", Lang._("Options")));
        }
        static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }
    }
}
