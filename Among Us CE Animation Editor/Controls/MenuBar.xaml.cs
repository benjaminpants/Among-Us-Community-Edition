using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Net.Http;
using Microsoft.Win32;
using AmongUsCE_AnimationEditor.ViewModels;

namespace AmongUsCE_AnimationEditor.Controls
{
    /// <summary>
    /// Interaction logic for MenuBar.xaml
    /// </summary>
    public partial class MenuBar : UserControl
    {
        private IList<MenuItem> RecentItems { get; set; } = new List<MenuItem>();

        private bool isInitalized = false;

        public MenuBar()
        {
            InitializeComponent();
            isInitalized = true;
        }

        public void SetEnabledState(bool isAnimationLoaded)
        {
            OpenMenuItem.IsEnabled = true;
            SaveAsMenuItem.IsEnabled = isAnimationLoaded;
            SaveMenuItem.IsEnabled = isAnimationLoaded;
            NewMenuItem.IsEnabled = true;
            ExitMenuItem.IsEnabled = true;
            UnloadMenuItem.IsEnabled = isAnimationLoaded;
        }

        private void NewFileEvent(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.NewFile();
        }

        private void OpenFileEvent(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.OpenFile();
        }

        private void SaveFileEvent(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.FileSave();
        }

        private void SaveFileAsEvent(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.FileSaveAs();
        }

        private void UnloadEvent(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.UnloadAnimation();
        }

        private void ExitEditor(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Close();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.CopyFrame();
        }

        private void PasteOffsetButton_Click(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.PasteFrameOffset();
        }

        private void SpecialFixButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isInitalized) return;
            AnimationModel.Instance.SpecialFix();
        }

        private void PasteAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isInitalized) return;
            AnimationModel.Instance.PasteAll();
        }

        private void FramesFromImages_Click(object sender, RoutedEventArgs e)
        {
            if (!isInitalized) return;
            AnimationModel.Instance.ImagesToFrames();
        }

        public void ShowCenterAlignmentLines_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitalized) return;
            if (ShowCenterAlignmentLines.IsChecked) AnimationModel.Instance.ShowAlignmentLines = true;
            else AnimationModel.Instance.ShowAlignmentLines = false;

            MainWindow.Instance.AnimationCanvas.ButtonShowCenter.IsChecked = AnimationModel.Instance.ShowAlignmentLines;

            MainWindow.Instance.UpdateUI();
        }

        private void AllowRemoveWarnings_Checked(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.ShowRemoveWarning = AllowRemoveWarnings.IsChecked;
        }

        private void DarkModeButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.UseDarkMode = !Properties.Settings.Default.UseDarkMode;
            Properties.Settings.Default.Save();

            if (Properties.Settings.Default.UseDarkMode)
            {
                App.ChangeSkin(Skin.Dark);
                DarkModeButton.IsChecked = true;
            }
            else
            {
                App.ChangeSkin(Skin.Light);
                DarkModeButton.IsChecked = false;
            }

            RefreshTheming();


            void RefreshTheming()
            {
                this.InvalidateVisual();
                foreach (UIElement element in Extensions.FindVisualChildren<UIElement>(this))
                {
                    element.InvalidateVisual();
                }
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Controls.About.AboutWindow about = new Controls.About.AboutWindow();
            about.Owner = MainWindow.Instance;
            about.ShowDialog();
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.PasteFrame();
        }

        #region Recent Files (Lifted from Maniac Editor)

        public void KeyDown_Recents(int recentItem)
        {
            RefreshRecentFiles(Properties.Settings.Default.RecentItems);
            if (RecentItems.Count >= recentItem && RecentItems.Count != 0) RecentItem_Click(RecentItems.ElementAt(recentItem), null);
        }

        private void OpenRecentsMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            RefreshRecentFiles(Properties.Settings.Default.RecentItems);
        }
        private void RecentItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as System.Windows.Controls.MenuItem;
            string selectedFile = menuItem.Tag.ToString();
            var recentFiles = Properties.Settings.Default.RecentItems;
            if (File.Exists(selectedFile))
            {
                AddItemToRecentsList(selectedFile);
                AnimationModel.Instance.LoadAnimation(AnimationHelpers.Load(new FileInfo(selectedFile)));
            }
            else
            {
                recentFiles.Remove(selectedFile);
                RefreshRecentFiles(recentFiles);

            }
            Properties.Settings.Default.Save();
        }
        private void AddItemToRecentsList(string item)
        {
            try
            {
                var mySettings = Properties.Settings.Default;
                var dataDirectories = mySettings.RecentItems;

                if (dataDirectories == null)
                {
                    dataDirectories = new System.Collections.Specialized.StringCollection();
                    mySettings.RecentItems = dataDirectories;
                }

                if (dataDirectories.Contains(item))
                {
                    dataDirectories.Remove(item);
                }

                if (dataDirectories.Count >= 10)
                {
                    for (int i = 9; i < dataDirectories.Count; i++)
                    {
                        dataDirectories.RemoveAt(i);
                    }
                }

                dataDirectories.Insert(0, item);

                mySettings.Save();

                RefreshRecentFiles(dataDirectories);


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("Failed to add data folder to recent list: " + ex);
            }
        }
        public void UpdateRecentsDropDown(string itemToAdd = "")
        {
            if (itemToAdd != "") AddItemToRecentsList(itemToAdd);
            RefreshRecentFiles(Properties.Settings.Default.RecentItems);
        }
        public void RefreshRecentFiles(System.Collections.Specialized.StringCollection recentDataDirectories)
        {
            if (Properties.Settings.Default.RecentItems?.Count > 0)
            {
                NoRecentFiles.Visibility = Visibility.Collapsed;
                CleanUpRecentList();

                var startRecentItems = OpenRecentsMenuItem.Items.IndexOf(NoRecentFiles);

                int index = 1;
                foreach (var dataDirectory in recentDataDirectories)
                {
                    int item_key;
                    if (index == 9) item_key = index;
                    else if (index >= 10) item_key = -1;
                    else item_key = index;
                    RecentItems.Add(CreateDataDirectoryMenuLink(dataDirectory, index));
                    index++;
                }



                foreach (MenuItem menuItem in RecentItems.Reverse())
                {
                    OpenRecentsMenuItem.Items.Insert(startRecentItems, menuItem);
                }
            }
            else
            {
                NoRecentFiles.Visibility = Visibility.Visible;
            }



        }
        private MenuItem CreateDataDirectoryMenuLink(string target, int index)
        {
            MenuItem newItem = new MenuItem();
            newItem.Header = target;
            if (index != -1) newItem.InputGestureText = string.Format("Ctrl + {0}", index);
            newItem.Tag = target;
            newItem.Click += RecentItem_Click;
            return newItem;
        }
        private void CleanUpRecentList()
        {
            foreach (var menuItem in RecentItems)
            {
                menuItem.Click -= RecentItem_Click;
                OpenRecentsMenuItem.Items.Remove(menuItem);
            }

            List<string> ItemsForRemoval = new List<string>();
            List<string> ItemsWithoutDuplicates = new List<string>();

            for (int i = 0; i < Properties.Settings.Default.RecentItems.Count; i++)
            {
                if (ItemsWithoutDuplicates.Contains(Properties.Settings.Default.RecentItems[i]))
                {
                    ItemsForRemoval.Add(Properties.Settings.Default.RecentItems[i]);
                }
                else
                {
                    ItemsWithoutDuplicates.Add(Properties.Settings.Default.RecentItems[i]);
                    if (File.Exists(Properties.Settings.Default.RecentItems[i])) continue;
                    else ItemsForRemoval.Add(Properties.Settings.Default.RecentItems[i]);
                }

            }
            foreach (string item in ItemsForRemoval)
            {
                Properties.Settings.Default.RecentItems.Remove(item);
            }

            Properties.Settings.Default.RecentItems.Cast<string>().Distinct().ToList();

            RecentItems.Clear();
        }

        #endregion
    }
}
