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
    public class DataTableGrid : DataGrid {
        //NOTE, this class not used, just here to keep the information handy. as a backup.
        //PROBS don't need to use this class. The CustomBoundColumn bit
        //cant handle scroll bars for some reason!
        public override void this_SelectionChanged(object sender, EventArgs e) {
            if (IsSourceTable && SelectedItem != null)
                (DataContext as DataTableVM).SelectedRow = (SelectedItem as DataRowView).Row;
        } //PROBLEM! SelectionChanged is called AFTER PropertyChanged!
        //BUT NO! nonsense, since this method must be called, in order for SelectedItem to be changed?

    }
}