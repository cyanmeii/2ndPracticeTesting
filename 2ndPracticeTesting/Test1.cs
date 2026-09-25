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

    [TestClass]
    public class ClassUnderTestTests
    {
        // Задача 1 и 2: Тесты для PublicMethod и CallAffectingMethod с параметризацией
        [TestMethod]
        [DataRow(5, 10, 10)]
        [DataRow(15, 10, 15)]
        public void PublicMethod_ReturnsCorrectValue(int arg, int val, int expected)
        {
            var mock = new Mock<IAffectingClass>();
            mock.Setup(x => x.Val).Returns(val);
            var sut = new ClassUnderTest(mock.Object);

            Assert.AreEqual(expected, sut.PublicMethod(arg));
        }

        [TestMethod]
        public void CallAffectingMethod_ReturnsDefault()
        {
            var aff = new AffectingClass();
            var sut = new ClassUnderTest2(aff);
            Assert.AreEqual(1, sut.CallAffectingMethod());
        }

        // Задача 3: Тест для ProtectedMethod и верификация исключения
        [TestMethod]
        public void ProtectedMethod_ThrowsArgumentException_WhenArgIsZero()
        {
            var mock = new Mock<IAffectingClass>();
            var sut = new TestableClassUnderTest(mock.Object);
            var ex = Assert.Throws<ArgumentException>(() => sut.ExposeProtectedMethod(0));

            Assert.IsTrue(ex.Message.Contains("arg is equal to zero"));
        }

        // Задача 4: Тест с использованием Moq (с SetUp и без)
        private Mock<IAffectingClass> _mock;
        private ClassUnderTest _sut;

        [TestInitialize]
        public void SetUp()
        {
            _mock = new Mock<IAffectingClass>();
            _mock.Setup(x => x.Val).Returns(42);
            _sut = new ClassUnderTest(_mock.Object);
        }

        [TestMethod]
        public void PublicMethod_WithSetUp_Returns42()
        {
            Assert.AreEqual(42, _sut.PublicMethod(10));
        }

        [TestMethod]
        public void PublicMethod_WithoutSetUp_ReturnsCorrect()
        {
            var mock = new Mock<IAffectingClass>();
            mock.Setup(x => x.Val).Returns(99);
            var sut = new ClassUnderTest(mock.Object);
            Assert.AreEqual(99, sut.PublicMethod(10));
        }

        // Задача 6: Верификация вызова свойства Val
        [TestMethod]
        public void PublicMethod_VerifyValIsCalled()
        {
            var mock = new Mock<IAffectingClass>();
            mock.Setup(x => x.Val).Returns(10);
            var sut = new ClassUnderTest(mock.Object);

            sut.PublicMethod(5);

            mock.Verify(x => x.Val, Times.Once);
        }

        // Задача 10: обертка для статического метода с помощью Pose
        [TestMethod]
        public void CallStatic_WithPoseShim_ReturnsShimmedValue()
        {
            var shim = Shim.Replace(() => IAffectingClass.StaticDependency()).With(() => 999);

            PoseContext.Isolate(() =>
            {
                var mock = new Mock<IAffectingClass>();
                var sut = new ClassUnderTest(mock.Object);
                Assert.AreEqual(999, sut.CallStatic());
            }, shim);
        }
    }
}