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

    protected virtual void OnShow()
    {
        if (Main != null)
        {
            Main.transform.localScale = Vector3.zero;
            GameManager.Instance.PauseGame();

            Main.transform
                .DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.25f)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    Main.transform
                        .DOScale(Vector3.one, 0.25f)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                        });
                });
        }
    }

    public virtual void DestroyPopup()
    {
        if (Main == null)
        {
            GameManager.Instance.ResumeGame();
            Destroy(gameObject);
            return;
        }

        Main.transform
            .DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.25f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                Main.transform
                    .DOScale(Vector3.zero, 0.25f)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        GameManager.Instance.ResumeGame();
                        Destroy(gameObject);
                    });
            });
    }




}