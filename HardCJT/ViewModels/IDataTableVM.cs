using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace HardCJT.ViewModels {
    interface IDataTableVM {

        DataTable DataTable { get; set; }
        string CommandText { get; set; }
        string FileName { get; set; }
        string FilePath { get; set; }
        DataRow SelectedRow { get; set; }

    }
}
