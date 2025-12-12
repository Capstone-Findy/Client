using System.Collections;
using System.Collections.Generic;
using System.IO;
using Findy.Define;
using UnityEngine;

public class CustomDataHandler : MonoBehaviour
{
    public static CustomDataHandler instance;
    
    private string SavePath => Application.persistentDataPath + "/CustomStages/";
    private const string META_FILE_NAME = "custom_stages_list.json";

    private void Awake()
    {
        if(instance == null) instance = this;
        if(!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);
    }

    public void SaveCustomStage(StageData data, string prompt)
    {
        string id = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        SaveTexture(data.originalImage.texture, id + "_orig.png");
        SaveTexture(data.wrongImage.texture, id + "_wrong.png");
        SaveTexture(data.answerImage.texture, id + "_answer.png");

        CustomStageInfo info = new CustomStageInfo
        {
            id = id,
            createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            prompt = prompt,
            answers = new List<Vector2>(data.answerPos)
        };

        CustomStageList list = LoadAllMeta();
        list.stages.Add(info);
        string json = JsonUtility.ToJson(list);
        File.WriteAllText(SavePath + META_FILE_NAME, json);

        Debug.Log($"[CustomSave] 스테이지 저장 완료: {id}");
    }

    private void SaveTexture(Texture2D texture, string fileName)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(SavePath + fileName, bytes);
    }

    public CustomStageList LoadAllMeta()
    {
        string path = SavePath + META_FILE_NAME;
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<CustomStageList>(json);
        }
        return new CustomStageList();
    }

    public StageData LoadStageData(CustomStageInfo info)
    {
        Texture2D orgTex = LoadTexture(info.id + "_orig.png");
        Texture2D wrgTex = LoadTexture(info.id + "_wrong.png");
        Texture2D ansTex = LoadTexture(info.id + "_answer.png");

        if(orgTex == null || wrgTex == null) return null;

        Sprite orgSprite = Sprite.Create(orgTex, new Rect(0, 0, orgTex.width, orgTex.height), new Vector2(0.5f, 0.5f));
        Sprite wrgSprite = Sprite.Create(wrgTex, new Rect(0, 0, wrgTex.width, wrgTex.height), new Vector2(0.5f, 0.5f));
        Sprite ansSprite = Sprite.Create(ansTex, new Rect(0, 0, ansTex.width, ansTex.height), new Vector2(0.5f, 0.5f));

        StageData stage = ScriptableObject.CreateInstance<StageData>();
        stage.stageName = "Custom Gallery";
        stage.gameId = -1;
        stage.originalImage = orgSprite;
        stage.wrongImage = wrgSprite;
        stage.answerImage = ansSprite;
        stage.answerPos = info.answers;
        stage.totalAnswerCount = info.answers.Count;
        stage.correctRange = 80f;
        stage.stageMission = info.prompt;
        stage.customId = info.id;

        return stage;
    }

    public Texture2D LoadTexture(string fileName)
    {
        string path = SavePath + fileName;
        if(File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            return tex;
        }
        return null;
    }
}
