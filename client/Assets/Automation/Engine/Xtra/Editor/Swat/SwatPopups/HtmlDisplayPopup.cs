using UnityEditor;
using System;
using System.Reflection;

namespace TrilleonAutomation {

	/// <summary>
	/// Open a webview within Unity editor. (similar as AssetStore window)
	/// </summary>
	public class HtmlDisplayPopup : SwatPopup {

		static string Url { get; set; }
		static BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
		static Type webView;
		static Assembly editorAssembly = typeof(Editor).Assembly;

		public override bool Visible() {

			return IsVisible;

		}
		bool IsVisible { get; set; }

		public override void PositionWindow() { }

		public static void Pop(string url) {
            
			GetWindow<HtmlDisplayPopup>().Close();
			CreateInstance<HtmlDisplayPopup>().Set(url);

		}
		public void Set(string url) {

			Url = url;
			webView = editorAssembly.GetType("UnityEditor.Web.WebViewEditorWindowTabs");
			var methodInfo = webView.GetMethod("Create", Flags);
			methodInfo = methodInfo.MakeGenericMethod(webView);
			methodInfo.Invoke(null, new object[] { "Trilleon", Url, 9999, 9999, 9999, 9999 });

		}

		public override void OnGUI() { }

	}

}