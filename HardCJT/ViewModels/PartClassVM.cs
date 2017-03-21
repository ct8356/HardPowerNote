using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;
using CJT;
using CJT.Models;
using CJT.ViewModels;
using HardCJT.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HardCJT.ViewModels {
    public class PartClassVM : EntryVM {
        //NOTE: Perhaps better to put some STATIC properties in here as well,
        //So can try and bind to them, and this is reflected in ALL PartClassVMs!
        //NOTE: I think COULD get away, with just ONE type of VM. But keep as is for now.
        public PartClass PartClass {
            get { return Entry as PartClass; }
            set {
                Entry = value;
                NotifyPropertyChanged("PartClass");
            }
        }

        public string PartNumber {
            get { return PartClass.OrderNumber; }
            set {
                PartClass.OrderNumber = value;
                NotifyPropertyChanged("PartNumber");
            }
        }

        public string Manufacturer {
            get { return PartClass.Manufacturer; }
            set {
                PartClass.Manufacturer = value;
                NotifyPropertyChanged("Manufacturer");
            }
        }

        public string Description {
            get { return PartClass.Description; }
            set {
                PartClass.Description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public PartClassVM(PartClass part, EFContext dbContext) {
            initialize(part, dbContext);
        }

        public PartClassVM(String name, EFContext dbContext) {
            //NOTE: this one creates the Entry, and THEN wraps it!!!
            PartClass newEntry = new PartClass(name);
            initialize(newEntry, dbContext);
            (DbContext as EFContext).Parts.Add(newEntry);
        }
       
        protected override void initializePropertyList() {
            //AHAH! BUT! Do want somewhere,
            //Where can store information about which properties to show.
            //AND that is what Important Properties is for!
            //AND, it kind of makes sense, to have a VM for each type,
            //OR at least, a STATIC VMClass for each Type,
            //So that can store, properties to show, in there!
            //AGREED! ESSENTIAL for MVVM that don't put these in Model!
            //BUT THEN AGAIN! Could put these things in the view!
            //BUT pointless. WOULD just require a look up table.
            //COULD have these properties, as properties in the VM.
            //I.e. Property Name, Property OrderNumber, etc.
            //BUT, I like this list better, CAUSE then don't even NEED reflection on the VM!
            //OK!
            //BUT NOTE, if were doing it by writing SQL statements,
            //THEN view info, i.e. meta data, I guess stored on column?
            //AH BUT! it would be specific to the VIEWER!
            //SO, it would have to be meta-data, saved to the app settings!
            //AND YES, would want them to WRAP AROUND the model properties.
            //ANY WAY to do this automatically?
            //HEY I guess could get columnNames as list, (when you click "Update columns from db")
            //Then fill a list<Property> with these names.
            //THEN save this list to settings, including things like "hidden" etc.
            //AHAH YES! another reason to have list, and not properties here.
            //AHAH BUT! if could get ImportantProperties list, from table,
            //THEN COULD Get it from Entry, AND SO WOULD NOT NEED ALL THESE VMS!
            //YES! OK! A way forward perhaps CURRENT REVISIT!
        }

        public override void insertEntry(EntryVM selectedEntryVM) {
            PartClassVM entryVM = new PartClassVM("blank", DbContext);
            insertEntry(entryVM, selectedEntryVM);
        }//easiest to keep here. (SINCE new is difficult with T generic classes!)
        //SO conclusion is, NOT really worth making the generic EntryVM.
        //SPECIALLY when I am under such time pressure!

        public override void insertSubEntry(EntryVM parentVM) {
            PartClassVM entryVM = new PartClassVM((parentVM.Entry as PartClass).Name + " child", DbContext); //create part.
            insertSubEntry(entryVM, parentVM);
        }//easiest to keep here.
        //This fails as generic because CANNOT create new() in generics with ANY PARAMETER!
        //Only way to do it is to pass constructor as a FUNCTION. Functional programming.
        //TOO COMPLICATED for now!
        //(WELL another option is to just set the property AFTER construction.)

        //NOTE: Say, ASSEMBLY, can have parts.
        //PARTS all have 1 PARENT assembly, that they belong to.
        //Could they have more than one? No, not really... maybe in very rare cases.
        //Could always have a MAIN parent anyway.
        //ASSEMBLIES, would have other assemblies, who are THEIR children. 
        //EACH assembly has one PARENT!
        //PARTS are different, BECAUSE they represent ONE part, in database.
        //They are, the LEAF!.
        //NO REAL NEED for jobs... But could just tag them with it.
        //TAG assemblies, so can just show relevant ones...
        //COULD have ASSEMBLY tags, so only get RELEVANT suggestions.
        //OR even JOB tags... //gets tricky.
        //WELL YES! job properties, SINCE each WILL DEFO have a JOB NUMBER!
        //FOR our company at least... //CAN STILL auto assign it.
        //AND SHOULD REALLY have the option, that create a part...
        //THEN you assign a CHILD...
        //SO!! YES!! way to achieve goal, is to make TAGS, entries.
        //THEN to show them, you show tags, AND all their children!
        //IF even needs to be, a parent child relationship...
        //So, assign child, the child is, or, the link is, the ASSEMBLY its part of.
        //SOMETIMES, you do need, parent child relation, to show DIRECTION.
        //TAGS, direction NOT so important...
        //the PARENT IS, the ASSEMBLY its part of...
        //THEN, can show all ASSEMBLIES as parents (BASED ON TREE TEMPLATE!!!).
        //THEN, SHOW all the relevant PARTS, as CHILDREN!!!
        //THEN EVENTUALLY, do similar thing, with some GRAPHICS!
        //GETS WAY TOO COMPLICATED FOR ME!!! //NEED GO FULL TIME!!

    }
}
