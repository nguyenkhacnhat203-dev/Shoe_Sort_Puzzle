using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ShoeBox : MonoBehaviour
{
    [SerializeField] private Transform _slotContainer;
    [SerializeField] private Transform _shelfContainer;
    [SerializeField] private Transform _ShoeBoxLid;
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

    private void Start()
    {
        _ShoeBoxLid.transform.DOKill();
        _ShoeBoxLid.transform.DOLocalMoveY(1f,1f);
        _ShoeBoxLid.GetComponent<SpriteRenderer>().DOFade(0, 1f);
    }

    public void OnInitBox(int totalShelf, List<Sprite> listShoe, bool forceNotFull)
    {
        int shoeCount;
        if (forceNotFull)
            shoeCount = Random.Range(1, 3);
        else
            shoeCount = Random.Range(1, _totalSlots.Count + 1);
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
            var availableShelves = remainShoe.Where(s => s.Count < 3).ToList();
            if (availableShelves.Count == 0) break; // Avoid infinite loop if no shelf has space

            int rans = Random.Range(0, availableShelves.Count);
            int n = Random.Range(0, listShoe.Count);
            availableShelves[rans].Add(listShoe[n]);
            listShoe.RemoveAt(n);
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
                Sequence seq = DOTween.Sequence().SetLink(gameObject);
                foreach (var slot in _totalSlots)
                {
                    Vector3 posImage = slot.ImageShoe.transform.localPosition;
                    slot.ImageShoe.transform.DOKill();
                    seq.Join(slot.ImageShoe.transform.DOLocalMoveY(0.2f, 0.2f));
                    seq.Join(slot.ImageShoe.DOFade(0, 0.2f).OnComplete(() =>
                    {
                        slot.OnActive(false);
                        slot.ImageShoe.transform.localPosition = posImage;
                    }));
                    seq.AppendInterval(0.1f);
                }
                seq.OnComplete(() =>
                {
                    this.OnPrepareShelf();
                });
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
            Sequence mainSeq = DOTween.Sequence();

            for (int i = 0; i < shelf.ShoeList.Count; i++)
            {
                SpriteRenderer img = shelf.ShoeList[i];
                if (img.gameObject.activeInHierarchy)
                {
                    mainSeq.Join(_totalSlots[i].OnPrepareItem(img));
                    mainSeq.AppendInterval(0.1f);
                    img.gameObject.SetActive(false);
                }
            }
            mainSeq.SetLink(gameObject).OnComplete(() =>
            {
                shelf.transform.DOKill();
                shelf.transform.DOLocalMoveY(0.05f, 0.5f).SetLink(shelf.gameObject);
                shelf.ImgShelf.DOKill();
                shelf.ImgShelf.DOFade(0, 0.5f).SetLink(shelf.gameObject).OnComplete(() =>
                {
                    shelf.gameObject.SetActive(false);
                });
            });
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
        var availableSlots = _totalSlots.Where(s => !s.HasShoe).ToList();
        if (availableSlots.Count == 0) return null;
        return availableSlots[Random.Range(0, availableSlots.Count)];
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
