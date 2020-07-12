using System;
using System.IO;
using UnityEngine;

[Serializable]
public class ScreenshotHandler : MonoBehaviour
{

    public bool screenshot_iOS = true, iOS_isPortrait = false;

    public bool screenshot_Android = true;

    public bool screenshot_PC = true;

    public bool transparent = false;

    public int android_width = 800, android_height = 480;

    public int pc_width = 1600, pc_height = 900;

    public int enlarge = 1;


    public KeyCode screenshotKey = KeyCode.Space;

    private int[] iOSRes = new int[] { 960, 640, 1136, 640, 1334, 750, 2208, 1242, 2048, 1536, 2732, 2048 };
    private int picture;
    private bool takeHiResShot = false;
    private TextureFormat transp = TextureFormat.ARGB32;
    private TextureFormat nonTransp = TextureFormat.RGB24;
    private string size;


    public static string ScreenShotName(int photoNumber, string plataform, int width, int height)
    {
        return string.Format("{0}/../screenshots/" + photoNumber + "_" + plataform + "screen_{1}x{2}_{3}.png", Application.dataPath, width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    void LateUpdate()
    {

        if (Input.GetKey(screenshotKey))
        {
            takeHiResShot = true;
        }

        if (takeHiResShot)
        {

            picture = PlayerPrefs.GetInt("PhotoNumber");
            picture++;
            PlayerPrefs.SetInt("PhotoNumber", picture);

            if (screenshot_iOS)
            {
                if (!iOS_isPortrait)
                {
                    LandScapeiOS();
                }
                else
                {
                    PortraitiOS();
                }
            }

            if (screenshot_Android)
            {
                ScreenShotAndroid();
            }

            if (screenshot_PC)
            {
                ScreenshotPC();
            }

            takeHiResShot = false;
        }
    }

    public void LandScapeiOS()
    {

        for (int i = 0; i < iOSRes.Length; i += 2)
        {

            TextureFormat textForm = nonTransp;

            if (transparent)
                textForm = transp;

            switch (i)
            {
                case 0:
                    size = "3.5";
                    break;
                case 2:
                    size = "4";
                    break;
                case 4:
                    size = "4.7";
                    break;
                case 6:
                    size = "5.5";
                    break;
                case 8:
                    size = "iPad";
                    break;
                case 10:
                    size = "iPadPro";
                    break;
            }

            RenderTexture rt = new RenderTexture(iOSRes[i], iOSRes[i + 1], 24);
            Camera.main.targetTexture = rt;
            Texture2D screenShot = new Texture2D(iOSRes[i], iOSRes[i + 1], textForm, false);
            Camera.main.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, iOSRes[i], iOSRes[i + 1]), 0, 0);
            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(picture, "IOS_" + size + "_LANDSCAPE+", iOSRes[i] * enlarge, iOSRes[i + 1] * enlarge);

            if (!Directory.Exists(Application.dataPath + "/../screenshots/"))
                Directory.CreateDirectory(Application.dataPath + "/../screenshots/");

            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
        }
    }

    public void PortraitiOS()
    {

        for (int i = 0; i < iOSRes.Length; i += 2)
        {

            TextureFormat textForm = nonTransp;

            if (transparent)
                textForm = transp;

            switch (i)
            {
                case 0:
                    size = "3.5";
                    break;
                case 2:
                    size = "4";
                    break;
                case 4:
                    size = "4.7";
                    break;
                case 6:
                    size = "5.5";
                    break;
                case 8:
                    size = "iPad";
                    break;
                case 10:
                    size = "iPadPro";
                    break;
            }

            RenderTexture rt = new RenderTexture(iOSRes[i + 1], iOSRes[i], 24);
            Camera.main.targetTexture = rt;
            Texture2D screenShot = new Texture2D(iOSRes[i + 1], iOSRes[i], textForm, false);
            Camera.main.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, iOSRes[i + 1], iOSRes[i]), 0, 0);
            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(picture, "IOS_" + size + "_PORTRAIT+", iOSRes[i + 1] * enlarge, iOSRes[i] * enlarge);

            if (!Directory.Exists(Application.dataPath + "/../screenshots/"))
                Directory.CreateDirectory(Application.dataPath + "/../screenshots/");

            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
        }
    }

    public void ScreenShotAndroid()
    {

        if (android_width == 0)
        {
            android_width = 800;
        }

        if (android_height == 0)
        {
            android_height = 480;
        }

        TextureFormat textForm = nonTransp;

    }

    public void ScreenshotPC()
    {

        if (pc_width == 0)
        {
            pc_width = 1600;
        }

        if (pc_height == 0)
        {
            pc_height = 900;
        }

        TextureFormat textForm = nonTransp;
        if (transparent)
            textForm = transp;
        RenderTexture rt = new RenderTexture(pc_width * enlarge, pc_height * enlarge, 24);
        Camera.main.targetTexture = rt;
        Texture2D screenShot = new Texture2D(pc_width * enlarge, pc_height * enlarge, textForm, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, pc_width * enlarge, pc_height * enlarge), 0, 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName(picture, "PC+", pc_width * enlarge, pc_height * enlarge);

        if (!Directory.Exists(Application.dataPath + "/../screenshots/"))
            Directory.CreateDirectory(Application.dataPath + "/../screenshots/");

        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }

    private void Capture(int width, int height, int enlargeCOEF, TextureFormat textForm = TextureFormat.RGB24)
    {
        if (transparent)
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
        string filename = ScreenShotName(picture, "ANDROID+", width * enlargeCOEF, height * enlargeCOEF);

        if (!Directory.Exists(Application.dataPath + "/../screenshots/"))
            Directory.CreateDirectory(Application.dataPath + "/../screenshots/");

        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }

}