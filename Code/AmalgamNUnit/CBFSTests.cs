using NLog;
using NUnit.Framework;

namespace AmalgamNUnit
{
    public class CBFSTests
    {
       static private readonly Logger Log = LogManager.GetCurrentClassLogger();

       [TestFixtureSetUp]
       public void Init()
       {
       }

       [TestFixtureTearDown]
       public void Dispose()
       {
       }
       [SetUp]
      public void SetUp()
      {
      }

      [TearDown]
      public void TearDown()
      {
      }



       [Test]
       [Description("Check that it is possible to perform a simple mount.")]
       public void A010SimpleLetterMount()
       {
       }
    }
}
