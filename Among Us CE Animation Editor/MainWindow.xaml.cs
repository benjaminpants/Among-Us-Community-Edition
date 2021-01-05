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


namespace AmongUsCE_AnimationEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        public static MainWindow Instance { get; set; }

        public bool Initalized = false;
        public bool AllowUpdate = true;

        #endregion

        public int SelectedIndex
        {
            get
            {
                return AnimationModel.Instance.SelectedIndex;
            }
            set
            {
                AnimationModel.Instance.SelectedIndex = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Initalized = true;
            Instance = this;
            this.Title = string.Format("{0} {1}", this.Title, Program.Version);
            if (Properties.Settings.Default.UseDarkMode) MenuBar.DarkModeButton.IsChecked = true;
            else MenuBar.DarkModeButton.IsChecked = false;
            UpdateUI();
        }

        #region Sprite Sheet Select Methods

        private void FileSpriteSelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnimationModel.Instance.isValidSelection)
            {
                UpdateFileSelectToolstrip();
                FileSpriteSelectButton.ContextMenu.IsOpen = true;
            }

        }
        private void UpdateFileSelectToolstrip()
        {
            CleanUpFileSelectToolStrip();
            string directory = AnimationHelpers.GetFileDirectory( AnimationModel.Instance.CurrentAnimation);
            if (Directory.Exists(directory))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                var files = directoryInfo.GetFiles().Where(s => s.Extension == ".bmp" || s.Extension == ".png");
                foreach (var file in files)
                {
                    if (File.Exists(file.FullName))
                    {
                        SpriteSheetFileList.Items.Add(GenerateInstalledVersionsToolstripItem(file.Name));
                    }
                }

            }

        }
        private void CleanUpFileSelectToolStrip()
        {
            foreach (var item in SpriteSheetFileList.Items.Cast<MenuItem>())
            {
                item.Click -= SelectSpriteFromToolstrip;
            }
            SpriteSheetFileList.Items.Clear();
        }
        private MenuItem GenerateInstalledVersionsToolstripItem(string name)
        {
            MenuItem item = new MenuItem();
            item.Header = name;
            item.Tag = name;
            item.Click += SelectSpriteFromToolstrip;
            return item;
        }
        private void SelectSpriteFromToolstrip(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            FileTextBox.Text = item.Tag.ToString();
        }

        #endregion

        #region Frame Control Events

        public void UpdateUI(bool AnimationLoad = false)
        {
            if (Initalized)
            {
                bool NewFrame = AnimationModel.Instance.IndexChanged || AnimationLoad;
                if (AnimationModel.Instance.IndexChanged) AnimationModel.Instance.IndexChanged = false;

                MenuBar.SetEnabledState(AnimationModel.Instance.isAnimationLoaded);
                SetMinimumMaximums();
                UpdateIndex();


                bool Avaliable = AnimationModel.Instance.isAnimationLoaded && AnimationModel.Instance.isValidSelection;
                UpdateFrameControls(Avaliable);

                if (Avaliable)
                {
                    if (NewFrame) GetNUDValues();
                    else SetNUDValues();
                    AnimationModel.Instance.GetFrameImage(NewFrame);
                    AnimationCanvas.SetCanvasView();
                }
                else
                {
                    if (AnimationModel.Instance.isAnimationLoaded)
                    {
                        if (NewFrame) GetNUDValues();
                        else SetNUDValues();
                    }
                    AnimationCanvas.NullCanvasClip();
                }
            }
        }
        public void OnAnimationUnload()
        {
            FrameViewer.ItemsSource = null;
            EntriesList.ItemsSource = null;
            AnimationCanvas.InvalidateVisual();
            UpdateUI();
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.RemoveFrame(EntriesList.SelectedIndex);
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.AddFrame();
        }
        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.MoveFrameDown();
        }
        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.MoveFrameUp();
        }
        public void UpdateFrameControls(bool enable)
        {
            PosXNUD.IsEnabled = enable;
            PosYNUD.IsEnabled = enable;
            WidthNUD.IsEnabled = enable;
            HeightNUD.IsEnabled = enable;
            CenterXNUD.IsEnabled = enable;
            CenterYNUD.IsEnabled = enable;
            NameTextBox.IsEnabled = enable;
            FileTextBox.IsEnabled = enable;
            FileSpriteSelectButton.IsEnabled = enable;
            PointFilteringCheckbox.IsEnabled = enable;
        }
        private void UpdateIndex()
        {
            if (!AnimationModel.Instance.isAnimationLoaded) return;
            AllowUpdate = false;
            if (EntriesList != null)
            {
                EntriesList.ItemsSource = null;
                EntriesList.ItemsSource =  AnimationModel.Instance.CurrentAnimation.FrameList;
                EntriesList.SelectedIndex = SelectedIndex;
            }
            if (FrameViewer != null)
            {
                FrameViewer.ItemsSource = null;
                FrameViewer.ItemsSource =  AnimationModel.Instance.CurrentAnimation.FrameList;
                FrameViewer.SelectedIndex = SelectedIndex;
            }
            AllowUpdate = true;
        }
        public void GetNUDValues()
        {
            AllowUpdate = false;

            if (AnimationModel.Instance.isValidSelection)
            {
                FileTextBox.Text =  AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].SpritePath;
                NameTextBox.Text = AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Name;

                PosYNUD.Value = (int)AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Position.y;
                PosXNUD.Value = (int)AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Position.x;
                WidthNUD.Value = (int)AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Size.x;
                HeightNUD.Value = (int)AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Size.y;

                CenterXNUD.Value = AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Offset.x;
                CenterYNUD.Value = AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Offset.y;

                NoHatBobbingFrameCheckbox.IsChecked = AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Hat_NoBobbing;
                HatInFrontFrameCheckbox.IsChecked = AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].InFront;
                UseColorFilteringCheckbox.IsChecked = AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].UseColorFiltering;
            }

            if (AnimationModel.Instance.isAnimationLoaded)
            {
                IDTextBox.Text = AnimationModel.Instance.CurrentAnimation.ID;
                SetNameTextBox.Text = AnimationModel.Instance.CurrentAnimation.Name;
                StoreNameTextBox.Text = AnimationModel.Instance.CurrentAnimation.StoreName;

                PointFilteringGloballyCheckbox.IsChecked = AnimationModel.Instance.CurrentAnimation.UsePointFilteringGlobally;
                UsePercentageBasedPivotCheckbox.IsChecked = AnimationModel.Instance.CurrentAnimation.UsePercentageBasedPivot;

                NoHatBobbingCheckbox.IsChecked = AnimationModel.Instance.CurrentAnimation.NoHatBobbing;
                HatInFrontCheckbox.IsChecked = AnimationModel.Instance.CurrentAnimation.HatInFront;
                IsHiddenCheckbox.IsChecked = AnimationModel.Instance.CurrentAnimation.IsHidden;
                UseColorFilteringGloballyCheckbox.IsChecked = AnimationModel.Instance.CurrentAnimation.UseColorFilteringGlobally;

                RelatedHatTextBox.Text = AnimationModel.Instance.CurrentAnimation.RelatedHat;
                RelatedSkinTextBox.Text = AnimationModel.Instance.CurrentAnimation.RelatedSkin;
            }


            AllowUpdate = true;

        }
        public void SetNUDValues()
        {
            AllowUpdate = false;
            if (AnimationModel.Instance.isValidSelection)
            {
                int x = (int)PosXNUD.Value.GetValueOrDefault();
                int y = (int)PosYNUD.Value.GetValueOrDefault();
                int width = (int)WidthNUD.Value.GetValueOrDefault();
                int height = (int)HeightNUD.Value.GetValueOrDefault();
                float pivotX = (float)CenterXNUD.Value.GetValueOrDefault();
                float pivotY = (float)CenterYNUD.Value.GetValueOrDefault();

                AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Position = new CE_Point(x, y);
                AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Size = new CE_Point(width, height);
                AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Offset = new CE_Point(pivotX, pivotY);

                AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].SpritePath = FileTextBox.Text;
                AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Name = NameTextBox.Text;
                AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].UsePointFiltering = PointFilteringCheckbox.IsChecked.GetValueOrDefault(false);

                AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].Hat_NoBobbing = NoHatBobbingFrameCheckbox.IsChecked.GetValueOrDefault(false);
                AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].InFront = HatInFrontFrameCheckbox.IsChecked.GetValueOrDefault(false);
                AnimationModel.Instance.CurrentAnimation.FrameList[SelectedIndex].UseColorFiltering = UseColorFilteringCheckbox.IsChecked.GetValueOrDefault(false);
            }

            if (AnimationModel.Instance.isAnimationLoaded)
            {
                AnimationModel.Instance.CurrentAnimation.ID = IDTextBox.Text;
                AnimationModel.Instance.CurrentAnimation.Name = SetNameTextBox.Text;
                AnimationModel.Instance.CurrentAnimation.StoreName = StoreNameTextBox.Text;

                AnimationModel.Instance.CurrentAnimation.UsePointFilteringGlobally = PointFilteringGloballyCheckbox.IsChecked.GetValueOrDefault(false);
                AnimationModel.Instance.CurrentAnimation.UsePercentageBasedPivot = UsePercentageBasedPivotCheckbox.IsChecked.GetValueOrDefault(false);

                AnimationModel.Instance.CurrentAnimation.NoHatBobbing = NoHatBobbingCheckbox.IsChecked.GetValueOrDefault(false);
                AnimationModel.Instance.CurrentAnimation.HatInFront = HatInFrontCheckbox.IsChecked.GetValueOrDefault(false);
                AnimationModel.Instance.CurrentAnimation.UseColorFilteringGlobally = UseColorFilteringGloballyCheckbox.IsChecked.GetValueOrDefault(false);
                AnimationModel.Instance.CurrentAnimation.IsHidden = IsHiddenCheckbox.IsChecked.GetValueOrDefault(false);

                AnimationModel.Instance.CurrentAnimation.RelatedHat = RelatedHatTextBox.Text;
                AnimationModel.Instance.CurrentAnimation.RelatedSkin = RelatedSkinTextBox.Text;
            }
            AllowUpdate = true;

        }
        public void SetMinimumMaximums()
        {
            PosXNUD.Minimum = int.MinValue;
            PosYNUD.Minimum = int.MinValue;
            WidthNUD.Minimum = int.MinValue;
            HeightNUD.Minimum = int.MinValue;
            CenterXNUD.Minimum = int.MinValue;
            CenterYNUD.Minimum = int.MinValue;

            CenterXNUD.Increment = 0.05f;
            CenterYNUD.Increment = 0.05f;

            PosXNUD.Maximum = int.MaxValue;
            PosYNUD.Maximum = int.MaxValue;
            WidthNUD.Maximum = int.MaxValue;
            HeightNUD.Maximum = int.MaxValue;
            CenterXNUD.Maximum = int.MaxValue;
            CenterYNUD.Maximum = int.MaxValue;
        }
        private void EntriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllowUpdate)
            {
                if (AnimationModel.Instance.SelectedIndex != EntriesList.SelectedIndex) AnimationModel.Instance.IndexChanged = true;
                AnimationModel.Instance.SelectedIndex = EntriesList.SelectedIndex;
                UpdateUI();
            }
        }
        private void FrameViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllowUpdate)
            {
                AnimationModel.Instance.SelectedIndex = FrameViewer.SelectedIndex;
                UpdateUI();
            }

        }
        private void NUD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsUp)
            {
                ControlLib.NumericUpDown objSend = (sender as ControlLib.NumericUpDown);
                if (objSend != null)
                {
                    if (e.Key == Key.Up && objSend.MaxValue > objSend.Value)
                    {
                        objSend.Value += 1;
                    }
                    else if (e.Key == Key.Down && objSend.MinValue < objSend.Value)
                    {
                        objSend.Value -= 1;
                    }
                }
            }

        }
        private void NUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
            {
                if (sender is Xceed.Wpf.Toolkit.IntegerUpDown) (sender as Xceed.Wpf.Toolkit.IntegerUpDown).Value = (int)e.OldValue;
                else if (sender is Xceed.Wpf.Toolkit.SingleUpDown) (sender as Xceed.Wpf.Toolkit.SingleUpDown).Value = (float)e.OldValue;
            }
            else if (AllowUpdate) UpdateUI();
        }
        private void FrameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded && AnimationModel.Instance.isValidSelection)
            {
                UpdateUI();
            }
        }
        private void NUD_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ControlLib.NumericUpDown objSend = (sender as ControlLib.NumericUpDown);
            if (objSend != null)
            {
                if (e.Delta >= 1 && objSend.MaxValue > objSend.Value)
                {
                    objSend.Value += 1;
                }
                else if (e.Delta <= -1 && objSend.MinValue < objSend.Value)
                {
                    objSend.Value -= 1;
                }
            }
        }
        private void AnimationScroller_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AllowUpdate)
            {
                AllowUpdate = false;
                if (AnimationScroller.Value == 3) AnimationModel.Instance.UpdateFrameIndex(false);
                if (AnimationScroller.Value == 1) AnimationModel.Instance.UpdateFrameIndex(true);
                AnimationScroller.Value = 2;
                AllowUpdate = true;
            }
        }
        private void PointFilteringCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded && AnimationModel.Instance.isValidSelection)
            {
                UpdateUI();
            }
        }
        public void InvalidateDiffViewer()
        {
            DiffBottom.Text = "0";
            DiffTop.Text = "0";
            DiffLeft.Text = "0";
            DiffRight.Text = "0";

            DiffBottom.Foreground = FindResource("NormalText") as Brush;
            DiffTop.Foreground = FindResource("NormalText") as Brush;
            DiffLeft.Foreground = FindResource("NormalText") as Brush;
            DiffRight.Foreground = FindResource("NormalText") as Brush;
        }
        public void UpdateDiffViewer(float x, float y, float width, float height, Int32Rect clipped)
        {
            if (clipped.X != x)
            {
                float diff;
                if (x > clipped.X) diff = x - clipped.X;
                else diff = clipped.X - x;
                DiffLeft.Text = diff.ToString();
                DiffLeft.Foreground = Brushes.Red;
                AnimationCanvas.FrameBorderLeft.Fill = Brushes.Red;
            }
            else
            {
                DiffLeft.Text = "0";
                DiffLeft.Foreground = FindResource("NormalText") as Brush;
                AnimationCanvas.FrameBorderLeft.Fill = Brushes.Black;
            }

            if (clipped.Y != y)
            {
                float diff;
                if (y > clipped.Y) diff = y - clipped.Y;
                else diff = clipped.Y - y;
                DiffTop.Text = diff.ToString();
                DiffTop.Foreground = Brushes.Red;
                AnimationCanvas.FrameBorderTop.Fill = Brushes.Red;
            }
            else
            {
                DiffTop.Text = "0";
                DiffTop.Foreground = FindResource("NormalText") as Brush;
                AnimationCanvas.FrameBorderTop.Fill = Brushes.Black;
            }


            if (clipped.Width != width)
            {
                float diff;
                if (width > clipped.Width) diff = width - clipped.Width;
                else diff = clipped.Width - width;
                DiffRight.Text = diff.ToString();
                DiffRight.Foreground = Brushes.Red;
                AnimationCanvas.FrameBorderRight.Fill = Brushes.Red;
            }
            else
            {
                DiffRight.Text = "0";
                DiffRight.Foreground = FindResource("NormalText") as Brush;
                AnimationCanvas.FrameBorderRight.Fill = Brushes.Black;
            }

            if (clipped.Height != height)
            {
                float diff;
                if (height > clipped.Height) diff = height - clipped.Height;
                else diff = clipped.Height - height;
                DiffBottom.Text = diff.ToString();
                DiffBottom.Foreground = Brushes.Red;
                AnimationCanvas.FrameBorderBottom.Fill = Brushes.Red;
            }
            else
            {
                DiffBottom.Text = "0";
                DiffBottom.Foreground = FindResource("NormalText") as Brush;
                AnimationCanvas.FrameBorderBottom.Fill = Brushes.Black;
            }
        }
        private void StoreNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }

        private void IDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }

        private void RelatedHatTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }

        private void RelatedSkinTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }

        private void PointFilteringGloballyCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }

        private void HatInFrontCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }

        private void NoHatBobbingCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }

        private void UsePercentageBasedPivotCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }

        private void SetNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }

        private void IDGeneratorButton_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                var guid = Guid.NewGuid().ToString();
                IDTextBox.Text = guid;
                UpdateUI();
            }
        }

        private void HatInFrontFrameCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded && AnimationModel.Instance.isValidSelection)
            {
                UpdateUI();
            }
        }

        private void NoHatBobbingFrameCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded && AnimationModel.Instance.isValidSelection)
            {
                UpdateUI();
            }
        }

        private void UseColorFilteringCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }

        private void UseColorFilteringGloballyCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }
        private void IsHiddenCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (AllowUpdate && AnimationModel.Instance.isAnimationLoaded)
            {
                UpdateUI();
            }
        }

        #endregion

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                //NewFile
                AnimationModel.Instance.NewFile();
            }
            else if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                //OpenFile
                if (MenuBar.OpenMenuItem.IsEnabled) AnimationModel.Instance.OpenFile();
            }
            else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                //SaveFile
                if (MenuBar.SaveMenuItem.IsEnabled) AnimationModel.Instance.FileSave();
            }
            else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                //SaveFileAs
                if (MenuBar.SaveAsMenuItem.IsEnabled) AnimationModel.Instance.FileSaveAs();
            }
            else if (e.Key == Key.U && Keyboard.Modifiers == ModifierKeys.Control)
            {
                //UnloadFile
                if (MenuBar.UnloadMenuItem.IsEnabled) AnimationModel.Instance.UnloadAnimation();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                int recentItem = -1;

                if (e.Key == Key.D1) recentItem = 0;
                else if (e.Key == Key.D2) recentItem = 1;
                else if (e.Key == Key.D3) recentItem = 2;
                else if (e.Key == Key.D4) recentItem = 3;
                else if (e.Key == Key.D5) recentItem = 4;
                else if (e.Key == Key.D6) recentItem = 5;
                else if (e.Key == Key.D7) recentItem = 6;
                else if (e.Key == Key.D8) recentItem = 7;
                else if (e.Key == Key.D9) recentItem = 8;
                else if (e.Key == Key.D0) recentItem = 9;

                if (recentItem != -1)
                {
                    MenuBar.KeyDown_Recents(recentItem);
                }
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateUI();
        }
    }


}
