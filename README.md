Privateer
=========

Privateer is a Unity Editor tool that simplifies the use of private and internal Unity Editor methods, properties, and variables.

Examples
========

Invocation
----------

Invoke a private instance method on a public type:
	
	SceneView sceneView = EditorWindow.GetWindow<SceneView> ();
	sceneView.Invoke ("SetSearchFilter", "t:Collider", 0, true);
	
Invoke a private static method on a public type:

	Privateer.Invoke<EditorUtility>
	
Invoke a private method on a private type:

	System.Type internalEditorUtility = Privateer.GetType (UnityNamespace.UnityEditorInternal, "InternalEditorUtility");
	Privateer.Invoke (internalEditorUtility, "SwitchSkinAndRepaintAllViews");

Invoke an instance method on a public type:

	


Getters
-------

Get a private static field in a private type:

	
	
Get a private instance field in a public type:
	
	SceneView sceneView = EditorWindow.GetWindow<SceneView> ();
	float dist = sceneView.Get<SceneView, float> ("cameraDistance");


Setters
-------

Set a private static field in a public type:

	Privateer.Set<SceneView> ("kSceneViewMidLight", Color.red);