using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShoeBox : MonoBehaviour
{
    [SerializeField] private Transform _slotContainer;
    [SerializeField] private Transform _shelfContainer;

    private List<ShoeSlot> _totalSlots;
    private List<ShoeShelf> _totalShelf;

    private void Awake()
    {
        _totalSlots = Utils.GetComponentChildren<ShoeSlot>(_slotContainer);
        _totalShelf = Utils.GetComponentChildren<ShoeShelf>(_shelfContainer);
    }

    public void OnInitBox(int totalShelf, List<Sprite> listShoe)
    {
        int shoeCount = Random.Range(1,_totalSlots.Count + 1);
        List<Sprite> list = listShoe;
        List<Sprite> listSlot = Utils.TakeAndRemoveRandom<Sprite>(list, shoeCount);
        Debug.Log("List slots: "+listSlot.Count);
        for (int i = 0; i < listSlot.Count; i++)
        {
            ShoeSlot slot = this.RandomSlot();
            slot.OnSetSlot(listSlot[i]);
        }

        List<List<Sprite>> remainShoe = new List<List<Sprite>>();

        for (int i = 0; i < totalShelf - 1; i++)
        {
            remainShoe.Add(new List<Sprite>());
            int n = Random.Range(0, listShoe.Count);
            remainShoe[i].Add(listShoe[n]);
            listShoe.RemoveAt(n);
        }

        while (listShoe.Count > 0)
        {
            int rans = Random.Range(0, remainShoe.Count);
            if (remainShoe[rans].Count < 4)
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
            }
        }
    }

    private ShoeSlot RandomSlot()
    {
    rerand: int n = Random.Range(0, _totalSlots.Count);
        if (_totalSlots[n].HasShoe) goto rerand;
        return _totalSlots[n];
    }
}
