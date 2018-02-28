using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrilleonAutomation {
	
	public class AutomationSelectFeedback : MonoBehaviour {

		public float InitialDiameter { get; set; }
		public float FinalDiameter { get; set; }
		public float PulseLifeSpan { get; set; }
		public float RingLifeSpan { get; set; }
		public float TimeBetweenPulses { get; set; }

		void InstantiateNewRing() {
			
			GameObject ring = new GameObject("ring_pulsar", typeof(SpriteRenderer), typeof(AutomationSelectRings));
			ring.GetComponent<AutomationSelectRings>().container = this;
			ring.transform.SetParent(this.transform, false);

		}

		void Start() {

			InitialDiameter = 0.1f;
			FinalDiameter = 0.25f;
			PulseLifeSpan = 2.75f;
			RingLifeSpan = 1.20f;
			TimeBetweenPulses = 1f;
			StartCoroutine(Pulse());

		}

		IEnumerator Pulse() {

			GameObject ring = new GameObject("ring_center", typeof(SpriteRenderer));
			ring.transform.SetParent(this.transform, false);
			ring.GetComponent<SpriteRenderer>().sprite = Driver.MouseHand;
			ring.GetComponent<SpriteRenderer>().sortingLayerName = "foreground";
			ring.GetComponent<SpriteRenderer>().sortingOrder = 99;
			ring.transform.localScale = new Vector3(0.075f, 0.075f, 0.075f);

			float timer = 0f;
			do {

				InstantiateNewRing();
				yield return StartCoroutine(Q.driver.WaitRealTime(TimeBetweenPulses));
				timer += TimeBetweenPulses;

			} while(timer < PulseLifeSpan);

			DestroyImmediate(ring);
			DestroyImmediate(this.gameObject);
			yield return null;

		}
		
	}

}