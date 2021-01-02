using System;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace AmongUsCE_AnimationEditor.Controls.About
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
	{
        #region Assembly Accessors
        public static class AssemblyAttributeAccessors
        {
            public static string AssemblyTitle
            {
                get
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    if (attributes.Length > 0)
                    {
                        AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                        if (titleAttribute.Title != "")
                        {
                            return titleAttribute.Title;
                        }
                    }
                    return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                }
            }

            public static string GetProgramType
            {
                get
                {
                    if (Environment.Is64BitProcess)
                    {
                        return "x64";
                    }
                    else
                    {
                        return "x86";
                    }
                }
            }

            public static string AssemblyProduct
            {
                get
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyProductAttribute)attributes[0]).Product;
                }
            }

            public static string AssemblyDescription
            {
                get
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyDescriptionAttribute)attributes[0]).Description;
                }
            }

            public static string AssemblyCopyright
            {
                get
                {
                    object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    if (attributes.Length == 0)
                    {
                        return "";
                    }
                    return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                }
            }

            public static string GetBuildTime
            {
                get
                {
                    DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
                    String buildTimeString = buildDate.ToString();
                    return buildTimeString;
                }

            }

            public static DateTime GetBuildDate
            {
                get
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    return GetLinkerTime(assembly);
                }
            }

            private static DateTime GetLinkerTime(Assembly assembly, TimeZoneInfo target = null)
            {
                var filePath = assembly.Location;
                const int c_PeHeaderOffset = 60;
                const int c_LinkerTimestampOffset = 8;

                var buffer = new byte[2048];

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    stream.Read(buffer, 0, 2048);

                var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
                var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

                var tz = target ?? TimeZoneInfo.Local;
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

                return localTime;
            }
        }
        public static Version RuntimeVersion { get; set; } = null;
        public static Version GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (IsDebug)
            {
                if (RuntimeVersion == null)
                {
                    string runtimeVer = AssemblyAttributeAccessors.GetBuildDate.ToString("y.M.d.0");
                    RuntimeVersion = new Version(runtimeVer);
                }
                return RuntimeVersion;
            }

            return version;
        }
        public static string GetCasualVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (IsDebug)
            {
                if (RuntimeVersion == null)
                {
                    string runtimeVer = AssemblyAttributeAccessors.GetBuildDate.ToString("y.M.d.0");
                    RuntimeVersion = new Version(runtimeVer);
                }

                string verString = RuntimeVersion.ToString();
                string devVersion = verString.TrimEnd(verString[verString.Length - 1]) + "DEV";
                return devVersion;
            }

            return version;
        }
        public static bool IsDebug
        {
            get
            {
                bool _isDebug = false;
                #if DEBUG
                _isDebug = true;
                #endif 
                return _isDebug;
            }

        }
        #endregion

        public AboutWindow()
		{
			InitializeComponent();
			Title = String.Format("About {0}", AssemblyAttributeAccessors.AssemblyTitle);
			labelProductName.Text = AssemblyAttributeAccessors.AssemblyProduct;
            llAbout.Text = AssemblyAttributeAccessors.AssemblyDescription;
			labelVersion.Text = String.Format("Version {0}",GetCasualVersion());
			buildDateLabel.Text = String.Format("Build Date: {0}", AssemblyAttributeAccessors.GetBuildTime) + Environment.NewLine + String.Format("Architecture: {0}", AssemblyAttributeAccessors.GetProgramType);
			labelCopyright.Text = AssemblyAttributeAccessors.AssemblyCopyright;
            GenerateCredits();
		}

		private void GenerateCredits()
		{
            CreditsRoll.Children.Add(GenerateString("Developed By:"));
            CreditsRoll.Children.Add(GenerateCreditLine("CarJem Generations", "https://github.com/CarJem", "", "https://www.youtube.com/channel/UC4-VHCZD7eLdxRr5aUXAQ5w", "https://twitter.com/carter5467_99"));
        }

        private TextBlock GenerateString(string message)
        {
            TextBlock item = new TextBlock();
            var bold = new Bold(new Run(message));
            item.Inlines.Add(bold);
            item.Foreground = FindResource("NormalText") as System.Windows.Media.Brush;
            return item;
        }

        private CreditEntry GenerateCreditLine(string title, string url, string message = "", string url2 = "", string url3 = "", string url4 = "")
        {
            CreditEntry item = new CreditEntry();
            item.SetCreditsEntry(title, url, message, url2, url3, url4);
            return item;
        }

		private void linkLabel3_LinkClicked(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}
