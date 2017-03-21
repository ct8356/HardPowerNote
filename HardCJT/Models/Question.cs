using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardCJT.Models {
    public class Question : Task {

        [InverseProperty("Questions")]
        public virtual ObservableCollection<Note> Answers { get; set; }

        public Question() : base() {
            Type = "HardCJT.Models.Question";
            Answers = new ObservableCollection<Note>();
        }

    }
}
