namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Helpers;
    using Xunit;

    public class ContextAddTests
    {
        private readonly TestContext context;
        private readonly List<Acorn> acorns;
        private readonly Acorn acorn1;
        private readonly Acorn acorn2;

        public ContextAddTests()
        {
            context = ContextFaker.CreateContext<TestContext>();
            acorns = TestDataFactory.AcornTestData.ToList();

            ContextFaker.ContextReturnsDbSet(() => context.Acorns, acorns);

            acorn1 = new Acorn
            {
                Id = 3,
                NutritionValue = 5
            };

            acorn2 = new Acorn
            {
                Id = 4,
                NutritionValue = 7
            };
        }

        [Fact]
        public void CanAddDbSetObjects()
        {
            var initialCount = acorns.Count;

            context.Acorns.Add(acorn1);

            var result = context.Acorns.ToArray();

            Assert.Equal(initialCount + 1, result.Length);
        }

        [Fact]
        [Trait("ParallelPair", "true")]
        public void CanAddThenQueryObjects()
        {
            context.Acorns.Add(acorn1);

            Assert.Equal(acorn1.NutritionValue, context.Acorns.Single(a => a.Id == acorn1.Id).NutritionValue);
        }

        [Fact]
        public void CanAddMultipleObjects()
        {
            context.Acorns.Add(acorn1);
            context.Acorns.Add(acorn2);

            Assert.Contains(acorn1, context.Acorns);
            Assert.Contains(acorn2, context.Acorns);
        }

        [Fact]
        public async Task CanAddThenAsyncQueryObjects()
        {
            context.Acorns.Add(acorn1);

            var result = await context.Acorns.SingleAsync(a => a.NutritionValue == acorn1.NutritionValue
            && a.Id == acorn1.Id);

            Assert.Equal(acorn1, result);
        }

        [Fact]
        public void CanAddRange()
        {
            context.Acorns.AddRange(new[] {acorn1, acorn2});

            Assert.Contains(acorn1, context.Acorns);
            Assert.Contains(acorn2, context.Acorns);
        }

        [Fact]
        public async Task CanAddAndQueryRangeAsync()
        {
            context.Acorns.AddRange(new[] {acorn1, acorn2});

            var result = await context.Acorns.Where(a => a.Id == acorn1.Id || a.Id == acorn2.Id).ToListAsync();

            Assert.Contains(acorn1, result);
            Assert.Contains(acorn2, result);
        }

        [Fact]
        public void AddReturnsExpectedRecord()
        {
            var result = context.Acorns.Add(acorn1);

            Assert.Equal(acorn1, result);
        }

        [Fact]
        public void AddRangesReturnsExpectedEnumerable()
        {
            var result = context.Acorns.AddRange(new[] {acorn1, acorn2});

            Assert.Equal(new[] {acorn1, acorn2}, result, new AcornComparer());
        }

        [Fact]
        public void AttachAddsRecord()
        {
            context.Acorns.Attach(acorn1);

            Assert.Contains(acorn1, context.Acorns);
        }

        [Fact]
        public void AttachTwiceOnlyAddsOnce()
        {
            context.Acorns.Attach(acorn1);
            context.Acorns.Attach(acorn1);

            Assert.Single(context.Acorns, acorn1);
        }

        [Fact]
        public void AttachReturnsExpectedRecord()
        {
            var result = context.Acorns.Attach(acorn1);
            Assert.Equal(acorn1, result);
        }

        [Fact]
        public void AddDoesNotDuplicateRecords()
        {
            context.Acorns.Add(acorn1);
            context.Acorns.Add(acorn1);

            Assert.Single(context.Acorns, acorn1);
        }

        [Fact]
        public void CanUseAsIDisposable()
        {
            using (context)
            {
                context.Acorns.Add(acorn1);
                context.SaveChanges();
            }

            Assert.Single(context.Acorns, acorn1);
        }
    }
}
