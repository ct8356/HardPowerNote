using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using CJT;
using HardCJT.Models;
using HardCJT.ViewModels;
using System.Collections.ObjectModel;

namespace HardCJT.ViewModels {
    public class CJTType {

        private string name;
        public string Name {
            get { return EntryType.Name; }
            set { name = value; }
        }

        public Type EntryType { get; set; }

        public Type EVMType { get; set; }

        public DbSet DbSet { get; set; }

        public ObservableCollection<Property> Properties;

        public CJTType(Type entryType, Type evmType, DbSet dbSet) {
            EntryType = entryType;
            EVMType = evmType;
            DbSet = dbSet;
            Properties = InitializeProperties();
        }

        public ObservableCollection<Property> InitializeProperties() {
            ObservableCollection<Property> properties = new ObservableCollection<Property>();
            if (EntryType == typeof(Entry)) {
                AddEntryProperties(properties);

            }
            if (EntryType == typeof(Note)) {
                AddEntryProperties(properties);
                properties.Add(new Property("Entries", InfoType.ListBox, false));
            }
            if (EntryType == typeof(PartClass)) {
                AddEntryProperties(properties);
                properties.Add(new Property("Name", InfoType.TextBox, false));
                properties.Add(new Property("OrderNumber", InfoType.TextBox, false));
                properties.Add(new Property("Manufacturer", InfoType.TextBox, false));
                properties.Add(new Property("PartInstances", InfoType.ListBox, false));
                properties.Add(new Property("InheritanceParents", InfoType.ListBox, true));
                properties.Add(new Property("InheritanceChildren", InfoType.ListBox, true));
            }
            if (EntryType == typeof(PartInstance)) {
                AddEntryProperties(properties);
                properties.Add(new Property("PartClasses", InfoType.ListBox, false));
                properties.Add(new Property("AssemblyParents", InfoType.ListBox, true));
                properties.Add(new Property("AssemblyChildren", InfoType.ListBox, true));
                properties.Add(new Property("Tasks", InfoType.ListBox, false));
                properties.Add(new Property("Sensors", InfoType.ListBox, true));
            }
            if (EntryType == typeof(Task)) {
                AddEntryProperties(properties);
                properties.Add(new Property("Parents", InfoType.ListBox, true));
                properties.Add(new Property("Children", InfoType.ListBox, true));
            }
            if (EntryType == typeof(Tag)) {
                AddEntryProperties(properties);
                properties.Add(new Property("Entries", InfoType.ListBox, false));
            }
            return properties;
        }//HELL Just do it all here! Easier!
         //COULD actually be a array rather than list,
         //COZ I don't think size of list will ever change in program!
         //COULD get them from the entry itself.
         //BUT would need some clever code.

        public void AddEntryProperties(ObservableCollection<Property> properties) {
            properties.Add(new Property("EntryID", InfoType.TextBlock, false));
            properties.Add(new Property("CreationDate", InfoType.TextBlock, false));
            properties.Add(new Property("Name", InfoType.TextBox, false));
            properties.Add(new Property("Notes", InfoType.ListBox, false));
            properties.Add(new Property("Tags", InfoType.ListBox, false));
            //NOTE YO! CURRENT!
            //MIGHT NEED separate list, 
            //one for primitives as above,
            //one for entries!!!
            //COZ the tree should only ever structure by entries!
        }

        public override string ToString() {
            return Name;
        }

    }
}
