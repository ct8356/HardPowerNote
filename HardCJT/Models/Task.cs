using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CJT.Models;

namespace HardCJT.Models {
    public class Task : Entry {
        //SOME people say, models should know nothing about your DALayer.
        //WHICH might be true. Just make DAL respond to events started here...

        int priority;
        public int Priority {
            get { return priority; }
            set { priority = value; NotifyPropertyChanged("Priority"); }
        }

        int duration;
        public int Duration {
            get {
                if (Children.Count == 0) { return duration; }
                else { return sumOfChildrensDurations(); }
            }
            set { duration = value; NotifyPropertyChanged("Duration"); }
        }

        bool completed;
        public bool Completed {
            get { return completed; }
            set { completed = value; NotifyPropertyChanged("Completed"); }
        }

        [InverseProperty("Children")]
        public virtual ObservableCollection<Task> Parents { get; set; }
        [InverseProperty("Parents")]
        public virtual ObservableCollection<Task> Children { get; set; }
        [InverseProperty("Tasks")]
        public virtual ObservableCollection<PartInstance> PartInstances { get; set; }

        public Task() : base() {
            Type = "HardCJT.Models.Task";
            Priority = 10;
            Duration = 10;
            Parents = new ObservableCollection<Task>();
            Children = new ObservableCollection<Task>();
            PartInstances = new ObservableCollection<PartInstance>();
        }

        public Task(string contents) : this() {
            Name = contents;
        }

        public Task(string contents, Tag tag) : this(contents) {
            Tags.Add(tag);
        }

        public int sumOfChildrensDurations() {
            int sum = 0;
            foreach (Task child in Children) {
                sum = sum + child.Duration;
            }
            return sum;
        }
    }
}
