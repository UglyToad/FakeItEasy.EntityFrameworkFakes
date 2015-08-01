namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Helpers;
    using Xunit;

    public class QueryContextTests
    {
        private static readonly AcornComparer Comparer = new AcornComparer();
        private readonly TestContext context;

        public QueryContextTests()
        {
            context = ContextFaker.CreateContext<TestContext>();
        }

        [Fact]
        public void CanCreateATestContext()
        {
            var createdContext = ContextFaker.CreateContext<TestContext>();

            Assert.NotNull(createdContext);
        }

        [Fact]
        public async Task CanQueryAnEmptyDbSet()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, new List<Acorn>());

            Assert.Empty(await context.Acorns.ToListAsync());
        }

        [Fact]
        [Trait("ParallelPair", "true")]
        public void CanQueryDbSetWithObjects()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            Assert.Equal(TestDataFactory.AcornTestData, context.Acorns.ToArray(), Comparer);
        }

        [Fact]
        public async Task CanQueryDbSetWithObjectsAsync()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            var result = await context.Acorns.ToListAsync();

            Assert.Equal(TestDataFactory.AcornTestData, result, Comparer);
        }

        [Fact]
        public void PassNullListTreatsAsEmptyList()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, null);

            Assert.Empty(context.Acorns);
        }

        [Fact]
        public void SupportsLinqQuerying()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            var result = context.Acorns.Where(a => (a.Id + a.NutritionValue)%2 == 0).OrderBy(a => a.Id);

            Assert.Equal(TestDataFactory.AcornTestData.Where(a => (a.Id + a.NutritionValue)%2 == 0).OrderBy(a => a.Id), result, Comparer);
        }

        [Fact]
        public async Task SupportsAsyncLinqQuerying()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            var result = await context.Acorns.Where(a => a.NutritionValue > 5).ToArrayAsync();

            Assert.Equal(TestDataFactory.AcornTestData.Where(a => a.NutritionValue > 5).OrderBy(a => a.Id), result.OrderBy(a => a.Id), Comparer);
        }

        [Fact]
        public void SingleNotPresentThrowsCorrectException()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            Assert.Throws<InvalidOperationException>(() => context.Acorns.Single(a => a.Id == int.MaxValue));
        }

        [Fact]
        public async Task SingleAsyncNotPresentThrowsCorrectException()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            await
                Assert.ThrowsAsync<InvalidOperationException>(
                    () => context.Acorns.SingleAsync(a => a.Id == int.MaxValue));
        }

        [Fact]
        public void FindWorksCorrectlyForFirstInList()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            var expected = TestDataFactory.AcornTestData.OrderBy(a => a.Id).First();

            var result = context.Acorns.Find(expected.Id);

            Assert.Equal(expected, result, Comparer);
        }

        [Fact]
        public void FindWorksCorrectlyForSecondInList()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            var expected = TestDataFactory.AcornTestData.OrderBy(a => a.Id).Skip(1).First();

            var result = context.Acorns.Find(expected.Id);

            Assert.Equal(expected, result, Comparer);
        }

        [Fact]
        public void FindReturnsNullCorrectly()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());
            
            var result = context.Acorns.Find(int.MaxValue);

            Assert.Null(result);
        }
    }
}