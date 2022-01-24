using System;
using System.Reflection;
using UnityEngine;

namespace NativeExtensions
{
    public static class GameObjectExtensions
    {
        public static T C<T>(this GameObject go)
            where T : Component
        {
            return go.GetComponent<T>();
        }

        public static T GetPrivate<T, O>(string fieldName, O o)
        {
            Type typ = typeof(O);
            FieldInfo type = typ.GetField("fieldName", BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)type.GetValue(o);
        }

        public static GameObject[] AllChildren(this GameObject go)
        {
            var allChildren = new GameObject[go.transform.childCount];

            int i = 0;
            //Find all child obj and store to that array
            foreach (Transform child in go.transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }

            return allChildren;
        }
    }
}