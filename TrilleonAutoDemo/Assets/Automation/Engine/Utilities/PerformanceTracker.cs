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
*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

namespace TrilleonAutomation{

	public class PerformanceTracker : MonoBehaviour {

        public static int GarbageCollectionMemoryLeakMaximumThresholdInMegabytes { get; set; }
        public static int GarbageCollectionMemoryLeakWarningThreasholdInMegabytes { get; set; }
        public static bool WarningHit { get; set; }
        public static bool CriticalHit { get; set; }
        public static double StartingGCValue { get; set; }
        static int _numberOfSamplesToAverageGCUsage = 10;

		public static double max_fps {
			get { 
				return _max_fps;
			}
		}
		private static double _max_fps = 0;

		public static double min_fps {
			get { 
				return _min_fps;
			}
		}
		private static double _min_fps = double.MaxValue;

		public static double total_fps {
			get { 
				return _total_fps;
			}
		}
		private static double _total_fps = 0;

		public static double max_gc_memory {
			get { 
				return _max_gc_memory;
			}
		}
		private static double _max_gc_memory = 0;

		public static double min_gc_memory {
			get { 
				return _min_gc_memory;
			}
		}
		private static double _min_gc_memory = long.MaxValue;

		public static double max_hs_memory {
			get { 
				return _max_hs_memory;
			}
		}
		private static double _max_hs_memory = 0;

		public static double min_hs_memory {
			get { 
				return _min_hs_memory;
			}
		}
		private static double _min_hs_memory = float.MaxValue;

		public static double total_gc_memory {
			get { 
				return _total_gc_memory;
			}
		}
		private static double _total_gc_memory = 0;

		public static double total_hs_memory {
			get { 
				return _total_hs_memory;
			}
		}
		private static double _total_hs_memory = 0;

		public static bool auto_track {
			get { return _auto_track; }
			set {_auto_track = value; }
		}
		private static bool _auto_track;

		public static int Screenshot_Interval {
			get { return _screenshot_Interval; }
			set {_screenshot_Interval = value; }
		}
		private static int _screenshot_Interval = 300;

		static List<KeyValuePair<string[],double>> GCEntries = new List<KeyValuePair<string[],double>>();
		static List<KeyValuePair<string[],double>> HSEntries = new List<KeyValuePair<string[],double>>();
		static List<KeyValuePair<string[],double>> FPSEntries = new List<KeyValuePair<string[],double>>();

        void Start() {
            
            GarbageCollectionMemoryLeakMaximumThresholdInMegabytes = AutomationMaster.ConfigReader.GetInt("GARBAGE_COLLECTION_MEMORY_LEAK_MAXIMUM_THRESHOLD_MB");
            GarbageCollectionMemoryLeakWarningThreasholdInMegabytes = AutomationMaster.ConfigReader.GetInt("GARBAGE_COLLECTION_MEMORY_LEAK_WARNING_THRESHOLD_MB");
            StartingGCValue = Math.Round((float)GC.GetTotalMemory(true) / 1000000, 0);

        }

        public IEnumerator TrackMemory(string key) {

			string now = DateTime.UtcNow.ToString();

			//Garbage Collector Allotted.
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			double thisGCMemory = Math.Round((float)GC.GetTotalMemory(true)/1000000, 0);
			_total_gc_memory += thisGCMemory;
			if(thisGCMemory < _min_gc_memory) {
				
				_min_gc_memory = thisGCMemory;

			} else if(thisGCMemory > _max_gc_memory) {
				
				_max_gc_memory = thisGCMemory;

			}
			GCEntries.Add(new KeyValuePair<string[],double>(new string[] { key, now }, thisGCMemory));

            if(thisGCMemory > StartingGCValue) {

                double averageSample = 0;
                double averageSum = 0;
                List<double> samples = GCEntries.ExtractListOfValuesFromKeyValList();
                samples.Reverse();
                for(int s = 0; s < _numberOfSamplesToAverageGCUsage; s++) {

                    if(s == samples.Count) {

                        averageSample = averageSum / s;
                        break;

                    }
                    averageSum += thisGCMemory;
                    if(s + 1 == _numberOfSamplesToAverageGCUsage) {

                        averageSample = averageSum / _numberOfSamplesToAverageGCUsage;

                    }

                }

                if(!CriticalHit && averageSample - StartingGCValue >= GarbageCollectionMemoryLeakMaximumThresholdInMegabytes) {

                    CriticalHit = true;
                    string message = string.Format("Garbage Collection MemoryLeak CRITICAL Threshold Exceeded! Order of execution [{0} > {1}]", string.Join(" > ", AutomationMaster.TestRunContext.CompletedTests.ToArray()), AutomationMaster.CurrentTestMethod);
                    yield return StartCoroutine(Q.assert.Fail(message));
                    Q.assert.CriticalTestRunFailure(message);

                } else if(!WarningHit && averageSample - StartingGCValue >= GarbageCollectionMemoryLeakWarningThreasholdInMegabytes) {

                    WarningHit = true;
                    string message = string.Format("Garbage Collection MemoryLeak WARNING Threshold Exceeded! Order of execution [{0}]", string.Join(" > ", AutomationMaster.TestRunContext.CompletedTests.ToArray()), AutomationMaster.CurrentTestMethod);
                    yield return StartCoroutine(Q.assert.Warn(message));

                }

            }

			if(!key.ToLower().ContainsOrEquals("interval")) {
				
				AutomationMaster.Arbiter.SendCommunication("memory_snapshot", string.Format("{0} : {1}", key, now));

			}

			long allMemory = 0; //TODO: GET THE HEAP SIZE? Profiler.usedHeapSize;

			//Asset Memory Usage.
			double thisRCMemory = Math.Round((float)allMemory/1000000, 0);
			_total_hs_memory += thisRCMemory;
			if(thisRCMemory < _min_hs_memory) {
				
				_min_hs_memory = thisRCMemory;

			} else if(thisRCMemory > _max_hs_memory) {
				
				_max_hs_memory = thisRCMemory;

			}
			HSEntries.Add(new KeyValuePair<string[],double>(new string[] { key, now }, thisRCMemory));

			//Frames per second.
			double thisFPS = Convert.ToDouble(Math.Round(1.0f / Time.deltaTime));
			_total_fps += thisFPS;
			if(thisFPS < _min_fps) {
				
				_min_fps = thisFPS;

			} else if(thisFPS > _max_fps) {
				
				_max_fps = thisFPS;

			}
			FPSEntries.Add(new KeyValuePair<string[],double>(new string[] { key, now }, thisFPS));

			yield return null;

		}

		public static double GetAverageGarbageCollectorMemoryUsageDuringTestRun() {

			return Math.Round(total_gc_memory / GCEntries.Count, 2);

		}

		public static double GetAverageHeapSizeMemoryUsageDuringTestRun() {

			return Math.Round(total_hs_memory / HSEntries.Count, 2);

		}

		public static double GetAverageFPSDuringTestRun() {

			return Math.Round(total_fps / FPSEntries.Count, 1);

		}

		public static string GetFpsJsonReportWithReset(){

			StringBuilder report = new StringBuilder();
			report.Append("[");
			for(int i = 0; i < FPSEntries.Count; i++) {
				string time = FPSEntries[i].Key[1];
				double fps = FPSEntries[i].Value;
				string message = FPSEntries[i].Key[0];
				report.Append(string.Format("{{ \"fps\" : {0}, \"time\" : \"{1}\", \"message\" : \"{2}\" }}", fps.ToString(), time, message));
				if(i + 1 != FPSEntries.Count) {
					report.Append(",");
				}
			}
			report.Append("]");
			_total_fps = 0;
			_max_fps = 0;
			_min_fps = double.MaxValue;
			GCEntries = new List<KeyValuePair<string[],double>>();
			return report.ToString();

		}

		public static string GetGarbageCollectorJsonReportWithReset(){

			StringBuilder report = new StringBuilder();
			report.Append("[");
			for(int i = 0; i < GCEntries.Count; i++) {
				
				string time = GCEntries[i].Key[1];
				double memory_kilobytes = GCEntries[i].Value;
				string message = GCEntries[i].Key[0];
				report.Append(string.Format("{{ \"memory\" : {0}, \"time\" : \"{1}\", \"message\" : \"{2}\" }}", memory_kilobytes.ToString(), time, message));
				if(i + 1 != GCEntries.Count) {
					
					report.Append(",");

				}

			}
			report.Append("]");
			_total_gc_memory = 0;
			_max_gc_memory = 0;
			_min_gc_memory = long.MaxValue;
			GCEntries = new List<KeyValuePair<string[],double>>();
			return report.ToString();

		}

		public static string GetHeapSizeCounterJsonReportWithReset() {

			StringBuilder report = new StringBuilder();
			report.Append("[");
			for(int i = 0; i < HSEntries.Count; i++) {
				string time = HSEntries[i].Key[1];
				double memory_kilobytes = HSEntries[i].Value;
				string message = HSEntries[i].Key[0];
				report.Append(string.Format("{{ \"memory\" : {0}, \"time\" : \"{1}\", \"message\" : \"{2}\" }}", memory_kilobytes.ToString(), time, message));
				if(i + 1 != HSEntries.Count) {
					
					report.Append(",");

				}

			}
			report.Append("]");
			_total_hs_memory = 0;
			_max_hs_memory = 0;
			_min_hs_memory = long.MaxValue;
			HSEntries = new List<KeyValuePair<string[],double>>();
			return report.ToString();

		}

	}

}
