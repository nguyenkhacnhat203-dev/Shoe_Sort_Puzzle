using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Popup_SettingInGame : PopupBase
{
    [Header("Music")]
    public Button btnMusicOn;
    public Button btnMusicOff;

    [Header("Sound")]
    public Button btnSoundOn;
    public Button btnSoundOff;

    [Header("Vibration")]
    public Button btnVibrationOn;
    public Button btnVibrationOff;

    private bool isMusicOn = true;
    private bool isSoundOn = true;
    private bool isVibrationOn = true;

    protected override void Start()
    {
        base.Start();


        btnMusicOn.onClick.AddListener(ClickMusicOn);
        btnMusicOff.onClick.AddListener(ClickMusicOff);

        btnSoundOn.onClick.AddListener(ClickSoundOn);
        btnSoundOff.onClick.AddListener(ClickSoundOff);

        btnVibrationOn.onClick.AddListener(ClickVibrationOn);
        btnVibrationOff.onClick.AddListener(ClickVibrationOff);

        UpdateUI();
    }


    void ClickMusicOn()
    {
        isMusicOn = false;
        UpdateUI();
    }

    void ClickMusicOff()
    {
        isMusicOn = true;
        UpdateUI();
    }


    void ClickSoundOn()
    {
        isSoundOn = false;
        UpdateUI();
    }

    void ClickSoundOff()
    {
        isSoundOn = true;
        UpdateUI();
    }


    void ClickVibrationOn()
    {
        isVibrationOn = false;
        UpdateUI();
    }

    void ClickVibrationOff()
    {
        isVibrationOn = true;
        UpdateUI();
    }


    void UpdateUI()
    {
        btnMusicOn.gameObject.SetActive(isMusicOn);
        btnMusicOff.gameObject.SetActive(!isMusicOn);


        btnSoundOn.gameObject.SetActive(isSoundOn);
        btnSoundOff.gameObject.SetActive(!isSoundOn);

        btnVibrationOn.gameObject.SetActive(isVibrationOn);
        btnVibrationOff.gameObject.SetActive(!isVibrationOn);
    }
}
