using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Rapid.NET.Wpf.Editors
{
    public partial class ControlStack : StackPanel
    {
        private const int MARGIN = 3;
        private Dictionary<Control, int> _Padding = new Dictionary<Control, int>();

        public int Level;

        public ControlStack()
        {
            
        }

        public double ControlWidth { get { return Width - 2 * MARGIN; } }

        public void AddStackedControl(Control control, int paddingAfter)
        {
            _Padding[control] = paddingAfter;

            // TODO??
            //if (this.VisualChildrenCount == 1)
            //    control.Top = MARGIN;

            control.Padding = new Thickness(
                MARGIN, control.Padding.Top, 
                control.Padding.Right, control.Padding.Bottom);
            
            control.Width = ControlWidth;
            Children.Add(control);
        }

        //private void ControlStack_Layout(object sender, LayoutEventArgs e)
        //{
        //    if (e.AffectedControl == this)
        //    {
        //        System.Diagnostics.Debug.Print("ControlStack_Layout[" + Level + "] this " + e.AffectedProperty);
        //        if (e.AffectedProperty == "Bounds")
        //        {
        //            foreach (Control control in this.Controls)
        //            {
        //                control.Width = this.ClientSize.Width - MARGIN - control.Left;
        //            }
        //        }
        //    }
        //    else if (this.Controls.Contains(e.AffectedControl))
        //    {
        //        Control above = null;
        //        foreach (Control control in Controls.Cast<Control>())
        //        {
        //            if (above == null)
        //            {
        //                above = control;
        //            }
        //            else
        //            {
        //                control.Top = above.Bottom + _Padding[above];
        //                above = control;
        //            }
        //        }
        //        System.Diagnostics.Debug.Print("ControlStack_Layout[" + Level + "] resizing height from " + this.ClientSize.Height + " to " + (Controls[Controls.Count - 1].Bottom + MARGIN));
        //        this.ClientSize = new Size(this.ClientSize.Width, Controls[Controls.Count - 1].Bottom + MARGIN);
        //    }
        //    else
        //    {
        //        System.Diagnostics.Debug.Print("ControlStack_Layout[" + Level + "] UNKNOWN TARGET " + e.AffectedControl.Name + " " + e.AffectedProperty);
        //    }
        //}
    }
}
