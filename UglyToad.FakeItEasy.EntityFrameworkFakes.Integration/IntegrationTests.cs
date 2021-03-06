﻿namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Integration
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Tests.DataAccess;
    using Tests.Domain;
    using Xunit;

    public class IntegrationTests
    {
        private readonly TestContext context;

        public IntegrationTests()
        {
            context = new TestContext();
        }

        [Fact]
        [Trait("Integration", "true")]
        public async Task TestCanRetrieveData()
        {
            var result = await context.Acorns.ToListAsync();

            Assert.NotEmpty(result);
        }

        [Fact]
        [Trait("Integration", "true")]
        public void AddTwiceOnlyAddsOnce()
        {
            var acorn = new Acorn
            {
                NutritionValue = 1
            };

            context.Acorns.Add(acorn);
            context.Acorns.Add(acorn);
            
            Assert.Single(context.Acorns.Local, a => a.NutritionValue == 1);
        }

        [Fact]
        [Trait("Integration", "true")]
        public void RemoveDoesNotThrow()
        {
            var toRemove = context.Acorns.First();

            context.Acorns.Remove(toRemove);
            context.Acorns.Remove(toRemove);
         
            Assert.NotEmpty(context.Acorns);
        }
    }
}
