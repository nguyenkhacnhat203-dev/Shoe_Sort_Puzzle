using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShoeBox : MonoBehaviour
{
    [SerializeField] private Transform _slotContainer;
    [SerializeField] private Transform _shelfContainer;
    [SerializeField] private BoxCollider2D _boxCollider;

    private List<ShoeSlot> _totalSlots;
    private List<ShoeShelf> _totalShelf;
    private Stack<ShoeShelf> _stackShelf = new Stack<ShoeShelf>();

    public List<ShoeSlot> TotalSlots => _totalSlots;
    public BoxCollider2D BoxCollider => _boxCollider;

    private void Awake()
    {
        _totalSlots = Utils.GetComponentChildren<ShoeSlot>(_slotContainer);
        _totalShelf = Utils.GetComponentChildren<ShoeShelf>(_shelfContainer);
    }

    public void OnInitBox(int totalShelf, List<Sprite> listShoe)
    {
        int shoeCount = Random.Range(1, _totalSlots.Count + 1);
        List<Sprite> list = listShoe;
        List<Sprite> listSlot = Utils.TakeAndRemoveRandom<Sprite>(list, shoeCount);
        for (int i = 0; i < listSlot.Count; i++)
        {
            ShoeSlot slot = this.RandomSlot();
            slot.OnSetSlot(listSlot[i]);
        }

        List<List<Sprite>> remainShoe = new List<List<Sprite>>();

        for (int i = 0; i < totalShelf; i++)
        {
            if (listShoe.Count == 0)
                break;
            remainShoe.Add(new List<Sprite>());
            int n = Random.Range(0, listShoe.Count);
            remainShoe[i].Add(listShoe[n]);
            listShoe.RemoveAt(n);
        }

        while (listShoe.Count > 0)
        {
            int rans = Random.Range(0, remainShoe.Count);
            if (remainShoe[rans].Count < 3)
            {
                int n = Random.Range(0, listShoe.Count);
                remainShoe[rans].Add(listShoe[n]);
                listShoe.RemoveAt(n);
            }
        }
        for (int i = 0; i < _totalShelf.Count; i++)
        {
            bool active = i < remainShoe.Count;
            _totalShelf[i].gameObject.SetActive(active);

            if (active)
            {
                _totalShelf[i].OnSetShoe(remainShoe[i]);
                _stackShelf.Push(_totalShelf[i]);
            }
        }
    }
    public void CheckMerge()
    {
        if (this.GetSlotNull() == null)
        {
            if (this.CanMerge())
            {
                foreach (var slot in _totalSlots)
                {
                    slot.OnActive(false);
                }
                this.OnPrepareShelf();
                GameManager.Instance.OnMinusShoe();
            }
        }
    }

    public void OnCheckPrepareShelf()
    {
        if (this.HasBoxEmpty())
            this.OnPrepareShelf();
    }
    private void OnPrepareShelf()
    {
        if (_stackShelf.Count > 0)
        {
            ShoeShelf shelf = _stackShelf.Pop();

            for (int i = 0; i < shelf.ShoeList.Count; i++)
            {
                SpriteRenderer img = shelf.ShoeList[i];
                if (img.gameObject.activeInHierarchy)
                {
                    _totalSlots[i].OnPrepareItem(img);
                    img.gameObject.SetActive(false);
                }
            }
            shelf.gameObject.SetActive(false);
        }
    }

    public void OnCheckShelfEmpty()
    {
        if (_stackShelf.Count > 0)
        {
            ShoeShelf shelf = _stackShelf.Peek();
            if (shelf.CheckEmpty())
            {
                _stackShelf.Pop();
                shelf.gameObject.SetActive(false);
            }
        }
    }

    private bool CanMerge()
    {
        string name = _totalSlots[0].ShoeSprite.name;
        for (int i = 1; i < _totalSlots.Count; i++)
        {
            if (_totalSlots[i].ShoeSprite.name != name)
                return false;
        }
        return true;
    }

    private ShoeSlot RandomSlot()
    {
    rerand: int n = Random.Range(0, _totalSlots.Count);
        if (_totalSlots[n].HasShoe) goto rerand;
        return _totalSlots[n];
    }

    public ShoeSlot GetSlotNull()
    {
        foreach (var slot in _totalSlots)
        {
            if (!slot.HasShoe)
                return slot;
        }
        return null;
    }

    public bool HasBoxEmpty()
    {
        foreach (var slot in _totalSlots)
        {
            if (slot.HasShoe)
                return false;
        }
        return true;
    }

    internal ShoeShelf GetFirstShelf()
    {
        if (_stackShelf.Count > 0)
            return _stackShelf.Peek();
        return null;
    }
}
