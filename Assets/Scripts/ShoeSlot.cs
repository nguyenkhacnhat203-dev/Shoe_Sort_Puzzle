using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShoeSlot : MonoBehaviour
{
    private SpriteRenderer _imageShoe;
    private ShoeBox _shoeBox;
    private Color _normalColor = new Color(1, 1, 1);
    private Color _fadeColor = new Color(1, 1, 1, 0.7f);

    public SpriteRenderer ImageShoe => _imageShoe;

    private void Awake()
    {
        _imageShoe = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        _imageShoe.gameObject.SetActive(false);

        _shoeBox = this.transform.GetComponentInParent<ShoeBox>();
    }

    internal void OnSetSlot(Sprite spr)
    {
        _imageShoe.gameObject.SetActive(true);
        _imageShoe.sprite = spr;
        // _imageShoe.SetNativeSize();
    }

    public void OnActive(bool active)
    {
        _imageShoe.gameObject.SetActive(active);
        _imageShoe.color = _normalColor;
    }

    public void OnFadeShoe()
    {
        this.OnActive(true);
        _imageShoe.color = _fadeColor;
    }
    public void OnHideShoe()
    {
        this.OnActive(false);
        _imageShoe.color = _normalColor;
    }

    public void OnCheckMerge()
    {
        _shoeBox.CheckMerge();
    }

    public Tween OnPrepareItem(SpriteRenderer img)
    {
        this.OnSetSlot(img.sprite);
        _imageShoe.color = _normalColor;

        _imageShoe.transform.position = img.transform.position;
        _imageShoe.transform.localScale = img.transform.localScale;
        _imageShoe.transform.localEulerAngles = img.transform.localEulerAngles;

        Sequence itemSeq = DOTween.Sequence().SetLink(gameObject);
        _imageShoe.transform.DOKill();
        itemSeq.Join(_imageShoe.transform.DOLocalMove(Vector3.zero, 0.2f));
        itemSeq.Join(_imageShoe.transform.DORotate(Vector3.zero, 0.2f));
        itemSeq.Join(_imageShoe.transform.DOScale(Vector3.one, 0.2f));
        return itemSeq;
    }

    public void OnPrepareShelf()
    {
        _shoeBox.OnCheckPrepareShelf();
    }

    internal void OnShake()
    {
        _imageShoe.transform.DOKill();
        _imageShoe.transform.DOShakePosition(0.5f, 0.1f, 180).SetLink(_imageShoe.gameObject);
    }

    public bool HasShoe => _imageShoe.gameObject.activeInHierarchy && _imageShoe.color == _normalColor;
    public Sprite ShoeSprite => _imageShoe.sprite;
    public ShoeSlot GetSlotNull => _shoeBox.GetSlotNull();
}
