using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace TrilleonAutomation {

	//This class/test will not be allowed to run until all other classes and methods lacking this attribute have run.
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class Deferr : Attribute { }

}