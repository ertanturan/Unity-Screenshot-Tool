
using UnityEngine;

public static class ExtensionHandler
{
    public static string Extension(PictureExtension extension)
    {
        return "." + extension.ToString().ToLower();
    }

    public static byte[] ByteArray(Texture2D texture,PictureExtension extension)
    {
        if(extension==PictureExtension.EXR)
        {
            return texture.EncodeToEXR();
        }
        else if(extension==PictureExtension.JPG)
        {
            return texture.EncodeToJPG();
        }
        else if(extension==PictureExtension.PNG)
        {
            return texture.EncodeToPNG();
        }
        else if(extension==PictureExtension.TGA)
        {
            return texture.EncodeToTGA();
        }
        else
        {
            Debug.LogError("Not possible to encode 'Texture2D' to byte array ... ");
            return null;
        }
    }
}
