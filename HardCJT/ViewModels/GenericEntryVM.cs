using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Windows.Input;
using System.Linq;
using System.Windows.Navigation;
using System.Windows;
using CJT;
using CJT.ViewModels;
using HardCJT.Models;
using AutoCompleteBox = CJT.AutoCompleteBox;

namespace HardCJT.ViewModels {
    public class GenericEntryVM<T> : GenericEntryVM<T, EFContext> where T : Entry, new() {
        public ObservableCollection<GenericEntryVM<T>> Parents { get; set; } //SHOULD BE HERE!
        //NOTE! I think proper way to do this, is to just MODIFY the entry,
        //BUT because the the EntryVM is bound to it, it will update itself accordingly!
        //i.e. this entryVM just has a public Entry Parent.
        //NOW, when that Entry is deleted, the EntryVM deletes itself. SO this EntryVM,
        //does not even need to know its own ParentVM???? maybe... not sure yet.
        public ObservableCollection<GenericEntryVM<T>> Children { get; set; }
        //DON'T want others adding to children. If I make it private, will that stop binding?
        //No, it won't, provided you KEEP this property public!!!
        //BUT problem is, then you can still add to Children...issue...
        public CJTType Type { get; set; }

        protected GenericEntryVM() {
            //do nought.
        }

        public GenericEntryVM(EFContext dbContext) {
            //USED only by TreeVM. (to create FilterVM)
            Entry = new T();
            initialize(dbContext);
            //NOTE, better off avoiding calling : base.
            //More flexible if you use initialise!
        }

        public GenericEntryVM(T entry, EFContext dbContext) {
            initialize(entry, dbContext);
        }

        public GenericEntryVM(String name, EFContext dbContext) {
            T entry = new T();
            entry.Name = name;
            initialize(entry, dbContext);
            Type.DbSet.Add(entry);
        }

        protected override void initialize() {
            IsExpanded = true;
            //SUBSCRIBE
            Entry.PropertyChanged += Entry_PropertyChanged;
            PropertyChanged += EntryVM_PropertyChanged;
        }

        protected void initialize(T entry, EFContext dbContext) {
            Entry = entry;
            initialize(dbContext);
            Parents = new ObservableCollection<GenericEntryVM<T>>();
            Children = new ObservableCollection<GenericEntryVM<T>>();
            string type = entry.Type;//DOES not seem to work, IF type retrieved from DBase!
            Type = dbContext.TypeList.Where(t => t.EntryType.FullName == entry.Type).First() as CJTType;
        }

        public void adoptSibling(GenericEntryVM<T> entryVM) {
            foreach (GenericEntryVM<T> parent in Parents) {
                parent.Children.Add(entryVM);
                entryVM.Parents.Add(parent);
                //parent.Entry.Children.Add(entryVM.Entry);
            }
        }

        public void adoptChild(GenericEntryVM<T> childVM) {
            Children.Add(childVM);
            childVM.Parents.Add(this); //Do this, just for view purposes.
            //Entry.Children.Add(childVM.Entry); //Here is bit where must add relationship. (to database)
        }

        public void adoptChildFromTreeVM(EntryVM orphan) {
            //Entry.Children.Add(orphan.Entry);
        }

        public void deleteEntry() {
            //REVISIT
        }

        public virtual void insertEntry(GenericEntryVM<T> selectedVM) {
            GenericEntryVM<T> entryVM = new GenericEntryVM<T>("blank", DbContext);
            insertEntry(entryVM, selectedVM);
            //Do nothing (only here to be overridden.)
        }

        public void insertEntry(GenericEntryVM<T> entryVM, GenericEntryVM<T> selectedVM) {
            if (Parents != null) {//IF it has a parent: make new one a sibling.
                adoptSibling(entryVM);
            }
            else { //else put it in FirstLevelVMs
                //TreeVM.FirstGenEntryVMs.Add(entryVM);
                //NO NEED, just refresh the tree.
            }
            DbContext.SaveChanges();
            //TreeVM.UpdateEntries(); //not needed? shouldn't be. Not needed for inserting tags...
        }

        public virtual void insertSubEntry(GenericEntryVM<T> selectedVM) {
            //Do nothing. (just here to be overriden by subclass)
            //who creates a NodeVM, and passes it to method below.
        }

        public void insertSubEntry(GenericEntryVM<T> entryVM, GenericEntryVM<T> selectedVM) {
            adoptChild(entryVM);
            //foreach (Tag tag in FilterTags.SelectedItems) {
            //    (entryVM.Entry as Entry).Tags.Add(tag);
            //} //REVISIT
            DbContext.SaveChanges();
            //ParentVM.updateEntries(); //nec //is it though? well yes. Is there another way?
            //is entry.Children observable? Yes. BUT real issue is: is it adding a VM? No!
            //so make it do so!
            //DONE! CBTL, works, but could be more issues here...
        }

        public override string ToString() {
            return Entry.ToString();
        }


    }
}
