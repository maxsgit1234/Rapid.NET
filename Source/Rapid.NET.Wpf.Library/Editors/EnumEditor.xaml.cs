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
    /// Interaction logic for EnumEditor.xaml
    /// </summary>
    public partial class EnumEditor : UserControl, IObjectEditor
    {
        public enum TrueFalse
        {
            False = 0,
            True = 1,
        }

        private readonly bool _IsNullable;
        private readonly Type _Type;

        public EnumEditor(Type type)
        {
            if (type.IsNullable(out Type underlying))
            {
                _IsNullable = true;
                type = underlying;
            }

            if (type == typeof(bool))
                type = typeof(TrueFalse);

            _Type = type;

            Initialized += EnumEditor_Initialized;
            InitializeComponent();
        }

        public bool HasValue
        {
            get { return _ValChk.IsChecked.HasValue && _ValChk.IsChecked.Value; }
        }

        private void EnumEditor_Initialized(object sender, EventArgs e)
        {
            if (_IsNullable)
                _ValChk.IsEnabled = true;

            object[] items = Enum.GetValues(_Type).Cast<object>().ToArray();
            foreach (object o in items)
                _Value.Items.Add(o);

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
            {
                object ret = _Value.SelectedItem;
                if (_Type == typeof(TrueFalse))
                    return (TrueFalse)ret == TrueFalse.True;
                else
                    return ret;
            }
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
                _Value.IsEnabled = true;
                if (_Type == typeof(TrueFalse))
                {
                    TrueFalse tf = (bool)value ? TrueFalse.True : TrueFalse.False;
                    _Value.SelectedItem = tf;
                }
            }

            _Value.SelectedItem = value;
        }
    }
}
