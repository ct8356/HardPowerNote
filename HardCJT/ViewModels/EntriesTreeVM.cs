using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq.Expressions;
using AutoCompleteBox = CJT.AutoCompleteBox;
using CJT;
using CJT.ViewModels;
using HardCJT.Models;

namespace HardCJT.ViewModels {
    //NOTE: if you can move most functionality to DBContext,
    //THEN won't need to pass THIS class to everything!
    public class EntriesTreeVM : GenericTreeVM<Entry,EntryVM,EFContext> {  
        public List<ListBoxPanelVM<Entry>> Filters { get; set; }
        public ListBoxPanelVM<Tag> TagFilter { get; set; }
        public ObservableCollection<Tag> AllTags { get; set; }
        public GenericComboBoxVM<CJTType> SystemTypePanelVM { get; set; } //Node Relation
        public GenericComboBoxVM<string> GroupPanelVM { get; set; }
        public GenericComboBoxVM<string> StructurePanelVM { get; set; }

        public EntriesTreeVM(EFContext parentVM) : base(parentVM) {

            //OK maybe just have a list of SystemTypes,
            //and whenever need the DBContext, just use a convertor method (maybe a static one).
            //MAYBE make a static class, that has list, and convertor, then all in one place...
            //YES that is fine, BUT, 
            //FILTER
            FilterEntryVM = new EntryVM(DbContext);
            //FILTER PANELS
            SystemTypePanelVM = new GenericComboBoxVM<CJTType>();
            SystemTypePanelVM.Items = new ObservableCollection<CJTType>(DbContext.TypeList);
            SystemTypePanelVM.SelectedItem = SystemTypePanelVM.Items.First();
            SystemTypePanelVM.PropertyChanged += FilterChanged;         
            //GROUPING AND STRUCTURING PANELS
            GroupPanelVM = new GenericComboBoxVM<string>();
            GroupPanelVM.PropertyChanged += StructureChanged;
            StructurePanelVM = new GenericComboBoxVM<string>();
            StructurePanelVM.PropertyChanged += StructureChanged;
            //UPDATE
            UpdateEntries(); //a listener would mean, would not have to call here. ignore.
        }

        public IQueryable<T> filterByType<T>(IQueryable<T> filteredEntries) where T : Entry {
            //string typeName = ((displayPanel.TypePanel.DataContext as TypePanelVM).SelectedObjects.First() as Type).FullName;
            //Type type = Type.GetType(typeName);
            //Type type = TypePanelVM.SelectedItem as Type;
            //filteredEntries = filteredEntries.AsEnumerable<Entry>().Where(e => e.GetType() == type);
            //AHAH! Think the above is necessary, IF want to do GetType (reflection) in a query!
            return filteredEntries;
        }//LATER if decide it takes too long to do several queries,
         //can turn this method into a "construct SQL" query, maybe...

        protected IQueryable<T> filterByTag<T>(IQueryable<T> entries) where T : Entry {
            return entries
            //.Include(e => e.ParentRelations)
            //.Include(e => e.ParentRelations.Select(r => r.ParentEntry))
            ;//WOW! How strange, getting rid of these includes,
            //Seemed to speed it up loads! How weird!
            //SO maybe, doing lots of little queries, really is not a problem?
            //INFACT, better, than doing one big query?
            //(even if the query ISN'T that big!)
            //WELL ACTUALLY not THAT much faster. Hard to tell really.
            //STILL, getting relations, and including all ENDS of relations,
            //GOT to be faster, than getting entries, and getting all kids right?
            //WELL maybe, maybe not, as you discovered... (drawn on envelope).

            //IT STILL takes a long time to load, after compiling though.
            //WHY IS THAT? Quite a simple program really.
            //Maybe it loads lots of irrelevant stuff?
        }

        protected IQueryable<T> filterEntries<T>(IQueryable<T> entries) where T : Entry {
            //int count = entries.Count();
            IQueryable<T> filteredEntries = entries;
            filteredEntries = filterByType(filteredEntries);
            //int count2 = filteredEntries.Count();
            filteredEntries = filterByTag<T>(filteredEntries);
            //orderBy(); //does nothing yet
            return filteredEntries;
        }

        protected IQueryable<Entry> getFilteredEntries() {
            IQueryable<Entry> entries = null;
            if (SystemTypePanelVM.SelectedItem != null) {
                CJTType selectedType = SystemTypePanelVM.SelectedItem;
                entries = filterEntries<Entry>(selectedType.DbSet as IQueryable<Entry>);
                //ABOVE IS THE MAGIC STEP!
                //COZ C# allows you to cast DbSet, into IQuer<Entry>!
            }
            return entries;
        }

        protected Expression<Func<T, bool>> EnumContainsPropertyEntryID<T>
            (IEnumerable<int> enumerable, string propertyName) {
            //AHAH! For SQL/Entity queries, have to do it with ID!
            //For normal LINQ to object queries, can use the objects themselves!
            ParameterExpression parameter = Expression.Parameter(typeof(T)); //input parameter e.
            MemberExpression member = Expression.Property(parameter, propertyName); //select the member
            MemberExpression member2 = Expression.Property(member, "EntryID"); //select the actual member
            MethodInfo method = typeof(Enumerable).GetMethods().
                    Where(x => x.Name == "Contains").
                    Single(x => x.GetParameters().Length == 2).
                    MakeGenericMethod(typeof(int)); //Choose a method
            ConstantExpression constant = Expression.Constant(enumerable, typeof(IEnumerable<int>)); //Set the constant
            Expression exp = Expression.Call(method, constant, member2);
            return Expression.Lambda<Func<T, bool>>(exp, parameter);
        }

        protected Expression<Func<T, bool>> EnumDoesNotContainPropertyEntryID<T>
            (IEnumerable<int> enumerable, string propertyName) {
            ParameterExpression e = Expression.Parameter(typeof(T)); //input parameter e.
            MemberExpression property = Expression.Property(e, propertyName); //select the member
            MemberExpression entryID = Expression.Property(property, "EntryID"); //select the actual member
            MethodInfo method = typeof(Enumerable).GetMethods().
                    Where(x => x.Name == "Contains").
                    Single(x => x.GetParameters().Length == 2).
                    MakeGenericMethod(typeof(int)); //Choose a method
            ConstantExpression enumer = Expression.Constant(enumerable, typeof(IEnumerable<int>)); //Set the constant
            Expression exp = Expression.Call(method, enumer, entryID);
            Expression exp2 = Expression.Not(exp);
            return Expression.Lambda<Func<T, bool>>(exp2, e);
        }

        protected Expression<Func<TE, bool>> EnumIntersectsPropertyEntryIDs<TE, TP>
            (IEnumerable<int> enumerable, string propertyName) {
            //problem now is, collection .Entries is of type Entry.
            //BUT in reality, HERE, it contains Tasks. (i.e. the tasks that have been Tagged).
            //AND I am showing the first row as Tags.
            //And I am looking in tag.Entries to find the Tasks in filteredEntries (to intersect with).
            //MAYBE, I just need to specify them as Entries (in super method), But get the Tasks?
            ParameterExpression p = Expression.Parameter(typeof(TP)); //input parameter p. Tag.
            MemberExpression entryID = Expression.Property(p, "EntryID"); //select the actual member
            LambdaExpression lam = Expression.Lambda<Func<TP, int>>(entryID, p);
            ParameterExpression e = Expression.Parameter(typeof(TE)); //input parameter e. Task. Entry.
            MemberExpression collection = Expression.Property(e, propertyName); //select the member
            MethodInfo select = typeof(Enumerable).GetMethods().
                   Where(x => x.Name == "Select").
                   First(x => x.GetParameters().Length == 2).
                   MakeGenericMethod(new Type[] { typeof(TP), typeof(int) });
                   //typeof(source: collection), typeof(result: entryID), 
            Expression exp1 = Expression.Call(select, collection, lam);
            MethodInfo intersect = typeof(Enumerable).GetMethods().
                    Where(x => x.Name == "Intersect").
                    Single(x => x.GetParameters().Length == 2).
                    MakeGenericMethod(typeof(int)); //Choose a method
            ConstantExpression enumer = Expression.Constant(enumerable, typeof(IEnumerable<int>));
            Expression exp2 = Expression.Call(intersect, enumer, exp1);
            MethodInfo any = typeof(Enumerable).GetMethods().
                  Where(x => x.Name == "Any").
                  Single(x => x.GetParameters().Length == 1).
                  MakeGenericMethod(typeof(int)); //Choose a method
            Expression exp3 = Expression.Call(any, exp2);
            return Expression.Lambda<Func<TE, bool>>(exp3, e);
        }//.Where(e => enumerable.Intersect(e.collection.Select(p => p.EntryID)).Any())

        protected Expression<Func<EntryVM, bool>> EntryEqualsChildsProperty<T>(T child, string propertyName) {
            //Get entry
            ParameterExpression entryVM = Expression.Parameter(typeof(EntryVM)); //the input parameter "evm"
            MemberExpression entry = Expression.Property(entryVM, "Entry");
            //Get constant
            PropertyInfo property = child.GetType().GetProperty(propertyName);
            ConstantExpression constant = Expression.Constant(property.GetValue(child, null), typeof(Entry));
            //and the rest
            Expression exp = Expression.Equal(entry, constant);
            return Expression.Lambda<Func<EntryVM, bool>>(exp, entryVM);
        }

        protected Expression<Func<EntryVM, bool>> EntryIsContainedInChildsProperty<TE,TP>
            (TE child, string propertyName) {
            //Get entry
            ParameterExpression entryVM = Expression.Parameter(typeof(EntryVM)); //the input parameter "evm"
            MemberExpression entry = Expression.Property(entryVM, "Entry");
            //Get constant
            PropertyInfo property = child.GetType().GetProperty(propertyName);
            ConstantExpression collection = Expression.Constant
                (property.GetValue(child, null), typeof(ObservableCollection<TP>));
            //and the rest
            MethodInfo method = typeof(Enumerable).GetMethods().
                Where(x => x.Name == "Contains").
                Single(x => x.GetParameters().Length == 2).
                MakeGenericMethod(typeof(Entry)); //Choose a method
            Expression exp = Expression.Call(method, collection, entry);
            return Expression.Lambda<Func<EntryVM, bool>>(exp, entryVM);
        } //entries.Where(evm => child.Parents.Contains(evm.Entry));
        
        protected Expression<Func<T, bool>> PropertyIsEmpty<T>(string propertyName) {
            //CURRENT, problem lies here. Entry does not have prop Parents...
            ParameterExpression parameter = Expression.Parameter(typeof(T)); //the input parameter e.
            MemberExpression member = Expression.Property(parameter, propertyName); //select the member
            MethodInfo method = typeof(Enumerable).GetMethods()
                .Where(x => x.Name == "Count")
                .Single(x => x.GetParameters().Length == 1)
                .MakeGenericMethod(typeof(T)); //Choose a method
            Expression exp = Expression.Call(method, member);
            ConstantExpression constant = Expression.Constant(0);
            Expression exp2 = Expression.Equal(exp, constant);
            return Expression.Lambda<Func<T, bool>>(exp2, parameter); //return Lambda function, with parameter as e.
        }//entries.Where(e => e.ParentInstances.Count() == 0).

        public Expression<Func<TE, bool>> PropertyIsNotEmpty<TE, TP>(string propertyName) {
            ParameterExpression parameter = Expression.Parameter(typeof(TE)); //the input parameter e.
            MemberExpression member = Expression.Property(parameter, propertyName); //select the member
            MethodInfo method = typeof(Enumerable).GetMethods()
                .Where(x => x.Name == "Count")
                .Single(x => x.GetParameters().Length == 1)
                .MakeGenericMethod(typeof(TP)); //Choose a method
            Expression exp = Expression.Call(method, member);
            ConstantExpression constant = Expression.Constant(0);
            Expression exp2 = Expression.Equal(exp, constant);
            Expression exp3 = Expression.Not(exp2);
            return Expression.Lambda<Func<TE, bool>>(exp3, parameter); //return Lambda function, with parameter as e.
        }//entries.Where(e => !e.ParentInstances.Count() == 0).

        protected Expression<Func<T, bool>> PropertyEqualsNull<T>(string propertyName) {
            //THIS is the reason for needing the GENERIC methods (specifying T).
            //PRETTY SURE it is necessary. (i.e. could not get away with typeof(Entry)).
            //YES! HERE IS THE GENIUS! DOES NOT NEED TO BE GENERIC,
            //COZ CAN JUST PASS typeof(T).... HAHAHH!!!!! CURRENT REVISIT!!!!
            //SIDENOTE: So if I'm making T = Entry every time,
            //THEN I need to pass Type type!
            //HOPEFULLY IT WORKS THEN!
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            //NOTE: using reflection below, SO may as well use it above!
            MemberExpression member = Expression.Property(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(null);
            Expression exp = Expression.Equal(member, constant);
            return Expression.Lambda<Func<T, bool>>(exp, parameter);
            //CRAP! you can't just pass type.
            //YOU NEED THe correct T for this lamba expression!
            //OK bite the bullet, gonna have to have IF STATEMENt IN GETFILteredEntries!
        }

        protected Expression<Func<T, bool>> PropertyNotNull<T>(string propertyName) {
            ParameterExpression parameter = Expression.Parameter(typeof(T)); //the input parameter e.
            MemberExpression member = Expression.Property(parameter, propertyName); //select the member
            //propName sensor is not defined for entry!
            ConstantExpression constant = Expression.Constant(null); //create constant "null"
            Expression exp = Expression.NotEqual(member, constant); //create expression
            return Expression.Lambda<Func<T, bool>>(exp, parameter); //return Lambda function, with parameter as e.
        }

        public IQueryable<TP> showGroups<TE, TP>
            (IQueryable<TE> entries, string propertyName) where TE : Entry where TP : Entry {
            IQueryable<TP> dbSet = DbContext.TypeList.Where(t => t.EntryType == typeof(TP))
                .Select(t => t.DbSet).First() as IQueryable<TP>; //could get with method saw on internet.
            IQueryable<TP> groups = null;
            IEnumerable<int> entryIDs = entries.Select(e => e.EntryID); //partInstances. or Tasks.
            //propName = PartClasses. or Tags.
            //RATHER than getting filteredEntries, then trying to almagate a List of Lists of Genders,
            //(COZ not sure how to do that elegantly!)
            //just get ANY Entries, that match some criteria, e.g.
            Attribute attribute = typeof(TE).GetProperty(propertyName).GetCustomAttribute(typeof(InversePropertyAttribute));
            string inversePropertyName = (attribute as InversePropertyAttribute).Property;
            groups = dbSet.Where
                (EnumIntersectsPropertyEntryIDs<TP, TE>(entryIDs, inversePropertyName)); //SWITCHED!
            //must be Entries, coz groups WONT be part of filteredEntries.
            //i.e. PartClasses.Where(pc => pc.propertyList.intersect(entries)); NO! WANT:
            //PartClasses.Where(pc => entries.Intersect(pc.propertyList)); OR BETTER:
            //PartClasses.Where(pc => entryIDs.Intersect(pc.propertyList.Select(p => p.EntryID)));
            foreach (Entry entry in groups) {
                EntryVM entryVM = EntryVM.WrapInCorrectVM(entry, DbContext);
                FirstGenEntryVMs.Add(entryVM);
                AllEntryVMs.Add(entryVM);
            }
            return groups;
        }

        public override void showLevels<TE, TP, TI>(IQueryable<TE> filteredEntries) {
            string groupPropertyName = GroupPanelVM.SelectedItem as string;
            string structurePropertyName = StructurePanelVM.SelectedItem as string;
            Type type = (SystemTypePanelVM.SelectedItem as CJTType).EntryType;
            IQueryable<TE> filteredParents = null;
            if (GroupPanelVM.IsApplied == true) {
                IQueryable<TP> groups = showGroups<TI, TP>(filteredEntries as IQueryable<TI>, groupPropertyName);
                if (groupPropertyName != null)
                    filteredParents = showMoreLevels<TE, TP>(filteredEntries, groups, groupPropertyName);
            }
            else if (GroupPanelVM.IsApplied == false) {
                filteredParents = showFirstLevel<TE>(filteredEntries, structurePropertyName);
                //Expression<Func<T, bool>> expression = PropertyNotNull<T>(columnName); //Won't work with relations
                for (int gen = 2; gen <= 3; gen++) {
                    if (structurePropertyName != null)
                        filteredParents = showMoreLevels<TE,TE>(filteredEntries, filteredParents, structurePropertyName);
                }
            }
        }

        protected IQueryable<T> showFirstLevel<T>(IQueryable<T> entries, string columnName) where T : Entry {
            //IEnumerable<int> filterTagIDs = Filter.SelectedObjects.Select(o => (o as Tag).TagID);
            IQueryable<T> filteredParents = entries;
            //CJTType selectedType = NRPanelVM.SelectedItem as CJTType;
            if (columnName != null)
                filteredParents = entries.Where(PropertyIsEmpty<T>(columnName));
            //NOTE! ALL PROPERTIES SHOULD BE COLLECTIONS!
            //IF YOU WANT LESS, THEN ADD CONSTRAINTS IN THE ENTRYVM! OR MODEL! OR SOMEWHERE!
            //filteredParents = entries.Where(e => filterTagIDs.Except(e.Parent.Tags.Select(t => t.TagID)).Any());
            //WHERE it has NO parent that matches the filter.
            int count0 = entries.Count();
            int count = filteredParents.Count();
            foreach (T entry in filteredParents) {
                EntryVM entryVM = EntryVM.WrapInCorrectVM(entry, DbContext);
                //SHOULD make this into Constructor really.
                FirstGenEntryVMs.Add(entryVM);
                AllEntryVMs.Add(entryVM); //CURRENT PROBLEM! Code does not reach this line! REVISIT!
            }
            return filteredParents;
        }

        public IQueryable<TE> showMoreLevels<TE, TP>
            (IQueryable<TE> entries, IQueryable<TP> filteredParents, string columnName) 
            where TE : Entry where TP : Entry {
            IQueryable<TE> filteredChildren = entries;
            IEnumerable<int> parentEntryIDs = filteredParents.Select(e => e.EntryID); //groups, tags.
            filteredChildren = entries.Where
                (EnumIntersectsPropertyEntryIDs<TE,TP>(parentEntryIDs, columnName));
            //WHY is this only returning "order groceries"? it should also return finish program.
            //NOTE! innefficiency above, in that it does this for parents too...
            //perhaps in previous cycle, could REMOVE parents. once placed. BUT then they
            //only show once. MIGHT not want that.
            foreach (TE child in filteredChildren) {
                IEnumerable<EntryVM> parentVMs = AllEntryVMs;
                parentVMs = AllEntryVMs
                        .Where(EntryIsContainedInChildsProperty<TE,TP>(child, columnName).Compile());
                //OH YEAH! Queryables can take expressions, Enumerables must take delegates!
                EntryVM childVM = EntryVM.WrapInCorrectVM(child, DbContext);
                foreach (EntryVM parentVM in parentVMs) { 
                    parentVM.Children.Add(childVM);
                    childVM.Parents.Add(parentVM);
                }
                AllEntryVMs.Add(childVM);
            }
            return filteredChildren;
        }

        public override void RefreshView() {
            FirstGenEntryVMs.Clear();
            AllEntryVMs.Clear();
            Type entryType = (SystemTypePanelVM.SelectedItem as CJTType).EntryType;
            string groupPropertyName = GroupPanelVM.SelectedItem as string;
            string structurePropertyName = StructurePanelVM.SelectedItem as string;
            if (groupPropertyName != null) {
                Attribute attribute = entryType.GetProperty(groupPropertyName)
                    .GetCustomAttribute(typeof(InversePropertyAttribute)); //attribute of Task.
                string inversePropertyName = (attribute as InversePropertyAttribute).Property; //prop of Tag

                string propertyName = GroupPanelVM.SelectedItem;
                Type propertyType = entryType.GetProperty(propertyName)
                    .PropertyType.GetGenericArguments()[0];
                //WANT to pass, TYPE of inverseProperty.
                Type inverseType = propertyType.GetProperty(inversePropertyName)
                    .PropertyType.GetGenericArguments()[0];
                MethodInfo method = typeof(EntriesTreeVM).GetMethod("showLevels");
                //NEED to give type, so it knows about more specific properties.
                //Maybe, need to give typeof(Entry), at certain times...
                //MAYBE, if propertyType, 
                //MAYBE must get inversePropName/inverseProp HERE,,
                //Then, get its type.
                //THEN pass type in below! Seems good!
                MethodInfo generic = method.MakeGenericMethod(new Type[] { entryType, propertyType, inverseType });
                generic.Invoke(this, new object[] { FilteredEntries });
            }
        }

        public override void UpdateEntries() { //Update and refresh are synonyms i think.
            FilteredEntries = getFilteredEntries();
            //GROUPING SETTINGS
            CJTType type = SystemTypePanelVM.SelectedItem; //defaults to first in list. (partInstance)
            ICollection<Property> properties = type.Properties;
            GroupPanelVM.Items.Clear();
            StructurePanelVM.Items.Clear();
            foreach (Property property in properties) {
                if (property.Type == InfoType.LinkedTextBlock
                    || property.Type == InfoType.ListBox) {
                    //above separates primitive props from entry props.
                    //REFLECTION CALL HERE!
                    MethodInfo method = typeof(EntriesTreeVM).GetMethod("UpdateStructureVMs");
                    PropertyInfo pi = type.EntryType.GetProperty(property.Name);
                    Type propertyType = pi.PropertyType.GetGenericArguments()[0];
                    MethodInfo generic = method.MakeGenericMethod
                        (new Type[] { type.EntryType, propertyType });
                    generic.Invoke(this, new object[] { FilteredEntries, property });
                }
            }
            if (GroupPanelVM.Items.Count > 0) {
                GroupPanelVM.SelectedItem = GroupPanelVM.Items.First();
            }
            if (StructurePanelVM.Items.Count > 0) {
                StructurePanelVM.SelectedItem = StructurePanelVM.Items.First(); //partC, comes up as empty? CURRENT
            }
            RefreshView();
        }

        public void UpdateStructureVMs<TE,TP>(IQueryable<TE> filteredEntries, Property property) {
            bool x = filteredEntries.Any(PropertyIsNotEmpty<TE,TP>(property.Name));
            bool anyEntriesHaveCollectionCountPos = x;
            if (anyEntriesHaveCollectionCountPos) {
                //WELL some ARE empty. i.e. partClass HAS no inheritance parent.
                //HENCE can't even show one level!
                GroupPanelVM.Items.Add(property.Name);
                if (property.IsSameTypeAsEntry)
                    StructurePanelVM.Items.Add(property.Name);
            }
        }
    }
}