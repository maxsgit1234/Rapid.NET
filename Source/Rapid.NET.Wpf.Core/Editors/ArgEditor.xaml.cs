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
    /// Interaction logic for ArgEditor.xaml
    /// </summary>
    public partial class ArgEditor : UserControl, IObjectEditor
    {
        //private readonly ObjectEditors.EditorType _Editor;
        private readonly IArgEditor _Editor;
        private readonly Type _Type;

        public ArgEditor(Type type)
        {
            _Type = type;
            
            var obj = ObjectEditors.GetEditorType(type);
            //if (type.IsNullable(out Type under))
            //    Console.WriteLine("adsflvkjn");

            switch (obj)
            {
                case ObjectEditors.EditorType.Array:
                    _Editor = new ArrayArgEditor(type); break;
                case ObjectEditors.EditorType.Numeric:
                    _Editor = new NumericArgEditor(type); break;
                //case ObjectEditors.EditorType.String:
                default:
                    _Editor = new StringArgEditor(); break;
            }

            //if (obj == ObjectEditors.EditorType.String)
            

            Initialized += ArgEditor_Initialized;
            InitializeComponent();
        }

        public bool HasValue
        {
            get { return _ValChk.IsChecked.HasValue && _ValChk.IsChecked.Value; }
        }

        private void ArgEditor_Initialized(object sender, EventArgs e)
        {
            //if (_Type.IsNullable(out Type underlying))
            //    Console.WriteLine("adsflkvjn");

            if (_Type.IsValueType && !_Type.IsNullable(out _))
            {
                _ValChk.IsChecked = true;
                _ValChk.IsEnabled = false;
            }

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

        public virtual object GetValue()
        {
            if (HasValue)
                return _Editor.GetValue(_Value.Text);
            else
                return null;
        }

        public virtual void SetValue(object value)
        {
            if (value == null)
            {
                _ValChk.IsChecked = false;
                _Value.IsEnabled = false;
            }
            else
            {
                _ValChk.IsChecked = true;
                _Value.Text = _Editor.GetString(value);
            }
        }
    }

    public class NumericArgEditor : IArgEditor
    {
        private readonly Type _Type;

        public NumericArgEditor(Type type)
        {
            _Type = type;
        }

        public string GetString(object value)
        {
            return JsonObject.Serialize(value);
        }

        public object GetValue(string text)
        {
            return JsonObject.Deserialize(text, _Type);
        }
    }

    public class ArrayArgEditor : IArgEditor
    {
        private readonly Type _Type;

        public ArrayArgEditor(Type type)
        {
            _Type = type;
        }

        public string GetString(object value)
        {
            return JsonObject.Serialize(value).Replace("\r\n", "");
        }

        public object GetValue(string text)
        {
            return JsonObject.Deserialize(text, _Type);
        }
    }

    public class StringArgEditor : IArgEditor
    {
        public object GetValue(string text)
        {
            return text;
        }

        public string GetString(object value)
        {
            return (string)value;
        }
    }

}
