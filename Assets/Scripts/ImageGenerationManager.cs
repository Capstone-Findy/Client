using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEngine.RenderTexture;


public class ImageGenerationManager : MonoBehaviour
{
    [Serializable]
    private class Payload
    {
        public string inputs;
        public string image;
        public string mask_image;
    }
    
    [Header("API 설정")]
    [SerializeField] private string huggingFaceAPIToken = "";
    [SerializeField] private string textToImageModelID = "stabilityai/stable-diffusion-2-1";
    [SerializeField] private string inpaintModelInferenceEndpoint = "";
    
    [Header("UI 요소")]
    [SerializeField] private TMP_InputField promptInputField;
    [SerializeField] private RawImage originalImageDisplay;
    [SerializeField] private RawImage modifiedImageDisplay;
    [SerializeField] private RawImage maskImageDisplay;
    [SerializeField] private Button generateButton;
    [SerializeField] private Button createPuzzleButton;

    private Texture2D originalTexture;
    private Texture2D modifiedTexture;
    private string lastPrompt;
    private List<Vector2Int> differencePositions = new();
    private int differenceCount = 5;
    private int differenceSize = 64;
    private const int IMAGE_SIZE = 512;

    private void Start()
    {
        generateButton.onClick.AddListener(GenerateImageFromPrompt);
        createPuzzleButton.onClick.AddListener(CreateSpotDifferencePuzzle);
    }
    
    public void GenerateImageFromPrompt()
    {
        var prompt = promptInputField.text;
        if (string.IsNullOrEmpty(prompt))
        {
            Debug.LogWarning("프롬프트를 입력해주세요.");
            return;
        }
        
        lastPrompt = prompt;
        StartCoroutine(RequestImageGeneration(prompt));
    }
    
    private IEnumerator RequestImageGeneration(string prompt)
    {
        string url = $"https://api-inference.huggingface.co/models/{textToImageModelID}";
        
        var requestData = new Dictionary<string, object>
        {
            { "inputs", prompt },
            { "parameters", new Dictionary<string, object>
                {
                    { "width", IMAGE_SIZE },
                    { "height", IMAGE_SIZE }
                }
            }
        };
        
        var jsonData = JsonConvert.SerializeObject(requestData);
        var bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {huggingFaceAPIToken}");
        request.SetRequestHeader("Content-Type", "application/json");
            
        yield return request.SendWebRequest();
            
        if (request.result == UnityWebRequest.Result.Success)
        {
            byte[] imageData = request.downloadHandler.data;
            originalTexture = new Texture2D(2, 2);
            if (originalTexture.LoadImage(imageData))
            {
                originalImageDisplay.texture = originalTexture;
                Debug.Log("이미지 생성 성공!");
                    
                // 수정할 이미지 복사
                modifiedTexture = DuplicateTexture(originalTexture);
                modifiedImageDisplay.texture = modifiedTexture;
            }
            else
            {
                Debug.LogError("이미지 로드 실패");
            }
        }
        else
        {
            Debug.LogError($"이미지 생성 API 요청 실패: {request.error}");
            Debug.LogError($"응답: {request.downloadHandler.text}");
        }
    }
        
    private Texture2D DuplicateTexture(Texture2D source)
    {
        var renderTex = GetTemporary(
            source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        
        Graphics.Blit(source, renderTex);
        var previous = active;
        active = renderTex;
        
        var result = new Texture2D(source.width, source.height);
        result.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        result.Apply();
        
        active = previous;
        ReleaseTemporary(renderTex);
        
        return result;
    }
    
    public void CreateSpotDifferencePuzzle()
    {
        if (originalTexture == null)
        {
            Debug.LogWarning("먼저 이미지를 생성해주세요.");
            return;
        }
        
        GenerateDifferencePositions();
        
        StartCoroutine(ApplyInpaintingToAllDifferences());
    }
    
    private void GenerateDifferencePositions()
    {
        differencePositions.Clear();
        int width = originalTexture.width;
        int height = originalTexture.height;
        
        // 이미지 가장자리에서 거리두기
        int margin = differenceSize;
        
        for (int i = 0; i < differenceCount; i++)
        {
            // 기존 차이점과 충분히 떨어진 위치 찾기
            bool validPosition = false;
            Vector2Int newPos = Vector2Int.zero;
            int attempts = 0;
            
            while (!validPosition && attempts < 100)
            {
                newPos = new Vector2Int(
                    UnityEngine.Random.Range(margin, width - margin),
                    UnityEngine.Random.Range(margin, height - margin)
                );
                
                validPosition = true;
                foreach (var pos in differencePositions)
                {
                    if (Vector2Int.Distance(pos, newPos) < differenceSize * 2)
                    {
                        validPosition = false;
                        break;
                    }
                }
                
                attempts++;
            }
            
            if (validPosition)
            {
                differencePositions.Add(newPos);
            }
        }
        
        Debug.Log($"{differencePositions.Count}개의 차이점 위치 생성 완료");
    }
    
    private IEnumerator ApplyInpaintingToAllDifferences()
    {
        // foreach (var position in differencePositions)
        // {
            // yield return StartCoroutine(ApplyInpainting(position));
        // }
        yield return StartCoroutine(ApplyInpainting(differencePositions[0]));
        
        Debug.Log("틀린그림 찾기 퍼즐 생성 완료!");
    }
    
    private IEnumerator ApplyInpainting(Vector2Int position)
    {
        var maskTexture = new Texture2D(modifiedTexture.width, modifiedTexture.height);
        var maskColors = new Color[maskTexture.width * maskTexture.height];
        for (int i = 0; i < maskColors.Length; i++)
        {
            maskColors[i] = Color.black;
        }
        
        for (int y = -differenceSize; y <= differenceSize; y++)
        {
            for (int x = -differenceSize; x <= differenceSize; x++)
            {
                if (x*x + y*y <= differenceSize*differenceSize)
                {
                    int posX = position.x + x;
                    int posY = position.y + y;
                    
                    if (posX >= 0 && posX < maskTexture.width && posY >= 0 && posY < maskTexture.height)
                    {
                        int index = posY * maskTexture.width + posX;
                        maskColors[index] = Color.white;
                    }
                }
            }
        }
        
        maskTexture.SetPixels(maskColors);
        maskTexture.Apply();
        maskImageDisplay.texture = maskTexture;
        
        var modifiedPrompt = lastPrompt;
        string[] modifications = {
            "with different colors", "in a different style", "with minor changes",
            "with altered details", "slight variation"
        };
        modifiedPrompt += ", " + modifications[UnityEngine.Random.Range(0, modifications.Length)];
        
        yield return StartCoroutine(RequestInpainting(modifiedTexture, maskTexture, modifiedPrompt, position,
            texture2D =>
            {
                Debug.Log($"[ApplyInpainting] 인페인팅 완료: {position}");
                modifiedImageDisplay.texture = texture2D;
            }));
    }
    
    private IEnumerator RequestInpainting(Texture2D imageTexture, Texture2D maskTexture, string prompt, Vector2Int position, Action<Texture2D> onResult)
    {
        var savePath = Path.Combine(Application.dataPath, "SavedImages");
        var imagePath = Path.Combine(savePath, "myTexture.png");
        var maskPath = Path.Combine(savePath, "myMaskTexture.png");
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        
        File.WriteAllBytes(imagePath, imageTexture.EncodeToPNG());
        File.WriteAllBytes(maskPath, maskTexture.EncodeToPNG());
        
        var imageBase64 = Convert.ToBase64String(imageTexture.EncodeToPNG());
        var maskBase64 = Convert.ToBase64String(maskTexture.EncodeToPNG());
        
        var payloadJson = JsonConvert.SerializeObject(new Payload
        {
            inputs = prompt,
            image = imageBase64,
            mask_image = maskBase64
        });

        using var request = new UnityWebRequest(inpaintModelInferenceEndpoint, "POST");
        var bodyRaw = Encoding.UTF8.GetBytes(payloadJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Authorization", $"Bearer {huggingFaceAPIToken}");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "image/png");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Request failed: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            var resultBytes = request.downloadHandler.data;
            var resultTexture = new Texture2D(2, 2);
            resultTexture.LoadImage(resultBytes);
            onResult?.Invoke(resultTexture);
        }
    }
}