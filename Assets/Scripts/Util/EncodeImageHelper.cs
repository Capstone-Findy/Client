using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncodeImageHelper : MonoBehaviour
{
    public string EncodeTextureToBase64(Texture2D texture)
    {
        if(texture == null)
        {
            return string.Empty;
        }

        byte[] imageBytes = texture.EncodeToPNG();
        string base64String = System.Convert.ToBase64String(imageBytes);

        return base64String;
    }
}
