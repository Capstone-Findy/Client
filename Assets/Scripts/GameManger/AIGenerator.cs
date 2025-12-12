using System.Collections;
using System.Collections.Generic;
using System.Text;
using Findy.Define;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AIGenerator : MonoBehaviour
{
    [Header("Settings")]
    private const string API_URL = "http://15.164.210.102:5000/spot-diff";

    [Header("UI")]
    [SerializeField] private TMP_InputField promptInput;
    [SerializeField] private Button generateBtn;
    [SerializeField] private Button startGameBtn;
    [SerializeField] private Image previewOriginalImage;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private Button helpButton;
    private Coroutine loadingCoroutine;

    [Header("Stage Data")]
    private StageData generatedStageData;
    private const float TARGET_WIDTH = 800f;
    private const float TARGET_HEIGHT = 600f;

    void Start()
    {
        generateBtn.onClick.AddListener(OnGenerateClicked);
        startGameBtn.onClick.AddListener(OnStartGameClicked);
        startGameBtn.interactable = false;

        if(helpButton != null)
            helpButton.onClick.AddListener(() =>
            {
                bool isActive = helpPanel.activeSelf;
                helpPanel.SetActive(!isActive); 
            });
    }

    void OnGenerateClicked()
    {
        string prompt = promptInput.text;
        if(string.IsNullOrEmpty(prompt))
        {
            statusText.text = "내용을 작성해주세요!";
            return;
        }

        generatedStageData = null;
        startGameBtn.interactable = false;

        StartCoroutine(CoRequestAISage(prompt));
    }

    void OnStartGameClicked()
    {
        if(generatedStageData != null)
        {
            if(CustomDataHandler.instance != null)
            {
                CustomDataHandler.instance.SaveCustomStage(generatedStageData, promptInput.text);
                Debug.Log("저장 성공!");
            }
            else
            {
                Debug.LogError("CustomDataHandler가 씬에 없습니다! 저장이 안 됩니다."); 
            }
            GameManager.instance.SelectStage(generatedStageData);
            GameManager.instance.LoadScene("GameScene");
        }
        else
        {
            statusText.text = "먼저 이미지를 생성해주세요.";
        }
    }

    IEnumerator CoRequestAISage(string prompt)
    {
        if(loadingCoroutine != null) StopCoroutine(loadingCoroutine);
        loadingCoroutine = StartCoroutine(CoLoadingTextAnim());

        generateBtn.interactable = false;

        var requestData = new { prompt = prompt };
        string jsonBody = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest req = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if(loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
                loadingCoroutine = null;
            }

            if(req.result == UnityWebRequest.Result.Success)
            {
                string rawJson = req.downloadHandler.text;
                statusText.text = "생성 완료!";
                try
                {
                    ProcessResponse(rawJson);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"데이터 처리 오류: {e.Message}\nStack Trace: {e.StackTrace}");
                    statusText.text = "데이터 처리 실패";
                    generateBtn.interactable = true;
                }
            }
            else
            {
                Debug.LogError($"서버 통신 오류: {req.error}");
                statusText.text = "서버 오류 발생";
                generateBtn.interactable = true;
            }
        }
    }

    void ProcessResponse(string jsonResponse)
    {
        AIDiffResponse data = JsonConvert.DeserializeObject<AIDiffResponse>(jsonResponse);

        if(data == null)
        {
            statusText.text = "잘못된 응답 데이터입니다.";
            generateBtn.interactable = true;
            return;
        }

        Sprite originalSprite = Base64ToSprite(data.base_image);
        Sprite wrongSprite = Base64ToSprite(data.diff_image);

        if(originalSprite == null || wrongSprite == null)
        {
            statusText.text = "이미지 변환 실패";
            generateBtn.interactable = true;
            return;
        }

        if(previewOriginalImage != null)
        {
            previewOriginalImage.sprite = originalSprite;
        }

        List<List<float>> targetCoords = new List<List<float>>();

        if (data.coordinates != null)
        {
            Debug.Log($"[Debug] coordinates 전체 구조: {data.coordinates}");

            foreach (var property in data.coordinates)
            {
                string key = property.Key;
                JToken value = property.Value;

                if (value is JArray arr && arr.Count > 0 && arr[0] is JArray)
                {
                    Debug.Log($"[Debug] 정답 좌표 필드를 찾았습니다: {key}");
                    targetCoords = value.ToObject<List<List<float>>>();
                    break; 
                }
            }
        }
        else
        {
            Debug.LogWarning("좌표 데이터가 없습니다.");
        }

        List<Vector2> answerPoints = ConvertCoordinates(targetCoords, originalSprite.texture.width, originalSprite.texture.height);
        
        CreateAndStartStage(originalSprite, wrongSprite, answerPoints);
        
        statusText.text = "생성 완료! 게임을 시작하세요.";
        startGameBtn.interactable = true;
        generateBtn.interactable = true;
    }

    private Sprite Base64ToSprite(string base64)
    {
        if(string.IsNullOrEmpty(base64)) return null;
        try 
        {
            byte[] imageBytes = System.Convert.FromBase64String(base64);
            Texture2D tex = new Texture2D(2, 2);
            if(tex.LoadImage(imageBytes))
            {
                tex.filterMode = FilterMode.Bilinear;
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError($"Base64 변환 오류: {e.Message}");
        }
        return null;
    }

    private List<Vector2> ConvertCoordinates(List<List<float>> serverCoords, int imgWidth, int imgHeight)
    {
        List<Vector2> result = new List<Vector2>();
        if (serverCoords == null) return result;

        float scaleX = TARGET_WIDTH / (float)imgWidth;
        float scaleY = TARGET_HEIGHT / (float)imgHeight;

        foreach(var box in serverCoords)
        {
            if(box.Count < 4) continue;

            float x1 = box[0];
            float y1 = box[1];
            float x2 = box[2];
            float y2 = box[3];

            float centerX = (x1 + x2) / 2f;
            float centerY = (y1 + y2) / 2f;

            // Y축 반전
            float unityTextureY = imgHeight - centerY;

            // 로컬 좌표
            float localX = centerX - (imgWidth / 2f);
            float localY = unityTextureY - (imgHeight / 2f);

            result.Add(new Vector2(localX * scaleX, localY * scaleY));
        }
        return result;
    }

    private void CreateAndStartStage(Sprite org, Sprite wrg, List<Vector2> answers)
    {
        generatedStageData = ScriptableObject.CreateInstance<StageData>();

        generatedStageData.stageName = "AI Custom Stage";
        generatedStageData.gameId = -1;
        generatedStageData.originalImage = org;
        generatedStageData.wrongImage = wrg;
        generatedStageData.answerPos = answers;
        generatedStageData.totalAnswerCount = answers.Count;
        generatedStageData.correctRange = 80f;

        generatedStageData.stageDescription = "AI가 생성한 세상에 단 하나뿐인 스테이지입니다.";
        generatedStageData.stageMission = "틀린 그림을 모두 찾아보세요!";
    }

    IEnumerator CoLoadingTextAnim()
    {
        string baseText = "AI가 이미지를 생성 중입니다. 1~2분 정도 소요됩니다.";
        int dotCount = 0;

        while(true)
        {
            string dots = "";
            for(int i = 0; i < dotCount; i++)
            {
                dots += ".";
            }
            statusText.text = $"{baseText}{dots}";
            dotCount++;
            if(dotCount > 3) dotCount = 0;

            yield return new WaitForSeconds(0.5f);
        }
    }
}