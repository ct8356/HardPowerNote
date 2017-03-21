using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJT;
using HardCJT.ViewModels;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Data;
using TextBlock = CJT.TextBlock;
using System.Windows;

namespace HardPowerNote {
    public class LinkedTextBlock : TextBlock {
        //NOTE: this class should really take EntryVMs as DataContext,
        //AND should make your entryVMs have properties to bind to, RATHER than binding to Entry.
        //BUT as a quick hack, I am going to make this take Entry as a DataContext
        //OR maybe I will allow it to take both... (with if statement).
        public IMainVM MainVM { get; set; } //NEED THIS!
        
        public LinkedTextBlock() : base() {
            Loaded += This_Loaded;
            //BorderBrush = new SolidColorBrush(Colors.LightGray);
            Background = new SolidColorBrush(Colors.LightCyan);
            Padding = new Thickness(2);
        }

        //Problem is, the wrapper EntryVM NEEDS to know about the tree,
        //SO that it can call the tree to update. Or delete itself from the tree.
        //(even though it might not be shown in the tree. BUT is a chance it is).
        //BUT how can we make it know about the tree,
        //IF all that is passed to this class is dataContext, with is Entry?
        //Entry does not know about the VM.
        //THIS view does not know about anyother views, it is just a template.
        //It is handed a load of entries by the listBox...
        //SO it is almost CERTAIN that I need to put these lists of EntryVMs in the EntryVM?
        //AH! WELL YES! You did with Children!
        //So, unless ONLY WANT to navigate one Children Dimension at a time,
        //THEN YES HAVE TO!
        //COULD i get away with navigating one dimension at a time?
        //WEll yes, but would have to just show them in the tree. Not very elegant.
        //ANOTHER way around it? 
        //LIKE making a ListBox that binds to Entries,
        //BUT you override the call that sets dataContext for each item,
        //AND instead, to pass them a Entry WRAPPED in a EntryVM?
        //MAYBE, but effort, AND maybe easier, and BETTER, to just add props
        //to the EntryVM.
        //IF do, back up before hand!

        public void This_Loaded(object sender, EventArgs e) {
            if (DataContext is EntryVM) {
                EntryVM dataContext;
                dataContext = DataContext as EntryVM; //NEED to set this OUTSIDE it!
                //MainVM = dataContext.TreeVM.ParentVM as IMainVM;
                MenuItem goToEntry = new MenuItem() { Header = "Go to entry" };
                goToEntry.Click += delegate { MainVM.SelectedEntryVM = dataContext; };//USE polymorphism!
                //NOTE, this method is not called for listBox! It has its own method.
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.Items.Add(goToEntry);
                ContextMenu = contextMenu;
            }
        }

    }
}
