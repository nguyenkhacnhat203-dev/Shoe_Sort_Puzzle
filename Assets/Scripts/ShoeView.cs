using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShoeView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ShoeSO shoeData;
    [SerializeField] private TextMeshProUGUI shoeIdText;

    public ShoeSO ShoeData => shoeData;
    public bool LockDrag = false;
    public Action OnChangedSlot;

    [HideInInspector] public Transform parentAfterDrag;
    private CanvasGroup canvasGroup;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        shoeIdText.text = shoeData.Id.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (LockDrag) return;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (LockDrag) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (LockDrag) return;
        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;
        canvasGroup.blocksRaycasts = true;
        OnChangedSlot?.Invoke();
    }
}
