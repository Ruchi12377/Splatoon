using UnityEngine;

public class PaintObject : MonoBehaviour
{
    private MeshRenderer meshRenderer = null;
    private Texture2D drawTexture = null;
    private Texture2D maskTexture = null;
    private Color[] buffer = new Color[0];
    private Color[] gray = new Color[0];
    private int width = 0;
    private int height = 0;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        Texture2D mainTexture = (Texture2D)meshRenderer.material.GetTexture("_SubTex");
        maskTexture = (Texture2D)meshRenderer.material.GetTexture("_MaskTex");
        width = mainTexture.width;
        height = mainTexture.height;
        Color[] pixels = mainTexture.GetPixels();
        Color[] maskpixels = maskTexture.GetPixels();
        buffer = new Color[pixels.Length];
        pixels.CopyTo(buffer, 0);
        gray = new Color[maskpixels.Length];
        maskpixels.CopyTo(gray, 0);

        drawTexture = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        maskTexture = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;
        maskTexture.filterMode = FilterMode.Point;
    }

    public bool Draw(Vector2 p, Color col ,float size)
    {
        bool state = false;
        p *= (height + width) / 2;
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Vector2 pos = new Vector2(x, y);
                float range = (p - pos).magnitude;
                if (range < size * 0.625f)
                {
                    buffer.SetValue(col, x + height * y);
                    gray.SetValue(Color.white, x + height * y);
                    state = true;
                }
            }
        }
        drawTexture.SetPixels(buffer);
        drawTexture.Apply();
        meshRenderer.material.SetTexture("_SubTex", drawTexture);

        maskTexture.SetPixels(gray);
        maskTexture.Apply();
        meshRenderer.material.SetTexture("_MaskTex", maskTexture);
        return state;
    }

    public Color[] GetSubTexColor()
    {
        return buffer;
    }
}
