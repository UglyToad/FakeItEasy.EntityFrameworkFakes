namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Net40.Tests
{
    using System.Data.Entity;
    using System.Linq;
    using EntityFrameworkFakes.Tests.DataAccess;
    using EntityFrameworkFakes.Tests.Helpers;
    using Xunit;

    public class IncludeTests
    {
        private readonly TestContext context;

        public IncludeTests()
        {
            context = ContextFaker.CreateContext<TestContext>();

            ContextFaker.ContextReturnsDbSet(() => context.Acorns, TestDataFactory.AcornTestData.ToList());
        }

        [Fact]
        public void IncludeDoesNothing()
        {
            var result = context.Acorns.Include("Some thing").ToList();

            Assert.Equal(TestDataFactory.AcornTestData.Count, result.Count);
        }

        [Fact]
        public void IncludeDoesNothingAfterWhere()
        {
            var result = context.Acorns.Where(a => a.Id == 2).Include("oops").ToList();

            Assert.Equal(1, result.Count);
        }
    }
}
