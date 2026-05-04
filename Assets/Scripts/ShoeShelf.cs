using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShoeShelf : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> _shoeSlots;
    [SerializeField] private List<List<Sprite>> _shoeList;
    [SerializeField] private SpriteRenderer _imgShelf;

    public List<SpriteRenderer> ShoeSlot => _shoeSlots;
    public List<List<Sprite>> ShoeList => _shoeList;
    public SpriteRenderer ImgShelf => _imgShelf;

    private int _currentList = 0;

    void Awake()
    {
        _shoeSlots = Utils.GetComponentChildren<SpriteRenderer>(this.transform);
        foreach (var shoe in _shoeSlots)
            shoe.gameObject.SetActive(false);
    }

    void Start()
    {
        this.OnSetShoe(_shoeList[_currentList]);
    }

    public void SetShoeList(List<List<Sprite>> shoes)
    {
        _shoeList = shoes;
    }

    public void OnPrepareShelf()
    {
        if (this.CheckEmpty())
            _currentList++;
        else
            return;
        if (_currentList >= _shoeList.Count)
        {
            _imgShelf.DOKill();
            _imgShelf.DOFade(0, 0.5f).OnComplete(() =>
            {
                this.gameObject.SetActive(false);
            });
            return;
        }
        OnSetShoe(_shoeList[_currentList]);
    }
    public void OnSetShoe(List<Sprite> items)
    {
        if (items.Count <= _shoeSlots.Count)
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
        var availableSlots = _shoeSlots.Where(s => !s.gameObject.activeInHierarchy).ToList();
        if (availableSlots.Count == 0) return null;
        return availableSlots[Random.Range(0, availableSlots.Count)];
    }

    public bool CheckEmpty()
    {
        foreach (var shoe in _shoeSlots)
        {
            if (shoe.gameObject.activeInHierarchy)
                return false;
        }
        return true;
    }
}
