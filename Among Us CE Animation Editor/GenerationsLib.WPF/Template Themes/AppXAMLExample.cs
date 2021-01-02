using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerationsLib.WPF.Styles
{
    public class AppXAMLExample
    {

    //-----------------
    //  Code Behind:
    //-----------------

    /*
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    namespace GenerationsLib.WPF
    {
        /// <summary>
        /// Interaction logic for App.xaml
        /// </summary>
        /// 

        public enum Skin { Dark, Light }
        public partial class App : Application
        {
            public static App Instance;

            public static Skin Skin { get; set; } = Skin.Dark;

            public static bool SkinChanged { get; set; } = false;


            public App()
            {
                if (GenerationsLib.WPF.Properties.Settings.Default.UseDarkTheme == true) ChangeSkin(Skin.Dark);
                else ChangeSkin(Skin.Light);

                Instance = this;
                this.InitializeComponent();
            }

            public void RunAutoBoot()
            {

                var auto = new AutoBootDialogV2();
                if (auto.ShowDialog() == true)
                {

                    if (Program.AutoBootCanceled == false) this.Run(new ModManager(true));
                    else this.Run(new ModManager(false));
                }

            }

            public void GBAPI(string Arguments)
            {

                this.Run(new ModManager(Arguments));
            }


            public void DefaultStart()
            {

                this.Run(new ModManager());
            }


            public static void ChangeSkin(Skin newSkin)
            {
                Skin = newSkin;

                foreach (ResourceDictionary dict in GenerationsLib.WPF.App.Current.Resources.MergedDictionaries)
                {

                    if (dict is SkinResourceDictionary skinDict)
                        skinDict.UpdateSource();
                    else
                        dict.Source = dict.Source;
                }
            }
        }
    }
    */

    //-----------------
    //  XAML Code:
    //-----------------

    /*
    <Application x:Class="Sonic3AIR_ModManager.App"
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:local="clr-namespace:Sonic3AIR_ModManager" ShutdownMode="OnExplicitShutdown">
        <Application.Resources>
            <ResourceDictionary>
                <ResourceDictionary.MergedDictionaries>
                    <local:SkinResourceDictionary DarkSource="Styles + Controls/Theming/ColorLibraryDark.xaml" LightSource="Styles + Controls/Theming/ColorLibraryLight.xaml" />
                    <ResourceDictionary Source="Styles + Controls/Stylers/IconLibrary.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/CheckboxStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/ButtonStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/ItemContainerStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/MenuItemContextStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/FlatButtonStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/ExpanderStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/TextBoxStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/TabItemStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/ComboBoxStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/ScrollBarStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/ScrollViewerStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/ListViewStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/RepeatButtonStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/ToolbarStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/SliderStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/AppSpecificStyle.xaml"></ResourceDictionary>
                    <ResourceDictionary Source="Styles + Controls/Stylers/GroupBoxStyle.xaml"></ResourceDictionary>
                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </Application.Resources>
    </Application>
    */
    }
}
