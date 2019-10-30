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
    public Canvas Canvas;

    [Tooltip("Width and Height of a desired screenshot .")]
    public Vector2[] PictureSpecs;

    [Tooltip("Keyboard key to actually take screenshot . ")]
    public KeyCode ScreenShotKey = KeyCode.Space;

    [Tooltip("Choosing a picture format from here")]
    private TextureFormat TextureFormat = TextureFormat.ARGB32;

    [Tooltip("Path to save screenshots ...")]
    public string ScreenshotPath;

    [Tooltip("Extension of the desired screenshot")]
    public PictureExtension Extension = PictureExtension.PNG;

    [Tooltip("Opens where your screenshots saved to..")]
    public bool OpenFileDirectory = true;

    #endregion

    #region Private

    private string _pictureName = "Screenshot";

    private static string _screenshotPath;

    private bool _isInProgess;

    private Camera _camera;

    private bool _captureOnNextFrame;

    #endregion

    private void Awake()
    {
        Instance = this;
        _camera = GetComponent<Camera>();
        _screenshotPath = Path.Combine(Application.persistentDataPath, "Screenshots");
    }

    private void Start()
    {


        if (ReadyToCapture())
        {
            Debug.Log("Screenshot tool is ready to capture !");
        }
        else
        {
            Debug.LogError("Screenshot tool is not set correctly . " +
                             "It won't work if not set correct.");
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
        if (!ReadyToCapture())
        {
            return;
        }

        if (_captureOnNextFrame)
        {
            try
            {
                for (int i = 0; i < PictureSpecs.Length; i++)
                {
                    var tempRT = RenderTexture.GetTemporary((int)PictureSpecs[i].x, (int)PictureSpecs[i].y);
                    Graphics.Blit(source, tempRT);

                    var tempTex = new Texture2D((int)PictureSpecs[i].x, (int)PictureSpecs[i].y,
                        TextureFormat.RGBA32, false);

                    tempTex.ReadPixels(new Rect(0, 0, (int)PictureSpecs[i].x,
                        (int)PictureSpecs[i].y), 0, 0, false);

                    tempTex.Apply();

                    byte[] byteArray;

                    byteArray = ExtensionHandler.ByteArray(tempTex, Extension);

                    string filePath = FilePath(tempRT);


                    WriteImageToFile(filePath, byteArray);


                    Graphics.Blit(source, destination);
                    Destroy(tempTex);
                    RenderTexture.ReleaseTemporary(tempRT);

                    Debug.Log("FINISHED");


                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }


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
            OnButtonDown();
            Debug.Log(TextureFormat);
        }

    }

    private string FilePath(RenderTexture rendTxt)
    {
        if (ScreenshotPath == "")
        {
            return CapturePath(rendTxt.width, rendTxt.height);
        }
        else
        {
            return ScreenshotPath;
        }
    }

    private void WriteImageToFile(string filePath, byte[] byteArray)
    {
        FileInfo file = new FileInfo(filePath);

        if (!file.Exists)
        {
            Debug.Log("ScreenshotPath doesn't exist at the given  .. ");
            file.Directory.Create();
            Debug.Log("Created given path..");
        }

        File.WriteAllBytes(file.FullName, byteArray);
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

    private void OnButtonDown()
    {
        if (!ReadyToCapture())
            return;

        if (!_isInProgess)
        {
            _captureOnNextFrame = true;
        }
        else
        {
            Debug.LogWarning("Another capture process in progress wait for a while ...");
        }
    }

    private void SetCanvas()
    {
        Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        Canvas.worldCamera = _camera;
        Canvas.planeDistance = 1;
    }

    private bool ReadyToCapture()
    {
        if (Extension == PictureExtension.EXR)
        {
            TextureFormat = TextureFormat.RGBAHalf;
        }

        if (ScreenShotKey == KeyCode.None)
        {
            Debug.LogError("No screenshot key selected...");
            return false;
        }
        if (TextureFormat == 0)
        {
            Debug.LogError("No Texture format selected...");
            return false;
        }
        if (ScreenshotPath == "")
        {
            if (File.Exists(ScreenshotPath))
            {
                Debug.LogWarning("No path given .. will use previously created.");
            }
            else
            {

                Debug.LogWarning("Neither path given nor path created.. Will create one ..");
            }
        }

        if (PictureSpecs.Length == 0)
        {
            Debug.LogError("Width and Height needed !");
            return false;
        }
        else
        {
            for (int i = 0; i < PictureSpecs.Length; i++)
            {
                if (PictureSpecs[i] == Vector2.zero)
                {
                    Debug.LogError("!! ATTENTION !! :Resolution is not set for at least 1 image !!." +
                                   "Remove it or set a non-zero value ..");
                    return false;
                }
            }
        }

        if (Canvas == null)
        {
            Debug.LogError("Canvas can't be nulll");
            return false;
        }

        if (Canvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            SetCanvas();
        }



        return true;
    }
}
