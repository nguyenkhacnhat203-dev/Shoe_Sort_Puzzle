using UnityEngine;
using System;
using System.Collections;
public class ResourceManager : Singleton<ResourceManager>
{
    #region Constants
    public const string FIRST_TIME = "First_Time";
    private const string LEVEL_KEY = "Current_Level";
    private const string COIN = "COIN";
    private const string HEART = "Heart";
    private const string MAGNET_BOOSTER = "Magnet_Booster";
    private const string SHUFFLE_BOOSTER = "Shuffle_Booster";
    private const string MORE_BOX_BOOSTER = "More_Box_Booster";
    private const string DATE_TIME = "Date_Time";

    private const int TIME_COUNT = 1800; //30p hồi 1 tim
    private const int MAX_HEART = 5; //tối đa 5 tim
    #endregion

    #region Variables
    private Coroutine _heartCoroutine;
    public float CurrentHeartTimer { get; private set; }
    #endregion

    #region Unity Lifecycle & Init
    void Start()
    {
        LoadHeartTimer();
    }
    public void InitResource()
    {
        if (PlayerPrefs.HasKey(FIRST_TIME))
            return;
        PlayerPrefs.SetInt(COIN, 1000);
        PlayerPrefs.SetInt(HEART, MAX_HEART);
        PlayerPrefs.SetInt(MAGNET_BOOSTER, 3);
        PlayerPrefs.SetInt(SHUFFLE_BOOSTER, 3);
        PlayerPrefs.SetInt(MORE_BOX_BOOSTER, 3);
        PlayerPrefs.SetInt(LEVEL_KEY, 1);
        PlayerPrefs.SetInt(FIRST_TIME, 1);
    }
    #endregion

    #region General Game Resources
    public bool CanBuy(int price)
    {
        int coin = this.GetCoin();
        if (price <= coin)
        {
            return true;
        }
        return false;
    }

    public int GetLevel()
    {
        return PlayerPrefs.GetInt(LEVEL_KEY);
    }
    public void SetLevel(int level)
    {
        PlayerPrefs.SetInt(LEVEL_KEY, level);
    }
    public int GetCoin()
    {
        return PlayerPrefs.GetInt(COIN);
    }
    public void ChangeCoin(int amount)
    {
        int newCoin = this.GetCoin() + amount;
        PlayerPrefs.SetInt(COIN, newCoin);
        UiManager.Instance.UpdateStats();
    }
    #endregion

    #region Heart Management
    public int GetHeart()
    {
        return PlayerPrefs.GetInt(HEART);
    }
    public bool IsHeartFull()
    {
        if (this.GetHeart() == MAX_HEART)
            return true;
        return false;
    }
    public void SetHeart(int amount)
    {
        int currentHeart = PlayerPrefs.GetInt(HEART);
        bool wasFull = currentHeart >= MAX_HEART;

        int newHeart = currentHeart + amount;
        if (newHeart >= MAX_HEART)
        {
            PlayerPrefs.SetInt(HEART, newHeart);
            PlayerPrefs.DeleteKey(DATE_TIME);
            if (_heartCoroutine != null)
            {
                StopCoroutine(_heartCoroutine);
                _heartCoroutine = null;
            }
            UiManager.Instance.UpdateStats();
            return;
        }

        if (newHeart < 0) newHeart = 0; // Tránh tim bị âm

        PlayerPrefs.SetInt(HEART, newHeart);

        // Bắt đầu đếm ngược thời gian hồi tim nếu bị mất đi lần đầu (từ mức tối đa)
        if (wasFull && amount < 0)
        {
            PlayerPrefs.SetString(DATE_TIME, DateTime.Now.ToBinary().ToString());
            StartHeartCoroutine(TIME_COUNT);
        }
        UiManager.Instance.UpdateStats();
    }
    private void LoadHeartTimer()
    {
        if (GetHeart() >= MAX_HEART) return; // Đã đầy tim

        if (PlayerPrefs.HasKey(DATE_TIME))
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString(DATE_TIME));
            DateTime oldTime = DateTime.FromBinary(temp);

            TimeSpan timePassed = DateTime.Now - oldTime;
            int heartsToAdd = (int)(timePassed.TotalSeconds / TIME_COUNT);

            if (heartsToAdd > 0)
            {
                int currentHeart = GetHeart();
                if (currentHeart + heartsToAdd >= MAX_HEART)
                {
                    PlayerPrefs.SetInt(HEART, MAX_HEART);
                    PlayerPrefs.DeleteKey(DATE_TIME);
                    return;
                }
                else
                {
                    PlayerPrefs.SetInt(HEART, currentHeart + heartsToAdd);

                    // Cập nhật lại mốc thời gian của tim tiếp theo
                    DateTime newTime = oldTime.AddSeconds(heartsToAdd * TIME_COUNT);
                    PlayerPrefs.SetString(DATE_TIME, newTime.ToBinary().ToString());

                    // Tính thời gian còn lại cho tim đang hồi dở
                    float secondsPassedForNextHeart = (float)(timePassed.TotalSeconds % TIME_COUNT);
                    StartHeartCoroutine(TIME_COUNT - secondsPassedForNextHeart);
                }
            }
            else
            {
                // Chưa đủ thời gian để hồi 1 tim, tiếp tục đếm ngược thời gian còn lại
                StartHeartCoroutine(TIME_COUNT - (float)timePassed.TotalSeconds);
            }
        }
        else
        {
            // Dự phòng: Không có thời gian lưu, bắt đầu đếm từ đầu
            PlayerPrefs.SetString(DATE_TIME, DateTime.Now.ToBinary().ToString());
            StartHeartCoroutine(TIME_COUNT);
        }
    }

    private void StartHeartCoroutine(float remainingTime)
    {
        if (_heartCoroutine != null)
        {
            StopCoroutine(_heartCoroutine);
        }
        _heartCoroutine = StartCoroutine(HeartCountdown(remainingTime));
    }

    private IEnumerator HeartCountdown(float remainingTime)
    {
        CurrentHeartTimer = remainingTime;

        while (CurrentHeartTimer > 0)
        {
            // Sử dụng WaitForSecondsRealtime để không bị ảnh hưởng bởi Time.timeScale = 0
            yield return new WaitForSecondsRealtime(1f);
            CurrentHeartTimer -= 1f;
        }

        // Hoàn thành đếm ngược, hồi 1 tim
        SetHeart(1);

        // Cập nhật giao diện
        if (UiManager.Instance != null)
        {
            UiManager.Instance.UpdateStats();
        }

        // Reset lại thời gian đếm ngược sau khi hoàn thành (nếu tim vẫn chưa đầy)
        if (GetHeart() < MAX_HEART)
        {
            PlayerPrefs.SetString(DATE_TIME, DateTime.Now.ToBinary().ToString());
            StartHeartCoroutine(TIME_COUNT);
        }
    }
    #endregion

    #region Booster Management
    public int GetCountBooster(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public int GetCountMagnet()
    {
        return PlayerPrefs.GetInt(MAGNET_BOOSTER);
    }
    public int GetCountShuffle()
    {
        return PlayerPrefs.GetInt(SHUFFLE_BOOSTER);
    }
    public int GetCountMoreBox()
    {
        return PlayerPrefs.GetInt(MORE_BOX_BOOSTER);
    }
    public bool CanUseBooster(string nameBooster)
    {
        int count = PlayerPrefs.GetInt(nameBooster);
        if (count > 0)
            return true;
        return false;
    }
    public void ChangeCountBooster(string nameBooster, int amount)
    {
        int currentAmount = PlayerPrefs.GetInt(nameBooster);
        PlayerPrefs.SetInt(nameBooster, currentAmount + amount);
        UiManager.Instance.UpdateStats();
    }
    #endregion
}