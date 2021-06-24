using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for DictionaryEditor.xaml
    /// </summary>
    public partial class DictionaryEditor : UserControl, IObjectEditor
    {
        class FieldResources
        {
            public Type Type;

            public FieldInfo Field;

            public IObjectEditor Editor;

            public DocumentationAttribute Documentation;
        }

        private readonly Type _ObjectType;
        
        private Func<object> _Constructor;
        private object _DefaultValue;
        
        private Dictionary<string, FieldResources> _Fields
            = new Dictionary<string, FieldResources>();

        public DictionaryEditor(Type type)
        {
            _ObjectType = type;

            _Constructor = ObjectEditors.GetDefaultMaker(_ObjectType);
            if (_Constructor == null)
                throw new InvalidOperationException(
                    "DictionaryEditor only supports objects which have a parameterless constructor");

            _DefaultValue = _Constructor();
            
            //_LastValue = _DefaultValue;
            //_DefaultJson = JsonObj.Parse(_DefaultValue);
            //if (_DefaultJson.ObjectType != JsonObject.Type.Dictionary)
            //    throw new InvalidOperationException("DictionaryEditor only supports objects that are serialized as JSON Dictionary types");

            FieldInfo[] fields = _ObjectType.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance);
            foreach (FieldInfo fi in fields)
            {
                //if (fi.IsInitOnly)
                //{
                //    Console.WriteLine("Skipping readonly field: " + _ObjectType.Name + "." + fi.Name);
                //    continue;
                //}

                var f = new FieldResources()
                {
                    Type = fi.FieldType,
                    Field = fi,
                    Documentation = fi.GetCustomAttribute<DocumentationAttribute>()
                };

                _Fields[fi.Name] = f;
            }

            Initialized += DictionaryEditor_Initialized;
            InitializeComponent();
        }

        public bool HasValue
        {
            get { return _ValChk.IsChecked.HasValue && _ValChk.IsChecked.Value; }
        }

        private void DictionaryEditor_Initialized(object sender, EventArgs e)
        {
            _ValChk.Checked += _ValChk_Checked;
            _ValChk.Unchecked += _ValChk_Unchecked;
            SetValue(_DefaultValue);
        }

        private void _ValChk_Unchecked(object sender, RoutedEventArgs e)
        {
            //_Value.IsEnabled = false;
            _Stack.Visibility = Visibility.Collapsed;
        }

        private void _ValChk_Checked(object sender, RoutedEventArgs e)
        {
            //_Value.IsEnabled = true;
            _Stack.Visibility = Visibility.Visible;
        }


        public object GetValue()
        {
            if (!HasValue)
                return null;
            
            object result = _Constructor();
            foreach (var kvp in _Fields)
            {
                if (kvp.Value.Editor != null)
                    kvp.Value.Field.SetValue(result, kvp.Value.Editor.GetValue());
            }
            return result;
        }

        public void TrySetValueFromArgs(string args)
        {
            if (args == null)
                return;

            try
            {
                object value = JsonObject.Deserialize(args, _ObjectType);
                SetValue(value);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Could not set args for " 
                    + _ObjectType + ": " + args + ": " + ex);
            }
        }

        public void SetValue(object value)
        {
            if (_Stack.Children.Count == 0)
                CreateInputFields();

            if (value == null)
            {
                _ValChk.IsChecked = false;
                _Stack.Visibility = Visibility.Collapsed;
            }
            else
            {
                _ValChk.IsChecked = true;
                _Stack.Visibility = Visibility.Visible;
                foreach (var kvp in _Fields)
                    kvp.Value.Editor.SetValue(kvp.Value.Field.GetValue(value));
            }
        }

        private void CreateInputFields()
        {
            foreach (var kvp in _Fields)
            {
                UserControl control = ObjectEditors.MakeEditor(kvp.Value.Type);
                kvp.Value.Editor = control as IObjectEditor;

                if (!kvp.Value.Field.IsPublic)
                    continue;

                var label = new Label()
                {
                    Content = kvp.Value.Field.Name + " (" 
                    + kvp.Value.Field.FieldType.NiceName() + ")",
                };

                label.FontWeight = FontWeights.Bold;
                if (!kvp.Value.Field.IsPublic)
                    label.Foreground = Brushes.Gray;


                //if (control is DictionaryEditor)
                //{
                    Border b = new Border();
                    b.BorderThickness = new Thickness(1, 0, 0, 0);
                    b.BorderBrush = Brushes.Gray;
                    StackPanel panel = new StackPanel();
                    b.Child = panel;
                    //panel.Background = Brushes.LightGray;
                    panel.Margin = new Thickness(10, 0, 0, 0);
                    panel.Children.Add(label);
                    panel.Children.Add(control);
                    _Stack.Children.Add(b);
                //}
                //else
                //{
                //    _Stack.Children.Add(label);
                //    _Stack.Children.Add(control);
                //}
                

                //if (kvp.Value.Documentation != null)
                //{
                //    var description = new TextBox()
                //    {
                //        //BorderStyle = BorderStyle.None,
                //        //BackColor = SystemColors.Control,
                //        //Enabled = false,
                //        //Multiline = true,
                //        Text = kvp.Value.Documentation.Description,
                //        //Width = csInputs.ControlWidth
                //    };
                //    //Size contentSize = TextRenderer.MeasureText(description.Text, description.Font, description.ClientSize, TextFormatFlags.WordBreak);
                //    //description.ClientSize = new Size(description.ClientSize.Width, contentSize.Height);
                //    //csInputs.AddStackedControl(description, 2);
                //}

                //csInputs.AddStackedControl(control, 4);
            }
        }

        // https://stackoverflow.com/a/25287378/651139
        private static string PrettyTypeName(Type t)
        {
            if (t.IsGenericType)
            {
                return string.Format(
                    "{0}<{1}>",
                    t.Name.Substring(0, t.Name.LastIndexOf("`", StringComparison.InvariantCulture)),
                    string.Join(", ", t.GetGenericArguments().Select(PrettyTypeName)));
            }

            return t.Name;
        }


    }

}

