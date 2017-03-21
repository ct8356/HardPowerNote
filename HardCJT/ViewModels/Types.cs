using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel;
using HardCJT.Models;

namespace HardCJT.ViewModels {
    public class Types {
        //NOTE a class called MyType, did not work
        //coz Could not have a List<Type<Entry>> and fill it with Type<Part> for example.
        //AND needed the Type<Part> to be able to return the correct DbSet<Part>!!!

        public static List<CJTType> List { get; set; } //maybe not needed, since could
        //loop through MyTypes in the list, and say typeof(myType). to get it.

        public static IQueryable<Entry> GetDbSet(Type type, EFContext dbContext) {
            IQueryable<Entry> dbSet = null;
            if (type == typeof(Entry))
                dbSet = dbContext.Entries;
            if (type == typeof(Note))
                dbSet = dbContext.Notes;
            if (type == typeof(PartClass))
                dbSet = dbContext.Parts;
            if (type == typeof(PartInstance))
                dbSet = dbContext.PartInstances;
            if (type == typeof(Task))
                dbSet = dbContext.Tasks;
            if (type == typeof(Tag))
                dbSet = dbContext.Tags;
            return dbSet as IQueryable<Entry>;
            //OH SHIT! So you CANNOT cast if making it equals,
            //BUT CAN if you make it the OUTPUT OF A METHOD! By jove!
            //OR even easier, for some reason,
            //CAN EASILY convert ANY of them, to IQueryable<Entry>, just not DbSet<Entry>!
        }

    }
}
