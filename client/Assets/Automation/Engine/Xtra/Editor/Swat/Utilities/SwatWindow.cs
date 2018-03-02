using UnityEngine;
using System.Collections;

public abstract class SwatWindow {

	public abstract void Set(); //Sets data that does not need to be changed with each render pass. Ran only once when window is first opened.
	public abstract void Render(); //Contains GUI elements to render and logic to perform with each pass. Effectively OnGUI.
	public abstract void OnTabSelected(); //Sets data that does not need to be changed with each render pass. Ran every time the tab is selected or re-selected.
	public abstract bool UpdateWhenNotInFocus(); //Should the window update/repaint when it is not in focus.

}