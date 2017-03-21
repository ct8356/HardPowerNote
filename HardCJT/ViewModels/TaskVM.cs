using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using CJT;
using CJT.Models;
using CJT.ViewModels;
using HardCJT.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HardCJT.ViewModels {

    public class TaskVM : EntryVM {

        public TaskVM(Task task, EFContext treeVM) {
            //NOTE: this constructor just WRAPS a student in a VM.
            initialize(task, treeVM);
        }

        public TaskVM(string name, EFContext dbContext) {
            //NOTE: this one creates the Entry, and THEN wraps it!!!
            Task newEntry = new Task(name);
            initialize(newEntry, dbContext);
            (DbContext as EFContext).Tasks.Add(newEntry);
        }

        public override void insertEntry(EntryVM selectedVM) {
            TaskVM entryVM = new TaskVM("blank", DbContext);
            insertEntry(entryVM, selectedVM);
        }

        public override void insertSubEntry(EntryVM selectedVM) {
            TaskVM entryVM = new TaskVM((selectedVM.Entry as Task).Name + " child", DbContext);
            //this creates an entry too!
            insertSubEntry(entryVM, selectedVM);
        }
    }
}
