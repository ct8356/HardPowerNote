using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HardCJT.ViewModels;

namespace HardPowerNote {
    public partial class DisplayPanel : DockPanel {
        //public SortPanel SortPanel { get; set; }
        public EntriesTreeView EntriesTreeView { get; set; }
        public List<String> ColumnNames { get; set; }
        public IMainVM MainVM { get; set; }

        public DisplayPanel() {
            InitializeComponent(); //NEED to do this first, so EntriesTreeView can reference OptionsPanel
            //TypePanel.DisplayMemberPath = "Name";
            //ComboBox.ValueMemberPath = "Name";
            //PANEL
            //SORT PANEL
            //SortPanel = new SortPanel();
            //Children.Add(SortPanel);
            //SetDock(SortPanel, Dock.Top);
            //SortPanel.ComboBox.SelectionChanged += ComboBox_SelectionChanged; //subscribe
            //COLUMN NAME PANEL
            //var colNames = typeof(Student).GetProperties().Select(a => a.Name).ToList();
            ColumnNames = new List<String>() { "Contents", "Priority" };
            //OTHER
            LastChildFill = false;
        }

    }
}

