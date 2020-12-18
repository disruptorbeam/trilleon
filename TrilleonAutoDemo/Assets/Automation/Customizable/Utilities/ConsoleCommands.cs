using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MiniJSON;

namespace TrilleonAutomation {

	//All game-specific cheat/commands go here.
	public class ConsoleCommands : ConsoleCommandsCore {

		/// <summary>
		/// Add all valid commands with accompanying logic to launch in region below. Command aliases cannot contains spaces.
		/// </summary>
		new void Start() {

			RegisteredCommands.Add(new Command("Set Score.", SetScore, 
				new List<KeyValuePair<string,string>> { 
					new KeyValuePair<string,string>("Score", "The value to set the current score to."),
				}, "S+", "SetScore"));
			RegisteredCommands.Add(new Command("Force Spawn Duck.", SpawnDuck, 
				new List<KeyValuePair<string,string>>(), "Quack"));
			ImplementCore();

		}

		//Your Commands Here!
		#region Console Command delegates

		static string SetScore(List<string> args) {

			if(args.First().ToInt() < 1) {

				return "Please set a valid score (whole number) greater than one.";

			}
			GameController.Points = args.First().ToInt();
			GameController.Self.ScoreText.text = string.Format("Score: {0}", args.First());
			return string.Format("Score set to {0}!", args.First());

		}

		static string SpawnDuck(List<string> args) {

			GameController.Self.SpawnDuck();
			return "A wild duck appears.";

		}

		#endregion

	}

}