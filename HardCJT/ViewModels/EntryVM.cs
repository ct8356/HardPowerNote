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
    public class EntryVM : GenericEntryVM<Entry,EFContext> {
        //public new EFContext DbContext { get; set; }
        //public new Entry Entry { get; set; } //REVISIT! hiding could be a problem..
        public ListBoxPanelVM<Tag> TagsVM { get; set; }
        public ListBoxPanelVM<Tag> FilterTags { get; set; }
        public ObservableCollection<EntryVM> Parents { get; set; } //SHOULD BE HERE!
        //NOTE! I think proper way to do this, is to just MODIFY the entry,
        //BUT because the the EntryVM is bound to it, it will update itself accordingly!
        //i.e. this entryVM just has a public Entry Parent.
        //NOW, when that Entry is deleted, the EntryVM deletes itself. SO this EntryVM,
        //does not even need to know its own ParentVM???? maybe... not sure yet.
        public ITreeVM TreeVM { get; set; }
        public ObservableCollection<EntryVM> Children { get; set; }
        //DON'T want others adding to children. If I make it private, will that stop binding?
        //No, it won't, provided you KEEP this property public!!!
        //BUT problem is, then you can still add to Children...issue...
        public CJTType Type { get; set; }

        protected EntryVM() {
            //do nought.
        }

        public EntryVM(ITreeVM treeVM) { 
            //USED only by TreeVM. (to create FilterVM)
            Entry = new Entry();
            initialize(treeVM);
        }

        public EntryVM(EFContext dbContext) {
            //USED only by TreeVM. (to create FilterVM)
            Entry = new Entry();
            initialize(dbContext);
        }

        protected override void initialize() {
            IsExpanded = true;
            //SUBSCRIBE
            Entry.PropertyChanged += Entry_PropertyChanged;
            PropertyChanged += EntryVM_PropertyChanged;
            //TagsInputVM.InputConfirmed += TagsVM.This_Add;
            //OH MY GOSH! So you are saying, should register to all lists
            //from here? What a pain in the bum!
            //SURELY it is much easier to PASS reference of this EntryVM,
            //TO the inputVM (or just use the ListBoxPanelVM!).
            //AND call the EntryVM to add, from there!!!
            //WELL, yes I am...
            //BUT this way is more flexible!
            //BUT maybe also a bit stupid.
            //COZ only reason did it, was so did not have to PASS an EntryVM.
            //AND you could just make anything with lists attached,
            //an EntryVM. Or a ListAttachedObject...

            //DON'T NEED to instantiate Entry here,
            //BECAUSE it is done in the class inheriting from EntryVM!
        }

        protected virtual void initialize(ITreeVM treeVM) {
            TreeVM = treeVM;
            DbContext = treeVM.DbContext;  
            initialize();
        }

        protected void initialize(Entry entry, EFContext dbContext) {
            Entry = entry;
            initialize(dbContext);
            Parents = new ObservableCollection<EntryVM>();
            Children = new ObservableCollection<EntryVM>();
            string type = entry.Type;//DOES not seem to work, IF type retrieved from DBase!
            //SO need to rely on the TYPE string held in the Entry! YES!
            Type = dbContext.TypeList.Where(t => t.EntryType.FullName == entry.Type).First() as CJTType;
            //NOTE wont work without First! 
            //IF i seed the database, the above thing works fine! //CURRENT
            //PROPERTIES
            initializePropertyList();
        }

        protected virtual void initializePropertyList() {
            //ONLY DO IT if not already something in there.
            //NOTE YO!
            //THIS could actually slow things down a lot, since allocated QUITE a lot of memory,
            //FOR every EntryVM created!
            //BETTER to make it static! I THINK can still change it in program,
            //JUST that it is the same for every EntryVM.
            //AH! Could be a problem then!
            //ESPECIALLY if you dream of one day having only EntryVM as the only EntryVM!
            //AH! HOW ABOUT, the CJTType Class, could hold ImportantProperties?
            //AND each entryVM just has a reference to a CJTType class? AH YES!
            //YES! ITS just a better way, than using reflection every time!
            //AND more flexible, coz can easier pass it to settings to be saved.
            //AND don't have to fudge it, by making some properties private, some public etc.
            //(not sure its a fudge actually, but whatever).
            //IT IS more flexible, because can easier say, show these props, don't show these props.

            //NOTE: can either pass a CJTType to EntryVM,
            //or pass Type to EntryVM,
            //OR just ask the Entry, what typeof it is. or GetType(). I think. Makes sense! or "is".
        }

        public void adoptChild(EntryVM childVM) {
            Children.Add(childVM); 
            childVM.Parents.Add(this); //Do this, just for view purposes.
            //Entry.Children.Add(childVM.Entry); //Here is bit where must add relationship. (to database)
            string relationName = TreeVM.StructurePanelVM.SelectedItem.ToString();
            //The new entry has ALREADY been added to db in NodeVM constructor,
            //SO it makes sense to do the SAME when you create a new Relationship/VM!
            //THEN changes get saved, in method that calls this method.
        } //CURRENT

        public void adoptSibling(EntryVM entryVM) {
            foreach (EntryVM parent in Parents) {
                parent.Children.Add(entryVM);
                entryVM.Parents.Add(parent);
                //parent.Entry.Children.Add(entryVM.Entry);
            }
        }

        public void adoptChildFromTreeVM(EntryVM orphan) {
            //Entry.Children.Add(orphan.Entry);
        }

        public void deleteEntry() {
            //REVISIT
        }

        public virtual void insertEntry(EntryVM selectedVM) {
            //Do nothing (only here to be overridden.)
        }

        public void insertEntry(EntryVM entryVM, EntryVM selectedVM) {
            if (Parents != null) {//IF it has a parent: make new one a sibling.
                adoptSibling(entryVM);
            }
            else { //else put it in FirstLevelVMs
                TreeVM.FirstGenEntryVMs.Add(entryVM);
            }
            DbContext.SaveChanges();
            TreeVM.UpdateEntries(); //not needed? shouldn't be. Not needed for inserting tags...
        }

        public virtual void insertSubEntry(EntryVM selectedVM) {
            //Do nothing. (just here to be overriden by subclass)
            //who creates a NodeVM, and passes it to method below.
        }

        public void insertSubEntry(EntryVM entryVM, EntryVM selectedVM) {
            adoptChild(entryVM);
            foreach (Tag tag in FilterTags.SelectedItems) {
                (entryVM.Entry as Entry).Tags.Add(tag);
            } //REVISIT
            DbContext.SaveChanges();
            //ParentVM.updateEntries(); //nec //is it though? well yes. Is there another way?
            //is entry.Children observable? Yes. BUT real issue is: is it adding a VM? No!
            //so make it do so!
            //DONE! CBTL, works, but could be more issues here...
        }

        public override string ToString() {
            return Entry.ToString();
        }

        public void UpdateSelectedEntryVM(EntryVM entryVM) {
            //TreeVM.ParentVM.SelectedEntryVM = entryVM;
        }

        public static EntryVM WrapInCorrectVM(Entry entry, EFContext dbContext) {
            EntryVM entryVM = null;
            if (entry is Note)
                entryVM = new NoteVM(entry as Note, dbContext);
            if (entry is PartClass)
                entryVM = new PartClassVM(entry as PartClass, dbContext);
            if (entry is PartInstance)
                entryVM = new PartInstanceVM(entry as PartInstance, dbContext);
            if (entry is Task)
                entryVM = new TaskVM(entry as Task, dbContext);
            if (entry is Tag)
                entryVM = new TagVM(entry as Tag, dbContext);
            return entryVM;
        }

    }
}
