using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJT;
using CJT.Models;
using CJT.ViewModels;
using HardCJT.Models;

namespace HardCJT.ViewModels {
    public class TagVM : EntryVM {
        public TagVM(Tag task, EFContext treeVM) {
            //NOTE: this constructor just WRAPS a student in a VM.
            initialize(task, treeVM);
        }

        public TagVM(string name, EFContext dbContext) {
            //NOTE: this one creates the Entry, and THEN wraps it!!!
            Tag newEntry = new Tag(name);
            initialize(newEntry, dbContext);
            (DbContext as EFContext).Tags.Add(newEntry);
        }

        public override void insertEntry(EntryVM selectedVM) {
            TagVM entryVM = new TagVM("blank", DbContext);
            insertEntry(entryVM, selectedVM);
        }

        public override void insertSubEntry(EntryVM selectedVM) {
            TagVM entryVM = new TagVM((selectedVM.Entry as Tag).Name + " child", DbContext);
            //this creates an entry too!
            insertSubEntry(entryVM, selectedVM);
        }
    }
}
