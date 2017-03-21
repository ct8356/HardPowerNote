using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows;
using CJT;
using HardCJT.Models;
using HardCJT.ViewModels;
using AutoCompleteBox = CJT.AutoCompleteBox;

namespace HardCJT.ExcelViewModels {
    public class EntryVM : CJT.ViewModels.EntryVM {
        //NOTE: MIGHT be able to make a generic version of this class,
        //BUT it is hard! lots of changing necessary.
        //TOO MUCH CHANGING considering how short on time I am!
        //NOTE: DataGrid AutoGenerateColumns will make a column
        //for every public property in this class.
        //BUT YO! I think the easiest way for NOW,
        //Is just to define the columns in the DataTable! 
        //REALLY won't take long to update it there!
        //OR maybe this VM should have a list?
        //BUT apparently Columns iS NOT a part of visual tree!
        //SO binding like that is difficult.
        //SO FOR NOW, just specify it in View.
        //NEED TO GO FASTER NOW!!! OR WILL NEVER GET ANYWHERE! REALLY! 2 YEARS GONE!
        public ITreeVM TreeVM { get; set; }
        public Entry Entry { get; set; }
        public ExcelContext DbContext { get; set; }
        public static ObservableCollection<object> Properties { get; set; }
        public ObservableCollection<Property> ImportantProperties { get; set; }
        public ListBoxPanelVM<Tag> TagsVM { get; set; }
        public ObservableCollection<EntryVM> Parents { get; set; }
        //NOTE! I think proper way to do this, is to just MODIFY the entry,
        //BUT because the the EntryVM is bound to it, it will update itself accordingly!
        //i.e. this entryVM just has a public Entry Parent.
        //NOW, when that Entry is deleted, the EntryVM deletes itself. SO this EntryVM,
        //does not even need to know its own ParentVM???? maybe... not sure yet.
        public ObservableCollection<EntryVM> Children { get; set; }
        //DON'T want others adding to children. If I make it private, will that stop binding?
        //No, it won't, provided you KEEP this property public!!!
        //BUT problem is, then you can still add to Children...issue...
        public ListBoxPanelVM<Tag> FilterTags { get; set; }
        public bool IsExpanded { get; set; }
        //SHOULD there be an option to emanciate??? (free from parent?)

        public EntryVM() {
            //do nothing.
            //NOTE: is this called? maybe. Even if it is, does not matter.
        }

        public EntryVM(ITreeVM treeVM) {
            Entry = new Entry();
            initialize(treeVM);
        }

        public EntryVM(Entry entry, ITreeVM treeVM) : this(treeVM) {
            initialize(entry, treeVM);
        }

        protected void initialize() {
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

        protected void initialize(ExcelContext dbContext) {
            DbContext = dbContext;
            initialize();    
        }

        protected virtual void initialize(ITreeVM treeVM) {
            TreeVM = treeVM;
            //DbContext = treeVM.DbContext as ExcelContext;
            initialize();
        }

        protected void initialize(Entry entry, ExcelContext dbContext) {
            Entry = entry;
            initialize(dbContext);
            Parents = new ObservableCollection<EntryVM>();
            Children = new ObservableCollection<EntryVM>();
            //PROPERTIES
            initializePropertyList();
        }

        protected void initialize(Entry entry, ITreeVM treeVM) {
            TreeVM = treeVM;
            //initialize(entry, treeVM.DbContext as ExcelContext);
        }

        protected virtual void initializePropertyList() {
            ImportantProperties = new ObservableCollection<Property>();
            ImportantProperties.Add(new Property("EntryID", Entry.EntryID, InfoType.TextBlock, false, DbContext));
            ImportantProperties.Add(new Property("CreationDate", Entry.CreationDate, InfoType.TextBlock, false, DbContext));
            ImportantProperties.Add(new Property("Name", Entry.Name, InfoType.TextBox, false, DbContext));
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
        }

        public void EntryVM_PropertyChanged(object sender, PropertyChangedEventArgs args) {
            DbContext.SaveChanges(sender); //THIS clearly does NOTHING for excel. Yes really.
            //INSTEAD, you will need PROBS to OVERRIDE it, in Excel context.
            //THIS will save the selected entry? (VIA submitting an update?)
            //BUT gets tricky if you update a massive selection!
            //TBH! keep it simple! OUGHT to just have a SAVE button...
            //BUT NAH! EVEN THAT complicated, if got to update MANY entries together.
            //WELL NOT THAT complex.Not EF easy though.
        }

        public void Entry_PropertyChanged(object sender, PropertyChangedEventArgs args) {
            DbContext.SaveChanges();
            //THIS does not seem to get called, not sure why.
            //Never mind, just try with entryVM
            //AHAH! ITs obvious!
            //You have subscribed to the TRANSACTION Entry,
            //NOT the PartClass Entry!
            //BUT NOTE: REVISIT CURRENT
            //I DO think it is better to subscribe to ViewModel,
            //AND NOT the model,
            //BECAUSE model should be PRIVATE, to each viewModel wrapper, I think.
        }

        public virtual void insertEntry(EntryVM selectedVM) {
            //Do nothing (only here to be overridden.)
        }

        public void insertEntry(EntryVM entryVM, EntryVM selectedVM) {
            if (Parents != null) {//IF it has a parent: make new one a sibling.
                adoptSibling(entryVM);
            }
            else { //else put it in FirstLevelVMs
                //TreeVM.FirstGenEntryVMs.Add(entryVM as EntryVM);
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
                entryVM.Entry.Tags.Add(tag);
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

        public static EntryVM WrapInCorrectVM(Entry entry, ITreeVM treeVM) {
            EntryVM entryVM = null;
            //REVISIT
            return entryVM;
        }

    }
}
