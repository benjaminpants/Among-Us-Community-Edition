using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Converters;
using System.Windows.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Point = System.Windows.Point;

namespace GenerationsLib.WPF.Controls
{
    public class EndlessStackPanel : Panel, IScrollInfo
    {
        // Fields
        private ScrollData _scrollData;
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(EndlessStackPanel), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(EndlessStackPanel.OnOrientationChanged)), new ValidateValueCallback(IsValidOrientation));
        private static bool IsValidOrientation(object o)
        {
            Orientation orientation = (Orientation)o;
            if (orientation != Orientation.Horizontal)
            {
                return (orientation == Orientation.Vertical);
            }
            return true;
        }

        // Methods
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection children = base.Children;
            bool flag = this.Orientation == Orientation.Horizontal;
            Rect finalRect = new Rect(arrangeSize);
            double width = 0.0;
            if (this.IsScrolling)
            {
                if (flag)
                {
                    finalRect.X = this.ComputePhysicalFromLogicalOffset(this._scrollData._computedOffset.X, true);
                    finalRect.Y = -1.0 * this._scrollData._computedOffset.Y;
                }
                else
                {
                    finalRect.X = -1.0 * this._scrollData._computedOffset.X;
                    finalRect.Y = this.ComputePhysicalFromLogicalOffset(this._scrollData._computedOffset.Y, false);
                }
            }
            int num2 = 0;
            int count = children.Count;
            while (num2 < count)
            {
                UIElement element = children[num2 % children.Count];
                if (element != null)
                {
                    if (flag)
                    {
                        finalRect.X += width;
                        width = element.DesiredSize.Width;
                        finalRect.Width = width;
                        if (finalRect.X <= element.DesiredSize.Width * -1)
                        {
                            count++;
                        }
                        finalRect.Height = Math.Max(arrangeSize.Height, element.DesiredSize.Height);
                    }
                    else
                    {
                        finalRect.Y += width;
                        width = element.DesiredSize.Height;
                        finalRect.Height = width;
                        if (finalRect.Y <= element.DesiredSize.Height * -1)
                        {
                            count++;
                        }
                        finalRect.Width = Math.Max(arrangeSize.Width, element.DesiredSize.Width);
                    }
                    element.Arrange(finalRect);
                }
                num2++;
            }
            return arrangeSize;
        }

        private static int CoerceOffsetToInteger(double offset, int numberOfItems)
        {
            if (double.IsNegativeInfinity(offset))
            {
                return 0;
            }
            if (double.IsPositiveInfinity(offset))
            {
                return (numberOfItems - 1);
            }
            return (int)offset;
        }

        private double ComputePhysicalFromLogicalOffset(double logicalOffset, bool fHorizontal)
        {
            double num = 0.0;
            UIElementCollection children = base.Children;
            for (int i = 0; i < logicalOffset; i++)
            {
                num -= fHorizontal ? children[((i + children.Count) % children.Count)].DesiredSize.Width : children[((i + children.Count) % children.Count)].DesiredSize.Height;
            }
            return num;
        }

        private void EnsureScrollData()
        {
            if (this._scrollData == null)
            {
                this._scrollData = new ScrollData();
            }
        }

        private int FindChildIndexThatParentsVisual(Visual child)
        {
            DependencyObject reference = child;
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != this)
            {
                reference = parent;
                parent = VisualTreeHelper.GetParent(reference);
                if (parent == null)
                {
                    throw new ArgumentException("Stack_VisualInDifferentSubTree", "child");
                }
            }
            return base.Children.IndexOf((UIElement)reference);
        }

        public void LineDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + ((this.Orientation == Orientation.Vertical) ? 1.0 : 16.0));
        }

        public void LineLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - ((this.Orientation == Orientation.Horizontal) ? 1.0 : 16.0));
        }

        public void LineRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + ((this.Orientation == Orientation.Horizontal) ? 1.0 : 16.0));
        }

        public void LineUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - ((this.Orientation == Orientation.Vertical) ? 1.0 : 16.0));
        }

        private double CoerceOffset(double offset, double extent, double viewport)
        {
            if (offset > (extent - viewport))
            {
                offset = extent - viewport;
            }
            if (offset < 0.0)
            {
                offset = 0.0;
            }
            return offset;
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            Vector newOffset = new Vector();
            Rect newRect = new Rect();
            if ((rectangle.IsEmpty || (visual == null)) || ((visual == this) || !base.IsAncestorOf(visual)))
            {
                return Rect.Empty;
            }
            rectangle = visual.TransformToAncestor(this).TransformBounds(rectangle);
            if (!this.IsScrolling)
            {
                return rectangle;
            }
            this.MakeVisiblePhysicalHelper(rectangle, ref newOffset, ref newRect);
            int childIndex = this.FindChildIndexThatParentsVisual(visual);
            this.MakeVisibleLogicalHelper(childIndex, ref newOffset, ref newRect);
            if (Orientation == Orientation.Horizontal)
                newOffset.Y = this.CoerceOffset(newOffset.Y, this._scrollData._extent.Height, this._scrollData._viewport.Height);
            else
                newOffset.X = this.CoerceOffset(newOffset.X, this._scrollData._extent.Width, this._scrollData._viewport.Width);
            if (!DoubleUtil.AreClose(newOffset, this._scrollData._offset))
            {
                this._scrollData._offset = newOffset;
                base.InvalidateMeasure();
                this.OnScrollChange();
            }
            return newRect;
        }

        private void MakeVisibleLogicalHelper(int childIndex, ref Vector newOffset, ref Rect newRect)
        {
            int x;
            int width;
            bool flag = this.Orientation == Orientation.Horizontal;
            double num4 = 0.0;
            if (flag)
            {
                x = (int)this._scrollData._computedOffset.X;
                width = (int)this._scrollData._viewport.Width;
            }
            else
            {
                x = (int)this._scrollData._computedOffset.Y % Children.Count;
                width = (int)this._scrollData._viewport.Height;
            }
            int num2 = x;
            bool needsScroll = (childIndex >= x && childIndex < x + width || childIndex + Children.Count < x + width) ? false : true;
            if (needsScroll)
            {
                int abst = (x + width - childIndex + Children.Count) % Children.Count;
                bool scrollUp = false;
                if ((childIndex < x) && (x - childIndex < abst))
                {
                    abst = x - childIndex;
                    scrollUp = true;
                }
                if (abst > (Children.Count - width) / 2) scrollUp = !scrollUp;

                if (scrollUp)
                {
                    num2 = childIndex;
                }
                else
                {
                    Size desiredSize = base.InternalChildren[childIndex].DesiredSize;
                    double num5 = flag ? desiredSize.Width : desiredSize.Height;
                    double num6 = this._scrollData._physicalViewport - num5;
                    int num7 = childIndex;
                    while (DoubleUtil.GreaterThanOrClose(num6, 0.0))
                    {
                        num7 = ((num7 - 1) + Children.Count) % Children.Count;
                        desiredSize = base.InternalChildren[num7].DesiredSize;
                        num5 = flag ? desiredSize.Width : desiredSize.Height;
                        num4 += num5;
                        num6 -= num5;
                    }
                    if ((num7 != childIndex) && DoubleUtil.LessThan(num6, 0.0))
                    {
                        num4 -= num5;
                        num7++;
                    }
                    num2 = num7;
                }
            }
            if (flag)
            {
                newOffset.X = num2;
                newRect.X = num4;
                newRect.Width = base.InternalChildren[childIndex].DesiredSize.Width;
            }
            else
            {
                newOffset.Y = num2;
                newRect.Y = num4;
                newRect.Height = base.InternalChildren[childIndex].DesiredSize.Height;
            }
        }

        private double ComputeScrollOffsetWithMinimalScroll(double topView, double bottomView, double topChild, double bottomChild)
        {
            bool flag = DoubleUtil.LessThan(topChild, topView) && DoubleUtil.LessThan(bottomChild, bottomView);
            bool flag2 = DoubleUtil.GreaterThan(bottomChild, bottomView) && DoubleUtil.GreaterThan(topChild, topView);
            bool flag3 = (bottomChild - topChild) > (bottomView - topView);
            if (flag && !flag3)
            {
                return topChild;
            }
            if (flag2 && flag3)
            {
                return topChild;
            }
            if (!flag && !flag2)
            {
                return topView;
            }
            return (bottomChild - (bottomView - topView));
        }

        private void MakeVisiblePhysicalHelper(Rect r, ref Vector newOffset, ref Rect newRect)
        {
            double y;
            double viewportHeight;
            double x;
            double height;
            bool flag = this.Orientation == Orientation.Horizontal;
            if (flag)
            {
                y = this._scrollData._computedOffset.Y;
                viewportHeight = this.ViewportHeight;
                x = r.Y;
                height = r.Height;
            }
            else
            {
                y = this._scrollData._computedOffset.X;
                viewportHeight = this.ViewportWidth;
                x = r.X;
                height = r.Width;
            }
            x += y;
            double num5 = this.ComputeScrollOffsetWithMinimalScroll(y, y + viewportHeight, x, x + height);
            double num6 = Math.Max(x, num5);
            height = Math.Max((double)(Math.Min((double)(height + x), (double)(num5 + viewportHeight)) - num6), (double)0.0);
            x = num6;
            x -= y;
            if (flag)
            {
                newOffset.Y = num5;
                newRect.Y = x;
                newRect.Height = height;
            }
            else
            {
                newOffset.X = num5;
                newRect.X = x;
                newRect.Width = height;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            int num;
            double width;
            UIElementCollection internalChildren = base.InternalChildren;
            Size size = new Size();
            Size availableSize = constraint;
            bool flag = this.Orientation == Orientation.Horizontal;
            int num2 = -1;
            if (flag)
            {
                availableSize.Width = double.PositiveInfinity;
                if (this.IsScrolling && this.CanVerticallyScroll)
                {
                    availableSize.Height = double.PositiveInfinity;
                }
                num = this.IsScrolling ? CoerceOffsetToInteger(this._scrollData._offset.X, internalChildren.Count) : 0;
                width = constraint.Width;
            }
            else
            {
                availableSize.Height = double.PositiveInfinity;
                if (this.IsScrolling && this.CanHorizontallyScroll)
                {
                    availableSize.Width = double.PositiveInfinity;
                }
                num = this.IsScrolling ? CoerceOffsetToInteger(this._scrollData._offset.Y, internalChildren.Count) : 0;
                width = constraint.Height;
            }
            int num5 = 0;
            int count = internalChildren.Count;
            while (num5 < count)
            {
                UIElement element = internalChildren[num5];
                if (element != null)
                {
                    double height;
                    element.Measure(availableSize);
                    Size desiredSize = element.DesiredSize;
                    if (flag)
                    {
                        size.Width += desiredSize.Width;
                        size.Height = Math.Max(size.Height, desiredSize.Height);
                        height = desiredSize.Width;
                    }
                    else
                    {
                        size.Width = Math.Max(size.Width, desiredSize.Width);
                        size.Height += desiredSize.Height;
                        height = desiredSize.Height;
                    }
                    if ((this.IsScrolling && (num2 == -1)) && (num5 >= num))
                    {
                        width -= height;
                        if (DoubleUtil.LessThanOrClose(width, 0.0))
                        {
                            num2 = num5;
                        }
                    }
                }
                num5++;
            }
            if (this.IsScrolling)
            {
                Size viewport = constraint;
                Size extent = size;
                Vector offset = this._scrollData._offset;
                if (num2 == -1)
                {
                    num2 = internalChildren.Count - 1;
                }

                int num10 = (num > Children.Count - 1) ? Children.Count : num;
                num10 = num10 < 0 ? num10 + Children.Count : num10;
                while (num10 > 0)
                {
                    double num7 = width;
                    if (flag)
                    {
                        num7 -= internalChildren[(num10 - 1) % Children.Count].DesiredSize.Width;
                    }
                    else
                    {
                        num7 -= internalChildren[(num10 - 1) % Children.Count].DesiredSize.Height;
                    }
                    if (DoubleUtil.LessThan(num7, 0.0))
                    {
                        break;
                    }
                    num10--;
                    width = num7;
                }

                int num8 = internalChildren.Count;
                int num9 = num2 - num10;
                if ((num9 == 0) || DoubleUtil.GreaterThanOrClose(width, 0.0))
                {
                    num9++;
                }
                if (flag)
                {
                    this._scrollData._physicalViewport = viewport.Width;
                    viewport.Width = num9;
                    extent.Width = num8;
                    offset.X = num;
                    offset.Y = Math.Max(0.0, Math.Min(offset.Y, extent.Height - viewport.Height));
                }
                else
                {
                    this._scrollData._physicalViewport = viewport.Height;
                    viewport.Height = num9;
                    extent.Height = num8;
                    offset.Y = num;
                    offset.X = Math.Max(0.0, Math.Min(offset.X, extent.Width - viewport.Width));
                }
                size.Width = Math.Min(size.Width, constraint.Width);
                size.Height = Math.Min(size.Height, constraint.Height);
                this.VerifyScrollingData(viewport, extent, offset);
            }
            return size;
        }

        public void MouseWheelDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + (SystemParameters.WheelScrollLines * ((this.Orientation == Orientation.Vertical) ? 1.0 : 16.0)));
        }

        public void MouseWheelLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - (3.0 * ((this.Orientation == Orientation.Horizontal) ? 1.0 : 16.0)));
        }

        public void MouseWheelRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + (3.0 * ((this.Orientation == Orientation.Horizontal) ? 1.0 : 16.0)));
        }

        public void MouseWheelUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - (SystemParameters.WheelScrollLines * ((this.Orientation == Orientation.Vertical) ? 1.0 : 16.0)));
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ResetScrolling(d as EndlessStackPanel);
        }

        private void OnScrollChange()
        {
            if (this.ScrollOwner != null)
            {
                this.ScrollOwner.InvalidateScrollInfo();
            }
        }

        public void PageDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);
        }

        public void PageLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth);
        }

        public void PageRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth);
        }

        public void PageUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.ViewportHeight);
        }

        private static void ResetScrolling(EndlessStackPanel element)
        {
            element.InvalidateMeasure();
            if (element.IsScrolling)
            {
                element._scrollData.ClearLayout();
            }
        }

        public void SetHorizontalOffset(double offset)
        {
            this.EnsureScrollData();
            while (offset < 0) offset += Children.Count;
            if (!DoubleUtil.AreClose(offset, this._scrollData._offset.X))
            {
                this._scrollData._offset.X = offset;
                base.InvalidateMeasure();
            }
        }

        public void SetVerticalOffset(double offset)
        {
            this.EnsureScrollData();
            while (offset < 0) offset += Children.Count;
            if (!DoubleUtil.AreClose(offset, this._scrollData._offset.Y))
            {
                this._scrollData._offset.Y = offset;
                base.InvalidateMeasure();
            }
        }

        private void VerifyScrollingData(Size viewport, Size extent, Vector offset)
        {
            bool flag = true;
            flag &= DoubleUtil.AreClose(viewport, this._scrollData._viewport);
            flag &= DoubleUtil.AreClose(extent, this._scrollData._extent);
            flag &= DoubleUtil.AreClose(offset, this._scrollData._computedOffset);
            this._scrollData._offset = offset;
            if (!flag)
            {
                this._scrollData._viewport = viewport;
                this._scrollData._extent = extent;
                this._scrollData._computedOffset = offset;
                this.OnScrollChange();
            }
        }

        // Properties
        [DefaultValue(false)]
        public bool CanHorizontallyScroll
        {
            get
            {
                if (this._scrollData == null)
                {
                    return false;
                }
                return this._scrollData._allowHorizontal;
            }
            set
            {
                this.EnsureScrollData();
                if (this._scrollData._allowHorizontal != value)
                {
                    this._scrollData._allowHorizontal = value;
                    base.InvalidateMeasure();
                }
            }
        }

        [DefaultValue(false)]
        public bool CanVerticallyScroll
        {
            get
            {
                if (this._scrollData == null)
                {
                    return false;
                }
                return this._scrollData._allowVertical;
            }
            set
            {
                this.EnsureScrollData();
                if (this._scrollData._allowVertical != value)
                {
                    this._scrollData._allowVertical = value;
                    base.InvalidateMeasure();
                }
            }
        }

        public double ExtentHeight
        {
            get
            {
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return this._scrollData._extent.Height;
            }
        }

        public double ExtentWidth
        {
            get
            {
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return this._scrollData._extent.Width;
            }
        }

        protected override bool HasLogicalOrientation
        {
            get
            {
                return true;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double HorizontalOffset
        {
            get
            {
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return (this.Orientation == Orientation.Horizontal) ? this._scrollData._computedOffset.X % Children.Count : this._scrollData._computedOffset.X;
            }
        }

        private bool IsScrolling
        {
            get
            {
                return ((this._scrollData != null) && (this._scrollData._scrollOwner != null));
            }
        }

        protected override Orientation LogicalOrientation
        {
            get
            {
                return this.Orientation;
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)base.GetValue(OrientationProperty);
            }
            set
            {
                base.SetValue(OrientationProperty, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ScrollViewer ScrollOwner
        {
            get
            {
                this.EnsureScrollData();
                return this._scrollData._scrollOwner;
            }
            set
            {
                this.EnsureScrollData();
                if (value != this._scrollData._scrollOwner)
                {
                    ResetScrolling(this);
                    this._scrollData._scrollOwner = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double VerticalOffset
        {
            get
            {
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return (this.Orientation == Orientation.Horizontal) ? this._scrollData._computedOffset.Y : this._scrollData._computedOffset.Y % Children.Count;
            }
        }

        public double ViewportHeight
        {
            get
            {
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return (this.Orientation == Orientation.Horizontal) ? this._scrollData._viewport.Height : 1.0;
            }
        }

        public double ViewportWidth
        {
            get
            {
                if (this._scrollData == null)
                {
                    return 0.0;
                }
                return (this.Orientation == Orientation.Horizontal) ? 1.0 : this._scrollData._viewport.Width;
            }
        }

        // Nested Types
        private class ScrollData
        {
            // Fields
            internal bool _allowHorizontal;
            internal bool _allowVertical;
            internal Vector _computedOffset = new Vector(0.0, 0.0);
            internal Size _extent;
            internal Vector _offset;
            internal double _physicalViewport;
            internal ScrollViewer _scrollOwner;
            internal Size _viewport;

            // Methods
            internal void ClearLayout()
            {
                this._offset = new Vector();
                Size size = new Size();
                this._extent = size;
                this._viewport = size;
                this._physicalViewport = 0.0;
            }
        }

    }

    public static class DoubleUtil
    {
        // Fields
        internal const double DBL_EPSILON = 2.2204460492503131E-16;
        internal const float FLT_MIN = 1.175494E-38f;

        // Methods
        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            double num = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 2.2204460492503131E-16;
            double num2 = value1 - value2;
            return ((-num < num2) && (num > num2));
        }

        public static bool AreClose(System.Windows.Point point1, System.Windows.Point point2)
        {
            return (AreClose(point1.X, point2.X) && AreClose(point1.Y, point2.Y));
        }

        public static bool AreClose(Rect rect1, Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            return (((!rect2.IsEmpty && AreClose(rect1.X, rect2.X)) && (AreClose(rect1.Y, rect2.Y) && AreClose(rect1.Height, rect2.Height))) && AreClose(rect1.Width, rect2.Width));
        }

        public static bool AreClose(Size size1, Size size2)
        {
            return (AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height));
        }

        public static bool AreClose(Vector vector1, Vector vector2)
        {
            return (AreClose(vector1.X, vector2.X) && AreClose(vector1.Y, vector2.Y));
        }

        public static int DoubleToInt(double val)
        {
            if (0.0 >= val)
            {
                return (int)(val - 0.5);
            }
            return (int)(val + 0.5);
        }

        public static bool GreaterThan(double value1, double value2)
        {
            return ((value1 > value2) && !AreClose(value1, value2));
        }

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (value1 <= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static bool IsBetweenZeroAndOne(double val)
        {
            return (GreaterThanOrClose(val, 0.0) && LessThanOrClose(val, 1.0));
        }

        public static bool IsNaN(double value)
        {
            NanUnion union = new NanUnion();
            union.DoubleValue = value;
            ulong num = union.UintValue & 18442240474082181120L;
            ulong num2 = union.UintValue & ((ulong)0xfffffffffffffL);
            if ((num != 0x7ff0000000000000L) && (num != 18442240474082181120L))
            {
                return false;
            }
            return (num2 != 0L);
        }

        public static bool IsOne(double value)
        {
            return (Math.Abs((double)(value - 1.0)) < 2.2204460492503131E-15);
        }

        public static bool IsZero(double value)
        {
            return (Math.Abs(value) < 2.2204460492503131E-15);
        }

        public static bool LessThan(double value1, double value2)
        {
            return ((value1 < value2) && !AreClose(value1, value2));
        }

        public static bool LessThanOrClose(double value1, double value2)
        {
            if (value1 >= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static bool RectHasNaN(Rect r)
        {
            if ((!IsNaN(r.X) && !IsNaN(r.Y)) && (!IsNaN(r.Height) && !IsNaN(r.Width)))
            {
                return false;
            }
            return true;
        }

        // Nested Types
        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            // Fields
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal ulong UintValue;
        }
    }
}
