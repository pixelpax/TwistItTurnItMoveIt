using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxyTrigger2D : MonoBehaviour
{
    // [SerializeField] public delegate void collisionEntered(Collider2D other);
    public Action<Collider2D> triggerEnterAction;
    public Action<Collider2D> triggerExitAction;
    public Action<Collider2D> triggerStayAction;

    private void OnEnable()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerEnterAction != null)
        {
            triggerEnterAction.Invoke(other);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (triggerExitAction != null)
        {
            triggerExitAction.Invoke(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (triggerStayAction != null)
        {
            triggerStayAction.Invoke(other);
        }
    }
}
