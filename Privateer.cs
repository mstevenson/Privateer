// MIT License
//
// Copyright (c) 2013 Michael Stevenson
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Reflection;

/// <summary>
/// Manipulate Unity's private fields, properties, and methods.
/// </summary>
public static class Privateer
{
    private const BindingFlags AccessibilityFlags = BindingFlags.NonPublic | BindingFlags.Public;
    private const BindingFlags InvocationFlags = AccessibilityFlags | BindingFlags.Static;
    private const BindingFlags GetterFlags = AccessibilityFlags | BindingFlags.GetField | BindingFlags.GetProperty;
    private const BindingFlags SetterFlags = AccessibilityFlags | BindingFlags.SetField | BindingFlags.SetProperty;
    
    public enum UnityNamespace
    {
        TreeEditor,
        UnityEditor,
        UnityEditorInternal,
        UnityEngine,
        UnityEngineInternal
    }
    
    /// <summary>
    /// Find a private or internal type using a public type in the same namespace as a reference point.
    /// </summary>
    public static Type GetType(Type siblingType, string targetTypeName)
    {
        return Assembly.GetAssembly(siblingType)?.GetType($"{siblingType.Namespace}.{targetTypeName}");
    }

    /// <summary>
    /// Find a private or internal type within a given Unity namespace.
    /// </summary>
    public static Type GetType(UnityNamespace ns, string targetTypeName)
    {
        return GetUnityAssembly(ns)?.GetType($"{ns.ToString()}.{targetTypeName}", true);
    }

    /// <summary>
    /// Get all public, private, and internal types from the given Unity namespace.
    /// </summary>
    public static Type[] GetAllTypes(UnityNamespace ns)
    {
        return GetUnityAssembly(ns)?.GetTypes();
    }

    /// <summary>
    /// Gets the Unity Assembly associated with the given namespace.
    /// </summary>
    private static Assembly GetUnityAssembly(UnityNamespace ns)
    {
        // The types passed to GetAssembly are arbitrary, we just
        // need a reference to a public type within each namespace
        // in order to retrieve a reference to its containing assembly
        Assembly asm;
        switch (ns)
        {
            case UnityNamespace.UnityEditor:
                #if UNITY_EDITOR
                asm = Assembly.GetAssembly(typeof(UnityEditor.Editor));
                #else
                Debug.LogError($"UnityEditor assembly is not accessible in builds.");
                #endif
                break;
            case UnityNamespace.TreeEditor:
                #if UNITY_EDITOR
                asm = Assembly.GetAssembly(typeof(TreeEditor.TreeData));
                #else
                Debug.LogError($"TreeEditor assembly is not accessible in builds.");
                #endif
                break;
            case UnityNamespace.UnityEditorInternal:
                #if UNITY_EDITOR
                asm = Assembly.GetAssembly(typeof(UnityEditorInternal.AssetStore));
                #else
                Debug.LogError($"UnityEditorInternal assembly is not accessible in builds.");
                #endif
                break;
            case UnityNamespace.UnityEngine:
                asm = Assembly.GetAssembly(typeof(UnityEngine.Types));
                break;
            case UnityNamespace.UnityEngineInternal:
                asm = Assembly.GetAssembly(typeof(UnityEngineInternal.MathfInternal));
                break;
            default:
                asm = null;
                break;
        }

        return asm;
    }

    /// <summary>
    /// Invoke a static method implemented by the type T.
    /// </summary>
    public static object Invoke<T>(string methodName, params object[] parameters)
    {
        return typeof(T).GetMethod(methodName, InvocationFlags)?.Invoke(null, parameters);
    }
    
    /// <summary>
    /// Invoke a static method implemented by a known type.
    /// </summary>
    public static object Invoke(this Type type, string methodName, params object[] parameters)
    {
        return type.GetMethod(methodName, InvocationFlags)?.Invoke(null, parameters);
    }

    /// <summary>
    /// Invoke a static method implemented by a type that is found by its namespace and type name.
    /// </summary>
    public static object Invoke(UnityNamespace ns, string typeName, string methodName, params object[] parameters)
    {
        return GetUnityAssembly(ns).GetType(typeName, true).GetMethod(methodName, InvocationFlags)?.Invoke(null, parameters);
    }

    /// <summary>
    /// Extension method to invoke a method implemented by type T.
    /// </summary>
    public static object Invoke<T>(this T instance, string methodName, params object[] parameters)
    {
        return typeof(T).GetMethod(methodName, InvocationFlags | BindingFlags.Instance)?.Invoke(instance, parameters);
    }

    /// <summary>
    /// Get the value of a static field or property
    /// </summary>
    /// <typeparam name='T'>
    /// The type that contains the given field
    /// </typeparam>
    /// <typeparam name='T2'>
    /// The field or property's return type
    /// </typeparam>
    public static T2 Get<T, T2>(string fieldName)
    {
        return (T2)typeof(T).InvokeMember(fieldName, GetterFlags | BindingFlags.Static, null, null, null);
    }

    /// <summary>
    /// Get the value of a static field or property
    /// </summary>
    /// <typeparam name='T2'>
    /// The field or property's return type
    /// </typeparam>
    public static T2 Get<T2>(Type type, string fieldName)
    {
        return (T2)type.InvokeMember(fieldName, GetterFlags | BindingFlags.Static, null, null, null);
    }

    /// <summary>
    /// Extension method to get the value of a field or property
    /// </summary>
    public static T2 Get<T, T2>(this T instance, string fieldName)
    {
        return (T2)typeof(T).InvokeMember(fieldName, GetterFlags | BindingFlags.Instance, null, instance, null);
    }

    /// <summary>
    /// Set the value of a static field or property
    /// </summary>
    public static void Set<T>(string fieldName, object val)
    {
        typeof(T).InvokeMember(fieldName, SetterFlags, null, null, new[] {val});
    }

    /// <summary>
    /// Set the value of a static field or property
    /// </summary>
    public static void Set(Type type, string fieldName, object val)
    {
        type.InvokeMember(fieldName, SetterFlags | BindingFlags.Static, null, null, new[] {val});
    }

    /// <summary>
    /// Extension method to set the value of a instanced field or property
    /// </summary>
    public static void Set<T>(this T instance, string fieldName, object val)
    {
        typeof(T).InvokeMember(fieldName, SetterFlags | BindingFlags.Instance, null, instance, new[] {val});
    }
}
