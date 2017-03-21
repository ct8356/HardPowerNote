using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using HardCJT.Models;
using HardCJT.ViewModels;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CJT;
using AutoCompleteBox = CJT.AutoCompleteBox;
using TextBlock = CJT.TextBlock;

namespace HardPowerNote {
    public class EntryPanel : StackPanel {
        public TextBlock TextBlock { get; set; } //NOTE: NEED THIS!

        public EntryPanel() {
            TextBlock = new TextBlock();
            Children.Add(TextBlock);
            Orientation = Orientation.Horizontal;
            //conditionalSubscribe(); //OF COURSE! WON'T WORK, coz datacontext don't exist yet.
            //REALLY, need to do conditional subscribe in XAML? But, complex, CBTL.
            //AUTOCOMPLETEBOX
            //autoCompleteBox.SelectionChanged += autoCompleteBox_SelectionChanged;
            //autoCompleteBox.LostFocus += autoCompleteBox_LostFocus;
            //TRY KEEPING below method here, BUT, call it from sub class...
            //MyAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
            //SUBSCRIBE TO EVENTS
            this.MouseUp += this_MouseUp;
            DataContextChanged += This_DataContextChanged;
        }

        protected void bindTextBlock(TextBlock textBox, Entry entry, string propertyName) {
            //SADLY, don't think TextDependencyProperty exists for both Box and Block together.
            Binding binding = new Binding(propertyName); //This is the MODEL property it binds to.
            binding.Source = entry; // the binding source (which must fire a PROP CHANGED event).
            textBox.SetBinding(TextBlock.TextProperty, binding); //fortunately, textBox already fires an event when changed.
            //YOU created the event for the dataSource. SO HOPEFULLY, we have 2 way binding now... we do :)
        }//SINCE gonna use this method in lots of VIEWS,
        //MIGHT be worth putting it in A SPECIAL CLASS as a static method!

        public void conditionalSubscribe() {
            //if ((DataContext as EntryVM).TreeVM.WaitingForParentSelection)
                this.MouseUp += sendToFosterParent;
        }

        public void changeParent_Click(object sender, RoutedEventArgs e) {
            //Entry entry = sender as Entry;
            //(DataContext as EntryVM).changeParent();
        }

        public void deleteEntry_Click(object sender, RoutedEventArgs e) {
            (DataContext as EntryVM).deleteEntry();
        }

        public void sendToFosterParent(object sender, RoutedEventArgs e) {
            //(DataContext as EntryVM).TreeVM.WaitingForParentSelection = false;
            this.MouseUp -= sendToFosterParent; //unsubscribe
            EntryPanel selectedParentPanel = sender as EntryPanel;
            //(selectedParentPanel.DataContext as EntryVM).adoptChildFromTreeVM();
        }

        public void This_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            //RIGHT CLICKS
            //DependencyProps is the real proper way anyway.
            EntryVM dataContext = DataContext as EntryVM;
            MenuItem insertEntry = new MenuItem() { Header = "Insert entry" };
            insertEntry.Click += delegate { dataContext.insertEntry(dataContext); };//USE polymorphism!
            MenuItem insertSubEntry = new MenuItem() { Header = "Insert sub-entry" };
            insertSubEntry.Click += delegate { dataContext.insertSubEntry(dataContext); };
            MenuItem deleteEntry = new MenuItem() { Header = "Delete entry" };
            deleteEntry.Click += delegate { dataContext.deleteEntry(); };
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(insertEntry);
            contextMenu.Items.Add(insertSubEntry);
            contextMenu.Items.Add(deleteEntry);
            ContextMenu = contextMenu;
            //TITLE
            //TextBlock.Text = (DataContext as EntryVM).ToString();
            //NOT really good enough. Should be a binding (or it wont update at right time)
            bindTextBlock(TextBlock, (DataContext as EntryVM).Entry, "Name");
        }

        public void this_MouseUp(object sender, EventArgs e) {
            (DataContext as EntryVM).UpdateSelectedEntryVM(DataContext as EntryVM);
        } //AHAH! perhaps you just need to call the DATACONTEXT of this,
        //THEN CALL the datacontext of the treeview,
        //THEN to the datacontext of the propertiesPanel,
        //THEN properties panel SEES it has changed, and so updates itself...
        //AND THAT is the case when THEN need a listener!!! listening to Datacontext, property changed!


    }
}
