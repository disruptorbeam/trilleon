using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace TrilleonAutomation {

	public class SceneMaster {

		/// <summary>
		/// All objects in scene.
		/// </summary>
		/// <value>The object pool.</value>
		public static List<GameObject> GetObjectPool(string sceneName) {

			return SceneManager.GetSceneByName(sceneName).GetRootGameObjects().ToList().GetChildren();

		}

		/// <summary>
		/// All objects in scene.
		/// </summary>
		/// <value>The object pool.</value>
		public static List<GameObject> GetObjectPool() {

			List<GameObject> results = new List<GameObject>();
			for(int s = 0; s < SceneManager.sceneCount; s++) {

				Scene thisScene = SceneManager.GetSceneAt(s);
				if(thisScene.isLoaded) {

					results.AddRange(SceneManager.GetSceneAt(s).GetRootGameObjects().ToList().GetChildren());

				}

			}
			return results.Distinct();

		}

		public static bool IsSceneLoaded(string sceneName) {

			try {

				Scene match = SceneManager.GetSceneByName(sceneName);
				return match.isLoaded;

			} catch {

				return false;

			}

		}

	}

}