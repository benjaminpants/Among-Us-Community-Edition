using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using Point = System.Drawing.Point;

namespace AmongUsCE_AnimationEditor.Classes
{
    public class ClippableBitmap
    {
        public Int32Rect SourceRect { get; set; }
        public BitmapSource Source
        {
            get
            {
                return RefrenceSource.Source;
            }
            set
            {
                RefrenceSource.Source = value;
            }
        }
        private CroppedBitmap RefrenceSource { get; set; }
        public ClippableBitmap(BitmapSource source, Int32Rect sourceRect)
        {
            SourceRect = ValidateRect(sourceRect, source);
            if (sourceRect.X < source.PixelWidth && sourceRect.Y < source.PixelHeight && sourceRect.Width != 0 && sourceRect.Height != 0)
            {
                try
                {
                    RefrenceSource = new CroppedBitmap(source, SourceRect);
                }
                catch
                {
                    RefrenceSource = null;
                }
            }
                 
        }
        public ImageSource GetSource()
        {
            return RefrenceSource;
        }
        private Int32Rect ValidateRect(Int32Rect value, BitmapSource source)
        {
            int PixelWidth = (source != null ? source.PixelWidth : Source.PixelWidth);
            int PixelHeight = (source != null ? source.PixelHeight : Source.PixelHeight);

            Int32Rect SourceValue = new Int32Rect(0, 0, PixelWidth, PixelHeight);

            bool Diff_X = value.X < 0;
            bool Diff_Y = value.Y < 0;

            if (Diff_X)
            {
                value.Width += value.X;
                value.X = 0;
            }
            
            if (Diff_Y)
            {
                value.Height += value.Y;
                value.Y = 0;
            }

            Rect Param1 = Extensions.Int32RectToRect(SourceValue);
            Rect Param2 = Extensions.Int32RectToRect(value);

            Int32Rect Result = Extensions.RectToInt32Rect(System.Windows.Rect.Intersect(Param1, Param2));

            return Result;
        }
    }
}
