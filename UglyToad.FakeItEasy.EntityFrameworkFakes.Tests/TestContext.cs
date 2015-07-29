namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests
{
    using System.Data.Entity;

    public class TestContext : DbContext
    {
        public virtual DbSet<Squirrel> Squirrels { get; set; }

        public virtual DbSet<Acorn> Acorns { get; set; }

        public TestContext() : base("default")
        {
            Database.SetInitializer<TestContext>(null);
        }
    }
}
