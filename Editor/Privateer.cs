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
/// Manipulate private fields, properties, and classes
/// </summary>
public static class Privateer {
	
	#region Class Discovery
	
	/// <summary>
	/// Find a private class in an assembly using a public class as a reference point.
	/// </summary>
	public static Type GetType (Type referenceClass, string targetClass)
	{
		Assembly asm = Assembly.GetAssembly (referenceClass);
		return asm.GetType (String.Format ("{0}.{1}", referenceClass.Namespace, targetClass));
	}
	
	
	/// <summary>
	/// Find a private class within a given Unity namespace.
	/// </summary>
	/// <remarks>
	/// We must use a reference to a public class as an anchor when searching
	/// for a private class within the same assembly. The classes we're referencing
	/// should hopefully not change with future Unity updates.
	/// </remarks>
	public static Type GetType (UnityNamespace ns, string targetClass)
	{
		Assembly asm = UnityAssembly (ns);
		return asm.GetType (String.Format ("{0}.{1}", ns.ToString (), targetClass));
	}
	
	
	public static Type[] GetTypes (UnityNamespace ns)
	{
		Assembly asm = UnityAssembly (ns);
		return asm.GetTypes ();
	}
	
	
	private static Assembly UnityAssembly (UnityNamespace ns)
	{
		Assembly asm;
		switch (ns) {
		case UnityNamespace.UnityEditor:
			asm = Assembly.GetAssembly (typeof(UnityEditor.Editor));
			break;
		case UnityNamespace.TreeEditor:
			asm = Assembly.GetAssembly (typeof(TreeEditor.TreeData));
			break;
		case UnityNamespace.UnityEditorInternal:
			asm = Assembly.GetAssembly (typeof(UnityEditorInternal.Macros));
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
	/// Invoke a static method
	/// </summary>
	public static object Invoke<T> (string methodName, params object[] parameters)
	{
		MethodInfo method = typeof(T).GetMethod (methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
		return method.Invoke (null, parameters);
	}
	
	
	/// <summary>
	/// Invoke a static method
	/// </summary>
	public static object Invoke (this Type type, string methodName, params object[] parameters)
	{
		MethodInfo method = type.GetMethod (methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
		return method.Invoke (null, parameters);
	}
	
	
	/// <summary>
	/// Extension method to invoke an instanced method
	/// </summary>
	public static object Invoke<T> (this T instance, string methodName, params object[] parameters)
	{
		var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
		MethodInfo method = typeof(T).GetMethod (methodName, flags);
		return method.Invoke (instance, parameters);
	}
	
	#endregion
	

	#region Setters
	
	/// <summary>
	/// Return the value of a static field or property
	/// </summary>
	public static object Get<T> (string fieldName)
	{
		var flags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
		return typeof(T).InvokeMember (fieldName, flags, null, null, null);
	}
	
	
	/// <summary>
	/// Return the value of a static field or property
	/// </summary>
	public static object Get (Type type, string fieldName)
	{
		var flags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
		return type.InvokeMember (fieldName, flags, null, null, null);
	}
	
	
	/// <summary>
	/// Return the value of a instanced field or property
	/// </summary>
	public static object Get<T> (this T instance, string fieldName)
	{
		var flags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		return typeof(T).InvokeMember (fieldName, flags, null, instance, null);
	}
	
	#endregion
	
	
	#region Getters
	
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
	/// Set the value of a instanced field or property
	/// </summary>
	public static void Set<T> (this T instance, string fieldName, object val)
	{
		var flags = BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		typeof(T).InvokeMember (fieldName, flags, null, instance, new System.Object[] { val });
	}
	
	#endregion
		
}
