using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragDropController : MonoBehaviour
{
    [SerializeField] private Image _imageShoe;
    private ShoeSlot _currentSlot;
    private bool _hasDrag;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _currentSlot = Utils.GetRayCastUI<ShoeSlot>(Input.mousePosition);
            if (_currentSlot != null && _currentSlot.HasShoe)
            {
                _hasDrag = true;
                _imageShoe.gameObject.SetActive(true);
                _imageShoe.sprite = _currentSlot.ShoeSprite;
                _imageShoe.transform.position = Input.mousePosition;
            }
        }
        if (_hasDrag)
            {
                _imageShoe.transform.position = Input.mousePosition;
            }
        if (Input.GetMouseButtonUp(0) && _hasDrag)
        {
            _hasDrag = false;
            _imageShoe.gameObject.SetActive(false);
        }
    }
}
