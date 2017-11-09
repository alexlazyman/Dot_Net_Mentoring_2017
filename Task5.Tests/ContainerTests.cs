using System;
using System.Reflection;
using NUnit.Framework;
using Task5.Tests.Model1;

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
            _model1Assembly = Assembly.Load("Task5.Tests.Model1");
            _model2Assembly = Assembly.Load("Task5.Tests.Model2");
        }

        [SetUp]
        public void SetUp()
        {
            _container = new Container();
        }

        [Test]
        public void AddAssembly_NullType_ExpectedArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _container.AddAssembly(null);
            });
        }

        [Test]
        public void AddTypeWith1Param_NullType_ExpectedArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _container.AddType(null);
            });
        }

        [Test]
        public void AddTypeWith1Param_SampleType_CreateInstanceofSampleType()
        {
            _container.AddType(typeof(Sample));

            var instance = _container.CreateInstance(typeof(Sample));

            Assert.AreEqual(typeof(Sample), instance.GetType());
        }

        [Test]
        [TestCase(null, typeof(bool))]
        [TestCase(typeof(bool), null)]
        [TestCase(null, null)]
        public void AddTypeWith2Params_NullType_ExpectedArgumentNullException(Type type1, Type type2)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _container.AddType(type1, type2);
            });
        }

        [Test]
        [TestCase(typeof(ISample))]
        [TestCase(typeof(AbstractSample))]
        public void AddType_TypeIsNotInstantiatable_ExpectedArgumentException(Type type)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                _container.AddType(type);
            });
        }

        [Test]
        public void AddTypeWith2Params_1stIsNotSubclassOf2nd_ExpectedArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                _container.AddType(typeof(BaseSample), typeof(DerivedSample));
            });
        }

        [Test]
        public void AddTypeWith2Params_1stIsDerivedSampleAnd2ndIsBaseSample_CreateInstanceOfDerivedSampleByBaseSample()
        {
            _container.AddType(typeof(DerivedSample), typeof(BaseSample));

            var instance = _container.CreateInstance(typeof(BaseSample));

            Assert.AreEqual(typeof(DerivedSample), instance.GetType());
        }

        [Test]
        public void AddTypeWith2Params_1stIsSampleAnd2ndIsSample_CreateInstanceOfSample()
        {
            _container.AddType(typeof(Sample), typeof(Sample));

            var instance = _container.CreateInstance(typeof(Sample));

            Assert.AreEqual(typeof(Sample), instance.GetType());
        }

        [Test]
        public void AddAssembly_AssemblyHasClassWithExportAttribute_CreateInstanceOfThisOne()
        {
            _container.AddAssembly(_model1Assembly);

            var instance = _container.CreateInstance(typeof(ExportSample));

            Assert.AreEqual(typeof(ExportSample), instance.GetType());
        }

        [Test]
        public void AddAssembly_AssemblyHasClassWithContractExportAttribute_CreateInstanceOfThisOne()
        {
            _container.AddAssembly(_model1Assembly);

            var instance = _container.CreateInstance(typeof(IContractExportSample));

            Assert.AreEqual(typeof(ContractExportSample), instance.GetType());
        }

        [Test]
        public void AddAssembly_AssemblyHasClassWithImportConstructorAttribute_CreateInstanceOfThisOne()
        {
            _container.AddAssembly(_model1Assembly);

            var instance = _container.CreateInstance(typeof(ImportCtorSample));

            Assert.AreEqual(typeof(ImportCtorSample), instance.GetType());
        }

        [Test]
        public void CreateInstance_TypeHas2Constructors_ExpectedInvalidOperationException()
        {
            _container.AddType(typeof(SampleWith2Ctors));

            Assert.Throws<InvalidOperationException>(() =>
            {
                _container.CreateInstance(typeof(SampleWith2Ctors));
            });
        }

        [Test]
        public void CreateInstance_TypeHasImportProperty_CreateInstanceofTypeAndPropertyShouldBeInitialized()
        {
            _container.AddType(typeof(Sample));
            _container.AddType(typeof(ImportSample));

            var instance = _container.CreateInstance(typeof(ImportSample));

            Assert.AreEqual(typeof(ImportSample), instance.GetType());
            Assert.NotNull(((ImportSample)instance).Sample);
        }

        [Test]
        public void CreateInstance_ThereIsComplexType_CreateInstanceOfThisOne()
        {
            _container.AddType(typeof(Sample), typeof(ISample));
            _container.AddType(typeof(Sample1));
            _container.AddType(typeof(Sample2), typeof(ISample2));
            _container.AddType(typeof(Sample3));
            _container.AddType(typeof(ComplexSample));

            var instance = _container.CreateInstance(typeof(ComplexSample));

            Assert.AreEqual(typeof(ComplexSample), instance.GetType());

            var complexSample = (ComplexSample)instance;

            Assert.AreEqual(typeof(Sample), complexSample.Sample.GetType());
            Assert.Null(complexSample.Sample_NotImport);

            Assert.AreEqual(typeof(Sample1), complexSample.Sample1.GetType());
            Assert.Null(complexSample.Sample1_NotImport);

            Assert.NotNull(complexSample.Sample2);
            Assert.AreEqual(typeof(Sample2), complexSample.Sample2.GetType());

            Assert.NotNull(complexSample.Sample2);
            Assert.AreEqual(typeof(Sample3), complexSample.Sample3.GetType());
        }
    }
}
