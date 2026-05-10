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
        GameManager.Instance.ResetGame();
        GameManager.Instance.OnPlay();
        this.DestroyPopup();
    }

    public virtual void ReturnHome()
    {
        AudioManager.Instance.BtnClick();
        GameManager.Instance.ResetGame();
        UiManager.Instance.Return_Home();
        this.DestroyPopup();
    }

    protected virtual void OnShow()
    {
        if (Main != null)
        {
            Main.transform.localScale = Vector3.zero;

            Main.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.35f).SetUpdate(true).OnComplete(() =>
            {
                Main.transform.DOScale(Vector3.one, 0.25f).SetUpdate(true);
            });
        }
    }

    public virtual void DestroyPopup()
    {
        AudioManager.Instance.BtnClick();
        bool isPause = GameManager.Instance.IsPause;
        if (isPause)
        {
            Time.timeScale = 1;
            GameManager.Instance.SetPause(true);
        }
        if (Main == null)
        {
            Destroy(gameObject);
            return;
        }

        Main.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.25f).OnComplete(() =>
        {
            Main.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
            {
                Destroy(gameObject);

            });
        });
    }
}
