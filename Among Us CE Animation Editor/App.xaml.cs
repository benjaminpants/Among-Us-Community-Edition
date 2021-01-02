using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AmongUsCE_AnimationEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public enum Skin { Dark, Light }
    public partial class App : Application
    {

        public static App Instance;

        public static Skin Skin { get; set; } = Skin.Dark;

        public static bool SkinChanged { get; set; } = false;

        public App()
        {
            if (AmongUsCE_AnimationEditor.Properties.Settings.Default.UseDarkMode) ChangeSkin(Skin.Dark);
            else ChangeSkin(Skin.Light);

            #if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            #endif

            Instance = this;
            this.InitializeComponent();
        }

        public void DefaultStart()
        {
            Program.Log.InfoFormat("Starting Animation Editor...");
            this.Run(new MainWindow());
        }


        public static void ChangeSkin(Skin newSkin)
        {
            Skin = newSkin;

            foreach (ResourceDictionary dict in Application.Current.Resources.MergedDictionaries)
            {

                if (dict is SkinResourceDictionary skinDict)
                    skinDict.UpdateSource();
                else
                    dict.Source = dict.Source;
            }
        }
    }
}
