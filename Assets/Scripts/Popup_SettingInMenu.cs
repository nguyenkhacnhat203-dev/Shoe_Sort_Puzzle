using UnityEngine;
using UnityEngine.UI;

public class Popup_SettingInMenu : PopupBase
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

    private const string SOUND = "Sound";
    private const string MUSIC = "Music";
    private const string VIBRATE = "Vibrate";

    protected override void Start()
    {
        base.Start();


        btnMusicOn.onClick.AddListener(ClickMusicOn);
        btnMusicOff.onClick.AddListener(ClickMusicOff);

        btnSoundOn.onClick.AddListener(ClickSoundOn);
        btnSoundOff.onClick.AddListener(ClickSoundOff);

        btnVibrationOn.onClick.AddListener(ClickVibrationOn);
        btnVibrationOff.onClick.AddListener(ClickVibrationOff);

        LoadSetting();
        UpdateUI();
    }

    void ClickMusicOn()
    {
        ToggleMusic();
        UpdateUI();
    }

    void ClickMusicOff()
    {
        ToggleMusic();
        UpdateUI();
    }


    void ClickSoundOn()
    {
        ToggleSound();
        UpdateUI();
    }

    void ClickSoundOff()
    {
        ToggleSound();
        UpdateUI();
    }


    void ClickVibrationOn()
    {
        ToggleVibrate();
        UpdateUI();
    }

    void ClickVibrationOff()
    {
        ToggleVibrate();
        UpdateUI();
    }

    #region BUTTON EVENTS
    public void ToggleSound()
    {
        AudioManager.Instance.BtnClick();
        isSoundOn = !isSoundOn;
        PlayerPrefs.SetInt("Sound", isSoundOn ? 1 : 0);

        AudioManager.Instance.AdjustSoundEffectsVolume(isSoundOn ? 1f : 0f);
        UpdateUI();
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.BtnClick();

        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("Music", isMusicOn ? 1 : 0);

        AudioManager.Instance.AdjustBackgroundMusicVolume(isMusicOn ? 1f : 0f);
        UpdateUI();
    }

    public void ToggleVibrate()
    {
        AudioManager.Instance.BtnClick();

        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("Vibrate", isVibrationOn ? 1 : 0);

#if UNITY_ANDROID || UNITY_IOS
        if (isVibrateOn)
            Handheld.Vibrate();
#endif
        UpdateUI();
    }
    #endregion
    #region LOAD & UI
    void LoadSetting()
    {
        isSoundOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        isMusicOn = PlayerPrefs.GetInt("Music", 1) == 1;
        isVibrationOn = PlayerPrefs.GetInt("Vibrate", 1) == 1;

        AudioManager.Instance.AdjustSoundEffectsVolume(isSoundOn ? 1f : 0f);
        AudioManager.Instance.AdjustBackgroundMusicVolume(isMusicOn ? 1f : 0f);
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
    #endregion
}