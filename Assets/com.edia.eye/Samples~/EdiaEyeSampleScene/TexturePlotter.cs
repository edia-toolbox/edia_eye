using UnityEngine;

/// <summary>
/// A class to plot points on a texture.
/// </summary>
/// <remarks>
/// This class is used to plot points on a texture for debugging purposes.
/// </remarks>
/// <seealso cref="Texture2D"/>
/// <seealso cref="Color"/>
[EdiaHeader("EDIA EYE", "Texture Plotter", "Plots points on a texture for debugging purposes.")]
public class TexturePlotter : MonoBehaviour {
    
    private Texture2D texture;
    private int       textureWidth  = 512;
    private int       textureHeight = 512;

    public bool DrawThickPixels = false;
    public Color BaseColor = Color.white;
    public Color DrawColor = Color.black;
    
    void Awake() {
        
        texture            = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode   = TextureWrapMode.Clamp;

        GetComponent<Renderer>().material.mainTexture = texture;
        ClearTexture();
    }

    private void ClearTexture() {
        Color[] colors = new Color[textureWidth * textureHeight];
        
        for (int i = 0; i < colors.Length; i++) {
            colors[i] = BaseColor;
        }

        texture.SetPixels(colors);
        texture.Apply();
    }

    /// <summary>
    /// Plots a point on the texture using UV coordinates (range 0-1)
    /// </summary>
    /// <param name="u">U coordinate (0-1)</param>
    /// <param name="v">V coordinate (0-1)</param>
    public void PlotPoint(Vector2 uv) {
        float u = Mathf.Clamp01(uv.x);
        float v = Mathf.Clamp01(uv.y);

        int x = Mathf.FloorToInt(u * (textureWidth - 1));
        int y = Mathf.FloorToInt(v * (textureHeight - 1));

        if (DrawThickPixels) {
            // Draw center pixel and surrounding pixels in a + pattern
            texture.SetPixel(x, y, DrawColor);
            texture.SetPixel(x - 1, y, DrawColor);
            texture.SetPixel(x + 1, y, DrawColor);
            texture.SetPixel(x, y - 1, DrawColor);
            texture.SetPixel(x, y + 1, DrawColor);
        }
        else {
            texture.SetPixel(x, y, DrawColor);
        }

        texture.Apply();
    }
}