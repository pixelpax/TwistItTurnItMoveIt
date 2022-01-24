using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public class GObjectPool
{
    // Might be able to do this faster with a dict
    private List<GameObject> availableMembers;
    private List<GameObject> allocatedMembers;

    private GameObject myPfab;

    private int sizeIncreaseIncrement = 15;
    private readonly Transform parent;

    private GObjectPool(GameObject pfab, GameObject parentObject)
    {
        myPfab = pfab;
        parent = parentObject.transform;
        allocatedMembers = new List<GameObject>();
        availableMembers = new List<GameObject>();
    }
    
    public void Release(GameObject member)
    {
        allocatedMembers.Remove(member);
        availableMembers.Add(member);
        if (availableMembers.Count < allocatedMembers.Count)
        {
            availableMembers.Capacity += sizeIncreaseIncrement;
            for (int i = 0; i < sizeIncreaseIncrement; i++)
            {
                availableMembers.Add(GameObject.Instantiate(myPfab));
            }
        }
    }

    public static GObjectPool CreatePrefabPool(GameObject pfab, GameObject parentObject, int startSize = 20)
    {
        var gop = new GObjectPool(pfab, parentObject);
        gop.IncreasePoolSize(startSize);
        return gop;
    }

    private void IncreasePoolSize(int amount)
    {
        availableMembers.Capacity += amount;
        allocatedMembers.Capacity += amount;
        for (int i = 0; i < amount; i++)
        {
            var go = GameObject.Instantiate(myPfab, parent);
            go.SetActive(false);
            availableMembers.Add(go);
        }
    }

    public static GObjectPool CreatePrefabPool(GameObject pfab, Action<GameObject> Initialize)
    {
        throw new System.NotImplementedException();
    }

    public GameObject Acquire()
    {
        var member = availableMembers.FirstOrDefault();
        if (member == null)
        {
            throw new KeyNotFoundException($"Tried to acquire new instance of {myPfab.name} from object pool but none were available");
        }
        
        allocatedMembers.Add(member);
        availableMembers.Remove(member);

        // Do async?
        if (availableMembers.Count < allocatedMembers.Count)
        {
            IncreasePoolSize(sizeIncreaseIncrement);
        }

        return member;
    }
}