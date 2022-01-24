using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class GSceneCompositionToolkit : OdinEditorWindow
{
    [MenuItem("GCustom/GSceneCompositionToolkit")]
    private static void OpenWindow()
    {
        GetWindow<GSceneCompositionToolkit>().Show();
    }

    public void ReplaceSelectedInstances()
    {
        foreach (var go in Selection.gameObjects)
        {
            // TODO: Make sure that replacement prefab exists and is prefab
            var replaced = (GameObject)PrefabUtility.InstantiatePrefab(replacementPrefab, go.transform.parent);
            replaced.transform.localPosition = go.transform.localPosition;
            replaced.transform.localRotation = go.transform.localRotation;
            replaced.name = go.name;
            foreach (Transform child in go.transform)
            {
                child.parent = replaced.transform;
            }

            DestroyImmediate(go);
        }
    }

    [FoldoutGroup("Replace prefabs", true)]
    public GameObject replacementPrefab;


    #region KMeans Pallette Chooser

    [FoldoutGroup("Build Palette", true)] public RenderTexture rt;

    [FoldoutGroup("Build Palette", true)] public Color[] createdPallette;

    private Color[] toColors(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex.GetPixels();
    }


    // The number of destination colors
    [FoldoutGroup("Build Palette", true)] public int k;

    // The number of tries before we pick the best
    [FoldoutGroup("Build Palette", true)] public int nTries;

    /* Constructs a reduced palette from the render texture and parameters */
    [FoldoutGroup("Build Palette", true)]
    [Button]
    public void KMeans()
    {
        var colors = toColors(rt);
        List<Vector3> hsvColors = colors.Select(c =>
            {
                float h, s, v;
                Color.RGBToHSV(c, out h, out s, out v);
                return new Vector3(h, s, v);
            }
        ).ToList();

        Dictionary<List<Vector3>, float> guessToVariance = new Dictionary<List<Vector3>, float>();

        for (int t = 0; t < nTries; t++)
        {
            // Initialize a new set
            List<int> startingClusterIndices = new List<int>();
            for (int i = 0; i < k; i++)
            {
                var newClusterIndex = (int)Mathf.Floor(Random.Range(0, hsvColors.Count()));
                startingClusterIndices.Add(newClusterIndex);
            }

            List<Vector3> clusterHsvGuesses = startingClusterIndices.Select((int i) => hsvColors[i]).ToList();

            List<Vector3> clusterMeansActual = new List<Vector3>();

            for (int i = 0; i < k; i++){
                clusterMeansActual.Add(Vector3.zero);
            }

            bool didAnythingChangeFromLastIteration = true;
            int totalMeanIterations = 0;
            List<Vector3> clusterMeansAggregates;
            while (didAnythingChangeFromLastIteration && totalMeanIterations < 20)
            {
                clusterMeansAggregates = new List<Vector3>();
                List<int> clusterMembershipCount = new List<int>();
                // Initialize
                for (int i = 0; i < k; i++)
                {
                    clusterMembershipCount.Add(0);
                    clusterMeansAggregates.Add(Vector3.zero);
                }
                totalMeanIterations += 1; // failsafe to make sure we don't infinite loop  in case of bug
                foreach (var pixelColor in hsvColors)
                {
                    int closestClusterIndex = -1;
                    var closestClusterDistance = float.MaxValue;
                    Vector3 closestClusterHsv = Vector3.zero;
                    for (int j = 0; j < clusterHsvGuesses.Count; j++)
                    {
                        var clusterHsv = clusterHsvGuesses[j];
                        var currentDistance = Vector3.Distance(clusterHsv, pixelColor);
                        if (currentDistance < closestClusterDistance)
                        {
                            closestClusterIndex = j;
                            closestClusterDistance = currentDistance;
                            closestClusterHsv = clusterHsv;
                        }
                    }

                    clusterMeansAggregates[closestClusterIndex] += closestClusterHsv;
                    clusterMembershipCount[closestClusterIndex] += 1;
                }

                for (int i = 0; i < k; i++)
                {
                    clusterMeansActual[i] = clusterMeansAggregates[i] / clusterMembershipCount[i];
                }


                didAnythingChangeFromLastIteration = false;
                for (int i = 0; i < k; i++)
                {
                    if ((clusterMeansActual[i] - clusterMeansAggregates[i]).magnitude > .001)
                    {
                        didAnythingChangeFromLastIteration = true;
                    }
                }
                
                clusterHsvGuesses = clusterMeansActual;
            }

            // Test variance to test quality of this initial guess
            List<float> distances = new List<float>();
            foreach (var pixelColor in hsvColors)
            {
                var closestClusterDistance = float.MaxValue;
                // Vector3 closestClusterHsv = Vector3.zero;
                for (int j = 0; j < clusterHsvGuesses.Count; j++)
                {
                    var clusterHsv = clusterMeansActual[j];
                    var currentDistance = Vector3.Distance(clusterHsv, pixelColor);
                    if (currentDistance < closestClusterDistance)
                    {
                        closestClusterDistance = currentDistance;
                    }
                }

                distances.Add(closestClusterDistance);
            }

            var totalVariance = distances.Average(d => Mathf.Pow(d, 2));
            guessToVariance.Add(clusterMeansActual, totalVariance);
            Debug.Log($"Finished attempt {t}");
        }
        // Order by variance and take the lowest
        Vector3[] finalClusters = guessToVariance.OrderByDescending((kv) => kv.Value).Last().Key.ToArray();
        var finalColors = finalClusters.Select((hsv) => Color.HSVToRGB(hsv.x, hsv.y, hsv.z)).ToArray();
        createdPallette = finalColors;
    }
    
    /* Constructs a reduced palette from the render texture and parameters */
    [FoldoutGroup("Build Palette", true)]
    [Button]
    public void WriteOutPaletteFile()
    {
        for (int i = 0; i < createdPallette.Length; i++)
        {
            var c = createdPallette[i];
            
            
        }
    }

    
}

#endregion