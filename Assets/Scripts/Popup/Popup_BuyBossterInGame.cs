using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_BuyBossterInGame : PopupBase
{
    public TextMeshProUGUI txtCoin, txtHeart;
    public Image BossterMagnet;
    public Image BossterShuffle;
    public Image BossterMoreBox;
    public TextMeshProUGUI title;
    private Booster _booster;
    private string _boosterKey;
    private int _price;
    protected override void OnEnable()
    {

    }


    protected override void OnDisable()
    {

    }

    protected override void Start()
    {
        txtCoin.text = ResourceManager.Instance.GetCoin().ToString();
        txtHeart.text = ResourceManager.Instance.GetHeart().ToString();
    }
    public void BuyBooster()
    {
        int coin = ResourceManager.Instance.GetCoin();
        if (_price > coin)
        {
            DestroyPopup();
            return;
        }
        ResourceManager.Instance.ChangeCountBooster(_boosterKey, 1);
        ResourceManager.Instance.ChangeCoin(-_price);
        DestroyPopup();
    }
    public void SetupPopupBooster(Booster booster, string key)
    {
        switch (booster)
        {
            case Booster.Magnet:
                BossterMagnet.gameObject.SetActive(true);
                title.text = "Buy booster Magnet with 800 Gold";
                _price = 800;
                break;
            case Booster.Shuffle:
                BossterShuffle.gameObject.SetActive(true);
                title.text = "Buy booster Shuffle with 300 Gold";
                _price = 300;
                break;
            case Booster.MoreBox:
                BossterMoreBox.gameObject.SetActive(true);
                title.text = "Buy booster MoreBox with 500 Gold";
                _price = 500;
                break;
        }
        _boosterKey = key;
        _booster = booster;
    }
}
