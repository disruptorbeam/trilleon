using UnityEngine;
using System.Collections.Generic;

namespace TrilleonAutomation {
   
   public class DependencyNode {

      public DependencyNode() : this(string.Empty) {}

      public DependencyNode(string testName) {
         _TestName = testName;
      }

      public string TestName {
         get { return _TestName; }
         set { _TestName = value; }
      }
      private string _TestName = string.Empty;

      public List<KeyValuePair<DependencyNodeConnectionType,string>> Dependencies {
         get {  return _Dependencies; }
         set { _Dependencies = value; }
      }
      private List<KeyValuePair<DependencyNodeConnectionType,string>> _Dependencies = new List<KeyValuePair<DependencyNodeConnectionType,string>>();

      public void AddDependency(DependencyNodeConnectionType type, string DependencyName){
         _Dependencies.Add(new KeyValuePair<DependencyNodeConnectionType,string>(type, DependencyName));
      }
     
      public Rect rect {
         get { return _rect; }
         set { _rect = value; }
      }
      private Rect _rect = new Rect();

   }

}