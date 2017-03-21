using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel; //this allows INotifyPropertyChanged
using CJT;
using CJT.ViewModels;
using HardCJT.Models;
using HardCJT.ViewModels;

namespace HardCJT.ViewModels {
    public class ListBoxPanelVM<T> : BaseClass, INotifyInputConfirmed, IListVM where T : Entry, new() {
        //NOTE: this class is not so flexible,
        //BUT it is used so often, that it is worth using!
        public delegate void ObjectEventHandler(object sender, ObjectEventArgs<T> e);
        public delegate void MessageEventHandler(object sender, MessageEventArgs e);
        public event MessageEventHandler InputConfirmed;
        public IMainVM MainVM { get; set; }
        public EntryVM EntryVM { get; set; }
        public EFContext DbContext { get; set; }
        public DbSet<T> DbSet { get; set; }
        public string RelationName { get; set; }
        public int RelationNumber { get; set; } //1 for parents, 2 for children
        private ObservableCollection<T> selectableItems;
        public ObservableCollection<T> SelectableItems {
            get { return selectableItems; }
            set { selectableItems = value; NotifyPropertyChanged("SelectableItems"); }
        }
        private ObservableCollection<T> selectedItems;
        public ObservableCollection<T> SelectedItems {
            get { return selectedItems; }
            set { selectedItems = value; NotifyPropertyChanged("SelectedItems"); }
        }

        public ListBoxPanelVM(EntryVM VM) {
            //OK, at one style of ListBoxPanelVM (the style that goes in entryVMs)
            //NEEDS to know about EntryVM,
            //OTHERWISE, it means the EntryVM has to subscribe to all the ListBoxPanVMs,
            //WHICH is a pain in the A!
            //ACTUALLY no it does not!
            //IF you just make THIS VM take care of adding, and saving Db!
            //AND NOTE: there DOES have to be a difference between 
            //ListBoxPanelVM for Filters,
            //AND for entries.
            //BECAUSE one for ENTRIES, WILL add it to the dataBase!
            //Entry one could EXTEND the other one though...
            SelectableItems = new ObservableCollection<T>();
            SelectedItems = new ObservableCollection<T>();
            EntryVM = VM;
            //MainVM = VM.TreeVM.ParentVM as IMainVM;
            DbContext = VM.DbContext as EFContext;
            ChooseCorrectDbSet();
            //SUBSCRIBE
            InputConfirmed += This_InputConfirmed;
        }

        public ListBoxPanelVM(EntryVM VM, ObservableCollection<T> list) : this(VM) { 
            //hopefully calls this(VM) first? Pretty sure it does.
            //PERHAPS this should wrap a list of tags, in a VM?
            //AND maybe that is easier, than doing it in the calling method? yes is.
            SelectedItems = list;
            //THIS creates a binding to the list passed (from the Entry).
            //WHICH IS WHY, if I remove something from SelectedItems,
            //It gets removed from the Entry also. Nice!
        }

        public void AddNewItem(string text) {
            Entry newItem = new T();
            //REVISIT: could just create the EntryVM here, then added to DBSet automatically!
            //HEY WOW! THOUGHT it was a real problem that I could not pass arguments to contruct.
            //BUT IS IT? Could you get away with setting properties afterwards? REVISIT
            newItem.Name = text;
            Type type = typeof(T);//MONITOR
            ChooseCorrectDbSet();
            DbSet.Add(newItem as T);
            DbContext.SaveChanges();
        }

        public void AddItem(string name) {
            ChooseCorrectDbSet();
            T entry = DbSet.Where(e => e.Name == name).FirstOrDefault();
            SelectedItems.Add(entry); //THIS should get updated on refresh anyway...
            //THIS also adds it to the Entry's navProperty, coz it is bound to it!
            //FIRE an event... or not.... use NotifyPropChanged?
            //ACTUALLY, FOR better DISTRIBUTION,
            //BETTER if this class just calls TreeVM to update itself!
            //FINALLY
            DbContext.SaveChanges();
            //MainVM.UpdateEntries();//bad, because deletes all entryVMs?
            //(sender as AutoCompleteBox).Text = null;
        }

        public void ChooseCorrectDbSet() {
            if (typeof(T) == typeof(Entry))
                DbSet = DbContext.Entries as DbSet<T>;
            //WHAT? Why is this allowed? Maybe its allowed, but wont actually work in runtime?
            //AH MAYBE IT DOES WORK, if Ts actually Match!!! COOL! REVISIT
            if (typeof(T) == typeof(PartClass))
                DbSet = DbContext.Parts as DbSet<T>;
            if (typeof(T) == typeof(PartInstance))
                DbSet = DbContext.PartInstances as DbSet<T>;
            if (typeof(T) == typeof(Task))
                DbSet = DbContext.Tasks as DbSet<T>;
            if (typeof(T) == typeof(Tag))
                DbSet = DbContext.Tags as DbSet<T>;
        }

        public void NotifyInputConfirmed(string input) {
            if (InputConfirmed != null)
                InputConfirmed(this, new MessageEventArgs(input));
        }

        public void GoTo(object item) {
            //MainVM.SelectedEntryVM = EntryVM.WrapInCorrectVM(item as T, DbContext);
            //MainVM.UpdateEntries(); //PERHAPS don't want to do this...
            //SINCE it updates whole list!
        }

        public void Remove(object item) {
            SelectedItems.Remove(item as T);
            DbContext.SaveChanges();
            //MainVM.UpdateEntries();
            //AGAIN perhaps dont want to do this,
            //SINCE it updates whole list? slow!
        }
        //NOTE if DELETE tag totally, GOT to remove it from AllTags!
        //REVISIT CURRENT!

        public void This_InputConfirmed(object sender, MessageEventArgs args) {
            if (!DbContext.Entries.Any(en => en.Name == args.Message)) {
                AddNewItem(args.Message); //REVISIT! Does it still add new entries to list?
                //NOTE YO! these comparisons, SEEM TO NOT WORK!
                //OFTEN IS ONE IN THERE,
                //AND YET STILL it makes a new one!
                //MAYBE, its just coz selectableItems is not uptodate?
            }
            if (!SelectedItems.Any(e => e.Name == args.Message)) {
                //HEYHEY! It seems like when you create an entry,
                //It gets added to the tree. EVEN if dont add it to DbSet.
                //SO REALLY just want to find it, then wrap it.
                AddItem(args.Message);
            }
        }

    }
}

