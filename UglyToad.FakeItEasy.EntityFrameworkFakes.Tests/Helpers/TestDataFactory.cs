namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests.Helpers
{
    using System.Collections.Generic;
    using Domain;

    internal static class TestDataFactory
    {
        public static IList<Acorn> AcornTestData => new[]
        {
            new Acorn
            {
                Id = 1,
                NutritionValue = 5
            },
            new Acorn
            {
                Id = 2,
                NutritionValue = 6
            }
        };
    }
}
