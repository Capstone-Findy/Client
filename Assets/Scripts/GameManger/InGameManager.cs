using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    [Header("GameManager")]
    public TouchManager touchManager;

    [Header("Game Information")]
    public StageData currentStage;
    public bool isGameOver = false;

    [Header("Slider")]
    public Slider timeSlider;
    private float totalTime = 60f;
    public float currentTime;

    void Start()
    {
        currentTime = totalTime;
        timeSlider.maxValue = totalTime;
        timeSlider.value = currentTime;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Input.mousePosition;
            touchManager.CheckAnswer(pos);

            if (touchManager.GetFoundAnswerCount() == currentStage.totalAnswerCount)
            {
                GoToNextStage();
            }
        }

        if (isGameOver) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timeSlider.value = currentTime;
        }
        else
        {
            currentTime = 0;
            GameOver();
        }
    }

    private void GoToNextStage()
    {
        // TODO : 다음 스테이지 이동 구현 및 패널 제외한 인게임은 터치 불가하도록 설정
        isGameOver = true;
        Debug.Log("다음 스테이지로 넘어갑니다.");
    }

    private void GameOver()
    {
        // TODO : 게임 실패 시 찾은 개수 및 재시도 여부 패널 띄움 구현
        isGameOver = true;
        Debug.Log("Game Over!!!");
    }
}
