namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests
{
    using System.Linq;
    using DataAccess;
    using Domain;
    using Helpers;
    using Xunit;

    public class ContextAddTests
    {
        private TestContext context;

        public ContextAddTests()
        {
            this.context = ContextFaker.CreateContext<TestContext>();
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
    }
}
