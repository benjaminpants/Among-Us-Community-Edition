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

namespace AmongUsCE_AnimationEditor.ViewModels
{
    public class AnimationModel
    {
        public static AnimationModel Instance { get; set; } = new AnimationModel();

        #region Mode Variables

        public bool ShowAlignmentLines { get; set; } = false;
        public bool ShowRemoveWarning { get; set; } = true;

        #endregion

        #region State Variables

        public bool AllowUpdate { get; set; } = true;
        public bool IndexChanged { get; set; } = false;

        #endregion

        #region Editor Variables

        private double _Zoom = 1.0;
        public double Zoom
        {
            get
            {
                return _Zoom;
            }
            set
            {
                //_Zoom = Math.Max(Math.Min(_Zoom, 16), 0.25);
            }
        }
        public int SelectedIndex { get; set; } = -1;
        public CE_FrameSet CurrentAnimation { get; set; }
        public CE_SpriteFrame Clipboard { get; set; } = null;
        public BitmapSource CurrentSpriteSheet { get; set; }
        public string CurrentSpriteSheetName { get; set; }

        #endregion

        #region Validation Variables

        public bool isValidSelection
        {
            get
            {
                if (CurrentAnimation != null)
                {
                    if (CurrentAnimation.FrameList != null)
                    {
                        if (CurrentAnimation.FrameList.Count > SelectedIndex && SelectedIndex != -1)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public bool isAnimationLoaded
        {
            get
            {
                return CurrentAnimation != null;
            }
        }

        #endregion

        #region Control Methods

        public void UnloadAnimation()
        {
            CurrentAnimation = null;
            CurrentSpriteSheet = null;
            MainWindow.Instance.OnAnimationUnload();
        }
        public void LoadAnimation(CE_FrameSet anim)
        {
            UnloadAnimation();
            CurrentAnimation = anim;
            MainWindow.Instance.UpdateUI(true);
        }
        public bool OpenFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "JSON Files (*.json) | *.json"
            };
            if (fileDialog.ShowDialog() == true)
            {
                LoadAnimation(AnimationHelpers.Load(new FileInfo(fileDialog.FileName)));
                MainWindow.Instance.MenuBar.UpdateRecentsDropDown(fileDialog.FileName);
                return true;
            }
            else return false;
        }
        public void NewFile()
        {
            var animation = new CE_FrameSet();
            foreach (var entry in AnimationHelpers.DefaultFrameNames)
            {
                CE_SpriteFrame frame = new CE_SpriteFrame();
                frame.Name = entry;
                animation.FrameList.Add(frame);
            }
            LoadAnimation(animation);
        }
        public void FileSaveAs()
        {
            SaveFileDialog fileDialog = new SaveFileDialog()
            {
                Filter = "JSON Files (*.json) | *.json"
            };
            if (fileDialog.ShowDialog() == true)
            {
                AnimationHelpers.Save(CurrentAnimation, fileDialog.FileName);
            }
        }
        public void FileSave()
        {
            if (CurrentAnimation.FilePath == "" || CurrentAnimation.FilePath == null) FileSaveAs();
            else AnimationHelpers.Save(CurrentAnimation);
        }
        public void AddFrame()
        {
            if (isAnimationLoaded)
            {
                if (isValidSelection)
                {
                    int index = SelectedIndex;
                    CurrentAnimation.FrameList.Insert(index, new CE_SpriteFrame());
                    MainWindow.Instance.UpdateUI();
                }
            }
        }
        public void RemoveFrame(int index)
        {
            if (isAnimationLoaded)
            {
                if (isValidSelection)
                {
                 //   bool Allowed = true;
                    if (ShowRemoveWarning)
                    {
                        CurrentAnimation.FrameList.RemoveAt(index);
                        MainWindow.Instance.UpdateUI();
                    }
                }
            }
        }
        public void MoveFrameUp()
        {
            if (isAnimationLoaded)
            {
                if (isValidSelection)
                {
                    int index = SelectedIndex;
                    if (index != 0)
                    {
                        CurrentAnimation.FrameList.Move(index, index - 1);
                        SelectedIndex = index - 1;
                        MainWindow.Instance.UpdateUI();
                    }
                }
            }
        }
        public void MoveFrameDown()
        {
            if (isAnimationLoaded)
            {
                if (isValidSelection)
                {
                    int index = SelectedIndex;
                    if (index != MainWindow.Instance.EntriesList.Items.Count - 1)
                    {
                        CurrentAnimation.FrameList.Move(index, index + 1);
                        SelectedIndex = index + 1;
                        MainWindow.Instance.UpdateUI();
                    }
                }
            }
        }
        public void CopyFrame()
        {
            if (isAnimationLoaded && isValidSelection)
            {
                Clipboard = CurrentAnimation.FrameList[SelectedIndex];
            }
        }
        public void PasteFrame()
        {
            if (isAnimationLoaded && isValidSelection)
            {
                CurrentAnimation.FrameList[SelectedIndex].Offset = Clipboard.Offset;
                CurrentAnimation.FrameList[SelectedIndex].Position = Clipboard.Position;
                CurrentAnimation.FrameList[SelectedIndex].Size = Clipboard.Size;
                CurrentAnimation.FrameList[SelectedIndex].SpritePath = Clipboard.SpritePath;
                MainWindow.Instance.GetNUDValues();
                MainWindow.Instance.UpdateUI();
            }
        }
        public void PasteFrameOffset()
        {
            if (isAnimationLoaded && isValidSelection)
            {
                CurrentAnimation.FrameList[SelectedIndex].Offset = Clipboard.Offset;
                MainWindow.Instance.GetNUDValues();
                MainWindow.Instance.UpdateUI();
            }
        }
        public void PasteAll()
        {
            if (isAnimationLoaded && isValidSelection)
            {
                foreach (var entry in CurrentAnimation.FrameList)
                {
                    int index = CurrentAnimation.FrameList.IndexOf(entry);
                    if (SelectedIndex != index)
                    {
                        entry.Offset = Clipboard.Offset;
                        entry.Position = Clipboard.Position;
                        entry.Size = Clipboard.Size;
                        entry.SpritePath = Clipboard.SpritePath;
                    }
                }

                MainWindow.Instance.GetNUDValues();
                MainWindow.Instance.UpdateUI();
            }
        }
        public void SpecialFix()
        {
            if (isAnimationLoaded && isValidSelection)
            {
                int x = 1;
                int y = 0;

                foreach (var entry in CurrentAnimation.FrameList)
                {
                    int position_x = 256 * x;
                    int position_y = 256 * y;
                    entry.Position = new CE_Point(position_x, position_y);
                    entry.SpritePath = "SkinTest.png";
                    if (x < 19) x++;
                    else
                    {
                        y++;
                        x = 0;
                    }
                }

                MainWindow.Instance.GetNUDValues();
                MainWindow.Instance.UpdateUI();
            }
        }
        public void ImagesToFrames()
        {
            FolderSelectDialog dir = new FolderSelectDialog();
            if (dir.ShowDialog())
            {
                var directory = new DirectoryInfo(dir.FileName);
                foreach (var item in directory.GetFiles())
                {
                    if (item.Extension == ".png")
                    {
                        System.Drawing.Bitmap png = new System.Drawing.Bitmap(item.FullName);
                        var newItem = new CE_SpriteFrame();
                        newItem.Offset = new CE_Point(0.5f, 0.5f);
                        newItem.Position = new CE_Point(0.0f, 0.0f);
                        newItem.Size = new CE_Point(png.Width, png.Height);
                        newItem.SpritePath = item.Name.Replace(AnimationHelpers.GetFileDirectory(CurrentAnimation), "");
                        newItem.Name = System.IO.Path.GetFileNameWithoutExtension(item.Name);
                        CurrentAnimation.FrameList.Add(newItem);
                        png.Dispose();
                    }
                }
                MainWindow.Instance.UpdateUI();
            }
        }
        public void ChangeZoomLevel(bool increase)
        {
            if (increase)
            {
                if (Zoom < 8)
                {
                    Zoom = Zoom + 1;
                }
            }
            else
            {
                if (Zoom > 1)
                {
                    Zoom = Zoom - 1;
                }
            }

            MainWindow.Instance.UpdateUI();
        }

        #endregion

        #region Internal Methods
        public void UpdateFrameIndex(bool subtract = false)
        {
            bool DidUpdateHappen = false;
            if (isAnimationLoaded)
            {
                if (isValidSelection)
                {
                    int index = SelectedIndex;
                    if (subtract && index != 0)
                    {
                        AllowUpdate = false;
                        DidUpdateHappen = true;
                        SelectedIndex = index - 1;

                    }
                    else if (index + 1 < CurrentAnimation.FrameList.Count())
                    {
                        AllowUpdate = false;
                        DidUpdateHappen = true;
                        SelectedIndex = index + 1;
                    }


                    if (DidUpdateHappen)
                    {
                        MainWindow.Instance.FrameViewer.ScrollIntoView(MainWindow.Instance.FrameViewer.SelectedItem);
                        MainWindow.Instance.EntriesList.ScrollIntoView(MainWindow.Instance.EntriesList.SelectedItem);
                        MainWindow.Instance.UpdateUI();
                        AllowUpdate = true;
                    }
                }
            }

        }
        public void GetFrameImage(bool NewFrame)
        {
            if (isValidSelection == false) return;
            if (CurrentSpriteSheetName != CurrentAnimation.FrameList[SelectedIndex].SpritePath || CurrentSpriteSheet == null || NewFrame)
            {
                Program.Log.InfoFormat("Collecting Main Image...");
                Dispose();
                CurrentSpriteSheetName = "";
                try
                {
                    if (File.Exists($"{AnimationHelpers.GetFileDirectory(CurrentAnimation)}\\{CurrentAnimation.FrameList[SelectedIndex].SpritePath}"))
                    {
                        string fileName = $"{AnimationHelpers.GetFileDirectory(CurrentAnimation)}\\{CurrentAnimation.FrameList[SelectedIndex].SpritePath}";
                        FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                        System.Drawing.Bitmap img = new System.Drawing.Bitmap(fileStream);
                        CurrentSpriteSheet = (BitmapSource)BitmapExtensions.ToWpfBitmap(img);
                        CurrentSpriteSheetName = CurrentAnimation.FrameList[SelectedIndex].SpritePath;
                        img.Dispose();
                        img = null;
                        fileStream.Close();

                    }
                    else
                    {
                        Dispose();
                    }
                    Program.Log.InfoFormat("Main Image Collected!");
                }
                catch
                {
                    Program.Log.InfoFormat("Main Image Collection Failed!");
                    Dispose();
                }
            }

            void Dispose()
            {
                CurrentSpriteSheet = null;
            }
        }
        #endregion
    }
}
