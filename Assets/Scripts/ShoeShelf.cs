using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShoeShelf : MonoBehaviour
{
    [SerializeField] private List<Image> _shoeList;

    public List<Image> ShoeList => _shoeList;

    void Awake()
    {
        _shoeList = Utils.GetComponentChildren<Image>(this.transform);
        foreach (var shoe in _shoeList)
            shoe.gameObject.SetActive(false);
    }
    public void OnSetShoe(List<Sprite> items)
    {
        if (items.Count <= _shoeList.Count)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Image slot = this.RandomSlot();
                slot.gameObject.SetActive(true);
                slot.sprite = items[i];
            }
        }
    }

    public Image RandomSlot()
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
