using UnityEngine;
using System.Collections;

namespace TrilleonAutomation {

   /// <summary>
   /// This class demonstrates how DependencyWeb attributes form relationships that determine
   /// the order tests execute in, along with whether tests are skipped or run.
   /// </summary>
   [AutomationClass]
   [DebugClass]
   public class DependencyWebTests : MonoBehaviour {

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyWeb("DependencyWebTest_03")]
      [Validate(Expect.Success)]
      [Validate(Expect.RanAfter, "DependencyWebTest_04")]
      public IEnumerator DependencyWebTest_01() {
         
         Q.assert.IsTrue(true, "Current logic guarantees that I will be the first test encountered by the test runner (AutomationMaster.cs).");
         Q.assert.IsTrue(true, "However, because I depend on a successful completion of \"DependencyWebTest_03\" to run, I will be deferred until later in the test run.");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyWeb("DependencyWebTest_01")]
      [Validate(Expect.Success)]
      [Validate(Expect.RanAfter, "DependencyWebTest_01")]
      public IEnumerator DependencyWebTest_02() {
         
         Q.assert.IsTrue(true, "I should be deferred until after \"DependencyWebTest_01\", which was also deferred, is successfully completed.");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [Validate(Expect.Success)]
      [Validate(Expect.RanBefore, "DependencyWebTest_01")]
      public IEnumerator DependencyWebTest_03() {
         
         Q.assert.IsTrue(true, "I should be the first test to execute in this class!");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyWeb("DependencyWebTest_03")]
      [Validate(Expect.Success)]
      [Validate(Expect.RanAfter, "DependencyWebTest_03")]
      public IEnumerator DependencyWebTest_04() {
         
         Q.assert.IsTrue(true, "I will be the second test to execute in this class! This is not explicitly apparent, however. This is due to deferrment logic.");
         Q.assert.IsTrue(true, "Note that \"DependencyWebTest_01\" and \"DependencyWebTest_02\" were discovered by the test runner before I was. And like the former, I directly rely on \"DependencyWebTest_03\".");
         Q.assert.IsTrue(true, "However, those tests were added to a \"deferred\" list. This list is not looked at until all other tests in the run have been encountered and handled.");
         Q.assert.IsTrue(true, "Because my dependency, \"DependencyWebTest_03\", succeeded before I was encountered, I did not get added to the deferred list, and was run immediately.");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [Validate(Expect.Failure)]
      public IEnumerator DependencyWebTest_05() {

         Q.assert.Fail("I will automatically fail!");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyWeb("DependencyWebTest_05")]
      [Validate(Expect.Skipped)]
      public IEnumerator DependencyWebTest_06() {

         Q.assert.Fail("You will not recieve this assertion in test run reports as all my test logic will automatically be skipped. This is because my dependency, \"DependencyWebTest_05\", has previously run and failed.");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [Validate(Expect.Failure)]
      public IEnumerator DependencyWebTest_07() {

         Q.assert.IsTrue(true, "I am here to demonstrate partial dependencies. There may be situations where a test is marked as a dependency of other tests because it accomplishes something that later tests rely on.");
         Q.assert.IsTrue(true, "However, this critical functionality may only represent a part of the dependency test. I can explicitly tell the test runner that I have accomplished what I needed to do for tests that rely on me.");
         Q.assert.IsTrue(true, "Once I declare this, the test runner will no longer skip tests that depend on me, should I fail later in my execution.");
         Q.assert.MarkTestAsRunDependenciesRegardlessOfFailure();
         Q.assert.Fail("I have failed after invoking \"Q.assert.MarkTestAsRunDependenciesRegardlessOfFailure\".");
         yield return null;

      }

      [Automation("Debug/Dependencies")]
      [Automation("Trilleon/Validation")]
      [DependencyWeb("DependencyWebTest_07")]
      [Validate(Expect.Success)]
      [Validate(Expect.RanAfter, "DependencyWebTest_07")]
      public IEnumerator DependencyWebTest_08() {

         Q.assert.IsTrue(true, "The test that I depend on, \"DependencyWebTest_07\", failed its execution. Despite that, I will still run because of \"Q.assert.MarkTestAsRunDependenciesRegardlessOfFailure\"");
         yield return null;

      }

   }

}