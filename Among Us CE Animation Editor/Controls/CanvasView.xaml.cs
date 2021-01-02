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
using AmongUsCE_AnimationEditor.ViewModels;

namespace AmongUsCE_AnimationEditor.Controls
{
    /// <summary>
    /// Interaction logic for CanvasView.xaml
    /// </summary>
    public partial class CanvasView : UserControl
    {
        public CanvasView()
        {
            InitializeComponent();
        }

        public bool IndexChanged
        {
            get
            {
                return AnimationModel.Instance.IndexChanged;
            }
            set
            {
                AnimationModel.Instance.IndexChanged = value;
            }
        }
        public bool isAnimationLoaded
        {
            get
            {
                return AnimationModel.Instance.isAnimationLoaded;
            }
        }
        public bool isValidSelection
        {
            get
            {
                return AnimationModel.Instance.isValidSelection;
            }
        }
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
        public CE_FrameSet CurrentAnimation
        {
            get
            {
                return AnimationModel.Instance.CurrentAnimation;
            }
            set
            {
                AnimationModel.Instance.CurrentAnimation = value;
            }
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MainWindow.Instance != null) MainWindow.Instance.UpdateUI();
        }
        private void CanvasView_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void CanvasView_KeyUp(object sender, KeyEventArgs e)
        {

        }
        private void CanvasView_MouseMove(object sender, MouseEventArgs e)
        {

        }
        private void CanvasView_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
        private void CanvasView_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void CanvasView_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }
        private void CanvasView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta >= 1)
            {
                AnimationModel.Instance.ChangeZoomLevel(true);
            }
            else if (e.Delta <= -1)
            {
                AnimationModel.Instance.ChangeZoomLevel(false);
            }
        }
        private void ButtonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.ChangeZoomLevel(true);
        }
        private void ButtonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            AnimationModel.Instance.ChangeZoomLevel(false);
        }
        private void CanvasImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsInitialized) UpdateFrameBorderSize();
        }

        private void ButtonShowCenter_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.MenuBar.ShowCenterAlignmentLines.IsChecked = !MainWindow.Instance.MenuBar.ShowCenterAlignmentLines.IsChecked;
            MainWindow.Instance.MenuBar.ShowCenterAlignmentLines_Checked(null, null);
        }
        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            ButtonHelp.ContextMenu.IsOpen = true;
        }

        public void SetCanvasView()
        {
            float x = CurrentAnimation.FrameList[SelectedIndex].Position.x;
            float y = CurrentAnimation.FrameList[SelectedIndex].Position.y;
            float width = CurrentAnimation.FrameList[SelectedIndex].Size.x;
            float height = CurrentAnimation.FrameList[SelectedIndex].Size.y;
            float pivotX = CurrentAnimation.FrameList[SelectedIndex].Offset.x;
            float pivotY = CurrentAnimation.FrameList[SelectedIndex].Offset.y;

            if (CurrentAnimation.UsePercentageBasedPivot)
            {
                var result = AnimationHelpers.GetPixelPivot(width, height, pivotX, pivotY);
                pivotX = result.X;
                pivotY = result.Y;
            }

            if (AnimationModel.Instance.CurrentSpriteSheet == null) NullCanvasClip();
            else
            {
                try { SetCanvasClip(x, y, width, height, pivotX, pivotY); }
                catch { NullCanvasClip(); }
            }

            MainCanvasView.InvalidateVisual();
            FrameBorder.InvalidateVisual();
            FrameBorder.InvalidateMeasure();
        }
        public void NullCanvasClip()
        {
            Canvas.SetLeft(CanvasImage, 0);
            Canvas.SetTop(CanvasImage, 0);

            Canvas.SetLeft(FrameBorder, 0);
            Canvas.SetTop(FrameBorder, 0);

            UpdateFrameBorderSize();

            CanvasImage.Source = null;
            CanvasImage.Visibility = Visibility.Collapsed;

            MainWindow.Instance.InvalidateDiffViewer();
        }
        private void SetCanvasClip(float x, float y, float width, float height, float pivotX, float pivotY)
        {
            CanvasImage.Visibility = Visibility.Visible;

            Int32Rect VisibleRect = new Int32Rect((int)x, (int)y, (int)(width), (int)(height));
            Classes.ClippableBitmap clipped = new Classes.ClippableBitmap(AnimationModel.Instance.CurrentSpriteSheet, VisibleRect);
            CanvasImage.Source = clipped.GetSource();
            CanvasImage.InvalidateVisual();

            MainWindow.Instance.UpdateDiffViewer(x, y, width, height, clipped.SourceRect);

            double SpriteLeft = (MainCanvasView.ActualWidth / 2) - pivotX;
            double SpriteTop = (MainCanvasView.ActualHeight / 2) - pivotY;

            CanvasImage.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);

            Canvas.SetLeft(CanvasImage, SpriteLeft);
            Canvas.SetTop(CanvasImage, SpriteTop);

            Canvas.SetLeft(FrameBorder, SpriteLeft);
            Canvas.SetTop(FrameBorder, SpriteTop);

            UpdateFrameBorderSize();
        }
        private void UpdateFrameBorderSize()
        {
            if (CanvasImage.Source != null)
            {
                FrameBorder.Visibility = Visibility.Visible;
                FrameBorder.Width = CanvasImage.ActualWidth;
                FrameBorder.Height = CanvasImage.ActualHeight;
            }
            else
            {
                FrameBorder.Visibility = Visibility.Collapsed;
                FrameBorder.Width = 0;
                FrameBorder.Height = 0;
            }
        }
    }
}
