using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Locate
{
    private static Dictionary<string, MonoBehaviour> _foundServices;
    
    public static THandle Singleton<THandle>() 
        where THandle : MonoBehaviour
    {
        if (_foundServices == null)
        {
            Init();
        }
        if (_foundServices.ContainsKey(typeof(THandle).Name))
        {
            // TypeCheck
            return ((THandle) _foundServices[typeof(THandle).Name]);
        }
        else
        {
            THandle serviceHandle = Object.FindObjectOfType<THandle>();
            // TODO throw an error if null
            if (serviceHandle == null)
            {
                throw new ArgumentException($"Tried to locate an instance of type {typeof(THandle).Name} but none was found");
            }
            _foundServices[typeof(THandle).Name] = serviceHandle;
            return serviceHandle;
        }
    }
        
    // public static TService Service<THandle, TService>() 
    //     where TService : IService
    //     where THandle : ServiceHandle<TService>
    // {
    //     if (_foundServices.ContainsKey(typeof(TService).Name))
    //     {
    //         // TypeCheck
    //         return ((THandle)_foundServices[typeof(THandle).Name]).I;
    //     }
    //     else
    //     {
    //         THandle serviceHandle = Object.FindObjectOfType<THandle>();
    //         _foundServices[typeof(THandle).Name] = serviceHandle;
    //         return serviceHandle.I;
    //     }
    // }
    //
    // public static void AndReplaceService<THandle, TService>()
    // where THandle: IServiceHandle
    // where TService: class, IService
    // {
    //     TService service = Substitute.For<TService>();
    //     Service<THandle, TService>() = 
    //     if (_foundServices.ContainsKey(typeof(THandle).Name))
    //     {
    //         _foundServices[typeof(THandle).Name].I = 
    //     }
    // }

    public static void Init()
    {
        // TODO: Invalidate on scene change
        _foundServices = new Dictionary<string, MonoBehaviour>();
    }
}