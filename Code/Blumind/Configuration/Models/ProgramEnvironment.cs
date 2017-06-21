using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Configuration
{
    /// <summary>
    ///    程序配置文件位置原则
    ///
    ///    初始化
    ///        安装版的配置文件放在用户目录
    ///        便携版的配置文件放在程序目录
    ///        
    ///    读取
    ///        首先检查程序目录, 如果有配置文件, 则读取配置文件. 记录为便携版
    ///        然后检查用户目录, 如果有配置文件, 则读取并覆盖相同的选项. 记录为安装版
    ///        
    ///    保存
    ///        如果是安装版, 保存到用户目录
    ///        如果是便携版, 优先保存到程序目录. 如果写失败, 则保存到用户目录
    ///        
    ///    可能的问题
    ///        便携版用户在一个已有安装版的计算机上运行程序时, 其随身携带的配置文件将失效
    ///        对于无法写的便携目录, 程序会写计算机用户目录, 导致”非完全的绿色版”
    /// </summary>
    static class ProgramEnvironment
    {
        static ProgramEnvironment()
        {
            RunMode = ProgramRunMode.Standard;
        }

        public static ProgramRunMode RunMode { get; set; }

        public static string ApplicationDataDirectory
        {
            get
            {
                return GetApplicationDataDirectory(RunMode);
            }
        }

        public static string GetApplicationDataDirectory(ProgramRunMode runMode)
        {
            switch (runMode)
            {
                case ProgramRunMode.Portable:
                    return Application.StartupPath;
                case ProgramRunMode.Standard:
                default:
                    return Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Blumind");
            }
        }

        public static string UIThemesDirectory
        {
            get { return Path.Combine(ApplicationDataDirectory, "UI Themes"); }
        }

        public static string LanguagesDirectory
        {
            get { return Path.Combine(ApplicationDataDirectory, "Languages"); }
        }

        public static string RecentsFilename
        {
            get { return Path.Combine(ApplicationDataDirectory, "Recents.xml"); }
        }
    }
}
