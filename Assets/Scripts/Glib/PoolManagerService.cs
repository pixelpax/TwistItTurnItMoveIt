using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEditorInternal;
using UnityEngine;

public class PoolManagerService : GMonoBehaviour
{
    
    private Dictionary<string, GObjectPool> pools = new Dictionary<string, GObjectPool>();

    public GameObject Acquire(GameObject pfab)
    {
        if (pfab == null)
        {
            throw new ArgumentNullException("pfab", "Provided invalid prefab to PoolManagerService.Acquire");
        }
        if (!pools.ContainsKey(pfab.name))
        {
            var poolGO = new GameObject(pfab.name + "Pool");
            poolGO.transform.parent = transform;
            pools[pfab.name] = GObjectPool.CreatePrefabPool(pfab,poolGO);
            // pools[pfab.name] = new GObjectPool<GameObject>(() =>
            // {
            //     var newInstance = GameObject.Instantiate(pfab);
            //     
            //     // Ensure self-reference is linked so that this object can remove itself
            //     newInstance.AddComponent<PoolLink>();
            //     newInstance.GetComponent<PoolLink>().Pool = pools[pfab.name];
            //     return newInstance;
            // });
        }
        var newInstance = pools[pfab.name].Acquire();
        newInstance.AddComponent<PoolLink>();
        newInstance.GetComponent<PoolLink>().Pool = pools[pfab.name];
        newInstance.SetActive(true);
        return newInstance;
        
    }
    
    public GameObject Acquire(GameObject pfab, Vector3 position, Quaternion rotation)
    {
        var go = Acquire(pfab);
        go.transform.position = position;
        go.transform.rotation = rotation;
        return go;
    }
}