using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScreenshotHandler : MonoBehaviour
{
    public static ScreenshotHandler Instance;

    #region Editor

    [Tooltip("Width and Height of a desired screenshot .")]
    public Vector2[] PictureSpecs;
    [Tooltip("Keyboard key to actually take screenshot . ")]
    public KeyCode ScreenShotKey;

    [Tooltip("Choosing a picture format from here")]
    private TextureFormat TextureFormat = TextureFormat.ARGB32;
    [Tooltip("Path to save screenshots ...")]
    public string ScreenshotPath;
    [Tooltip("Extension of the desired screenshot")]
    public PictureExtension Extension;

    [Tooltip("Opens where your screenshots saved to..")]
    public bool OpenFileDirectory = true;
    public Canvas Canvas;

    #endregion


    private string _pictureName = "Screenshot";

    private static string _screenshotPath;

    private bool _isInProgess;

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
        if (PictureSpecs.Length == 0)
        {
            Debug.LogError("Width and Height needed !");
        }

        if (Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogWarning("!! ATTENTION !! : Capturing screenshot " +
                             "while your canvas's rendermode is`Screen Space - Overlay` " +
                             "won't work with UI . Try to switch rendermode to something else... ");
        }

        if (Extension == PictureExtension.EXR)
        {
            TextureFormat = TextureFormat.RGBAHalf;
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
                filePath = CapturePath(rendTexture.width, rendTexture.height);
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


            _captureOnNextFrame = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(ScreenShotKey))
        {
            if (!_isInProgess)
            {
                StartCoroutine(ScreenshotWithDelay());
            }
            else
            {
                Debug.LogWarning("Another capture process in progress wait for a while ...");
            }
        }

    }

    private IEnumerator ScreenshotWithDelay()
    {
        int index = 0;
        while (index != PictureSpecs.Length)
        {
            yield return new WaitForSeconds(2f);

            try
            {
                CaptureShot((int)PictureSpecs[index].x, (int)PictureSpecs[index].y);
                index++;
                _isInProgess = true;
            }
            catch (Exception e)
            {
                Debug.LogError("There has been an error : " + e.Message);
                StopAllCoroutines();
                _isInProgess = false;
            }

            yield return null;
        }

        if (OpenFileDirectory)
        {
            System.Diagnostics.Process.Start(_screenshotPath);
        }

        _isInProgess = false;
    }

    private void CaptureShot(int width, int height)
    {
        _camera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        _captureOnNextFrame = true;
    }

    private string CapturePath(int width, int height)
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
        return Path.Combine(_screenshotPath, _pictureName + "_" + width.ToString() + "_" + height.ToString()
                                             + "_" +
         sb + ExtensionHandler.Extension(Extension));

    }

}
