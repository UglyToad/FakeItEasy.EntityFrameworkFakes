namespace UglyToad.FakeItEasy.EntityFrameworkFakes
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    using global::FakeItEasy;

    public class ContextFaker
    {
        public static T CreateContext<T>() where T : DbContext
        {
            return A.Fake<T>();
        }

        public static void ContextReturnsDbSet<T>(Expression<Func<DbSet<T>>> dbSetAccessor) where T : class
        {
            ContextReturnsDbSet(dbSetAccessor, null);
        }

        public static void ContextReturnsDbSet<T>(Expression<Func<DbSet<T>>> dbSetAccessor, List<T> data)
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
               builder.Implements(typeof(IDbAsyncEnumerable<T>));
               builder.Implements(typeof(IQueryable<T>));
           });
        }

        private static void SetUpFakeDbSetBehaviour<T>(DbSet<T> dbSet, List<T> data) where T : class
        {
            SetUpAsyncQueryingForFakeDbSet(dbSet, data);

            SetUpAddForFakeDbSet(dbSet, data);

            SetUpRemoveForDbSet(dbSet, data);

            A.CallTo(() => dbSet.Find(A<object[]>.Ignored)).Returns(dbSet.FirstOrDefault());
        }

        private static void SetUpAsyncQueryingForFakeDbSet<T>(DbSet<T> dbSet, ICollection<T> data) where T : class
        {
            A.CallTo(() => ((IDbAsyncEnumerable<T>)dbSet).GetAsyncEnumerator())
                .ReturnsLazily(() => new TestDbAsyncEnumerator<T>(data.AsQueryable().GetEnumerator()));

            A.CallTo(() => ((IQueryable<T>)dbSet).Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.AsQueryable().Provider));

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
    }
}
