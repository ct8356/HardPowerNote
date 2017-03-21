using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJT.Models;

namespace HardCJT.Models {
    public class Transaction : Entry {

        public virtual Content Content { get; set; }

        int quantity = 0;
        public int Quantity {
            get { return quantity; }
            set { quantity = value; NotifyPropertyChanged("Quantity"); }
        }

        private DateTime dateMade;
        public DateTime DateMade {
            get { return dateMade; }
            set { dateMade = value; NotifyPropertyChanged("DateMade"); }
        }

        private int jobNumber;
        public int JobNumber {
            get { return jobNumber; }
            set { jobNumber = value; NotifyPropertyChanged("JobNumber"); }
        }

        public Transaction() {
            Content = new Content();
        }

    }
}
