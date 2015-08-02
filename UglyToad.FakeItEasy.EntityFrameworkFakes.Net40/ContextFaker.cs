namespace UglyToad.FakeItEasy.EntityFrameworkFakes
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using global::FakeItEasy;

    /// <summary>
    /// Class containing utility methods for faking the EntityFramework DbContext and DbSet.
    /// </summary>
    public class ContextFaker
    {
        private static ConcurrentDictionary<Type, object> idGetters =
            new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Create a fake context of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the context to fake.</typeparam>
        /// <returns>The faked DbContext of type T.</returns>
        public static T CreateContext<T>() where T : DbContext
        {
            return A.Fake<T>();
        }

        /// <summary>
        /// Configure the property to access when calling DbSet.Find() for a given type of entity.
        /// </summary>
        /// <typeparam name="T">The type of entity this find method corresponds to.</typeparam>
        /// <param name="func">The func for accessing the property of an entity.</param>
        public static void AddIdGetterForType<T>(Func<T, object> func) where T : class
        {
            idGetters.TryAdd(typeof(T), func);
        }

        /// <summary>
        /// Configure a fake DbSet for the given context DbSet. The DbSet uses the data passed in.
        /// </summary>
        /// <typeparam name="T">The type of the entity for this DbSet.</typeparam>
        /// <param name="dbSetAccessor">The func for accessing the DbSet from the context.</param>
        /// <param name="data">The list of data this DbSet will use for add/remove/query operations. Be aware this list will be modified by operations against the DbSet.</param>
        public static void ContextReturnsDbSet<T>(Expression<Func<DbSet<T>>> dbSetAccessor, List<T> data = null)
            where T : class
        {
            if (data == null)
            {
                data = new List<T>();
            }

            var fakeSet = GetFakeDbSet<T>();

            SetUpFakeDbSetBehaviour(fakeSet, data);

            A.CallTo(dbSetAccessor).Returns(fakeSet);
        }

        private static DbSet<T> GetFakeDbSet<T>() where T : class
        {
            return A.Fake<DbSet<T>>(builder =>
            {
                builder.Implements(typeof(IQueryable<T>));
            });
        }

        private static void SetUpFakeDbSetBehaviour<T>(DbSet<T> dbSet, List<T> data) where T : class
        {
            SetUpAsyncQueryingForFakeDbSet(dbSet, data);

            SetUpAddForFakeDbSet(dbSet, data);

            SetUpRemoveForDbSet(dbSet, data);

            SetUpFindForDbSet(dbSet, data);

            SetUpIncludeForDbSet(dbSet, data);
        }

        private static void SetUpAsyncQueryingForFakeDbSet<T>(DbSet<T> dbSet, List<T> data) where T : class
        {
            A.CallTo(() => ((IQueryable<T>)dbSet).Provider)
                .Returns(data.AsQueryable().Provider);

            A.CallTo(() => ((IQueryable<T>)dbSet).Expression).Returns(data.AsQueryable().Expression);
            A.CallTo(() => ((IQueryable<T>)dbSet).ElementType).Returns(data.AsQueryable().ElementType);
            A.CallTo(() => ((IQueryable<T>)dbSet).GetEnumerator()).ReturnsLazily(() => data.AsQueryable().GetEnumerator());
        }

        private static void SetUpAddForFakeDbSet<T>(DbSet<T> dbSet, List<T> data) where T : class
        {
            A.CallTo(() => dbSet.Add(A<T>.Ignored))
                .Invokes((T item) =>
                {
                    // Add is a no-op if the entity is already in the context in the Added state.
                    if (!data.Contains(item))
                    {
                        data.Add(item);
                    }
                })
                .ReturnsLazily((T item) => item);

            A.CallTo(() => dbSet.AddRange(A<IEnumerable<T>>.Ignored))
                .Invokes((IEnumerable<T> items) => data.AddRange(items))
                .ReturnsLazily((IEnumerable<T> items) => items);

            A.CallTo(() => dbSet.Attach(A<T>.Ignored)).Invokes((T item) =>
            {
                // Attach is a no-op if the entity is already in the context in the Unchanged state.
                if (!data.Contains(item))
                {
                    data.Add(item);
                }
            }).ReturnsLazily((T item) => item);
        }

        private static void SetUpRemoveForDbSet<T>(DbSet<T> dbSet, List<T> data) where T : class
        {
            A.CallTo(() => dbSet.Remove(A<T>.Ignored))
                .Invokes((T item) => data.Remove(item))
                .ReturnsLazily((T item) => item);

            A.CallTo(() => dbSet.RemoveRange(A<IEnumerable<T>>.Ignored))
                .Invokes((IEnumerable<T> items) =>
                {
                    foreach (var item in items)
                    {
                        if (data.Contains(item))
                        {
                            data.Remove(item);
                        }
                    }
                }).ReturnsLazily((IEnumerable<T> items) => items);
        }

        private static void SetUpFindForDbSet<T>(DbSet<T> dbSet, List<T> data) where T : class
        {
            A.CallTo(() => dbSet.Find(A<object[]>.Ignored))
                .ReturnsLazily((object[] ids) =>
                {
                    var type = typeof(T);
                    Func<T, object> func;

                    if (ids.Length != 1)
                    {
                        // We do not support 
                        return null;
                    }

                    if (idGetters.ContainsKey(type))
                    {
                        func = (Func<T, object>)idGetters[type];
                    }
                    else
                    {
                        func = ReflectionHelper.GetId<T>();
                        idGetters.TryAdd(type, func);
                    }

                    if (func == null)
                    {
                        return null;
                    }

                    foreach (var item in data)
                    {
                        if (func(item).Equals(ids[0]))
                        {
                            return item;
                        }
                    }

                    return null;
                });
        }

        private static void SetUpIncludeForDbSet<T>(DbSet<T> dbSet, List<T> data) where T : class
        {
            A.CallTo(() => dbSet.Include(A<string>.Ignored)).Returns(dbSet);
        }
    }
}
