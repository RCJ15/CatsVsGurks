using UnityEngine;
using ZXing;
using ZXing.QrCode;

public class QRCodeToQuad : MonoBehaviour
{
    [TextArea] public string content = "https://example.com";
    public int size = 512;

    Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        var tex = GenerateQRTexture(content, size, size);
        rend.material.mainTexture = tex;
    }

    Texture2D GenerateQRTexture(string text, int width, int height)
    {
        var writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 2
            }
        };

        var pixelData = writer.Write(text);

        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;     // keep it sharp
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.LoadRawTextureData(pixelData.Pixels);
        tex.Apply();

        return tex;
    }
}

