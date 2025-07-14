using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public TouchManager touchManager;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Input.mousePosition;
            touchManager.CheckAnswer(pos);
        }
    }
}
