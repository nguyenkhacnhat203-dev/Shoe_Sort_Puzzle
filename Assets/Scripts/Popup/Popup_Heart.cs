using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Heart : PopupBase
{
    public TextMeshProUGUI TimeCount;
    public Button btnOneHeart;
    public Button btnFullHeart;

    private Action onPurchase;

    public void SetPurchaseAction(Action action)
    {
        onPurchase = action;
    }

    void Update()
    {
        UpdateTime();
    }

    void UpdateTime()
    {
        if (ResourceManager.Instance.IsHeartFull())
        {
            TimeCount.text = "Full";
            return;
        }
        int heartTimeCount = (int)(ResourceManager.Instance.CurrentHeartTimer);
        int minutes = heartTimeCount / 60;
        int second = heartTimeCount % 60;
        TimeCount.text = string.Format("{0:00}:{1:00}", minutes, second);
    }
    public void AddOneHeart(Transform button)
    {
        int price = int.Parse(button.GetComponentInChildren<TextMeshProUGUI>().text);
        if (price > ResourceManager.Instance.GetCoin())
            return;
        ResourceManager.Instance.ChangeCoin(-price);
        ResourceManager.Instance.SetHeart(1);
        UiManager.Instance.UpdateStats();
        
        Action tempAction = onPurchase;
        base.DestroyPopup();
        tempAction?.Invoke();
    }
    public void AddFullHeart(Transform button)
    {
        int price = int.Parse(button.GetComponentInChildren<TextMeshProUGUI>().text);
        if (price > ResourceManager.Instance.GetCoin())
            return;
        ResourceManager.Instance.ChangeCoin(-price);
        ResourceManager.Instance.SetHeart(5);
        UiManager.Instance.UpdateStats();
        
        Action tempAction = onPurchase;
        base.DestroyPopup();
        tempAction?.Invoke();
    }
}
