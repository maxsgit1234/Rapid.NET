using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Rapid.NET.Wpf
{
    public delegate void RunScript(bool newProcess, bool newThread);

    public class RunPanel : StackPanel
    {
        public event RunScript Run;
        public event Action Cancel;

        private Button _RunBtn, _CancelBtn;
        private CheckBox _NewProcessChk, _NewThreadChk;

        public RunPanel()
        {
            Initialized += RunPanel_Initialized;
        }

        private void RunPanel_Initialized(object sender, EventArgs e)
        {
            Orientation = Orientation.Horizontal;

            _NewProcessChk = new CheckBox { Content = "New Process", IsChecked = false };
            _NewThreadChk = new CheckBox { Content = "New Thread", IsChecked = false };
            _CancelBtn = new Button { Content = "Cancel" };
            _RunBtn = new Button { Content = "Run" };

            _NewProcessChk.Margin = new Thickness(10);
            _NewThreadChk.Margin = new Thickness(10);
            _CancelBtn.Margin = new Thickness(10);
            _RunBtn.Margin = new Thickness(10);

            Children.Add(_NewProcessChk);
            Children.Add(_NewThreadChk);
            Children.Add(_CancelBtn);
            Children.Add(_RunBtn);

            _RunBtn.Click += _RunBtn_Click;
            _CancelBtn.Click += _CancelBtn_Click;
        }

        private void _RunBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Run?.Invoke(
                _NewProcessChk.IsChecked.Value, 
                _NewThreadChk.IsChecked.Value);
        }

        private void _CancelBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Cancel?.Invoke();
        }

    }
}
