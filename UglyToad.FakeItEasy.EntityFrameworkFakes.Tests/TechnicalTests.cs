namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using DataAccess;
    using Domain;
    using global::FakeItEasy;
    using Xunit;

    public class TechnicalTests
    {
        [Fact]
        public void CallDbSetFakeDirectly()
        {
            var context = ContextFaker.CreateContext<TestContext>();

            var data = new Acorn[] { };

            IQueryable<Acorn> dataQueryable = data.AsQueryable();
            IEnumerator<Acorn> dataEnumerator = dataQueryable.GetEnumerator();

            var mockSet = A.Fake<DbSet<Acorn>>(builder =>
            {
                builder.Implements(typeof(IDbAsyncEnumerable<Acorn>));
                builder.Implements(typeof(IQueryable<Acorn>));
            });

            var mockEnumerable = (IDbAsyncEnumerable<Acorn>)mockSet;

            A.CallTo(() => (mockEnumerable).GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Acorn>(dataEnumerator));

            A.CallTo(() => ((IQueryable<Acorn>)mockSet).Provider)
                .Returns(new TestDbAsyncQueryProvider<Acorn>(dataQueryable.Provider));

            A.CallTo(() => ((IQueryable<Acorn>)mockSet).Expression).Returns(dataQueryable.Expression);
            A.CallTo(() => ((IQueryable<Acorn>)mockSet).ElementType).Returns(dataQueryable.ElementType);
            A.CallTo(() => ((IQueryable<Acorn>)mockSet).GetEnumerator()).Returns(dataEnumerator);

            A.CallTo(() => context.Acorns).Returns(mockSet);

            Assert.Empty(context.Acorns.ToArray());
        }
    }
}
