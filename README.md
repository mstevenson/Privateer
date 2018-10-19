Privateer
=========

Privateer is a Unity Editor tool that simplifies the use of private and internal Unity Editor methods, properties, and fields.

Examples
========

Invocation
----------

Invoke a private instance method on a public type:

```csharp
SceneView sceneView = EditorWindow.GetWindow<SceneView>();
sceneView.Invoke("SetSearchFilter", "t:Collider", 0, true);
```
	
Invoke a private method on a private type:

```csharp
System.Type internalEditorUtility = Privateer.GetType(UnityNamespace.UnityEditorInternal, "InternalEditorUtility");
Privateer.Invoke(internalEditorUtility, "SwitchSkinAndRepaintAllViews");
```


Getters
-------

Get a private instance field in a public type:
	
```csharp
SceneView sceneView = EditorWindow.GetWindow<SceneView>();
float dist = sceneView.Get<SceneView, float>("cameraDistance");
```


Setters
-------

Set a private static field in a public type:

```csharp
Privateer.Set<SceneView>("kSceneViewMidLight", Color.red);
```
