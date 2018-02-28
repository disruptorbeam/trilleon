using UnityEngine;
using System.Collections;

namespace TrilleonAutomation {

   /// <summary>
   /// This class demonstrates how DependencyTest attributes affect test execution when used in classes that lack
   /// the DependencyClass attribute. Note that all tests are, by default, grouped and run in the order they are discovered.
   /// This means that all tests under a class will be handled (executed, skipped, deferred etc) before the test runner moves on to another class.
   /// </summary>
   [AutomationClass]
   [DebugClass]
   public class DependencyMasterlessTests : MonoBehaviour {

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyTest(3)]
      [Validate(Expect.Success)]
      public IEnumerator DependencyMasterlessTest_01() {
         
         Q.assert.IsTrue(true, "When the logic gathers all tests for the test run, I will automatically be sorted in the order declared by my DependencyTest attribute. Therefore, I will run 4th.");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [Validate(Expect.Success)]
      [Validate(Expect.RanAfter, "DependencyMasterlessTest_01")]
      public IEnumerator DependencyMasterlessTest_02() {
         
         Q.assert.IsTrue(true, "I appear as the second test in this class visually, but I lack a DependencyTest attribute. Because of this, any test with a DependencyTest will be executed before me.");
         Q.assert.IsTrue(true, "After all DependencyTest tests are executed, I will then be the first test without a DependencyTest attribute to run, because I am the first without that attribute to appear in this class.");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyTest(2)]
      [Validate(Expect.RanBefore, "DependencyMasterlessTest_01")]
      public IEnumerator DependencyMasterlessTest_03() {
         
         Q.assert.IsTrue(true, "I appear as the third test in this class visually, but I will run second.");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyTest(1)]
      [Validate(Expect.RanBefore, "DependencyMasterlessTest_02")]
      public IEnumerator DependencyMasterlessTest_04() {
         
         Q.assert.IsTrue(true, "I appear as the final test in this class visually, but I will run first because of my DependencyTest attribute with an order of 1.");
         yield return null;

      }

   }

}