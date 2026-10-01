using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using nunittest1;
using Pose;
using System;
using Shim = Pose.Shim;

namespace UnitTestProject2
{
    public class TestableClassUnderTest : ClassUnderTest
    {
        public TestableClassUnderTest(IAffectingClass pAff) : base(pAff) { }
        public int ExposeProtectedMethod(int arg) => base.ProtectedMethod(arg);
    }

    public class ParameterizedAffectingClass : AffectingClass
    {
        private readonly int _value;
        public ParameterizedAffectingClass(int value) => _value = value;
        public override int Method() => _value;
    }

    [TestClass]
    public class ClassUnderTestTests
    {
        #region Задание 1. Параметризованные тесты

        [TestMethod]
        [DataRow(5, 10, 10)]
        [DataRow(15, 10, 15)]
        [DataRow(10, 10, 10)]
        public void PublicMethod_ReturnsCorrectValue(int arg, int val, int expected)
        {
            var mock = new Mock<IAffectingClass>();
            mock.Setup(x => x.Val).Returns(val);
            var sut = new ClassUnderTest(mock.Object);
            Assert.AreEqual(expected, sut.PublicMethod(arg));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(5)]
        [DataRow(100)]
        public void CallAffectingMethod_ReturnsExpectedValue(int expected)
        {
            var affecting = new ParameterizedAffectingClass(expected);
            var sut = new ClassUnderTest2(affecting);
            Assert.AreEqual(expected, sut.CallAffectingMethod());
        }

        #endregion

        #region Задание 2. ProtectedMethod и исключение

        [TestMethod]
        public void ProtectedMethod_ThrowsArgumentException_WhenArgIsZero()
        {
            var mock = new Mock<IAffectingClass>();
            var sut = new TestableClassUnderTest(mock.Object);
            var ex = Assert.Throws<ArgumentException>(() => sut.ExposeProtectedMethod(0));
            Assert.AreEqual("arg", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("arg is equal to zero"));
        }

        [TestMethod]
        public void ProtectedMethod_ReturnsVal_WhenArgIsNotZero()
        {
            var mock = new Mock<IAffectingClass>();
            mock.SetupGet(x => x.Val).Returns(42);
            var sut = new TestableClassUnderTest(mock.Object);
            Assert.AreEqual(42, sut.ExposeProtectedMethod(10));
        }

        #endregion

        #region Задание 3. Moq: с SetUp и без SetUp

        private Mock<IAffectingClass> _mock = null!;
        private ClassUnderTest _sut = null!;

        [TestInitialize]
        public void SetUp()
        {
            _mock = new Mock<IAffectingClass>();
            _mock.SetupGet(x => x.Val).Returns(42);
            _sut = new ClassUnderTest(_mock.Object);
        }

        [TestMethod]
        public void PublicMethod_WithSetUp_Returns42()
        {
            Assert.AreEqual(42, _sut.PublicMethod(10));
        }

        [TestMethod]
        public void PublicMethod_WithoutSetUp_ReturnsCorrectValue()
        {
            var mock = new Mock<IAffectingClass>();
            mock.SetupGet(x => x.Val).Returns(99);
            var sut = new ClassUnderTest(mock.Object);
            Assert.AreEqual(99, sut.PublicMethod(10));
        }

        #endregion

        #region Задание 5. Mock.Of и new Mock

        [TestMethod]
        public void MockOf_CanBeUsedAsDependency()
        {
            IAffectingClass stub = Mock.Of<IAffectingClass>(x => x.Val == 25);
            var sut = new ClassUnderTest(stub);
            Assert.AreEqual(25, sut.PublicMethod(10));
        }

        #endregion

        #region Задание 6. Верификация вызова свойства Val

        [TestMethod]
        public void PublicMethod_VerifyValGetterIsCalled()
        {
            var mock = new Mock<IAffectingClass>();
            mock.SetupGet(x => x.Val).Returns(10);
            var sut = new ClassUnderTest(mock.Object);
            sut.PublicMethod(5);
            mock.VerifyGet(x => x.Val, Times.AtLeastOnce);
        }

        #endregion

        #region Дополнительное задание. Pose shim для static

        [TestMethod]
        public void CallStatic_WithPoseShim_ReturnsShimmedValue()
        {
            var mock = new Mock<IAffectingClass>();
            var sut = new ClassUnderTest(mock.Object);

            var shim = Shim.Replace(() => IAffectingClass.StaticDependency()).With(() => 999);

            int result = 0;
            PoseContext.Isolate(() =>
            {
                result = sut.CallStatic();
            }, shim);

            Assert.AreEqual(999, result);
        }

        #endregion
    }
}