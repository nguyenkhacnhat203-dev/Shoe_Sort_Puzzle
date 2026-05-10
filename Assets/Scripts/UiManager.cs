using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    public GameObject Popup_Win;
    public GameObject Popup_Fail;
    public GameObject Popup_Setting_In_Game;
    public GameObject Popup_Setting_In_Menu;


    public Transform popupParent;
    public Bar Bar;
    private GameObject currentPopup;



    public List<GameObject> Game;
    public List<GameObject> Menu;

    private const int MENU_FOCUS = 2;

    private void Start()
    {
        Application.targetFrameRate = 60;
        Button[] allButtons = this.GetComponentsInChildren<Button>(true);
        foreach (Button btn in allButtons)
        {
            btn.onClick.RemoveListener(AudioManager.Instance.BtnClick);
            btn.onClick.AddListener(AudioManager.Instance.BtnClick);
        }
    }

    private GameObject CreatePopup(GameObject prefab)
    {
        AudioManager.Instance.BtnClick();
        if (prefab == null)
        {
            return null;
        }

        currentPopup = Instantiate(prefab, popupParent);
        return currentPopup;
    }

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
            Bar.ChangeFocus(MENU_FOCUS);
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
    public void Show_Setting()
    {
        bool isPlaying = GameManager.Instance.IsPlaying;
        GameObject setting;
        if (isPlaying)
        {
            GameManager.Instance.SetPause(false);
            Time.timeScale = 0;
            GameManager.Instance.SetPause(true);
            setting = CreatePopup(Popup_Setting_In_Game);
            setting.SetActive(true);
        }
        else
        {
            setting = CreatePopup(Popup_Setting_In_Menu);
            setting.SetActive(true);
        }
    }

    public void Show_Win_Lose(bool isWin)
    {
        if (isWin)
        {
            GameObject popup = CreatePopup(Popup_Win);
            popup.SetActive(true);
        }
        else
        {
            GameObject popup = CreatePopup(Popup_Fail);
            popup.SetActive(true);
        }
    }
    public void Show_Menu_Game(bool isPlaying)
    {
        if (isPlaying)
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
        ShowMenu();
    }
    public void Hide_Win_Lose()
    {
        Popup_Win.SetActive(false);
        Popup_Fail.SetActive(false);
    }

}