namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests
{
    using System.Linq;
    using DataAccess;
    using Helpers;
    using Xunit;

    public class FindTests
    {
        private static readonly AcornComparer Comparer = new AcornComparer();
        private readonly TestContext context;

        public FindTests()
        {
            context = ContextFaker.CreateContext<TestContext>();
            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());
        }

        [Fact]
        public void FindWorksCorrectlyForFirstInList()
        {
            var expected = TestDataFactory.AcornTestData.OrderBy(a => a.Id).First();

            var result = context.Acorns.Find(expected.Id);

            Assert.Equal(expected, result, Comparer);
        }

        [Fact]
        public void FindWorksCorrectlyForSecondInList()
        {
            var expected = TestDataFactory.AcornTestData.OrderBy(a => a.Id).Skip(1).First();

            var result = context.Acorns.Find(expected.Id);

            Assert.Equal(expected, result, Comparer);
        }

        [Fact]
        public void FindReturnsNullCorrectly()
        {
            var result = context.Acorns.Find(int.MaxValue);

            Assert.Null(result);
        }
    }
}
