using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace AmongUsCE_AnimationEditor
{
    static class Program
    {
        #region Variables
        public static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool isDebug;
        public static bool isDeveloper { get; set; } = false;

        [ConditionalAttribute("DEBUG")]
        public static void isDebugging()
        {
            isDebug = true;
        }

        #endregion

        #region Version Variables

        private static string VersionString
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public static string Version { get => GetVersionString(); }

        private static string GetVersionString()
        {
            return (isDebug ? "DEV" : "v." + VersionString);
        }

        public static Version InternalVersion { get; } = new Version(VersionString);

        #endregion


        #region Main Region
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            isDebugging();
            StartLogging();
            RealMain(args);
            EndLogging();
        }

        static void RealMain(string[] args)
        {
            Log.InfoFormat("Starting Animation Editor...");
            StartApplication(args);
            Log.InfoFormat("Shuting Down!");
        }
        #endregion

        static void StartApplication(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            var app = new App();
            app.DefaultStart();

        }

        #region Logging

        static void StartLogging()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) => {
                if (e.Exception.TargetSite.DeclaringType.Assembly == Assembly.GetExecutingAssembly())
                {
                    Log.ErrorFormat("Exception Thrown: {0} {1}", RemoveNewLineChars(e.Exception.Message), RemoveNewLineChars(e.Exception.StackTrace));
                }
                else
                {
                    Log.ErrorFormat("Exception Thrown: {0} {1}", RemoveNewLineChars(e.Exception.Message), RemoveNewLineChars(e.Exception.StackTrace));
                }
            };



        }

        static string RemoveNewLineChars(string string_to_search, string replacement_string = " ")
        {
            return System.Text.RegularExpressions.Regex.Replace(string_to_search, @"\t|\n|\r", replacement_string);
        }

        static void CleanUpLogsFolder()
        {
            string appdatafolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "//Sonic 3 A.I.R. Animation Editor";
            string app_log_filepath = Path.Combine(appdatafolder, "app.log");
            string log_folder_location = Path.Combine(appdatafolder, "Logs");

            if (!Directory.Exists(log_folder_location)) Directory.CreateDirectory(log_folder_location);

            string log_filename = string.Format("S3AIR_AE_{0}_{1}.log", string.Format("[{0}]", GetVersionString()), DateTime.Now.ToString("[M-dd-yyyy]_[hh-mm-ss]"));
            string log_filepath = Path.Combine(log_folder_location, log_filename);

            if (File.Exists(app_log_filepath)) File.Copy(app_log_filepath, log_filepath);
            DirectoryInfo logsFolder = new DirectoryInfo(log_folder_location);
            var fileList = logsFolder.GetFiles("*.log", SearchOption.AllDirectories).ToList();
            if (fileList.Count > 10)
            {
                foreach (var file in fileList.OrderByDescending(file => file.CreationTime).Skip(10))
                {
                    file.Delete();
                }
            }
        }

        static void EndLogging()
        {
            CleanUpLogsFolder();
        }

        #endregion




    }
}
