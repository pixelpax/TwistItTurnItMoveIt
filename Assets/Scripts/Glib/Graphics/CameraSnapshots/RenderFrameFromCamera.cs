using System.IO;
// using Sirenix.OdinInspector;
using UnityEngine;
 
/// <summary>
/// Used to capture a rendered frame from the scene
/// Attach to a camera and also do Context>Create>Render Texture and add the render texture to the Camera
/// You could probably make a child Camera in order to get this working right
/// </summary>
public class  RenderFrameFromCamera: MonoBehaviour
{
    public RenderTexture rt;

    public string path;
    // Use this for initialization
    // [Button]
    public void SaveTexture () {
        byte[] bytes = toTexture2D(rt).EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
    }
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width,rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}