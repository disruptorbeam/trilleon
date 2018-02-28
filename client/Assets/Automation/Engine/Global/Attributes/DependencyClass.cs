using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace TrilleonAutomation {

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DependencyClass : Attribute {

		public int order { get; private set; }

		public DependencyClass(int requestedOrder) {

			order = requestedOrder;

		}

	}

}