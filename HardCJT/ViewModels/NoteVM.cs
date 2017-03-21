using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJT.Models;
using CJT.ViewModels;
using HardCJT.Models;
using HardCJT.ViewModels;

namespace HardCJT.ViewModels {
    public class NoteVM : EntryVM {

        public NoteVM(Note entry, EFContext dbContext) {
            //NOTE: this constructor just WRAPS a student in a VM.
            initialize(entry, dbContext);
        }

        public NoteVM(String name, EFContext dbContext) {
            //NOTE: this one creates the Entry, and THEN wraps it!!!
            Note newEntry = new Note(name);
            initialize(newEntry, dbContext);
            (DbContext as EFContext).Notes.Add(newEntry);
        }

        public override void insertEntry(EntryVM selectedVM) {
            NoteVM entryVM = new NoteVM("blank", DbContext);
            insertEntry(entryVM, selectedVM);
        }

        public override void insertSubEntry(EntryVM selectedVM) {
            NoteVM entryVM = new NoteVM((selectedVM.Entry as Note).Name + " child", DbContext);
            //this creates an entry too!
            insertSubEntry(entryVM, selectedVM);
        }

    }
}
