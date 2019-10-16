using System;
using System.IO;
using System.Text;
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
            string filePath = CapturePath();

            FileInfo file = new FileInfo(filePath);
            //if (!file.Exists)
            //{
            //    Debug.Log("File doesn't exist at the given path .. ");
            //    file.Directory.Create();
            //    Debug.Log("Created new file");
            //}
            //else
            //{
            //    Debug.Log("File exists ... Will be overwritten..");
            //}

            File.WriteAllBytes(file.FullName, byteArray);

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
            Debug.Log("SPACE");
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
        DateTime current = DateTime.Now;
        string time = current.ToLongTimeString().Trim(' ').Split(' ')[0];
        char[] temp = time.ToCharArray();
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i]!=':')
            {
                sb.Append(temp[i]);
            }
        }


        sb.Append(current.Ticks.ToString());
        return Path.Combine(_screenshotPath, _pictureName + "_" +
         sb+ _extension);

    }


}
