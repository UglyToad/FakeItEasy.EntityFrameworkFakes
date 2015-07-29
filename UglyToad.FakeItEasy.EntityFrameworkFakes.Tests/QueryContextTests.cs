namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using global::FakeItEasy;
    using Xunit;

    public class QueryContextTests
    {
        private readonly TestContext context;

        public QueryContextTests()
        {
            context = ContextFaker.CreateContext<TestContext>();
        }

        [Fact]
        public void CanCreateATestContext()
        {
            TestContext createdContext = ContextFaker.CreateContext<TestContext>();

            Assert.NotNull(createdContext);
        }

        [Fact]
        public async Task CanQueryAnEmptyDbSet()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, new List<Acorn>());

            Assert.Empty(await context.Acorns.ToListAsync());
        }

        [Fact]
        public void CallDbSetFakeDirectly()
        {
            var data = new Acorn[] { };

            IQueryable<Acorn> dataQueryable = data.AsQueryable();
            IEnumerator<Acorn> dataEnumerator = dataQueryable.GetEnumerator();

            var mockSet = A.Fake<DbSet<Acorn>>(builder =>
            {
                builder.Implements(typeof(IDbAsyncEnumerable<Acorn>));
                builder.Implements(typeof(IQueryable<Acorn>));
            });

            var mockEnumerable = (IDbAsyncEnumerable<Acorn>)mockSet;

            A.CallTo(() => (mockEnumerable).GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Acorn>(dataEnumerator));

            A.CallTo(() => ((IQueryable<Acorn>)mockSet).Provider)
                .Returns(new TestDbAsyncQueryProvider<Acorn>(dataQueryable.Provider));

            A.CallTo(() => ((IQueryable<Acorn>)mockSet).Expression).Returns(dataQueryable.Expression);
            A.CallTo(() => ((IQueryable<Acorn>)mockSet).ElementType).Returns(dataQueryable.ElementType);
            A.CallTo(() => ((IQueryable<Acorn>)mockSet).GetEnumerator()).Returns(dataEnumerator);

            A.CallTo(() => context.Acorns).Returns(mockSet);

            Assert.Empty(context.Acorns.ToArray());
        }

        [Fact]
        public void CanQueryDbSetWithObjects()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            Assert.Equal(TestDataFactory.AcornTestData, context.Acorns.ToArray(), new AcornComparer());
        }

        [Fact]
        public async Task CanQueryDbSetWithObjectsAsync()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            var result = await context.Acorns.ToListAsync();

            Assert.Equal(TestDataFactory.AcornTestData, result, new AcornComparer());
        }

        [Fact]
        public void CanAddDbSetObjects()
        {
            var acorns = TestDataFactory.AcornTestData.ToList();

            ContextFaker.ContextReturnsDbSet(() => context.Acorns, acorns);

            context.Acorns.Add(new Acorn
            {
                Id = 1,
                NutritionValue = 5
            });

            var result = context.Acorns.ToArray();
        }

        private class AcornComparer : IEqualityComparer<Acorn>
        {
            public bool Equals(Acorn x, Acorn y)
            {
                return x.Id == y.Id && x.NutritionValue == y.NutritionValue;
            }

            public int GetHashCode(Acorn obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
