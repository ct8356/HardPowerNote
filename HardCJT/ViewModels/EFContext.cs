using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CJT;
using HardCJT.Models;
using HardCJT.ViewModels;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Collections.ObjectModel;

namespace HardCJT.ViewModels {
    public class EFContext : CJT.EFContext {
        public DbSet<Entry> Entries { get; set; }
        public DbSet<PartClass> Parts { get; set; }
        public DbSet<PartInstance> PartInstances { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public IEnumerable<CJTType> TypeList { get; set; }

        public EFContext() {
            TypeList = new ObservableCollection<CJTType> {
                new CJTType(typeof(Entry), typeof(EntryVM), Entries),
                new CJTType(typeof(PartClass), typeof(PartClassVM), Parts),
                new CJTType(typeof(PartInstance), typeof(PartInstanceVM), PartInstances),
                new CJTType(typeof(Note), typeof(NoteVM), Notes),
                new CJTType(typeof(Tag), typeof(TagVM), Tags),
                new CJTType(typeof(Task), typeof(TaskVM), Tasks),
            };
        }


    }
}
