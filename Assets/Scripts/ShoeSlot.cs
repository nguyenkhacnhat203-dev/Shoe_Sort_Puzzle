using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShoeSlot : MonoBehaviour, IDropHandler
{
    public Action OnItemDropped;
    public Action OnItemRemoved;
    private ShoeView trackedShoe;
    public ShoeView TrackedShoe => trackedShoe;

    void Start()
    {
        ShoeView shoe = GetComponentInChildren<ShoeView>();
        if (shoe != null)
        {
            trackedShoe = shoe;
            shoe.OnChangedSlot += HandleShoeChanged;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (trackedShoe == null)
        {
            ShoeView shoe = eventData.pointerDrag.GetComponent<ShoeView>();
            if (shoe != null)
            {
                trackedShoe = shoe;
                shoe.parentAfterDrag = transform;
                shoe.OnChangedSlot += HandleShoeChanged;
            }
        }
        OnItemDropped?.Invoke();
    }

    public void AddShoe(ShoeView shoe)
    {
        if(trackedShoe == null)
        {
            trackedShoe = shoe;
            shoe.parentAfterDrag = transform;
            shoe.OnChangedSlot += HandleShoeChanged;
        }
    }

    private void HandleShoeChanged()
    {
        if (trackedShoe != null && trackedShoe.transform.parent != transform)
        {
            trackedShoe.OnChangedSlot -= HandleShoeChanged;
            trackedShoe = null;
            OnItemRemoved?.Invoke();
        }
    }
}
