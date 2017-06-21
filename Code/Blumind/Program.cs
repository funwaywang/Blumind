using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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

#if DEBUG
//            NotesWidget nw = new NotesWidget()
//            {
//                Remark = @"
//<P><STRONG>发布时间：<SPAN>2014年4月29日</SPAN> </STRONG></P>
//<DIV>
//<P>美国总统奥巴马8天4国的亚太之行，在今天画上句点。美国与日本、韩国、马来西亚以及<WBR>&shy;
//菲律宾等亚太盟国的关系是否更加紧密，亚太局势是否取得平衡？奥巴马极力推动的跨太平<WBR>&shy;洋伙伴协议TPP是否有所进展？
//而虽然不在奥巴马的访问行程之内，但正如中国外交部发<WBR>&shy;言人秦刚所说:&quot 你来，或者不来，我就在这里&quot，中国就像&quot房间里的大象&quot，
//            是美国与盟国即使不直接说<WBR>&shy;明白，却也无法视而不见的议题。奥巴马此行取得哪些成果？我们邀请到两位嘉宾来为我们<WBR>&shy;
//            解读。一位是香港亚洲周刊资深特派员纪硕鸣先生，另外一位是中国复旦大学美国研究中心<WBR>&shy;沈丁立教授。</P></DIV>"
//            };
//            var dlg = new RemarkDialog();
//            dlg.ShowInTaskbar = true;
//            dlg.Widget = nw;
//            dlg.ShowDialog();

//            dlg.Widget = new NotesWidget()
//            {
//                Remark = "abc"
//            };

//            dlg.ShowDialog();

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
    }
}
