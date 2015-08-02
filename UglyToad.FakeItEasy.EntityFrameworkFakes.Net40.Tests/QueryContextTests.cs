namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Net40.Tests
{
    using System;
    using System.Linq;
    using EntityFrameworkFakes.Tests.DataAccess;
    using EntityFrameworkFakes.Tests.Helpers;
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
        public void CanQueryDbSetWithObjects()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            Assert.Equal(TestDataFactory.AcornTestData, context.Acorns.ToArray(), Comparer);
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
        public void SingleNotPresentThrowsCorrectException()
        {
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());

            Assert.Throws<InvalidOperationException>(() => context.Acorns.Single(a => a.Id == int.MaxValue));
        }
    }
}