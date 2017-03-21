using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CJT.Models;
using CJT.ViewModels;

namespace HardCJT.ViewModels {
    public abstract class DataTableVM : DataListVM, IDataTableVM {
        public DataTable DataTable { get; set; }
        public string CommandText { get; set; }
        public string FileName { get; set; }// = "ElectricalCupboardContents";
        public string FilePath { get; set; }
        public string TableName { get; set; }

        private IList<EntryVM> dataList;
        public override IList<EntryVM> DataList {
            get{ return dataList;  }
            set{ dataList = value; NotifyPropertyChanged("DataList"); }
        }

        private EntryVM selectedItem;
        public override EntryVM SelectedItem {
            get { return selectedItem; }
            set { selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
                //UpdateSelectedRow();
                //SINCE NOT REALLY POSS to instantiate DataRow or DRView yourself,
                //CANT really use this methodology.
                ////INSTEAD, gonna JUST PASS EntryVM, rather than passing DataRow.
            }
        }

        private DataRow selectedRow;
        public DataRow SelectedRow {
            get { return selectedRow; }
            set { selectedRow = value; NotifyPropertyChanged("SelectedRow"); }
        }

        public override void UpdateDataList() {
            DataTable = DbContext.GetDataTable(CommandText, "", FilePath);
            DataList = GetDataList("");
        }

        public override void UpdateDataList(string search) {
            DataTable = DbContext.GetDataTable(CommandText, search);
            DataList = GetDataList(search);
        }

        public abstract void UpdateSelectedRow();

    }
}
