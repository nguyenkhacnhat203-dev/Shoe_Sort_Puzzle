using DG.Tweening;
using UnityEngine;

public class DragDropController : MonoBehaviour
{
    [SerializeField] private float _timeSuggest = 3f;
    [SerializeField] private SpriteRenderer _imageShoe;
    private ShoeSlot _currentSlot, _cachedSlot;
    private bool _hasDrag;
    private float _timeCount = 0;

    void Update()
    {
        _timeCount += Time.deltaTime;
        if (_timeCount >= _timeSuggest)
        {
            _timeCount = 0;
            GameManager.Instance.OnCheckAndShake();
        }
        if (Input.GetMouseButtonDown(0))
        {
            _currentSlot = Utils.GetRayCastWorld2D<ShoeSlot>(Input.mousePosition);
            if (_currentSlot != null && _currentSlot.HasShoe)
            {
                _hasDrag = true;
                _cachedSlot = _currentSlot;
                _imageShoe.gameObject.SetActive(true);
                _imageShoe.sprite = _currentSlot.ShoeSprite;
                _imageShoe.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                _currentSlot.OnHideShoe();
            }
        }
        if (_hasDrag)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            _imageShoe.transform.position = worldPos;

            ShoeSlot slot = Utils.GetRayCastWorld2D<ShoeSlot>(Input.mousePosition);
            if (slot != null)
            {
                if (!slot.HasShoe)
                {
                    if (_cachedSlot == null || slot.GetInstanceID() != _cachedSlot.GetInstanceID())
                    {
                        _cachedSlot?.OnHideShoe();
                        _cachedSlot = slot;
                        _cachedSlot.OnFadeShoe();
                        _cachedSlot.OnSetSlot(_currentSlot.ShoeSprite);
                    }
                }
                else
                {
                    ShoeSlot availableSlot = slot.GetSlotNull;
                    if (availableSlot != null)
                    {
                        _cachedSlot?.OnHideShoe();
                        _cachedSlot = availableSlot;
                        _cachedSlot.OnFadeShoe();
                        _cachedSlot.OnSetSlot(_currentSlot.ShoeSprite);
                    }
                    else
                        this.ClearCache();
                }
            }
            else
            {
                _cachedSlot?.OnHideShoe();
                this.ClearCache();
            }
        }
        if (Input.GetMouseButtonUp(0) && _hasDrag)
        {
            if (_cachedSlot != null)
            {
                _imageShoe.transform.DOKill();

                _imageShoe.transform.DOMove(_cachedSlot.transform.position, 0.2f)
                    .SetLink(_imageShoe.gameObject)
                    .OnComplete(() =>
                    {
                        _imageShoe.gameObject.SetActive(false);
                        _cachedSlot.OnSetSlot(_currentSlot.ShoeSprite);
                        _cachedSlot.OnActive(true);
                        _cachedSlot.OnCheckMerge();
                        _currentSlot.OnPrepareShelf();
                        _cachedSlot = null;
                        _currentSlot = null;
                    });
            }
            else
            {
                _imageShoe.transform.DOKill();

                _imageShoe.transform.DOMove(_currentSlot.transform.position, 0.2f)
                    .SetLink(_imageShoe.gameObject)
                    .OnComplete(() =>
                    {
                        _imageShoe.gameObject.SetActive(false);
                        _currentSlot.OnActive(true);
                    });
            }

            _hasDrag = false;
        }
    }

    private void ClearCache()
    {
        if (_cachedSlot != null)
        {
            _cachedSlot.OnHideShoe();
            _cachedSlot = null;
        }
    }

    void OnDestroy()
    {
        if (_imageShoe != null)
            _imageShoe.transform.DOKill();
    }
}
