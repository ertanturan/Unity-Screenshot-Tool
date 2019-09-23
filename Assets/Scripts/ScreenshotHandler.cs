using System;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScreenshotHandler : MonoBehaviour
{

    private string _extension = ".png";
    private string _pictureName = "Screenshot";

    private static string _screenshotPath;

    public static ScreenshotHandler Instance;

    private Camera _camera;

    private bool _captureOnNextFrame;

    private void Awake()
    {
        Instance = this;
        _camera = GetComponent<Camera>();
        _screenshotPath = Path.Combine(Application.persistentDataPath, "Screenshots");
    }

    private void OnPostRender()
    {
        Debug.Log("on post render");
        if (_captureOnNextFrame)
        {
            Debug.Log("capture true");

            RenderTexture rendTexture = _camera.targetTexture;
            Debug.Log("renderexture taken");
            Texture2D result = new Texture2D(rendTexture.width, rendTexture.height,
                TextureFormat.ARGB32, false);

            Rect rect = new Rect(0, 0, rendTexture.width, rendTexture.height);

            result.ReadPixels(rect, 0, 0);

            byte[] byteArray = result.EncodeToPNG();

            File.WriteAllBytes(CapturePath(), byteArray);
            FileStream stream = new FileStream(_screenshotPath, FileMode.Create);

            RenderTexture.ReleaseTemporary(rendTexture);

            _camera.targetTexture = null;
            Debug.Log("captured");
            _captureOnNextFrame = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CaptureShot(500, 500);
        }
    }

    private void CaptureShot(int width, int height)
    {
        _camera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        _captureOnNextFrame = true;
    }

    private string CapturePath()
    {
        return Path.Combine(_screenshotPath, _pictureName + "_" +
                   DateTime.Now.ToShortTimeString().Trim(' ') + _extension);
    }


}
