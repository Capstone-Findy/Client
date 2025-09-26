using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnPosition : MonoBehaviour
{
    [SerializeField] private RectTransform targetArea; // 기준 RectTransform (Canvas 또는 layoutArea)

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 screenPos = Input.mousePosition;
            LogNormalized(screenPos);
        }

        // 모바일 터치 지원 (선택)
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
                LogNormalized(t.position);
        }
    }

    void LogNormalized(Vector2 screenPos)
    {
        if (targetArea == null)
        {
            Debug.LogWarning("[UIPositionDebuggerSimple] targetArea is not assigned.");
            return;
        }

        Vector2 localPoint;
        // 카메라는 Screen Space - Overlay일 때 null, Screen Space - Camera일 땐 canvas camera 넣어야 함
        Canvas canvas = targetArea.GetComponentInParent<Canvas>();
        Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(targetArea, screenPos, cam, out localPoint))
        {
            Debug.LogWarning("[UIPositionDebuggerSimple] Could not convert screen point to local point.");
            return;
        }

        Vector2 size = targetArea.rect.size;
        Vector2 pivot = targetArea.pivot;
        Vector2 adjusted = localPoint + size * pivot;
        Vector2 normalized = new Vector2(adjusted.x / size.x, adjusted.y / size.y);

        Debug.Log($"Clicked Normalized Pos: {normalized}");
    }
}
