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
    /// Interaction logic for ArrayEditor.xaml
    /// </summary>
    public partial class ArrayEditor : UserControl, IObjectEditor
    {
        private readonly Type _Type;
        
        public ArrayEditor(Type type) 
        {
            _Type = type;
            
            Initialized += ArrayEditor_Initialized;
            InitializeComponent();
        }

        public bool HasValue
        { 
            get { return _ValChk.IsChecked.HasValue && _ValChk.IsChecked.Value; }
        }

        private void ArrayEditor_Initialized(object sender, EventArgs e)
        {
            _ValChk.Checked += _ValChk_Checked;
            _ValChk.Unchecked += _ValChk_Unchecked;
        }

        private void _ValChk_Unchecked(object sender, RoutedEventArgs e)
        {
            _Value.IsEnabled = false;
        }

        private void _ValChk_Checked(object sender, RoutedEventArgs e)
        {
            _Value.IsEnabled = true;
        }

        public object GetValue()
        {
            if (HasValue)
                return JsonObject.Deserialize(_Value.Text, _Type);
            else
                return null;
        }

        public void SetValue(object value)
        {
            if (value == null)
            {
                _ValChk.IsChecked = false;
                _Value.IsEnabled = false;
            }
            else
            {
                _ValChk.IsChecked = true;
                _Value.Text = JsonObject.Serialize(value).Replace("\r\n", "");
            }
        }
    }
}
