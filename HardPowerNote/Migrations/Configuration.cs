using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using HardCJT.Models;
using HardCJT.ViewModels;
using System.Collections.ObjectModel;

namespace HardPowerNote.Migrations {

    internal sealed class Configuration : DbMigrationsConfiguration<EFContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = true; //was False, but error suggested
            //I change it to true
        }

        public EFContext DbContext { get; set; }

        public void callSeed(EFContext context) {
            DbContext = context;
            Seed(context);
        }
        //RIGHT, so this configuration, WON'T DO nice migrations.
        //BUT, can look into that, when really need it.
        //For now, can just delete, and recreate the database.
        //NOW I have updated to EF v6, will it do nice migs now?

        protected override void Seed(EFContext context) {
            
            //OK! Perhaps, it is actually THIS method, that creates the database.
            //NOT the create context instantiation...
            //(I guess create(), will still do the trick. but NOT context instantiation...nec.) ??

            string type = "Type";
            string gender = "Gender";
            string parent = "Parent";
            string friend = "Friend";

            PartInstance airService = new PartInstanceVM("Air service unit", DbContext).Entry as PartInstance;
            PartInstance pressureSwitch = new PartInstanceVM("Pressure switch", DbContext).Entry as PartInstance;
                pressureSwitch.AssemblyParents.Add(airService);
                Note set6 = new NoteVM("Set to 6 bar", DbContext).Entry as Note;
                pressureSwitch.Notes.Add(set6);
                PartClass oneSeven = new PartClassVM("175250", DbContext).Entry as PartClass;
                pressureSwitch.PartClasses.Add(oneSeven);
            PartInstance dumpValve = new PartInstanceVM("Dump valve", DbContext).Entry as PartInstance;
                dumpValve.AssemblyParents.Add(airService);
                PartClass midi = new PartClassVM("MIDI", DbContext).Entry as PartClass;
                dumpValve.PartClasses.Add(midi);
            Task order = new TaskVM("Order groceries", DbContext).Entry as Task;
                Tag urgent = new TagVM("Urgent", DbContext).Entry as Tag;
                order.Tags.Add(urgent);
                Tag home = new TagVM("Home", DbContext).Entry as Tag;
                order.Tags.Add(home);
            Task fixB = new TaskVM("Fix bike", DbContext).Entry as Task;
                fixB.Tags.Add(home);
            Task finish = new TaskVM("Finish program", DbContext).Entry as Task;
                finish.Tags.Add(home);
                finish.Tags.Add(urgent);
            Task seed = new TaskVM("Seed the database", DbContext).Entry as Task;
                seed.Parents.Add(finish);
            Task test = new TaskVM("Test the program", DbContext).Entry as Task;
                test.Parents.Add(finish);
            //NOTE: above is the way forward (i.e. using strings).
            //COZ THEN, does not matter WHERE you define the variable first!
            //ALL you have to do, is define the STRING first! YES!
            //although, entries would work too...
            //BUT strings is that much easier, since can just put string in,
            //then it either finds it for you, OR creates a new entry for you!
            context.SaveChanges();
        } //NOTE YO! Making a builderClass, would actually be BETTER than using constructors I think!
        //YES! Seems to be way forward! 
        //DEFO the way to go! COZ ListBoxPanelVM require MainVMs! That's too painful!
        //ALSO, scripts, i.e. methods for here, can be very effective...

        private void Add<T>(ObservableCollection<T> collection, string entryName) where T : Entry, new() {
            T entry = null;
            //CHECK if entryName already exists first.
            entry = new T();
            collection.Add(entry);
            //AHH poo,
            //NOT SURE this method even needed!
            //I THINK if you ADD to the collection,
            //THEN IT DOES AUTO add it in the reverse direction!
            //ONLY if you say, make parent = dad.
            //THEN it does not do it in reverse direction!
            //YES pretty sure!
        }

    }
}
