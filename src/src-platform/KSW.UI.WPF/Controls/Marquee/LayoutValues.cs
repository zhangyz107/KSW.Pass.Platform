using KSW.UI.WPF.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KSW.UI.WPF.Controls
{
    public class LayoutValues
    {
        public Size Bounds { get; set; }
        public Size PresenterSize { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Diff { get; set; }
        public Direction Direction { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
    }
}
