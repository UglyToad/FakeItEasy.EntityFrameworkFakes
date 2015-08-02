namespace UglyToad.FakeItEasy.EntityFrameworkFakes.Tests
{
    using System;
    using System.Collections.Generic;
    using Domain;
    using Helpers;
    using Xunit;

    public class ReflectionHelperTests
    {
        private readonly Acorn acorn = new Acorn {Id = 7, NutritionValue = 3};
        private readonly Squirrel squirrel = new Squirrel { Id = 9, Color = FurColor.Amber, Health = 10, Name = "Freddy"};

        [Fact]
        public void AddToListTwice()
        {
            var list = new List<Acorn>(TestDataFactory.AcornTestData);

            list.Add(acorn);
            list.Add(acorn);

            Assert.Equal(TestDataFactory.AcornTestData.Count + 2, list.Count);
        }

        [Fact]
        public void ReturnsIdProperty()
        {
            var func = ReflectionHelper.GetId<Acorn>();

            Assert.Equal(acorn.Id, func(acorn));
        }

        [Fact]
        public void ReturnsIdPropertySquirrel()
        {
            var func = ReflectionHelper.GetId<Squirrel>();

            Assert.Equal(squirrel.Id, func(squirrel));
        }

        [Fact]
        public void ReturnsIdPropertyTestClass()
        {
            var guid = new Guid("180287A2-67FE-42A6-9FE8-50C806E96C92");
            var testClass = new TestClass(guid);

            var func = ReflectionHelper.GetId<TestClass>();

            Assert.Equal(guid, func(testClass));
        }

        [Fact]
        public void ClassWithNoIdReturnsNullFunc()
        {
            var func = ReflectionHelper.GetId<NoIdClass>();

            Assert.Null(func);
        }

        private class TestClass
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public Guid TestClassId { get; private set; }

            public TestClass(Guid testClassId)
            {
                TestClassId = testClassId;
            }
        }

        private class NoIdClass
        { 
        }
    }
}
