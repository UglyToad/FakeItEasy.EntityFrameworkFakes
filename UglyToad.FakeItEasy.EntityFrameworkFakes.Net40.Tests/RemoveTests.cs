namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Net40.Tests
{
    using System.Linq;
    using EntityFrameworkFakes.Tests.DataAccess;
    using EntityFrameworkFakes.Tests.Domain;
    using EntityFrameworkFakes.Tests.Helpers;
    using Xunit;

    public class RemoveTests
    {
        private readonly TestContext context;
        private readonly int initialCount = TestDataFactory.AcornTestData.Count;

        public RemoveTests()
        {
            context = ContextFaker.CreateContext<TestContext>();

            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());
        }

        [Fact]
        public void CanRemove()
        {
            context.Acorns.Remove(context.Acorns.First());

            Assert.Equal(initialCount - 1, context.Acorns.Count());
        }

        [Fact]
        public void CanRemoveTwice()
        {
            var toRemove = context.Acorns.First();

            context.Acorns.Remove(toRemove);
            context.Acorns.Remove(toRemove);

            var acorns = context.Acorns.ToList();

            Assert.Equal(initialCount - 1, context.Acorns.Count());
        }

        [Fact]
        public void CanRemoveTwoEntries()
        {
            context.Acorns.Remove(context.Acorns.First());
            context.Acorns.Remove(context.Acorns.First());

            Assert.Equal(initialCount - 2, context.Acorns.Count());
        }

        [Fact]
        public void CanAddThenRemoveEntry()
        {
            var acorn = new Acorn {Id = 0, NutritionValue = 25};

            context.Acorns.Add(acorn);
            context.Acorns.Remove(acorn);

            Assert.Equal(initialCount, context.Acorns.Count());
        }

        [Fact]
        public void CanAddThenRemoveThenAddAgain()
        {
            var acorn = new Acorn {Id = 0, NutritionValue = 25};

            context.Acorns.Add(acorn);
            context.Acorns.Remove(acorn);
            context.Acorns.Add(acorn);

            Assert.Contains(acorn, context.Acorns.ToList());
        }

        [Fact]
        public void RemoveReturnsExpectedResult()
        {
            var toRemove = context.Acorns.First();

            var result = context.Acorns.Remove(toRemove);

            Assert.Equal(initialCount - 1, context.Acorns.Count());
            Assert.Equal(toRemove, result);
        }

        [Fact]
        public void CanRemoveRange()
        {
            var removeRange = context.Acorns.ToArray();

            context.Acorns.RemoveRange(removeRange);

            Assert.Empty(context.Acorns.ToList());
        }

        [Fact]
        public void RemoveReturnsExpectedResults()
        {
            var removeRange = context.Acorns.ToList();

            var result = context.Acorns.RemoveRange(removeRange);

            Assert.Equal(removeRange, result);
        }
    }
}
