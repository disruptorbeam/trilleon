using UnityEditor;

public abstract class SwatPopup : EditorWindow {

	public virtual void Pop() {} //Creates new window and calls Set to render. Any number of parameters.
	public virtual void Set() {} //Sets data that does not need to be changed with each render pass. Should call PositionWindow once. Any number of parameters.

	public abstract void PositionWindow(); //Sets position of window within the context of its parent caller.
	public abstract void OnGUI(); //Actual renderable GUI code.
	public abstract bool Visible(); //Is the window currently visible?

}