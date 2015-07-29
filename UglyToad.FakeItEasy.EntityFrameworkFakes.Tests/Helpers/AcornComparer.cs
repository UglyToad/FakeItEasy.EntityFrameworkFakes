namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests.Helpers
{
    using System.Collections.Generic;
    using Domain;

    internal class AcornComparer : IEqualityComparer<Acorn>
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