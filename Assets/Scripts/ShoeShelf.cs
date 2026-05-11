using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShoeShelf : MonoBehaviour
{
    #region Variables
    [SerializeField] private List<SpriteRenderer> _shoeSlots;
    [SerializeField] private List<List<Sprite>> _shoeList;
    [SerializeField] private SpriteRenderer _imgShelf;
    private int _currentList = 0;
    #endregion

    #region Properties
    public List<SpriteRenderer> ShoeSlot => _shoeSlots;
    public List<List<Sprite>> ShoeList => _shoeList;
    public SpriteRenderer ImgShelf => _imgShelf;
    #endregion

    #region Unity Lifecycle
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
    #endregion

    #region Initialization & Logic
    public void SetShoeList(List<List<Sprite>> shoes)
    {
        this.gameObject.SetActive(true);
        _shoeList = shoes;
    }

    public void OnPrepareShelf()
    {
        if (this.CheckEmpty() && this.gameObject.activeInHierarchy)
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
    #endregion

    #region Helper Methods
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
    #endregion
}
