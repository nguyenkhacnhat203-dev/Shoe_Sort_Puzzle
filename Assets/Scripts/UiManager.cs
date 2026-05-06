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
    public GameObject popup_Next;
    public GameObject popup_Settings;
  

    public Transform popupParent;

    private GameObject currentPopup;


    public List<GameObject> Game;
    public List<GameObject> Menu;


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

  
    public void ShowPopup_Settings() => CreatePopup(popup_Settings);

    public void ShowPopup_Next(string state)
    {
        GameObject obj = CreatePopup(popup_Next);

        Popup_Next popup = obj.GetComponent<Popup_Next>();
        if (popup != null)
        {
            popup.action = state;
        }
    }
}