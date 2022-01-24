/*
 * Manages a collection of games
 */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SeriesSessionManager : GMonoBehaviour
{
    public static float sPerMicroGame = 8f;
    
    // Iff we're in edit mode, allow the scene to be reloaded
    #if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }
    }
    #endif
    
}