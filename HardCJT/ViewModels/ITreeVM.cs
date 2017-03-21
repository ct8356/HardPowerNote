using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJT;
using CJT.Models;
using CJT.ViewModels;
using HardCJT.Models;
using System.Collections.ObjectModel;

namespace HardCJT.ViewModels {
    public interface ITreeVM : IGenericTreeVM<Entry, EntryVM, EFContext> {

        ObservableCollection<Tag> AllTags { get; set; }

        GenericComboBoxVM<CJTType> SystemTypePanelVM { get; set; }

        GenericComboBoxVM<string> StructurePanelVM { get; set; }

    }
}
