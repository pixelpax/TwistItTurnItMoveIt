using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxyTrigger : MonoBehaviour
{
    // [SerializeField] public delegate void collisionEntered(Collider2D other);
    public Action<Collider> triggerEnterAction;
    public Action<Collider> triggerExitAction;
    public Action<Collider> triggerStayAction;

    private void OnEnable()
    {
        // Force to register existing intersections
        GetComponent<Collider>().enabled = false;
        GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerEnterAction != null)
        {
            triggerEnterAction.Invoke(other);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (triggerExitAction != null)
        {
            triggerExitAction.Invoke(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (triggerStayAction != null)
        {
            triggerStayAction.Invoke(other);
        }
    }
}