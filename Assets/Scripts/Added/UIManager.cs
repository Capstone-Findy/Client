using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private int _order = 10;
    private Stack<UI_Popup> _popupPool = new();

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }
    
    public void Init()
    {
        var root = GameObject.Find("@UI_Root");
        if (root == null)
        {
            root = new GameObject { name = "@UI_Root" };
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        var canvas= Utils.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }
    
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if(string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        var obj = Resources.Load<GameObject>($"UI/Popup/{name}.prefab");
        var go = Object.Instantiate(obj);

        if (_popupPool.Count != 0)
        {
            var prevPopup = _popupPool.Peek();
            prevPopup.OnOffDim(false);
        }
        
        var popup = Utils.GetOrAddComponent<T>(go);
        _popupPool.Push(popup);

        Debug.Log($"[UIManager.ShowPopupUI] {popup.name}를 생성하였습니다.");

        go.transform.SetParent(Root.transform);
        
        return popup;
    }
    

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupPool.Count == 0)
        {
            Debug.Log($"[UIManager.ClosePopupUI] Count == 0 -> {popup.name}를 닫지 못했습니다.");
            return;
        }
        
        if(_popupPool.Peek() != popup)
        {
            Debug.Log($"[UIManager.ClosePopupUI] Peek() != popup -> {popup.name}를 닫지 못했습니다.");
            return;
        }

        ClosePopupUI();
    }
    
    public void ClosePopupUI()
    {
        if (_popupPool.Count == 0)
            return;
        
        var popup = _popupPool.Pop();
        if (_popupPool.Count != 0)
        {
            var currPopup = _popupPool.Peek();
            currPopup.Refresh();
            currPopup.OnOffDim(true);
        }

        Debug.Log($"[UIManager.ClosePopupUI] {popup.gameObject.name} 팝업을 닫았습니다.");
        Object.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }
    
    public void CloseAllPopupUI()
    {
        while (_popupPool.Count>0)
            ClosePopupUI();
    }
    
    public UI_Popup GetCurrentPopupUI()
    {
        return _popupPool.Count == 0 ? null : _popupPool.Peek();
    }
}