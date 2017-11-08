using System;
using NUnit.Framework;
using System.Reflection;
using Task5.Model1;

namespace Task5.Tests
{
    [TestFixture]
    public class ContainerTests
    {
        private Container _container;
        private Assembly _model1Assembly;
        private Assembly _model2Assembly;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _model1Assembly = Assembly.Load("Task5.Model1");
            _model2Assembly = Assembly.Load("Task5.Model2");
        }

        [SetUp]
        public void SetUp()
        {
            _container = new Container();

            //_container.AddAssembly(assembly1);
            //_container.AddAssembly(assembly2);

            //_container.AddType(typeof(CustomerBLL));
            //_container.AddType(typeof(Logger));
            //_container.AddType(typeof(CustomerDAL), typeof(ICustomerDAL));
        }

        [Test]
        public void AddType_Logger_CreateLogger()
        {
            _container.AddType(typeof(Logger));

            var logger = _container.CreateInstance(typeof(Logger));

            Assert.True(logger is Logger);
        }
    }
}
