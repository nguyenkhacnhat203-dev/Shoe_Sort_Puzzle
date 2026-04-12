using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShoeSlot : MonoBehaviour
{
    private Image _imageShoe;
    
    private void Awake() {
        _imageShoe = this.transform.GetChild(0).GetComponent<Image>();
        _imageShoe.gameObject.SetActive(false);
    }

    internal void OnSetSlot(Sprite spr)
    {
        _imageShoe.gameObject.SetActive(true);
        _imageShoe.sprite = spr;
        _imageShoe.SetNativeSize();
    }

    public bool HasShoe => _imageShoe.gameObject.activeInHierarchy;
    public Sprite ShoeSprite => _imageShoe.sprite;
}
