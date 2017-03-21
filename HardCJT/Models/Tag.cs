using System.Collections.Generic;
using System.ComponentModel;
using CJT.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HardCJT.Models {
    public class Tag : Entry {

        //NOTE: It makes life a lot easier to make this an ENTRY,
        //because then you can cast objects into type T in generic classes.
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] //?? CBTL? important?

        //If you find yourself using more than 3 or 4 tags for one entry,
        //you should think about making a new property for that entry.

        [InverseProperty("Tags")]
        public virtual ICollection<Entry> Entries { get; set; }

        public Tag() : base() {
            Type = "HardCJT.Models.Tag";
            //do nothing
        }

        public Tag(string name) : this() {
            Name = name;
        }

    }
}
