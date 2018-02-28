using UnityEngine;
using System.Collections;

namespace TrilleonAutomation {

   /// <summary>
   /// This class demonstrates how DependencyClass and  DependencyTest attributes used in unison affect test execution.
   /// </summary>
   [AutomationClass]
   [DependencyClass(0)]
   [DebugClass]
   public class DependencyMasterTests : MonoBehaviour {

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyTest(1)]
      [Validate(Expect.Success)]
      [Validate(Expect.OrderRan, "1")]
      public IEnumerator DependencyMasterTest_01() {
         
         Q.assert.IsTrue(true, "If I am included in a test run AT ALL, I will be the FIRST test run, period. This is, firstly, because my class is marked as a DependencyClass with an order of 1.");
         Q.assert.IsTrue(true, "The attribute DependencyClass grants a class priority over all other tests based on the provided order.");
         Q.assert.IsTrue(true, "Since I also am marked as DependencyTest 1, I am the first test in the first class that will be run.");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyTest(2)]
      [Validate(Expect.Success)]
      [Validate(Expect.OrderRan, "2")]
      public IEnumerator DependencyMasterTest_02() {

         Q.assert.IsTrue(true, "Because I have a DependencyTest attribute with an order of 2, I am the second test in the first class that will be run.");
         Q.assert.IsTrue(true, "While I appear visually as the second test in this class, if I did not have the DependencyTest attribute, I would not be the second test to run.");
         Q.assert.IsTrue(true, "This is because there are other tests below me with a DependencyTest attribute that always take priority.");
         Q.assert.IsTrue(true, "Note that \"DependencyMasterTest_03\" does not appear below me anywhere. Don't worry, that's intended. Find it in the ExampleDemoTests class!");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyTest(5)]
      [Validate(Expect.Skipped)]
      [Validate(Expect.OrderRan, "5")]
      public IEnumerator DependencyMasterTest_04() {
         
         Q.assert.IsTrue(true, "I am set to run fifth within the context of this DependencyClass. However, I rely on the tests that ran before me.");
         Q.assert.IsTrue(true, "Because \"DependencyMasterTest_05\" failed, my logic will be skipped, and you will not see my assertions in a test report.");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyTest(4)]
      [Validate(Expect.Failure)]
      [Validate(Expect.OrderRan, "4")]
      public IEnumerator DependencyMasterTest_05() {
         
         Q.assert.Fail("Iam set to fail, which will affect any tests in this class that run after me and have a DependencyTest attribute!");
         yield return null;

      }

   }

}