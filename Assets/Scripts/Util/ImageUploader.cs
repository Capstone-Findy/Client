using UnityEngine;
using UnityEngine.UI;

public class ImageUploader : MonoBehaviour
{
    public Image targetImage;

    public void OnClickUploadButton()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if(path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 1024);
                
                if(texture != null)
                {
                    Sprite newSprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    targetImage.sprite = newSprite;
                    targetImage.preserveAspect = true;
                }
            }
        }, "사진을 선택하세요.", "image/*");
    }
}
