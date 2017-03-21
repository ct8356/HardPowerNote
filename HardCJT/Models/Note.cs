using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJT.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HardCJT.Models {
    public class Note : Entry {

        [InverseProperty("Notes")]
        public virtual ICollection<Entry> Entries { get; set; }

        [InverseProperty("Answers")]
        public virtual ICollection<Question> Questions { get; set; }

        public Note() : base() {
            Type = "HardCJT.Models.Note";
        }

        public Note(string contents) : this() {
            Name = contents;
        }

        public Note(string contents, Tag tag) : this(contents) {
            Tags.Add(tag);
        }

    }
}
