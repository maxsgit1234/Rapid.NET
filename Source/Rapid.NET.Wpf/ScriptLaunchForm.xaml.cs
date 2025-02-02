﻿using Rapid.NET.Wpf.Editors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
using static Rapid.NET.ScriptMethods;


namespace Rapid.NET.Wpf
{
    /// <summary>
    /// Interaction logic for ScriptLaunchForm.xaml
    /// </summary>
    public partial class ScriptLaunchForm : UserControl
    {
        private readonly Dictionary<string, Script> _Scripts = new Dictionary<string, Script>();
        private readonly Dictionary<Script, ScriptNode> _Nodes = new Dictionary<Script, ScriptNode>();

        
        private readonly InvocationHistory _History;
        private readonly WindowConfig _Window;
        private readonly Window _Parent;

        public ScriptLaunchForm(Window parent, 
            List<Script> scripts)
        {
            
            _Parent = parent;
            _Scripts = scripts.OrderBy(s => s.FullName).ToDictionary(s => s.FullName, s => s);
            _History = InvocationHistory.FromFile(null);
            _Window = WindowConfig.FromFile(null);
            
            Initialized += ScriptSelectorTree_Initialized;
            InitializeComponent();
        }

        private IObjectEditor _ArgEditor;
        private Script _Selected;

        private void ScriptSelectorTree_Initialized(object sender, EventArgs e)
        {
            _Window.SetWindowArea(_Parent);

            _ScriptTree.SelectedItemChanged += _ScriptTree_SelectedItemChanged;
            _RunPanel.Run += _RunPanel_Run;
            _RunPanel.Cancel += _RunPanel_Cancel;
            BuildTree(_ScriptTree, _Scripts
                .ToDictionary(s => s.Value.FullName.Split('.'), s => s.Value));

            ScriptInvocation[] recent = _History.MostRecentOfEach();
            int toBold = 10;
            int k = 0;
            foreach (ScriptInvocation si in recent)
            {
                if (!_Scripts.TryGetValue(si.ScriptName, out Script s))
                    continue;

                ComboBoxItem item = new ComboBoxItem();
                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
                string name = si.ReverseComNames(out string first);
                panel.Children.Add(
                    new Label { Content = si.LocalTime() });
                panel.Children.Add(
                    new Label { Content = first, FontWeight = FontWeights.Bold });
                panel.Children.Add(
                    new Label { Content = name });

                //item.Content = si.ShortString();
                item.Content = panel;
                _RecentCombo.Items.Add(item);
                _RecentScripts.Add(item, s);

                if (k < toBold)
                {
                    if (_Nodes.TryGetValue(s, out ScriptNode sn))
                    {
                        sn.Label.FontWeight = FontWeights.Bold;
                        GroupNode parent = sn.Parent as GroupNode;
                        while (parent != null)
                        {
                            parent.Label.FontWeight = FontWeights.Bold;
                            parent = parent.Parent as GroupNode;
                        }
                    }
                }
                k++;
            }

            ScriptInvocation last = _History.MostRecent();
            if (last != null)
            {
                if (_Scripts.TryGetValue(last.ScriptName, out Script s))
                    SetSelectedItem(s);
            }

            _RecentCombo.SelectionChanged += _RecentCombo_SelectionChanged;
            _PrevRuns.SelectionChanged += _PrevRuns_SelectionChanged;
        }

        private void _PrevRuns_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = _PrevRuns.SelectedItem as ComboBoxItem;

            if (cbi == null)
                return;

            if (!_PrevArgs.TryGetValue(cbi, out ScriptInvocation item))
                return;

            if (item.ScriptName == _Selected.FullName)
                PopulateArgs(item);
        }

        private Dictionary<ComboBoxItem, Script> _RecentScripts
            = new Dictionary<ComboBoxItem, Script>();

        private void _RecentCombo_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            var combo = _RecentCombo.SelectedItem as ComboBoxItem;
            if (combo == null)
                return;

            if (!_RecentScripts.TryGetValue(combo, out Script script))
                return;

            SetSelectedItem(script);
        }

        private static void JumpToNode(TreeViewItem tvi, string NodeName)
        {
            if (tvi.Name == NodeName)
            {
                tvi.IsExpanded = true;
                tvi.BringIntoView();
                return;
            }
            else
                tvi.IsExpanded = false;

            if (tvi.HasItems)
            {
                foreach (var item in tvi.Items)
                {
                    TreeViewItem temp = item as TreeViewItem;
                    JumpToNode(temp, NodeName);
                }
            }
        }

        private void _RunPanel_Cancel()
        {
            Print("Script Selection UI was canceled.");
            _Parent.Close();
        }

        private void _RunPanel_Run(bool newProcess, bool newThread)
        {
            if (_Selected == null)
            {
                Warn("Cannot run script because none is selected.");
                return;
            }

            Print("Attempting to run script: " + _Selected.FullName);
            object arg = null;
            if (_ArgEditor != null)
            {
                if (!_ArgEditor.TryGetValue(out arg))
                {
                    Warn("Script execution canceled because of invalid arguments.");
                    return;
                }
            }

            _History.AddRun(_Selected.FullName, arg);
            _History.WriteToFile();
            
            _Window.WriteToFile(_Parent);

            if (newProcess)
            {
                Print("Launching new process to run script.");
                RunInNewProcess(_Selected, arg);
            }
            else if (newThread)
            {
                var t = new Thread(() =>
                {
                    Print("Running script in new thread: " + _Selected.FullName);
                    try
                    {
                        _Selected.Run(arg);
                    }
                    catch (TargetInvocationException ex)
                    {
                        Console.Error.WriteLine(ex.InnerException);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }
                    Print("Script thread ran to completion: " + _Selected.FullName);
                });
                t.Start();
            }
            else
            {
                Print("Running script in current thread.");
                _Selected.Run(arg);
                Print("Script ran to completion.");
            }
        }

        private void RunInNewProcess(Script script, object args)
        {
            string location = Assembly.GetEntryAssembly().Location;

            string argString = "";
            if (args != null)
            {
                argString = JsonObject.Serialize(args)
                    .Replace("\r\n", "").Replace("\"", "\"\"");
                argString = "\"" + argString + "\"";
            }

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(location,
                script.FullName + " " + argString);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.Start();
        }

        private void _ScriptTree_SelectedItemChanged(
            object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ScriptNode item = e.NewValue as ScriptNode;
            if (item == null)
                return;

            SetSelectedItem(item.Script);
        }

        private Dictionary<ComboBoxItem, ScriptInvocation> _PrevArgs
            = new Dictionary<ComboBoxItem, ScriptInvocation>();

        private void SetSelectedItem(Script item)
        {
            if (_Nodes.TryGetValue(item, out ScriptNode node))
                JumpToNode(node, node.Name);

            _Selected = item;
            _ArgTypeLbl.Content = _Selected.ArgumentType == null ? "(none)" :
                "{" + _Selected.ArgumentType.NiceName() + "}";

            DocumentationAttribute doc = item.HostClass
                .GetCustomAttribute<DocumentationAttribute>();
            if (doc == null)
            {
                _ScriptDocLbl.Content = "";
                _ScriptDocLbl.Visibility = Visibility.Collapsed;
            }
            else
            {
                _ScriptDocLbl.Content = doc.Description;
                _ScriptDocLbl.Visibility = Visibility.Visible;
            }

            if (_RecentCombo.SelectedItem == null)
            {
                if (_RecentScripts.Any(i => i.Value == item))
                {
                    ComboBoxItem toSelect = _RecentScripts
                        .Where(i => i.Value == item).First().Key;
                    _RecentCombo.SelectedItem = toSelect;
                }
            }

            ScriptInvocation[] prev = _History
                .DistinctPreviousInvocations(item.FullName);
            ScriptInvocation last = prev.FirstOrDefault();

            _PrevRuns.Items.Clear();
            foreach (ScriptInvocation p in prev)
            {
                ComboBoxItem pi = new ComboBoxItem();
                //pi.Content = p.HistoryString();

                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
                Label lbl = new Label { Content = p.HistoryString() };
                panel.Children.Add(lbl);
                pi.Content = panel;

                _PrevArgs.Add(pi, p);
                _PrevRuns.Items.Add(pi);
            }
            if (prev.Length > 0)
                _PrevRuns.SelectedIndex = 0;

            _ScriptName.Content = item.FullName;

            PopulateArgs(last);
            Print("Selected item: " + _Selected.FullName);
        }

        private void PopulateArgs(ScriptInvocation last)
        {
            string args = null;
            if (last != null)
                args = last.Args;

            if (_ArgEditor != null)
            {
                _ArgStack.Content = null;
                _ArgEditor = null;
            }

            if (_Selected.ArgumentType != null)
            {
                _ArgEditor = ObjectEditors.MakeEditor(_Selected.ArgumentType) as IObjectEditor;
                //_ArgEditor = new DictionaryEditor();
                _ArgEditor.TrySetValueFromArgs(args, _Selected.ArgumentType);
                _ArgStack.Content = _ArgEditor as UserControl;
            }
        }

        private void BuildTree(ItemsControl parent, 
            Dictionary<string[], Script> scripts)
        {
            var groupings = scripts.GroupBy(kvp => kvp.Key[0]);

            foreach (var g in OrderGroupings(groupings))
            {
                var members = g.ToList();
                if (members.Count == 1)
                {
                    var n = new ScriptNode(members[0].Key.Aggregate((a, b) => a + "." + b), members[0].Value);
                    _Nodes[members[0].Value] = n;
                    
                    parent.Items.Add(n);
                }
                else
                {
                    var node = new GroupNode(g.Key);
                    BuildTree(node, members.ToDictionary(m => m.Key.Skip(1).ToArray(), m => m.Value));
                    parent.Items.Add(node);
                }
            }
        }

        private IEnumerable<IGrouping<string, KeyValuePair<string[], Script>>> OrderGroupings(
            IEnumerable<IGrouping<string, KeyValuePair<string[], Script>>> groupings)
        {
            var withChildren = groupings.Where(i => i.Count() > 1);
            foreach (var g in withChildren.OrderBy(i => i.Key))
                yield return g;

            var others = groupings.Where(i => i.Count() <= 1);
            foreach (var g in others.OrderBy(i => i.Key))
                yield return g;
        }

    }

    public class GroupNode : TreeViewItem
    {
        public readonly Label Label;

        public GroupNode(string name)
        {
            Label = new Label { Content = name };
            Header = Label;
        }
    }

    public class ScriptNode : TreeViewItem
    {
        public readonly Script Script;
        public readonly Label Label;

        public ScriptNode(string name, Script script)
        {
            Label = new Label { Content = name };
            Header = Label;
            Script = script;
        }

    }
}
