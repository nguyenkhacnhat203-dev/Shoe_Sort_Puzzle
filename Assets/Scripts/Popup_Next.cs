using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Popup_Next : PopupBase
{
    private Image img;

    public string action = "onGame";

    protected override void Start()
    {
        OnShow();

    }

    protected override void OnShow()
    {
        if (Main == null) return;
        img = Main.GetComponent<Image>();

        if (img == null) return;

        Color c = img.color;
        c.a = 0f;
        img.color = c;

        img.DOFade(1f, 0.5f).OnComplete(() =>
        {

            img.DOFade(0f, 0.1f).OnComplete(() =>
            {
                Destroy(gameObject);
            });

            if (action == "onGame")
            {
                UiManager.Instance.ShowGame();

            }
            else if (action == "onMenu")
            {
                UiManager.Instance.ShowMenu();

            }

           
        });
    }
}