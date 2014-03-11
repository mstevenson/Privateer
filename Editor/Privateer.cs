using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System;


public enum UnityNamespace
{
	TreeEditor,
	UnityEditor,
	UnityEditorInternal,
	UnityEngine,
	UnityEngineInternal
}


/// <summary>
/// Manipulate Unity's private fields, properties, and methods.
/// </summary>
public static class Privateer {
	
	#region Type Discovery
	
	/// <summary>
	/// Find a private or internal type using a public type in the same namespace as a reference point.
	/// </summary>
	public static Type GetType (Type siblingType, string targetTypeName)
	{
		Assembly asm = Assembly.GetAssembly (siblingType);
		return asm.GetType (String.Format ("{0}.{1}", siblingType.Namespace, targetTypeName));
	}
	
	/// <summary>
	/// Find a private or internal type within a given Unity namespace.
	/// </summary>
	public static Type GetType (UnityNamespace ns, string targetTypeName)
	{
		Assembly asm = GetUnityAssembly (ns);
		return asm.GetType (String.Format ("{0}.{1}", ns.ToString (), targetTypeName), true);
	}

	/// <summary>
	/// Get all public, private, and internal types from the given Unity namespace.
	/// </summary>
	public static Type[] GetAllTypes (UnityNamespace ns)
	{
		Assembly asm = GetUnityAssembly (ns);
		return asm.GetTypes ();
	}

	/// <summary>
	/// Gets the Unity Assembly associated with the given namespace.
	/// </summary>
	private static Assembly GetUnityAssembly (UnityNamespace ns)
	{
		// The types passed to GetAssembly are arbitrary, we just
		// need a reference to a public type within each namespace
		// in order to retrieve a reference to its containing assembly
		Assembly asm;
		switch (ns) {
		case UnityNamespace.UnityEditor:
			asm = Assembly.GetAssembly (typeof(UnityEditor.Editor));
			break;
		case UnityNamespace.TreeEditor:
			asm = Assembly.GetAssembly (typeof(TreeEditor.TreeData));
			break;
		case UnityNamespace.UnityEditorInternal:
			asm = Assembly.GetAssembly (typeof(UnityEditorInternal.AssetStore));
			break;
		case UnityNamespace.UnityEngine:
			asm = Assembly.GetAssembly (typeof(UnityEngine.Types));
			break;
		case UnityNamespace.UnityEngineInternal:
			asm = Assembly.GetAssembly (typeof(UnityEngineInternal.Reproduction));
			break;
		default:
			asm = null;
			break;
		}
		return asm;
	}
	
	#endregion
	
	
	#region Method Invocation
	
	/// <summary>
	/// Invoke a static method implemented by the type T.
	/// </summary>
	public static object Invoke<T> (string methodName, params object[] parameters)
	{
		MethodInfo method = typeof(T).GetMethod (methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
		return method.Invoke (null, parameters);
	}
	
	/// <summary>
	/// Invoke a static method implemented by a known type.
	/// </summary>
	public static object Invoke (this Type type, string methodName, params object[] parameters)
	{
		MethodInfo method = type.GetMethod (methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
		return method.Invoke (null, parameters);
	}

	/// <summary>
	/// Invoke a static method implemented by a type that is found by its namespace and type name.
	/// </summary>
	public static object Invoke (UnityNamespace ns, string typeName, string methodName, params object[] parameters)
	{
		var assembly = GetUnityAssembly (ns);
		System.Type type = assembly.GetType (typeName, true);
		MethodInfo method = type.GetMethod (methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
		return method.Invoke (null, parameters);
	}
	
	/// <summary>
	/// Extension method to invoke a method implemented by type T.
	/// </summary>
	public static object Invoke<T> (this T instance, string methodName, params object[] parameters)
	{
		var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
		MethodInfo method = typeof(T).GetMethod (methodName, flags);
		return method.Invoke (instance, parameters);
	}
	
	#endregion
	

	#region Getters
	
	/// <summary>
	/// Get the value of a static field or property
	/// </summary>
	/// <typeparam name='T'>
	/// The type that contains the given field
	/// </typeparam>
	/// <typeparam name='U'>
	/// The field or property's return type
	/// </typeparam>
	public static U Get<T, U> (string fieldName)
	{
		var flags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
		return (U)typeof(T).InvokeMember (fieldName, flags, null, null, null);
	}
	
	/// <summary>
	/// Get the value of a static field or property
	/// </summary>
	/// <typeparam name='U'>
	/// The field or property's return type
	/// </typeparam>
	public static U Get<U> (Type type, string fieldName)
	{
		var flags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
		return (U)type.InvokeMember (fieldName, flags, null, null, null);
	}
	
	/// <summary>
	/// Extension method to get the value of a field or property
	/// </summary>
	public static U Get<T, U> (this T instance, string fieldName)
	{
		var flags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		return (U)typeof(T).InvokeMember (fieldName, flags, null, instance, null);
	}
	
	#endregion
	
	
	#region Setters
	
	/// <summary>
	/// Set the value of a static field or property
	/// </summary>
	public static void Set<T> (string fieldName, object val)
	{
		var flags = BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
		typeof(T).InvokeMember (fieldName, flags, null, null, new System.Object[] { val });
	}
	
	/// <summary>
	/// Set the value of a static field or property
	/// </summary>
	public static void Set (Type type, string fieldName, object val)
	{
		var flags = BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
		type.InvokeMember (fieldName, flags, null, null, new System.Object[] { val });
	}
	
	/// <summary>
	/// Extension method to set the value of a instanced field or property
	/// </summary>
	public static void Set<T> (this T instance, string fieldName, object val)
	{
		var flags = BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		typeof(T).InvokeMember (fieldName, flags, null, instance, new System.Object[] { val });
	}
	
	#endregion
		
}
