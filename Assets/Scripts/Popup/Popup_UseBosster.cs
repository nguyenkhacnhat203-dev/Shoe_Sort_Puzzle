using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_UseBosster : PopupBase
{

    public Image BossterMagnet;
    public Image BossterShuffle;
    public Image BossterMoreBox;
    public TextMeshProUGUI title;
    private Booster _booster;

    protected override void OnEnable()
    {

    }


    protected override void OnDisable()
    {

    }

    protected override void Start()
    {

    }

    public void UseBooster()
    {
        switch (_booster)
        {
            case Booster.Magnet:
                GameManager.Instance.OnMagnet();
                break;
            case Booster.Shuffle:
                GameManager.Instance.OnShuffle();
                break;
            case Booster.MoreBox:
                GameManager.Instance.OnMoreBox();
                break;
        }
        GameManager.Instance.ChangeState(GameState.OnGame);
        DestroyPopup();
    }

    public override void DestroyPopup()
    {
        base.DestroyPopup();
        GameManager.Instance.ChangeState(GameState.OnGame);
    }

    public void SetupPopupBooster(Booster booster)
    {
        switch (booster)
        {
            case Booster.Magnet:
                BossterMagnet.gameObject.SetActive(true);
                title.text = "Pull 3 matching shoes";
                break;
            case Booster.Shuffle:
                BossterShuffle.gameObject.SetActive(true);
                title.text = "Shuffle all shoes";
                break;
            case Booster.MoreBox:
                BossterMoreBox.gameObject.SetActive(true);
                title.text = "Add one Box to game level";
                break;
        }
        _booster = booster;
    }

}
public enum Booster
{
    Magnet,
    Shuffle,
    MoreBox
}
