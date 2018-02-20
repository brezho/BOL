using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Reflection;
using X.Repository.Attributes;
using System.Dynamic;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace X.Repository.Analysis
{
    static class Analyser
    {
        static ConcurrentDictionary<string, List<PropertyInfo>> AllPropertiesByType = new ConcurrentDictionary<string, List<PropertyInfo>>();
        static ConcurrentDictionary<string, List<PropertyInfo>> KeyMembersByType = new ConcurrentDictionary<string, List<PropertyInfo>>();
        static ConcurrentDictionary<string, List<PropertyInfo>> ReadOnlyMembersByType = new ConcurrentDictionary<string, List<PropertyInfo>>();
        static ConcurrentDictionary<string, List<PropertyInfo>> IgnoreMembersByType = new ConcurrentDictionary<string, List<PropertyInfo>>();

        public static SelectCommand<T> BuildSelectCommand<T>(Expression<Func<T, bool>> whereClause)
        {
            var result = new SelectCommand<T>();
            result.Where = whereClause;

            var objectType = typeof(T);

            // var allProperties = GetAllMembers(objectType);
            //  var propertiesToIgnore = GetIgnoreMembers(objectType);

            var propertiesToSelect = GetAllMembers(objectType)
                .Where(p =>
                    !GetIgnoreMembers(objectType).Contains(p)
                ).Select(x => x.Name);


            var properties = DynamicsCache
                .GetPropertiesAccessors<T>()
                .Where(x => propertiesToSelect.Contains(x.PropertyName))
                .Select(x => x.PropertyName);

            result.Projection = properties.ToList();

            return result;
        }

        public static InsertManyCommand<T> BuildInsertManyCommand<T>(IEnumerable<T> objList)
        {

            // Write in repository
            // every property which is not marked with Ignore or ReadOnly attribute

            if (objList == null) return null;
            var result = new InsertManyCommand<T>();


            var objectType = typeof(T);


            //var propertiesToIgnore = new List<PropertyInfo>();
            //propertiesToIgnore.AddRange(GetIgnoreMembers(objectType));
            //propertiesToIgnore.AddRange(GetReadOnlyMembers(objectType));


            var propertiesToSet = GetAllMembers(objectType)
                .Where(p =>
                    !GetIgnoreMembers(objectType).Contains(p)
                    && !GetReadOnlyMembers(objectType).Contains(p)
                ).Select(x => x.Name);




            var properties = DynamicsCache
                .GetPropertiesAccessors<T>()
                .Where(x => propertiesToSet.Contains(x.PropertyName));


            foreach (var obj in objList)
            {
                var insert = new InsertCommand<T>();
                foreach (var x in properties)
                {
                    insert.Values.Add(x.PropertyName, x.Getter.Invoke(obj));
                }
                result.InsertComands.Add(insert);
            }

            return result;
        }

        public static SaveCommand<T> BuildSaveCommand<T>(T obj)
        {
            // Write in repository
            // every property which is not marked with Ignore or ReadOnly attribute

            if (obj == null) return null;
            var result = new SaveCommand<T>();
            var objectType = obj.GetType();

            var keyPropertyNames = GetKeyMembers(objectType);

            var columnsForWhereClause = DynamicsCache
                .GetPropertiesAccessors<T>()
                .Where(x => keyPropertyNames.Select(p => p.Name).Contains(x.PropertyName));


            var propertiesToSet = GetAllMembers(objectType)
                .Where(p =>
                    !GetIgnoreMembers(objectType).Contains(p)
                    && !GetReadOnlyMembers(objectType).Contains(p)
                    && !GetKeyMembers(objectType).Contains(p)
                ).Select(x => x.Name);


            var columnsToSet = DynamicsCache.GetPropertiesAccessors<T>()
                .Where(x => propertiesToSet.Contains(x.PropertyName));

            foreach (var x in columnsToSet)
            {
                result.ColumnValues[x.PropertyName] = x.Getter.Invoke(obj);
            }

            foreach (var x in columnsForWhereClause)
            {
                result.KeyValues[x.PropertyName] = x.Getter.Invoke(obj);
            }

            return result;
        }


        public static InsertCommand<T> BuildInsertCommand<T>(T obj)
        {
            // Write in repository
            // every property which is not marked with Ignore or ReadOnly attribute

            if (obj == null) return null;
            var result = new InsertCommand<T>();

            var objectType = obj.GetType();

            var propertiesToIgnore = new List<PropertyInfo>();
            propertiesToIgnore.AddRange(GetIgnoreMembers(objectType));
            propertiesToIgnore.AddRange(GetReadOnlyMembers(objectType));


            var properties = DynamicsCache
                .GetPropertiesAccessors<T>()
                .Where(x => !propertiesToIgnore.Select(p => p.Name).Contains(x.PropertyName));

            foreach (var x in properties)
            {
                result.Values.Add(x.PropertyName, x.Getter.Invoke(obj));
            }

            return result;
        }

        public static UpdateCommand<T> BuildUpdateCommand<T>(T obj)
        {
            // Write in repository
            // every property which is not marked with Ignore or ReadOnly attribute

            if (obj == null) return null;
            var result = new UpdateCommand<T>();

            var objectType = obj.GetType();

            var keyPropertyNames = GetKeyMembers(objectType);

            var columnsForWhereClause = DynamicsCache
                .GetPropertiesAccessors<T>()
                .Where(x => keyPropertyNames.Select(p => p.Name).Contains(x.PropertyName));

            //var propertyNamesToIgnore = new List<PropertyInfo>();
            //propertyNamesToIgnore.AddRange(GetIgnoreMembers(objectType));
            //propertyNamesToIgnore.AddRange(GetReadOnlyMembers(objectType));
            //propertyNamesToIgnore.AddRange(keyPropertyNames);

            var propertiesToSet = GetAllMembers(objectType)
                .Where(p =>
                    !GetIgnoreMembers(objectType).Contains(p)
                    && !GetReadOnlyMembers(objectType).Contains(p)
                    && !GetKeyMembers(objectType).Contains(p)
                ).Select(x => x.Name);

            var columnsToSet = DynamicsCache
                .GetPropertiesAccessors<T>()
                .Where(x => propertiesToSet.Contains(x.PropertyName));

            foreach (var x in columnsToSet)
            {
                result.Set.Add(x.PropertyName, x.Getter.Invoke(obj));
            }


            var equalities = new List<Expression>();
            var parameter = Expression.Parameter(objectType, "x");
            foreach (var k in columnsForWhereClause)
            {
                var property = objectType.GetProperty(k.PropertyName);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var constantValue = Expression.Constant(k.Getter.Invoke(obj));
                var equality = Expression.Equal(propertyAccess, constantValue);
                equalities.Add(equality);
            }

            if (equalities.Count() == 1)
            {
                result.Where = Expression.Lambda<Func<T, bool>>(equalities.First(), parameter);
            }
            else
            {
                var eq = equalities.First();
                Expression and = null;
                for (int i = 1; i < equalities.Count; i++)
                {
                    if (i == 1) and = Expression.AndAlso(eq, equalities[i]);
                    else and = Expression.AndAlso(and, equalities[i]);
                }
                result.Where = Expression.Lambda<Func<T, bool>>(and, parameter);
            }

            return result;
        }

        public static DeleteCommand<T> BuildDeleteCommand<T>(Expression<Func<T, bool>> whereClause)
        {
            return new DeleteCommand<T> { Where = whereClause };
        }


        public static DeleteAllCommand<T> BuildDeleteAllCommand<T>()
        {
            return new DeleteAllCommand<T>();
        }

        public static DeleteCommand<T> BuildDeleteCommand<T>(T obj)
        {
            if (obj == null) return null;
            var result = new DeleteCommand<T>();

            var objectType = obj.GetType();

            var keyPropertyNames = GetKeyMembers(objectType);

            var keysUsedInDelete = DynamicsCache
                .GetPropertiesAccessors<T>()
                .Where(x => keyPropertyNames.Select(p => p.Name).Contains(x.PropertyName))
                .ToList();



            var equalities = new List<Expression>();
            var parameter = Expression.Parameter(objectType, "x");
            keysUsedInDelete.ForEach(k =>
            {
                var property = objectType.GetProperty(k.PropertyName);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var constantValue = Expression.Constant(k.Getter.Invoke(obj));
                var equality = Expression.Equal(propertyAccess, constantValue);
                equalities.Add(equality);
            });

            if (equalities.Count() == 1)
            {
                result.Where = Expression.Lambda<Func<T, bool>>(equalities.First(), parameter);
            }
            else
            {
                var eq = equalities.First();
                Expression and = null;
                for (int i = 1; i < equalities.Count; i++)
                {
                    if (i == 1) and = Expression.AndAlso(eq, equalities[i]);
                    else and = Expression.AndAlso(and, equalities[i]);
                }
                result.Where = Expression.Lambda<Func<T, bool>>(and, parameter);
            }

            return result;
        }



        public static IEnumerable<PropertyInfo> GetAllMembers(Type t)
        {
            return AllPropertiesByType.GetOrAdd(t.FullName, x =>
            {
                return t
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .ToList();
            });
        }

        public static IEnumerable<PropertyInfo> GetKeyMembers(Type t)
        {
            return KeyMembersByType.GetOrAdd(t.FullName, x =>
            {
                var res = t
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(p =>
                        p.GetCustomAttributes(typeof(KeyAttribute), true)
                        .Cast<KeyAttribute>()
                        .Select(y => y != null).Any()

                        )
                    //.Select(p => p.Namespace)
                    .ToList();

                if (res.Count == 0)
                {
                    res = t
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(p => p.Name == "Id" || p.Name == t.Name + "Id")
                        //.Select(p => p.Namespace)
                        .ToList();
                }

                return res;
            });
        }

        public static IEnumerable<PropertyInfo> GetReadOnlyMembers(Type t)
        {
            return ReadOnlyMembersByType.GetOrAdd(t.FullName, x =>
            {

                //var tr = t
                //    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                //    .Where(p => p.GetSetMethod(true) == null);

                return t
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(p =>
                            p.GetCustomAttributes(typeof(CalculatedAttribute), true)
                                .Cast<CalculatedAttribute>()
                                .Select(y => y != null)
                                .Any()
                    ||
                    (p.GetSetMethod(true) == null)
                        )
                    //.Select(p => p.Namespace)
                    .ToList();
            });
        }

        public static IEnumerable<PropertyInfo> GetIgnoreMembers(Type t)
        {
            return IgnoreMembersByType.GetOrAdd(t.FullName, x =>
            {
                var res = t
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic)
                    .Where(p =>
                            p.GetCustomAttributes(typeof(NonPersistentAttribute), true)
                                .Cast<NonPersistentAttribute>()
                                .Select(y => y != null)
                                .Any()
                        )
                    //.Select(p => p.Namespace)
                    .ToList();

                return res;
            });
        }

    }
}
