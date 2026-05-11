using UnityEngine;
using DG.Tweening;

public abstract class PopupBase : MonoBehaviour
{
    public GameObject Main;


    protected virtual void OnEnable()
    {

    }


    protected virtual void OnDisable()
    {

    }

    protected virtual void Start()
    {
        OnShow();
    }

    public virtual void OnPlay()
    {
        AudioManager.Instance.BtnClick();
        if (ResourceManager.Instance.GetHeart() == 0)
        {
            UiManager.Instance.Show_Popup_Heart(() => {
                GameManager.Instance.ResetGame();
                GameManager.Instance.OnPlay();
                this.DestroyPopup();
            });
            return;
        }
        GameManager.Instance.ResetGame();
        GameManager.Instance.OnPlay();
        this.DestroyPopup();
    }

    public virtual void ReturnHome()
    {
        AudioManager.Instance.BtnClick();
        UiManager.Instance.Return_Home();
        this.DestroyPopup();
    }

    protected virtual void OnShow()
    {
        if (Main != null)
        {
            Main.transform.localScale = Vector3.zero;

            Main.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.25f).SetUpdate(true).OnComplete(() =>
            {
                Main.transform.DOScale(Vector3.one, 0.15f).SetUpdate(true);
            });
        }
    }

    public virtual void DestroyPopup()
    {
        AudioManager.Instance.BtnClick();
        if (Main == null)
        {
            Destroy(gameObject);
            return;
        }

        Main.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.15f).SetUpdate(true).OnComplete(() =>
        {
            Main.transform.DOScale(Vector3.zero, 0.15f).SetUpdate(true).OnComplete(() =>
            {
                Destroy(gameObject);

            });
        });
    }
}
