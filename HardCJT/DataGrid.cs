using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Data;
using HardCJT.ViewModels;

namespace HardCJT {
    public class DataGrid : System.Windows.Controls.DataGrid {
        //NOTE, this class not used, just here to keep the information handy. as a backup.
        //PROBS don't need to use this class. The CustomBoundColumn bit
        //cant handle scroll bars for some reason!
        public bool IsSourceTable { get; set; }

        public DataGrid() {
            MaxColumnWidth = 500;
            AutoGenerateColumns = false;
            //SUBSCRIBE
            Loaded += this_Loaded; //Loaded seems better than datacontext changed.
            //datacontextChanged seems to call it just BEFORE dataContext actually changed!
            //JUST make sure you ALWAYS set the DataContext for each table!
            SelectionChanged += this_SelectionChanged;
        }

        public virtual void this_Loaded(object sender, EventArgs e) {
            //AddColumnsFromDataTable(); //Not the issue.
            //Don't need to do this now thanks to EditableComboBox class?
        }

        public virtual void this_SelectionChanged(object sender, EventArgs e) {
            if (IsSourceTable && SelectedItem != null) {
                (DataContext as DataListVM).SelectedItem = SelectedItem as EntryVM;  
            }
            //THING IS, for above to work, NEED DataGrid to bind to DataTable IF DataTableVM...
            //i.e. need DataTableVM to have DataTable,  and (may as well SelectedItem),
            //AS none interface/abstract defined props, BUT do need to be there!
            //BUT should be OK as long as PROJECTS inherit from DataListVM, OR DataTableVM!
        } //PROBLEM! SelectionChanged is called AFTER PropertyChanged!
        //BUT NO! nonsense, since this method must be called, in order for SelectedItem to be changed?

    }
}