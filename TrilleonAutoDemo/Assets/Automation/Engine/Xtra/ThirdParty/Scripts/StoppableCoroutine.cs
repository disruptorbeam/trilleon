using UnityEngine;
using System.Collections;

namespace TrilleonAutomation {

	/// <summary>
	/// Created by Bunny83 05-07-2016 as an offered solution(Unity Answers) for the problem of stopped 
	/// child coroutines failing to return control to a parent coroutine that called it by yeild return.
	/// </summary>
	public class StoppableCoroutine {

		bool terminated = false;
		IEnumerator payload;
		Coroutine nested;
		MonoBehaviour mb;

		public StoppableCoroutine(MonoBehaviour mb, IEnumerator payload) {
			
			this.payload = payload;
			this.mb = mb;

		}

		public Coroutine WaitFor() {
			
			/*
          	* Moved 'nested' from constructor. Fixes "bug" that prevents retrieval of most current data if accessing instantiated StoppableCoroutine from another location before the current coroutine completes.
          	* However, this causes another "bug" of its own. One cannot have more than a single StoppableCoroutine active in the Trilleon Framework at any one time. This is a fair trade-off from the previous
          	* issue because there should NEVER be more than one active at a time. Stoppable Coroutines are SetUpTearDown-type methods, or Automation test methods only.
	        */
			this.nested = mb.StartCoroutine(wrapper()); 
			return mb.StartCoroutine(wait());

		}

		public void Stop() {
			
			terminated = true;
			try{

				if(nested == null) {

					mb.StopAllCoroutines();

				} else {

					mb.StopCoroutine(nested);

				}

			} catch {}

		}

		private IEnumerator wrapper() {
			
			while(payload.MoveNext()) {
				
				yield return payload.Current;

			}
			
			terminated = true;

		}

		private IEnumerator wait() {
			
			while(!terminated) {
				
				yield return null;

			}
			
		}

	}

	public static class MonoBehaviourExtension {

		public static StoppableCoroutine StartCoroutineEx(this MonoBehaviour mb, IEnumerator coroutine) {
			
			return new StoppableCoroutine(mb, coroutine);

		}

	}

}