using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJT;
using CJT.Models;
using CJT.ViewModels;
using HardCJT.Models;
using HardCJT.ViewModels;

namespace HardCJT.ExcelViewModels {
    public class TransactionVM : EntryVM {
        public Transaction Transaction {
            get { return Entry as Transaction; }
            set { Entry = value; }
        } //NOTE CURRENT REVISIT: I do think this should be PRIVATE,
        //BUT will have to do later.

        public string PartNumber {
            get { return Transaction.Content.PartClass.OrderNumber; }
            set {
                Transaction.Content.PartClass.OrderNumber = value;
                NotifyPropertyChanged("PartNumber");
            }
        }

        public string Manufacturer {
            get { return Transaction.Content.PartClass.Manufacturer; }
            set {
                Transaction.Content.PartClass.Manufacturer = value;
                NotifyPropertyChanged("Manufacturer");
            }
        }

        public string Description {
            get { return Transaction.Content.PartClass.Description; }
            set {
                Transaction.Content.PartClass.Description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public int Quantity {
            get { return Transaction.Quantity; }
            set {
                Transaction.Quantity = value;
                NotifyPropertyChanged("Quantity");
            }
        }

        public string Condition {
            get { return Transaction.Content.Condition; }
            set {
                Transaction.Content.Condition = value;
                NotifyPropertyChanged("Condition");
            }
        }

        public string Shelf {
            get { return Transaction.Content.Location; }
            set {
                Transaction.Content.Location = value;
                NotifyPropertyChanged("Shelf");
            }
        }

        public DateTime DateMade {
            get { return Transaction.DateMade; }
            set {
                Transaction.DateMade = value;
                NotifyPropertyChanged("DateMade");
            }
        }

        public int JobNumber {
            get { return Transaction.JobNumber; }
            set {
                Transaction.JobNumber = value;
                NotifyPropertyChanged("JobNumber");
            }
        }

        public TransactionVM(ExcelContext dbContext) {
            Transaction = new Transaction();
            initialize(dbContext);
        }

        public TransactionVM(Transaction trans, ITreeVM treeVM, DbContext dbContext) {
            initialize(trans, treeVM);
        }

        public void SaveChanges() {
            string commandString =
            "UPDATE [Sheet1$] SET [Part number] = @partNumber, " +
            "[Manufacturer] = @manufacturer, [Description] = @description, " +
            "[Quant] = @quantity, [Condition] = @condition, [Shelf] = @shelf, " +
            "[Date Made] = @dateMade, [Job number] = @jobNumber " +
            "WHERE [Part number] = @partNumber";
                //THIS is how mainstream databases do it apparently! (except those using ORM).
                string[] propertyNames =
                    new string[8] {"partNumber","manufacturer","description","quantity",
                        "condition","shelf","dateMade","jobNumber" };
                string[] propertyValues =
                new string[8] { PartNumber,
                                Manufacturer,
                                Description,
                                Quantity.ToString(),
                                Condition,
                                Shelf,
                                DateMade.ToString(),
                                JobNumber.ToString()};
            (DbContext as ExcelContext).UpdateEntry(commandString, propertyNames, propertyValues);
        }

    }
}
