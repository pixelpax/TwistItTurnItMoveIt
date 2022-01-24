using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ColorReduceFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PassSettings
    {
        // Where/when the render pass should be injected during the rendering process.
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        // Used for any potential down-sampling we will do in the pass.
        [Range(1, 4)] public int downsample = 1;

        public Texture2D palette;
        public Texture3D colorMap;
    }

    // References to our pass and its settings.
    ColorReducePass pass;
    public PassSettings passSettings = new PassSettings();

    // Gets called every time serialization happens.
    // Gets called when you enable/disable the renderer feature.
    // Gets called when you change a property in the inspector of the renderer feature.
    public override void Create()
    {
        passSettings.colorMap = MakeMapping3DTextures(passSettings.palette);
        // Pass the settings as a parameter to the constructor of the pass.
        pass = new ColorReducePass(passSettings);
    }

    // Injects one or multiple render passes in the renderer.
    // Gets called when setting up the renderer, once per-camera.
    // Gets called every frame, once per-camera.
    // Will not be called if the renderer feature is disabled in the renderer inspector.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Here you can queue up multiple passes after each other.
        renderer.EnqueuePass(pass);
    }

    private Vector3Int[] directions = new[]
    {
        Vector3Int.up, Vector3Int.down, Vector3Int.back, Vector3Int.forward, Vector3Int.left, Vector3Int.right
    };

    private Texture3D MakeMapping3DTextures(Texture2D palette)
    {
        // Configure the texture
        int size = 32;
        TextureFormat format = TextureFormat.RGBA32;
        TextureWrapMode wrapMode = TextureWrapMode.Clamp;

        // Create the texture and apply the configuration
        Texture3D texture = new Texture3D(size, size, size, format, false);
        texture.wrapMode = wrapMode;

        // Create a 3-dimensional array to store color data
        Color[] mapColors = new Color[size * size * size];

        ////// Get all the colors from the palette texture


        ////// Voronoi bfs algo
        // Store a list of lists of frontiers
        Dictionary<Color, List<HashSet<Vector3Int>>> frontiers
            = new Dictionary<Color, List<HashSet<Vector3Int>>>(); // color -> voxel frontiers

        // And just a plain set of visited
        HashSet<Vector3> visited = new HashSet<Vector3>();

        // Initialize based on the palette colors
        foreach (var paletteColor in palette.GetPixels())
        {
            Vector3 paletteColorV3 = (Vector4)paletteColor * size;

            if (!frontiers.ContainsKey(paletteColor))
            {
                var firstFrontier = new List<HashSet<Vector3Int>>();
                firstFrontier.Add(new HashSet<Vector3Int> { Vector3Int.FloorToInt(paletteColorV3) });
                frontiers[paletteColor] = firstFrontier;
                visited.Add(paletteColorV3);
            }
        }


        // while we had changes in the last round...
        bool changesLastRound = true;
        while (changesLastRound)
        {
            changesLastRound = false;
            // For each frontier voxel v
            foreach (var entry in frontiers)
            {
                var paletteColorFrontiers = entry.Value;
                Color paletteColor = (Vector4)entry.Key;
                var currentFrontier = paletteColorFrontiers.Last();
                HashSet<Vector3Int> newFrontier = new HashSet<Vector3Int>();
                paletteColorFrontiers.Add(newFrontier);
                foreach (var frontierVoxel in currentFrontier)
                {
                    foreach (Vector3Int direction in directions)
                    {
                        var currentCoordinate = frontierVoxel + direction;
                        if (currentCoordinate.x < size && currentCoordinate.x >= 0 && currentCoordinate.y < size &&
                            currentCoordinate.y >= 0 && currentCoordinate.z < size && currentCoordinate.z >= 0 &&
                            !visited.Contains(currentCoordinate)) // TODO: If using alternative distance, check here?
                        {
                            visited.Add(currentCoordinate);
                            newFrontier.Add(currentCoordinate);
                            changesLastRound = true;
                            mapColors[
                                currentCoordinate.x * size * size + currentCoordinate.y * size +
                                currentCoordinate.z] = paletteColor;
                        }
                    }
                }
            }
        }

        // Copy the color values to the texture
        texture.SetPixels(mapColors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();
        return texture;
    }
}