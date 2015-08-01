namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests.Domain
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Acorn")]
    public class Acorn
    {
        public int Id { get; set; }

        public int NutritionValue { get; set; }
    }
}