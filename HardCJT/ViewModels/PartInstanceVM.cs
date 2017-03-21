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
using System.Data.Entity;
using System.Collections.ObjectModel;
using CJT;
using CJT.ViewModels;
using HardCJT.Models;

namespace HardCJT.ViewModels {
    public class PartInstanceVM : EntryVM {
        public static IEnumerable<string> PropertyNames { get; set; }
        public static ObservableCollection<string> Structures { get; set; }
        public static string SelectedStructure { get; set; }
        //NOTE: might be a good idea to make own DbSet class,
        //AND then put these statics, in there.
        public IEnumerable<PartClass> NickNames { get; set; }

        public PartInstanceVM(Entry entry, EFContext parentVM) {
            //NOTE: this constructor just WRAPS an entry in a VM.
            initialize(entry, parentVM);
            IQueryable<PartClass> partClasses = (DbContext as EFContext).Parts;
            PartClass partClass = partClasses.First();
            //(entry as PartInstance).PartClass = partClass;
            //WHAT THE!!??? NOW IT WORKS???
            //Weird.
        }

        public PartInstanceVM(string name, EFContext dbContext) {
            //NOTE: this one creates the Entry, and THEN wraps it!!!
            PartInstance newEntry = new PartInstance(name);
            initialize(newEntry, dbContext);
            (DbContext as EFContext).PartInstances.Add(newEntry);
            DbContext.SaveChanges();
        }

        public override void insertEntry(EntryVM selectedVM) {
            PartInstanceVM entryVM = new PartInstanceVM("blank", DbContext);
            insertEntry(entryVM, selectedVM);
        }

        public override void insertSubEntry(EntryVM parentVM) {
            PartInstanceVM entryVM = new PartInstanceVM
                ((parentVM.Entry as PartInstance).Name + " child", DbContext); //create part.
            insertSubEntry(entryVM, parentVM);
        }

    }
}