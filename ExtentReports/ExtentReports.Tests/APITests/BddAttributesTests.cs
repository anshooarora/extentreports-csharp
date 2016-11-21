using NUnit.Framework;

using AventStack.ExtentReports.Gherkin.Model;

namespace AventStack.ExtentReports.Tests.APITests
{
    [TestFixture]
    public class BddAttributesTests : Base
    {
        [Test]
        public void InitialTestIsOfBddType()
        {
            ExtentTest feature = _extent.CreateTest<Feature>(TestContext.CurrentContext.Test.Name);

            Assert.True(feature.GetModel().IsBehaviorDrivenType);
            Assert.IsInstanceOf(typeof(Feature), feature.GetModel().BehaviorDrivenType);
        }

        [Test]
        public void TestIsOfBddTypeWithBddChild()
        {
            ExtentTest feature = _extent.CreateTest<Feature>(TestContext.CurrentContext.Test.Name);
            ExtentTest scenario = feature.CreateNode<Scenario>("Scenario");

            Assert.True(feature.GetModel().IsBehaviorDrivenType);
            Assert.True(scenario.GetModel().IsBehaviorDrivenType);
            Assert.IsInstanceOf(typeof(Feature), feature.GetModel().BehaviorDrivenType);
            Assert.IsInstanceOf(typeof(Scenario), scenario.GetModel().BehaviorDrivenType);
        }
    }
}
