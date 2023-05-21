using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Utilities
{
    public static class Extensions
        {
            
            public static List<Transform> GetChildrenFirstDepth(this Transform transform)
            {
                return GetComponentsInChildrenFirstDepthWithoutTheParent<Transform>(transform);
            }
            public static List<Transform> GetChildrenRecursively(this Transform transform, List<Transform> children = null) 
            {
                return GetComponentsInChildrenRecursivelyWithoutTheParent<Transform>(transform);
            }

            public static List<T> GetComponentsInChildrenFirstDepthWithoutTheParent<T>(this Transform transform) where T : Object
            {
                List<T> children = new List<T>();

                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).TryGetComponent(out T component))
                    {
                        children.Add(component);
                    }
                }

                return children;
            }
            public static List<T> GetComponentsInChildrenFirstDepthWithoutTheParent<T>(this RectTransform rectTransform) where T : Object
            {
                List<T> children = new List<T>();

                for (int i = 0; i < rectTransform.childCount; i++)
                {
                    if (rectTransform.GetChild(i).TryGetComponent(out T component))
                    {
                        children.Add(component);
                    }
                }

                return children;
            }
            public static List<T> GetComponentsInChildrenRecursivelyWithoutTheParent<T>(this Transform transform, List<T> children = null) where T : Object
            {
                if (children == null) children = new List<T>();

                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);

                    child.GetComponentsInChildrenRecursivelyWithoutTheParent(children);
                    if (child.TryGetComponent(out T TChild))
                    {
                        children.Add(TChild);
                    }
                }

                return children;
            }

            public static void DestroyChildren(this Transform transform)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(transform.GetChild(i).gameObject);
                    }
                    else
                    {
                        Object.Destroy(transform.GetChild(i).gameObject);
                    }
                }

                List<Transform> a = new List<Transform>();
            }

            public static void ResetVelocities(this Rigidbody rigidbody)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }

            public static void ForEach<T>(this IList<T> target, Action<T> action)
            {
                for (int i = 0; i < target.Count; i++)
                {
                    action.Invoke(target[i]);
                }
            }
            
            public static void ForInRange<T>(this IList<T> target, Action<int, T> action)
            {
                for (int i = 0; i < target.Count; i++)
                {
                    action.Invoke(i, target[i]);
                }
            }

            public static String SeparateContent(this String @string)
            {
                return string.Concat(@string.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
            }
        }
}

    