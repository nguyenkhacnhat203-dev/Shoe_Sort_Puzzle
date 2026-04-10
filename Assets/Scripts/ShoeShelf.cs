using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ShoeShelf : MonoBehaviour
{
    [SerializeField] private ShoeSlot[] slots;

    void Start()
    {
        slots = GetComponentsInChildren<ShoeSlot>();
        foreach (var slot in slots)
        {
            ShoeView shoe = slot.transform.GetComponentInChildren<ShoeView>();
            if (shoe != null)
            {
                shoe.LockDrag = true;
            }
                
        }
    }
    public IEnumerator MoveShoeToSlot(ShoeSlot[] slotTarget)
    {
        yield return new WaitForEndOfFrame();

        float duration = 0.5f;

        for (int i = 0; i < slots.Length; i++)
        {
            ShoeView shoe = slots[i].GetComponentInChildren<ShoeView>();

            if (shoe != null)
            {
                int targetIndex = i;
                shoe.transform.SetParent(transform.root);

                shoe.transform.DOMove(slotTarget[targetIndex].transform.position, duration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        if (shoe != null && slotTarget[targetIndex] != null)
                        {
                            shoe.transform.SetParent(slotTarget[targetIndex].transform);
                            shoe.transform.localPosition = Vector3.zero;
                            shoe.LockDrag = false;
                        }
                    });

                // Phóng to đồng thời
                shoe.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
