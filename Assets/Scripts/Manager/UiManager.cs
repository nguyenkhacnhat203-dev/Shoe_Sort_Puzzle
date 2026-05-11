using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    #region Variables
    [Header("Bar")]
    public Bar Bar;
    [Header("Stats")]
    public List<TextMeshProUGUI> txtGold;
    public List<TextMeshProUGUI> txtHeart;
    public TextMeshProUGUI txtCountMagnet;
    public GameObject icAddMagnet;
    public TextMeshProUGUI txtCountShuffle;
    public GameObject icAddShuffle;
    public TextMeshProUGUI txtCountMoreBox;
    public GameObject icAddMoreBox;

    [Header("Popup")]
    public GameObject PopupWin;
    public GameObject PopupFail;
    public GameObject PopupSettingInGame;
    public GameObject PopupSettingInMenu;
    public GameObject PopupHeart;
    public GameObject PopupBooster;
    public GameObject PopupBuyBooster;
    public GameObject PopupAvatar;
    public Transform popupParent;
    private GameObject currentPopup;

    [Header("Panel")]
    public List<GameObject> Game;
    public List<GameObject> Menu;

    private const int MENUFOCUS = 2;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        Application.targetFrameRate = 60;
        this.UpdateStats();
        Button[] allButtons = this.GetComponentsInChildren<Button>(true);
        foreach (Button btn in allButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(AudioManager.Instance.BtnClick);
        }
    }
    #endregion

    #region Stats Update
    public void UpdateStats()
    {
        foreach (var gold in txtGold)
        {
            gold.text = ResourceManager.Instance.GetCoin().ToString();
        }
        foreach (var heart in txtHeart)
        {
            heart.text = ResourceManager.Instance.GetHeart().ToString();
        }

        if (ResourceManager.Instance.GetCountMagnet() == 0)
        {
            Transform parent = txtCountMagnet.transform.parent;
            parent.gameObject.SetActive(false);
            icAddMagnet.SetActive(true);
        }
        else
        {
            txtCountMagnet.text = ResourceManager.Instance.GetCountMagnet().ToString();
            Transform parent = txtCountMagnet.transform.parent;
            parent.gameObject.SetActive(true);
            icAddMagnet.SetActive(false);
        }

        if (ResourceManager.Instance.GetCountShuffle() == 0)
        {
            Transform parent = txtCountShuffle.transform.parent;
            parent.gameObject.SetActive(false);
            icAddShuffle.SetActive(true);
        }
        else
        {
            txtCountShuffle.text = ResourceManager.Instance.GetCountShuffle().ToString();
            Transform parent = txtCountShuffle.transform.parent;
            parent.gameObject.SetActive(true);
            icAddShuffle.SetActive(false);
        }

        if (ResourceManager.Instance.GetCountMoreBox() == 0)
        {
            Transform parent = txtCountMoreBox.transform.parent;
            parent.gameObject.SetActive(false);
            icAddMoreBox.SetActive(true);
        }
        else
        {
            txtCountMoreBox.text = ResourceManager.Instance.GetCountMoreBox().ToString();
            Transform parent = txtCountMoreBox.transform.parent;
            parent.gameObject.SetActive(true);
            icAddMoreBox.SetActive(false);
        }
    }
    #endregion

    #region View Management
    public void ShowMenu()
    {
        foreach (GameObject go in Game)
        {
            if (go != null)
                go.SetActive(false);
        }

        foreach (GameObject go in Menu)
        {
            if (go != null)
                go.SetActive(true);
            Bar.ChangeFocus(MENUFOCUS);
        }
    }
    public void ShowGame()
    {
        foreach (GameObject go in Game)
        {
            if (go != null)
                go.SetActive(true);
        }

        foreach (GameObject go in Menu)
        {
            if (go != null)
                go.SetActive(false);
        }
    }
    public void Show_Menu_Game()
    {
        if (GameManager.Instance.CurrentState == GameState.OnGame)
        {
            ShowGame();
        }
        else
        {
            ShowMenu();
        }
    }
    public void Return_Home()
    {
        GameManager.Instance.ResetGame();
        GameManager.Instance.ChangeState(GameState.OnMenu);
        UpdateStats();
        ShowMenu();
    }
    #endregion

    #region Popup Management
    private GameObject CreatePopup(GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }

        currentPopup = Instantiate(prefab, popupParent);
        return currentPopup;
    }

    public void Show_Setting()
    {
        GameObject setting;
        if (GameManager.Instance.CurrentState == GameState.OnGame)
        {
            setting = CreatePopup(PopupSettingInGame);
            setting.SetActive(true);
        }
        else
        {
            setting = CreatePopup(PopupSettingInMenu);
            setting.SetActive(true);
        }
    }

    public void Show_Win_Lose()
    {
        if (GameManager.Instance.CurrentState == GameState.Win)
        {
            GameObject popup = CreatePopup(PopupWin);
            popup.SetActive(true);
        }
        if (GameManager.Instance.CurrentState == GameState.Lose)
        {
            GameObject popup = CreatePopup(PopupFail);
            popup.SetActive(true);
        }
    }

    public void Show_Popup_Heart()
    {
        GameObject popup = CreatePopup(PopupHeart);
    }
    public void Show_Popup_Heart(System.Action onPurchaseAction = null)
    {
        GameObject popup = CreatePopup(PopupHeart);
        if (popup != null)
        {
            Popup_Heart script = popup.GetComponent<Popup_Heart>();
            if (script != null)
            {
                script.SetPurchaseAction(onPurchaseAction);
            }
        }
    }
    public void Show_Popup_Booster(int indexBooster)
    {
        Booster? booster = null;
        string boosterKey = null;
        switch (indexBooster)
        {
            case 0:
                booster = Booster.Magnet;
                boosterKey = "MagnetBooster";
                break;
            case 1:
                booster = Booster.Shuffle;
                boosterKey = "ShuffleBooster";
                break;
            case 2:
                booster = Booster.MoreBox;
                boosterKey = "MoreBoxBooster";
                break;
        }
        if (ResourceManager.Instance.GetCountBooster(boosterKey) == 0)
        {
            GameObject popup = CreatePopup(PopupBuyBooster);
            Popup_BuyBossterInGame script = popup.GetComponent<Popup_BuyBossterInGame>();
            GameManager.Instance.ChangeState(GameState.Pause);
            script.SetupPopupBooster(booster.Value, boosterKey);
        }
        else
        {
            if (booster != null)
            {
                GameObject popup = CreatePopup(PopupBooster);
                Popup_UseBosster script = popup.GetComponent<Popup_UseBosster>();
                GameManager.Instance.ChangeState(GameState.Pause);
                script.SetupPopupBooster(booster.Value);
            }
        }
    }
    public void Show_PopupAvatar()
    {
        CreatePopup(PopupAvatar);
    }
    #endregion
}