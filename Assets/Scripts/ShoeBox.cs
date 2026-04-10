using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoeBox : MonoBehaviour
{
    [SerializeField] private ShoeSlot[] slots;
    [SerializeField] private ShoeShelf shoeShelf;

    void Start()
    {
        slots = GetComponentsInChildren<ShoeSlot>();
        foreach (var slot in slots)
        {
            slot.OnItemDropped += CheckMatch;
            slot.OnItemRemoved += CheckSlotEmpty;
        }
    }

    private void CheckMatch()
    {
        StartCoroutine(CheckMatchRoutine());
    }

    private void CheckSlotEmpty()
    {
        StartCoroutine(CheckSlotEmptyRoutine());
    }

    private IEnumerator CheckSlotEmptyRoutine()
    {
        yield return new WaitForEndOfFrame();
        foreach (var slot in slots)
        {
            if (slot.TrackedShoe != null)
            {
                Debug.Log("At least one slot is not empty, cannot move shoes back to shelf.");
                yield break;
            }
        }
        StartCoroutine(shoeShelf.MoveShoeToSlot(slots));
    }

    private IEnumerator CheckMatchRoutine()
    {
        yield return new WaitForEndOfFrame();

        if (slots == null || slots.Length == 0) yield break;

        foreach (var slot in slots)
        {
            if (slot.transform.childCount == 0)
            {
                yield break;
            }
        }

        int firstShoeId = slots[0].transform.GetComponentInChildren<ShoeView>().ShoeData.Id;
        Debug.Log("First Shoe ID: " + firstShoeId);
        bool isMatch = true;

        foreach (var slot in slots)
        {
            ShoeView shoe = slot.transform.GetComponentInChildren<ShoeView>();
            if (shoe == null || shoe.ShoeData.Id != firstShoeId)
            {
                isMatch = false;
                yield break;
            }
        }

        if (isMatch)
        {
            foreach (var slot in slots)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }

            StartCoroutine(shoeShelf.MoveShoeToSlot(slots));
        }
    }
}
