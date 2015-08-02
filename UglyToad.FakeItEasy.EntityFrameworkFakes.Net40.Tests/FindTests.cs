namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Net40.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EntityFrameworkFakes.Tests.DataAccess;
    using EntityFrameworkFakes.Tests.Domain;
    using EntityFrameworkFakes.Tests.Helpers;
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

        [Fact]
        public void CanFindWithCustomId()
        {
            var guid = new Guid("7C45F3DF-60B4-4E7D-A6EF-E76DFBA9E9B3");
            var energy = 10000000;

            ContextFaker.AddIdGetterForType((Photon photon) => photon.WavePacketIdentifier);
            ContextFaker.ContextReturnsDbSet(() => context.Photons,
                new List<Photon>(new[] {new Photon(guid, energy), new Photon(new Guid(), 13215516451)}));

            var result = context.Photons.Find(guid);

            Assert.Equal(energy, result.Energy);
        }
    }
}
