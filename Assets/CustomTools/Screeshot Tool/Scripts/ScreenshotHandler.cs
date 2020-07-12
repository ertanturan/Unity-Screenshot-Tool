using System;
using System.IO;
using UnityEngine;

[Serializable]
public class ScreenshotHandler : MonoBehaviour
{
    public bool IsTransparent = false;
    private TextureFormat transp = TextureFormat.ARGB32;
    private TextureFormat nonTransp = TextureFormat.RGB24;

    public KeyCode ShotKey = KeyCode.Space;

    public Resolution[] Resolutions;



    private void LateUpdate()
    {
        if (Input.GetKeyDown(ShotKey))
        {
            if (Resolutions.Length == 0)
            {
                Debug.LogWarning("no resolution found !");
                return;



            }
            else
            {
                for (int i = 0; i < Resolutions.Length; i++)
                {
                    if (Resolutions[i].X == 0 || Resolutions[i].Y == 0)
                    {
                        Debug.LogWarning("Resolution can't be 0 !");
                        return;
                    }
                    else
                    {
                        Capture(Resolutions[i].X, Resolutions[i].Y, 1);
                    }
                }
            }
        }
    }

    private void Capture(int width, int height, int enlargeCOEF)
    {
        TextureFormat textForm = nonTransp;

        if (IsTransparent)
            textForm = transp;
        RenderTexture rt = new RenderTexture(width * enlargeCOEF, height * enlargeCOEF, 24);
        Camera.main.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width * enlargeCOEF, height * enlargeCOEF, textForm, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width * enlargeCOEF, height * enlargeCOEF), 0, 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenshotName("ANDROID+", (width * enlargeCOEF).ToString(), (height * enlargeCOEF).ToString());

        if (!Directory.Exists(Application.dataPath + "/../screenshots/"))
            Directory.CreateDirectory(Application.dataPath + "/../screenshots/");

        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }

    private string ScreenshotName(string platform, string width, string heigth)
    {
        return platform + width + heigth;
    }

}