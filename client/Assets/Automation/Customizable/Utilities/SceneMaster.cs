/* 
+   This file is part of Trilleon.  Trilleon is a client automation framework.
+  
+   Copyright (C) 2017 Disruptor Beam
+  
+   Trilleon is free software: you can redistribute it and/or modify
+   it under the terms of the GNU Lesser General Public License as published by
+   the Free Software Foundation, either version 3 of the License, or
+   (at your option) any later version.
+
+   This program is distributed in the hope that it will be useful,
+   but WITHOUT ANY WARRANTY; without even the implied warranty of
+   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
+   GNU Lesser General Public License for more details.
+
+   You should have received a copy of the GNU Lesser General Public License
+   along with this program.  If not, see <http://www.gnu.org/licenses/>.
+*/

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
			
			#if UNITY_EDITOR
            //Thanks to yasirkula for this fix to getting DontDestroyOnLoad GameObjects.
            GameObject temp = null;
            try
            {
                temp = new GameObject();
                Object.DontDestroyOnLoad(temp);
                Scene dontDestroyOnLoad = temp.scene;
                Object.DestroyImmediate(temp);
                temp = null;
                results.AddRange(dontDestroyOnLoad.GetRootGameObjects().ToList().GetChildren());
            }
            finally
            {
                if (temp != null)
                    Object.DestroyImmediate(temp);
            }
			#endif
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
