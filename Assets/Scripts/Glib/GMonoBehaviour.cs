using System;
using System.Collections.Generic;
using NativeExtensions;
using UnityEngine;

public class GMonoBehaviour : MonoBehaviour
{
    public T C<T>() where T: Component
    {
        // TODO: Option to cache?
        return GetComponent<T>();
    }

    public static void ReleaseOrDestroy(GameObject go)
    {
        var poolLink = go.C<PoolLink>();
        if (poolLink != null)
        {
            poolLink.ReleaseSelf();
        }
        else
        {
            Destroy(go);
        }
    }
    
    
    // // TEMP
    // private Dictionary<string, object> componentCache;
    // public T CacheProp<T>(string key, Func<T> make)
    // {
    //     if (componentCache == null)
    //     {
    //         componentCache = new Dictionary<string, object>();
    //     }
    //
    //     if (!componentCache.ContainsKey(key))
    //     {
    //         componentCache[key] = make.Invoke();
    //     }
    //
    //     return (T)componentCache[key];
    // }
}
