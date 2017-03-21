using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using CJT.Models;

namespace HardCJT.Models {
    public class PartInstance : Entry {

        [Display(Name = "My String Display Name")]
        public String MyString { get { return "my string"; } set { value = value; } }
        [InverseProperty("PartInstances")]
        public virtual ObservableCollection<PartClass> PartClasses { get; set; }
        [InverseProperty("AssemblyChildren")]
        public virtual ObservableCollection<PartInstance> AssemblyParents { get; set; }
        //virtual is there to take advantage of lazy loading.
        //Navigation properties HAVE to be marked virtual.
        [InverseProperty("AssemblyParents")]
        public virtual ObservableCollection<PartInstance> AssemblyChildren { get; set; }
        [InverseProperty("PartInstances")]
        public virtual ObservableCollection<Task> Tasks { get; set; }
        [InverseProperty("Sensors")] 
        public virtual ObservableCollection<PartInstance> SensedParts { get; set; }
        [InverseProperty("SensedParts")] //NOT SURE you need it over the non-collection bit? causes an error?
        public virtual ObservableCollection<PartInstance> Sensors { get; set; }
        //NOTE: A PartInstance, CAN be an EXAMPLE partInstance. i.e. imaginary.
        //Does not actually have to exist. //THIS way, is bit more work, but more flexible, I think.

        public PartInstance()
            : base() {
            Type = "HardCJT.Models.PartInstance";
            PartClasses = new ObservableCollection<PartClass>();
            AssemblyParents = new ObservableCollection<PartInstance>();
            AssemblyChildren = new ObservableCollection<PartInstance>();
            SensedParts = new ObservableCollection<PartInstance>();
            Sensors = new ObservableCollection<PartInstance>();
            Tasks = new ObservableCollection<Task>();
        }//FOR some bizzare reason, AddOrUpdate method requires this...
        //even though, it is never used.

        public PartInstance(string functionText)
            : this() {
            Name = functionText;
        }

        //public PartInstance(string functionText, PartClass partClass, ObservableCollection<Tag> tags)
          //  : this(functionText) {
          //  PartClass = partClass;
         //   Tags = tags;
        //}

    }
}
