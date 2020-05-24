using UnityEngine;

//塗られるオブジェクト
public class PaintObject : MonoBehaviour
{
    private MeshRenderer meshRenderer = null;
    private Texture2D drawTexture = null;//インクを塗られるテクスチャー
    private Texture2D maskTexture = null;//マスクテクスチャ
    private Color[] buffer = new Color[0];//drawTextureに反映されるピクセル情報
    private Color[] gray = new Color[0];//maskTextureに反映されるピクセル情報
    private int width = 0;
    private int height = 0;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        Texture2D mainTexture = (Texture2D)meshRenderer.material.GetTexture("_SubTex");
        maskTexture = (Texture2D)meshRenderer.material.GetTexture("_MaskTex");
        width = mainTexture.width;
        height = mainTexture.height;
        Color[] mainpixels = mainTexture.GetPixels();
        Color[] maskpixels = maskTexture.GetPixels();
        buffer = new Color[mainpixels.Length];
        mainpixels.CopyTo(buffer, 0);//bufferにピクセル情報をコピー
        gray = new Color[maskpixels.Length];//grayにピクセル情報をコピー
        maskpixels.CopyTo(gray, 0);

        //texture生成
        drawTexture = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        maskTexture = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;
        maskTexture.filterMode = FilterMode.Point;
    }

    //塗られる処理
    public bool Draw(Vector2 uv, Color col ,float size)
    {
        bool state = false;
        uv.x *= width;//0-1をピクセルに変換する
        uv.y *= height;
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Vector2 pos = new Vector2(x, y);
                float range = (uv - pos).magnitude;//円
                if (range < size * 0.625f)//size以下なら
                {
                    buffer.SetValue(col, x + height * y);
                    gray.SetValue(Color.white, x + height * y);//対象のピクセルを塗りつぶす
                    state = true;
                }
            }
        }
        drawTexture.SetPixels(buffer);//textureにピクセルを割り当てて更新
        drawTexture.Apply();
        meshRenderer.material.SetTexture("_SubTex", drawTexture);

        maskTexture.SetPixels(gray);
        maskTexture.Apply();
        meshRenderer.material.SetTexture("_MaskTex", maskTexture);
        return state;
    }

    //maintextureのピクセル情報を返す
    //パーセンテージを計算するときなどに使う
    public Color[] GetSubTexColor()
    {
        return buffer;
    }
}
