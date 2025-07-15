using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public TouchManager touchManager;
    public StageData currentStage;
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
    }

    private void GoToNextStage()
    {
        // TODO : 다음 스테이지 이동 구현
        Debug.Log("다음 스테이지로 넘어갑니다.");
    }
}
