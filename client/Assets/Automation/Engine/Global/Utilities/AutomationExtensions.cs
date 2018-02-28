using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System;

namespace TrilleonAutomation {

	public static class AutomationExtensions {

		#region Non-Generic Extensions

		/// <summary>
		/// Convert string into numerical value. Return -1 if not convertable.
		/// </summary>
		/// <param name="isMoneyString">If set to <c>true</c>, remove characters often seen in currency, such as comma and $ sign.</param>
		public static int ToInt(this string value, bool isMoneyString = false) {

			string formattedInteger = isMoneyString ? value.Replace(",", string.Empty).Replace("$", string.Empty) : value.Replace(",", string.Empty);
			int parseVal = 0;
			int.TryParse(formattedInteger, out parseVal);
			return parseVal;

		}
		public static float ToFloat(this string value, bool isMoneyString = false) {

			string formattedFloat = isMoneyString ? value.Replace(",", string.Empty).Replace("$", string.Empty) : value.Replace(",", string.Empty);
			float parseVal = 0;
			float.TryParse(formattedFloat, out parseVal);
			return parseVal;

		}

		/// <summary>
		/// Offers option to retrieve length of array as count, so that there is consistancy between list count calls and array count calls.
		/// Arguably, Length makes more sense in determining array character count, rather than how many items are contained in an object.
		/// </summary>
		public static int Count<T>(this T[] vals) {

			return vals.Length;

		}

		/// <summary>
		/// Does this string contain a supplied substring, or (also) is it equal to that string? Replaces need to check both seperately.
		/// </summary>
		public static bool ContainsOrEquals(this string val, string subString) {

			return val.Contains(subString) || val == subString;

		}
			
		/// <summary>
		/// Diplicate a provided string the number of requested times.
		/// </summary>
		public static string Duplicate(this string val, int howManyTimesToDuplicate) {

			StringBuilder returnVal = new StringBuilder();
			for(int x = 0; x < howManyTimesToDuplicate; x++) {

				returnVal.Append(val);

			}
			return returnVal.ToString();

		}


		public static string FirstCharToUpper(this string val) {

			return string.Format("{0}{1}", val.ToCharArray().First().ToString().ToUpper(), val.Substring(1));

		}

		#endregion

		#region Instantiation Helpers

		/// <summary>
		/// Use with 'new List()' to instantiate a list of specific size where all values are the supplied default.
		/// </summary>
		public static List<T> OfSpecificValues<T>(this List<T> list, int size, T defaultValue) {

			List<T> newList = new List<T>();
			for(int x = 0; x < size; x++) {

				newList.Add(defaultValue);

			}
			return newList;

		}

		#endregion

		#region LINQ Surrogates

		/// <summary>
		/// Takes a list of GameObjects or Components, and returns a list of requested components that are attached to each GameOject(or Component.gameObject).
		/// </summary>
		/// <returns>The componenent list.</returns>
		public static List<T> ToComponenentList<T>(this List<GameObject> list){

			if(list == null) {
				return new List<T>();
			}

			List<T> returnList = new List<T>();
			for(int x = 0; x < list.Count; x++) {

				returnList.AddIfNotDefault<T>(list[x].GetComponent<T>());

			}
			return returnList;

		}
		public static List<T> ToComponenentList<T>(this List<Component> list){

			if(list == null) {
				return new List<T>();
			}

			List<T> returnList = new List<T>();
			for(int x = 0; x < list.Count; x++) {

				returnList.AddIfNotDefault<T>(list[x].gameObject.GetComponent<T>());

			}
			return returnList;

		}
		public static List<GameObject> ToGameObjectList(this List<Component> list){

			if(list == null) {
				return new List<GameObject>();
			}

			List<GameObject> returnList = new List<GameObject>();
			for(int x = 0; x < list.Count; x++) {

				returnList.Add(list[x].gameObject);

			}
			return returnList;

		}
		public static List<GameObject> ToGameObjectList<T>(this List<T> list) where T : MonoBehaviour {

			if(list == null) {
				return new List<GameObject>();
			}

			List<GameObject> returnList = new List<GameObject>();
			for(int x = 0; x < list.Count; x++) {

				returnList.Add(((MonoBehaviour)list[x]).gameObject);

			}
			return returnList;

		}

		/// <summary>
		/// Remove duplicate items in a list.
		/// </summary>
		public static List<T> Distinct<T>(this List<T> List) {

			return new List<T>(new HashSet<T>(List));

		}

		/// <summary>
		/// Get the first item in the list, if not empty.
		/// </summary>
		public static T First<T>(this List<T> list) {

			if(!Any(list)) {
				return default(T);
			}
			if(list.Count > 0) {
				return list[0];
			}         

			return default(T);

		}
		public static T First<T>(this T[] array) {

			if(array.Length == 0) {
				return default(T);
			}

			return First<T>(ToList(array));

		}

		/// <summary>
		/// Get the last item in the list, if not empty.
		/// </summary>
		public static T Last<T>(this List<T> list) {

			if(!Any(list)) {
				return default(T);
			}
			if(list.Count > 0) {
				return list[list.Count - 1];
			}

			return default(T);

		}
		public static T Last<T>(this T[] array) {

			if(array.Length == 0) {
				return default(T);
			}

			return Last<T>(ToList(array));

		}

		/// <summary>
		/// Return the index of the matching item in a list.
		/// </summary>
		/// <returns>The index of.</returns>
		/// <param name="list">List.</param>
		/// <param name="item">Item.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static int FindIndexOf<T>(this List<T> list, T item) {

			if (!Any(list)) {
				return -1;
			}

			int index = 0;

			for(int l = 0; l < list.Count; l++) {

				if(EqualityComparer<T>.Default.Equals(list[l], item)) {
					return index;
				}
				index++;

			}

			return -1;

		}

		/// <summary>
		/// Gets the item at the requested index. Enforces failure if item does not exist at that index. 
		/// Different than square bracket index call in that no exception occurs, and any currently-executing test is automatically failed.
		/// </summary>
		public static T GetIndex<T>(this List<T> list, int index) {

			if (!Any(list)) {
				return default(T);
			}

			if(list.Count >= index + 1) {
				return list[index];
			} else {
				Q.assert.Fail(string.Format("Supplied list in AutoList.GetIndex had less items that expected ({0}), as indicated by supplied index of {1}.", list.Count, index));
				return default(T);
			}

		}

		/// <summary>
		/// Gets the item at the requested order (index + 1). Does not err if item does not exist at that order. 
		/// </summary>
		public static T GetItemNumber<T>(this T[] array, int order) {

			if(array.Length == 0) {
				return default(T);
			}

			return GetItemNumber<T>(ToList(array), order);

		}
		public static T GetItemNumber<T>(this List<T> list, int order) {

			if(!Any(list)) {
				return default(T);
			}
			if(list.Count >= order) {
				return list[order - 1];
			}         

			return default(T);

		}

		public static bool Any<T>(this List<T> list){

			if(list == null) {
				return false;
			}

			return list.Count > 0;

		}

		public static List<T> ToList<T>(this T[] array){

			if (array.Length == 0) {
				return new List<T>();
			}
			List<T> list = new List<T>();
			for(int a = 0; a < array.Length; a++) {
				list.Add(array[a]);

			}

			return RemoveNulls(list);

		}

		public static T Random<T>(this List<T> list) {

			if (!Any(list)) {
				return default(T);
			}

			return list[Q.help.RandomIndex(list.Count - 1)];

		}

		public static KeyValuePair<K,V> Random<K,V>(this List<KeyValuePair<K,V>> list) {

			if (!Any(list)) {
				return default(KeyValuePair<K,V>);
			}

			return list[Q.help.RandomIndex(list.Count - 1)];

		}

		#endregion

		#region Custom Functionality

		/// <summary>
		/// Alternative to base "Add(item)". Adds item to list if item is not default (ex: null/empty).
		/// Intended for use if null values are not checked in code preceding list add call.
		/// </summary>
		public static void AddIfNotDefault<T>(this List<T> list, T itemToAdd) {

			if(typeof(T).IsPrimitive || !EqualityComparer<T>.Default.Equals(itemToAdd, default(T))) {

				list.Add(itemToAdd);

			}

		}

		public static void AddIfNotDuplicate<T>(this List<T> list, T itemToAdd) {

			if(!list.Contains(itemToAdd)) {

				list.Add(itemToAdd);

			}

		}

		public static List<KeyValuePair<string, T>> OrderByKeys<T>(this List<KeyValuePair<string, T>> keyValList) {

			List<string> results = ExtractListOfKeysFromKeyValList(keyValList);
			results.Sort();

			List<KeyValuePair<string, T>> newOrder = new List<KeyValuePair<string, T>>();
			for(int x = 0; x < results.Count; x++){

				newOrder.AddIfNotDefault(keyValList.Find(k => k.Key == results[x]));

			}
			return newOrder;

		}

		public static List<KeyValuePair<string, T>> OrderByValues<T>(this List<KeyValuePair<string, T>> keyValList) {

			List<T> results = ExtractListOfValuesFromKeyValList(keyValList);
			results.Sort();

			List<KeyValuePair<string, T>> newOrder = new List<KeyValuePair<string, T>>();
			for(int x = 0; x < results.Count; x++){

				newOrder.AddIfNotDefault(keyValList.Find(k => EqualityComparer<T>.Default.Equals(k.Value, results[x])));

			}
			return newOrder;

		}

		public static List<string> ToListOfNames(this List<GameObject> list) {

			if (!Any(list)) {
				return new List<string>();
			}
			List<string> newTypeList = new List<string>();
			for(int l = 0; l < list.Count; l++) {
				newTypeList.Add(list[l].name);
			}

			return newTypeList;

		}

		public static List<string> ToListOfNames(this List<Component> list) {

			if (!Any(list)) {
				return new List<string>();
			}
			List<string> newTypeList = new List<string>();
			for(int l = 0; l < list.Count; l++) {
				newTypeList.Add(list[l].GetType().Name);
			}

			return newTypeList;

		}

		public static List<Type> Transmute(this List<GameObject> list, Type component) {

			if (!Any(list)) {
				return new List<Type>();
			}
			List<Type> newTypeList = new List<Type>();
			for(int l = 0; l < list.Count; l++) {
				newTypeList.Add(list[l].GetComponent(component).GetType());
			}

			return newTypeList;

		}

		public static List<GameObject> Transmute(this List<UnityEngine.Object> list) {

			if (!Any(list)) {
				return new List<GameObject>();
			}
			List<GameObject> results = new List<GameObject>();
			for(int l = 0; l < list.Count; l++) {
				results.Add(list[l] as GameObject );
			}

			return results;

		}

		public static List<T> AddAt<T>(this List<T> list, int index, T item) {

			List<T> newList = new List<T>();
			if(index == list.Count) {

				newList = list;
				newList.Add(item);
				return newList;

			}

			for(int x = 0; x < list.Count; x++) {

				if(x == index) {
					newList.Add(item);
				}

				newList.Add(list[x]);

			}
			return newList;

		}

		public static List<T> ReplaceAt<T>(this List<T> list, int index, T item) {

			List<T> newList = new List<T>();
			for(int x = 0; x < list.Count; x++) {

				if(x == index) {

					newList.Add(item);

				} else {

					newList.Add(list[x]);

				}

			}
			return newList;

		}

		public static List<string> RemoveNullAndEmpty(this List<string> list) {

			if (!Any(list)) {
				return new List<string>();
			}
			List<string> results = new List<string>();
			for(int l = 0; l < list.Count; l++) {
				if(list[l] != default(string) && list[l] != string.Empty) {
					results.Add(list[l]);
				}
			}

			return results;

		}

		public static List<T> RemoveNulls<T>(this List<T> list) {

			if (!Any(list)) {
				return new List<T>();
			}
			List<T> results = new List<T>();
			for(int l = 0; l < list.Count; l++) {

				if(list[l] != null) {

					results.Add(list[l]);

				}

			}

			return results;

		}

		public static List<T> ExtractListOfValuesFromKeyValList<X,T>(this List<KeyValuePair<X,T>> list) {

			if (!Any(list)) {
				new List<T>();
			}
			List<T> valueList = new List<T>();
			for(int l = 0; l < list.Count; l++) {

				valueList.Add(list[l].Value);

			}

			return valueList;

		}

		public static List<T> ExtractListOfKeysFromKeyValList<T,X>(this List<KeyValuePair<T,X>> list, bool isKeyDelimited = false, int delimitedSplitIndexToReturn = 0) {

			if(!Any(list)) {

				new List<T>();

			}
			List<T> keyList = new List<T>();
			for(int l = 0; l < list.Count; l++) {

				if(typeof(T) == typeof(String) && isKeyDelimited) {

					string[] pieces = ((string)(object)list[l].Key).Split(AutomationMaster.DELIMITER);
					keyList.Add((T)(object)pieces[delimitedSplitIndexToReturn]);

				} else {

					keyList.Add(list[l].Key);

				}

			}

			return keyList;

		}

		public static bool KeyValListContainsKey<T>(this List<KeyValuePair<string,T>> list, string val, bool keyContains = false) {

			if(!Any(list)) {

				return false;

			}
			for(int l = 0; l < list.Count; l++) {

				if(keyContains) {

					if(list[l].Key.Contains(val)) {

						return true;

					}

				} else {

					if(list[l].Key == val) {

						return true;

					}

				}

			}

			return false;

		}

		/// <summary>
		/// Return all of the children objects in the provided GameObject(s).
		/// </summary>
		/// <returns>The children of these.</returns>
		/// <param name="objs">Objects.</param>
		public static List<GameObject> GetChildren(this List<GameObject> objs) {

			List<GameObject> results = new List<GameObject>();
			foreach(GameObject obj in objs) {

				results.Add(obj);
				results.AddRange(obj.GetChildren());

			}

			return results;

		}

		public static List<GameObject> GetChildren(this List<Component> objs) {

			List<GameObject> results = new List<GameObject>();
			foreach(Component obj in objs) {

				results.Add(obj.gameObject);
				results.AddRange(obj.GetChildren());

			}

			return results;

		}

		public static List<GameObject> GetChildren(this GameObject obj) {

			return GetChildrenFromTop(obj);

		}

		public static List<GameObject> GetChildren(this Component obj) {

			return GetChildrenFromTop(obj.gameObject);

		}

		private static List<GameObject> GetChildrenFromTop(GameObject obj) {

			List<GameObject> objs = new List<GameObject>();
			if(obj != null) {

				GetChildrenRecursive(obj, ref objs);

			}
			return objs;

		}
		private static void GetChildrenRecursive(GameObject transformForSearch, ref List<GameObject> objs) {

			foreach(Transform trans in transformForSearch.transform) {

				objs.Add(trans.gameObject);
				GetChildrenRecursive(trans.gameObject, ref objs);

			} 

		}

		/// <summary>
		/// Return only active and visible objects from the provided set.
		/// </summary>
		/// <returns>The active and visible objects in list.</returns>
		public static List<GameObject> GetActiveAndVisibleObjectsInList(this List<GameObject> objs) {

			List<GameObject> results = new List<GameObject>();
			for(int x = 0; x < objs.Count; x++) {

				if(Q.driver.IsActiveVisibleAndInteractable(objs[x])) {
					
					results.Add(objs[x]);

				}

			}

			return results;

		}
		public static List<GameObject> GetActiveAndVisibleObjectsInList(this List<Component> objs) {

			List<GameObject> results = new List<GameObject>();
			for(int x = 0; x < objs.Count; x++) {

				if(Q.driver.IsActiveVisibleAndInteractable(objs[x].gameObject)) {
					
					results.Add(objs[x].gameObject);

				}

			}

			return results;

		}

		/// <summary>
		/// Return unique values between two lists.
		/// </summary>
		/// <returns>The unique objects between two lists.</returns>
		/// <param name="listOne">List one.</param>
		/// <param name="listTwo">List two.</param>
		public static List<T> GetUniqueObjectsBetween<T>(this List<T> thisList, List<T> otherList) {

			List<T> unique = new List<T>();
			for(int i = 0; i < thisList.Count; i++) {
				
				if(!otherList.Contains(thisList[i])){
					
					unique.Add(thisList[i]);

				}

			}

			for(int x = 0; x < otherList.Count; x++) {
				
				if(!thisList.Contains(otherList[x])){
					
					unique.Add(otherList[x]);

				}

			}

			return unique;

		}

		#endregion

	}

}