using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolLink: MonoBehaviour
{
    private GObjectPool _pool;
    private bool released;

    public GObjectPool Pool
    {
        get => _pool;
        set => _pool = value;
    }

    private void OnEnable()
    {
        released = false;
    }

    public void ReleaseSelf()
    {
        if (!released)
        {
            _pool.Release(gameObject);
            released = true;
            gameObject.SetActive(false);
        }
    }

}