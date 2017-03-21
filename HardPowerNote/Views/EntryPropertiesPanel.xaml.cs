using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using CJT;
using AutoCompleteBox = CJT.AutoCompleteBox;
using HardCJT.Models;
using HardCJT.ViewModels;
using ListBox = CJT.ListBox;
using TextBlock = CJT.TextBlock;

namespace HardPowerNote {
    public partial class EntryPropertiesPanel : StackPanel {
        protected List<Property> EntryPropertyList { get; set; }

        public EntryPropertiesPanel() {
            EntryPropertyList = new List<Property>();
            InitializeComponent();
            //SUBSCRIBE
            DataContextChanged += this_DataContextChanged;
            //Loaded += this_DataContextChanged;
        }

        protected void bindAutoBox(AutoCompleteBox autoBox, Property property) {
            Binding binding = new Binding("SelectableItems");
            binding.Source = property.Value; //works coz value is a collection here.
            autoBox.SetBinding(AutoCompleteBox.ItemsSourceProperty, binding);
        }

        protected void bindListBox(ListBox listBox, Property property) {
            Binding binding = new Binding("SelectedItems");
            binding.Source = property.Value; //works coz value is actually a collection here.
            listBox.SetBinding(ListBox.ItemsSourceProperty, binding);
            //PREVIOUSLY, it bound to "Tags" property, of entry.
            //WHICH, in VALUE, happened to have a ListBoxPanelVM, (for purposes of GotoEntry).
            //BUT ALSO, of course, had a ObservColl<Tag> Tags.
            //SO ListBox.ItemsSourceProp bound this ObservColl<Tag> nicely.
            //BUT, will it bind to a ListBoxPanelVM? WELL NO!
            //BUT, could we make the source = property.Value,
            //AND make the property, SelectedItems? well i guess so!

            //NOTE: before this, I only ever used property.Value,
            //to help me set the DataContext of the ListBox, and LinkedTextBlock, to the correct EntryVM.
            //To help with GotoEntry.
        }

        protected void bindListBox1(ListBox listBox, Entry entry, string propertyName) {
            Binding binding = new Binding(propertyName);
            binding.Source = entry;
            listBox.SetBinding(ListBox.ItemsSourceProperty, binding);
        }

        protected void bindTextBlock(TextBlock textBlock, Entry entry, string propertyName) {
            Binding binding = new Binding(propertyName);
            binding.Source = entry;
            textBlock.SetBinding(TextBlock.TextProperty, binding);
        }

        protected void bindTextBox(TextBox textBox, Entry entry, string propertyName) {
            Binding binding = new Binding(propertyName); //This is the MODEL property it binds to.
            binding.Source = entry; // the binding source (which must fire a PROP CHANGED event).
            textBox.SetBinding(TextBox.TextProperty, binding); //fortunately, textBox already fires an event when changed.
            //YOU created the event for the dataSource. SO HOPEFULLY, we have 2 way binding now... we do :)
        }

        public void DataContext_PropertyChanged(object sender, EventArgs e) {
        }

        public void this_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            updateControls();
        }

        protected void addEntryProperty(EntryVM selectedEntryVM, string propertyName) {
            Property property = new Property();
            property.Name = propertyName;
            property.Value = selectedEntryVM.Entry; //Not needed, don't need to set DataContext
            property.Type = InfoType.LinkedTextBlock;
            EntryPropertyList.Add(property);
        }

        protected void addTextProperty(EntryVM selectedEntryVM, string propertyName) {
            Property property = new Property();
            property.Name = propertyName;
            property.Value = selectedEntryVM.Entry; //Not needed, don't need to set DataContext
            property.Type = InfoType.TextBox;
            EntryPropertyList.Insert(0, property);
        }

        public void addListProperty<TP>(EntryVM selectedEntryVM, string propertyName) 
            where TP : Entry, new() {
            Property property = new Property();
            property.Name = propertyName;
            ObservableCollection<TP> entries = selectedEntryVM.Entry.GetType().GetProperty
                    (propertyName).GetValue(selectedEntryVM.Entry, null) as ObservableCollection<TP>;
            ListBoxPanelVM<TP> listBoxVM = new ListBoxPanelVM<TP>(selectedEntryVM, entries);
            property.Value = listBoxVM;
            property.Type = InfoType.ListBox;
            EntryPropertyList.Add(property);
        }

        protected void addPropertyPanel(EntryVM selectedEntryVM, Property property) {
            PropertyPanel propertyPanel = new PropertyPanel(property.Name);
            Children.Add(propertyPanel);
            //HEY! PERHAPS ALL I NEED TO DO!
            //is to get Property.Value in some other way.
            //i.e. by reflection maybe. (which might take longer, NO! FINE FOR ENTRYPROPertiesPAnel!)
            //Got PropertyName, Got selectedEntryVM and Entry, SO, just get value (object) by reflection?
            //ASK google.
            //property.Value = selectedEntryVM.Entry.GetType().GetProperty(property.Name)
                //.GetValue(selectedEntryVM.Entry, null);
                //NOT sure above even needed, since its done in addListProperty.
            if (property.Value != null) {
                switch (property.Type) {
                    case InfoType.TextBlock:
                        TextBlock textBlock = new TextBlock();
                        bindTextBlock(textBlock, selectedEntryVM.Entry, property.Name);
                        propertyPanel.Children.Add(textBlock);
                        break;
                    case InfoType.LinkedTextBlock:
                        LinkedTextBlock linkedTextBlock = new LinkedTextBlock();
                        bindTextBlock(linkedTextBlock, selectedEntryVM.Entry, property.Name);
                        linkedTextBlock.DataContext = EntryVM.WrapInCorrectVM(property.Value as Entry, selectedEntryVM.DbContext);
                        propertyPanel.Children.Add(linkedTextBlock);
                        break;
                    case InfoType.TextBox:
                        TextBox textBox = new TextBox();
                        bindTextBox(textBox, selectedEntryVM.Entry, property.Name);
                        propertyPanel.Children.Add(textBox);
                        textBox.VerticalContentAlignment = VerticalAlignment.Center;
                        break;
                    case InfoType.ComboBox:
                        break;
                    case InfoType.ListBox:
                        ListBox listBox = new ListBox();
                        bindListBox(listBox, property);
                        //bindListBox1(listBox, selectedEntryVM.Entry, property.Name);
                        //ABOVE line is needed to bind the listBox to the list.
                        //AHAH! Reason above wont work now, I think,
                        //Is coz propertyName is RelationName = Family,
                        //AND THERE IS NO property called Family in Entry! DUH!
                        listBox.DataContext = property.Value;
                        //AHAH! really easy solve!
                        //JUST set property.Value for lists, to the TagsVM etc. easy!
                        //ABOVE line is needed, otherwise pretty sure it just inherits DC from its EntryVM.
                        //AHAH! reason you did the above,
                        //was so ListBox would work.
                        //BECAUSE DataContext of ListBox needs to be ListBoxPanelVM,
                        //in order for "Go to entry" to work! HAH!
                        propertyPanel.Children.Add(listBox);
                        FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(LinkedTextBlock));
                        Binding binding = new Binding("Name");
                        textBlockFactory.SetBinding(LinkedTextBlock.TextProperty, binding); //not needed?
                        //I think the above line is needed to bind the textblock to the entry in the list!
                        DataTemplate dataTemplate = new DataTemplate();
                        dataTemplate.VisualTree = textBlockFactory;
                        listBox.ItemTemplate = dataTemplate;
                        //FUNNY THING is, DataContext of ListBox,
                        //Is coming out as a EntryVM.
                        //When it SHOULD be a ListBoxPanelVM...
                        //AND the AUTOCOMPLETEBOX:
                        AutoCompleteBox autoBox = new AutoCompleteBox();
                        autoBox.KeyUp += autoBox.This_KeyUp;
                        bindAutoBox(autoBox, property);
                        autoBox.DataContext = property.Value;
                        //So autoBox knows whose NotifyInputConfirmed method to call
                        propertyPanel.Children.Add(autoBox);
                        break;
                    case InfoType.CheckBox:
                        break;
                }
            }
        }

        public void updateControls() {
            Children.Clear();
            EntryPropertyList.Clear();
            Children.Add(new TextBlock() { Text = "Properties", FontWeight = FontWeights.Bold });
            EntryVM selectedEntryVM = (DataContext as EntryVM);
            //GET PROPERTY names
            IEnumerable<Property> properties = selectedEntryVM.Type.Properties;
            //NOTE: selectedEntryVM Type is not being updated? was PartClass for pressure switch!
            //since Properties are avail, makes sense to use them rather than just strings.
            //Parents and children?
            //foreach parent, and each child, addListProperty?
            foreach (Property property in properties) {
                if (property.Type == InfoType.LinkedTextBlock
                    || property.Type == InfoType.ListBox) {
                    //AHAH! don't do this for ALL props, only LIST props!
                    MethodInfo method = typeof(EntryPropertiesPanel).GetMethod("addListProperty");
                    PropertyInfo pi = selectedEntryVM.Type.EntryType.GetProperty(property.Name);
                    Type propertyType = pi.PropertyType.GetGenericArguments()[0];
                    MethodInfo generic = method.MakeGenericMethod(new Type[] { propertyType });
                    generic.Invoke(this, new object[] { selectedEntryVM, property.Name });
                }
            }
            EntryPropertyList = EntryPropertyList.OrderBy(p => p.Name).ToList();
            addTextProperty(selectedEntryVM, "Name");
            foreach (Property property in EntryPropertyList) {
                //Children.Add(new PropertyPanel(property)); //OR
                addPropertyPanel(selectedEntryVM, property);
            }
        }

    }
}
