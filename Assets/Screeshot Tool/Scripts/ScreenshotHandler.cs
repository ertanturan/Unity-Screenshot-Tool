using System;
using System.IO;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScreenshotHandler : MonoBehaviour
{
    [Tooltip("Width and Height of a desired screenshot .")]
    public Vector2 PictureSpecs;
    [Tooltip("Keyboard key to actually take screenshot . ")]
    public KeyCode ScreenShotKey;
    [Tooltip("Choosing a picture format from here")]
    public TextureFormat TextureFormat;
    [Tooltip("Path to save screenshots ...")]
    public string ScreenshotPath;
    [Tooltip("Extension of the desired screenshot")]
    public PictureExtension Extension;

    [Tooltip("Opens where your screenshots saved to..")]
    public bool OpenFileDirectory;
    public Canvas Canvas;
    // Directory Path
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

    private void Start()
    {
        if (ScreenShotKey == KeyCode.None)
        {
            Debug.LogError("No screenshot key selected...");
        }
        if (TextureFormat == 0)
        {
            Debug.LogError("No Texture format selected...");
        }
        if (ScreenshotPath == "")
        {
            Debug.LogWarning("No path given .. Will create one ..");
        }
        if (PictureSpecs == Vector2.zero)
        {
            Debug.LogError("Width and Height needed !");
        }

        if (Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogWarning("!! ATTENTION !! : Capturing screenshot " +
                             "while your canvas's rendermode is`Screen Space - Overlay` " +
                             "won't work with UI . Try to switch rendermode to something else... ");
        }
    }

    private void OnPostRender()
    {
        if (_captureOnNextFrame)
        {
            Debug.Log("Capture True..");
            RenderTexture rendTexture = _camera.targetTexture;
            Debug.Log("Rendertexture taken..");
            Texture2D result = new Texture2D(rendTexture.width, rendTexture.height,
                TextureFormat, false);

            Rect rect = new Rect(0, 0, rendTexture.width, rendTexture.height);

            result.ReadPixels(rect, 0, 0);
            byte[] byteArray;

            byteArray = ExtensionHandler.ByteArray(result, Extension);

            string filePath;

            if (ScreenshotPath == "")
            {
                filePath = CapturePath();
            }
            else
            {
                filePath = ScreenshotPath;
            }

            FileInfo file = new FileInfo(filePath);

            if (!file.Exists)
            {
                Debug.Log("ScreenshotPath doesn't exist at the given  .. ");
                file.Directory.Create();
                Debug.Log("Created given path..");
            }

            File.WriteAllBytes(file.FullName, byteArray);

            RenderTexture.ReleaseTemporary(rendTexture);

            _camera.targetTexture = null;
            Debug.Log("Screen captured and saved to   '" + filePath + "' ");

            if (OpenFileDirectory)
            {
                System.Diagnostics.Process.Start(_screenshotPath);
            }
            _captureOnNextFrame = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(ScreenShotKey))
        {
            Debug.Log("SPACE");
            CaptureShot((int)PictureSpecs.x, (int)PictureSpecs.y);
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
            if (temp[i] != ':')
            {
                sb.Append(temp[i]);
            }
        }


        sb.Append(current.Ticks.ToString());
        return Path.Combine(_screenshotPath, _pictureName + "_" +
         sb + ExtensionHandler.Extension(Extension));

    }

}
