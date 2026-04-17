using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShoeShelf : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> _shoeList;
    [SerializeField] private SpriteRenderer _imgShelf;

    public List<SpriteRenderer> ShoeList => _shoeList;
    public SpriteRenderer ImgShelf => _imgShelf;

    void Awake()
    {
        _shoeList = Utils.GetComponentChildren<SpriteRenderer>(this.transform);
        foreach (var shoe in _shoeList)
            shoe.gameObject.SetActive(false);
    }
    public void OnSetShoe(List<Sprite> items)
    {
        if (items.Count <= _shoeList.Count)
        {
            for (int i = 0; i < items.Count; i++)
            {
                SpriteRenderer slot = this.RandomSlot();
                slot.gameObject.SetActive(true);
                slot.sprite = items[i];
            }
        }
    }

    public SpriteRenderer RandomSlot()
    {
    rerand: int n = Random.Range(0, _shoeList.Count);
        if (_shoeList[n].gameObject.activeInHierarchy) goto rerand;

        return _shoeList[n];
    }

    public bool CheckEmpty()
    {
        foreach (var shoe in _shoeList)
        {
            if (shoe.gameObject.activeInHierarchy)
                return false;
        }
        return true;
    }
}
