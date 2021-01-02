using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AmongUsCE_AnimationEditor
{
    public class CustomUniformGrid : UniformGrid
    {

        protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
        {
            Size size = sizeInfo.NewSize;
            if (size.Width >= 200) this.Columns = 1;
            if (size.Width >= 250) this.Columns = 2;
            if (size.Width >= 300) this.Columns = 3;
            if (size.Width >= 350) this.Columns = 4;
        }
    }
}
