Examples
========

Invocation
----------

Invoke a private method on a public type:

	Privateer.Invoke<EditorApplication> ("");
	
Invoke a private method on a private type:

	System.Type internalEditorUtility = Privateer.GetType (UnityNamespace.UnityEditorInternal, "InternalEditorUtility");
	Privateer.Invoke (internalEditorUtility, "SwitchSkinAndRepaintAllViews");

Invoke an instance method on a public type:


Getters
-------

Get a private static field in a private type:

	
	
Get a private instance field in a public type:

	Privateer.Get<SceneView, float> ("cameraDistance");


Setters
-------

Set a private static field in a public type:

	Privateer.Set<SceneView> ("kSceneViewMidLight", Color.red);