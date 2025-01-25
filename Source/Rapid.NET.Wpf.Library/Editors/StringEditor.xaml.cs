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

namespace Rapid.NET.Wpf.Editors
{
    /// <summary>
    /// Interaction logic for StringEditor.xaml
    /// </summary>
    public partial class StringEditor : UserControl, IObjectEditor
    {
        private readonly Type _Type;

        public StringEditor(Type type)
        {
            _Type = type;
            InitializeComponent();
        }

        public object GetValue()
        {
            return _Value.Text;
        }

        public void SetValue(object newValue)
        {
            _Value.Text = (string)newValue;
        }
    }
}
