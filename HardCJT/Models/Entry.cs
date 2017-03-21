using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CJT;
using CJT.Models;

namespace HardCJT.Models {
    public class Entry : BaseEntry {

        [InverseProperty("Entries")]
        public virtual ObservableCollection<Tag> Tags { get; set; }
        //tags are basically a last resort way of categorizing things.
        //incase there is not a property for that category already.

        [InverseProperty("Entries")]
        public virtual ObservableCollection<Note> Notes { get; set; }

        public Entry() : base() {
            Tags = new ObservableCollection<Tag>();
            Notes = new ObservableCollection<Note>();
        }

        public Entry(string name) : this() {
            Name = name;
        } //NOTE YO! I really don't think should be allowed to do this!
        //ALMOST needs to be an ABSTRACT class!

        public override string ToString() {
            return Name;
        }

    }
}
