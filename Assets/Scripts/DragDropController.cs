using System.Linq;
using DG.Tweening;
using UnityEngine;

public class DragDropController : MonoBehaviour
{
    [SerializeField] private float _timeSuggest = 3f;
    [SerializeField] private SpriteRenderer _imageShoe, _lineShoe;
    private ShoeSlot _currentSlot, _cachedSlot;
    private bool _hasDrag = false, _hasPress = false;
    private bool _isCompletingDrag = false, _isCompletingPress = false;
    private float _timeCount = 0, _startTimePress = 0;
    private Vector3 _startPosition;

    public void Reset()
    {
        _imageShoe.gameObject.SetActive(false);
        _hasDrag = _hasPress = _isCompletingDrag = _isCompletingPress = false;
        _currentSlot = _cachedSlot = null;
        _timeCount = _startTimePress = 0;
        _startPosition = Vector3.zero;
    }

    void Update()
    {
        _timeCount += Time.deltaTime;
        if (_timeCount >= _timeSuggest)
        {
            _timeCount = 0;
            GameManager.Instance.OnCheckAndShake();
        }
        if (_isCompletingPress || _isCompletingDrag) return;
        if (_hasPress && Input.GetMouseButtonDown(0))
        {
            AudioManager.Instance.Move();
            _isCompletingPress = true;
            ShoeSlot slot = Utils.GetRayCastWorld2D<ShoeSlot>(Input.mousePosition);
            if (slot != null)
            {
                if (!slot.HasShoe)
                {
                    if (slot.GetInstanceID() != _cachedSlot.GetInstanceID())
                    {
                        _cachedSlot?.OnHideShoe();
                        _cachedSlot = slot;

                        _imageShoe.transform.DOKill();
                        _imageShoe.transform.DOScale(1, 0.2f).SetLink(_imageShoe.gameObject);
                        _imageShoe.transform.DOMove(_cachedSlot.transform.position, 0.2f)
                            .SetLink(_imageShoe.gameObject)
                            .OnComplete(() =>
                            {
                                if (_imageShoe != null && _cachedSlot != null && _currentSlot != null)
                                {
                                    _imageShoe.gameObject.SetActive(false);
                                    _cachedSlot.OnSetSlot(_currentSlot.ShoeSprite);
                                    _cachedSlot.OnActive(true);
                                    _cachedSlot.OnCheckMerge();
                                    _currentSlot.OnPrepareShelf();
                                }
                                _cachedSlot = null;
                                _currentSlot = null;
                                _isCompletingPress = false;
                            });
                    }
                    else
                    {
                        _imageShoe.transform.DOKill();
                        _imageShoe.transform.DOScale(1, 0.2f).SetLink(_imageShoe.gameObject).OnComplete(() =>
                        {
                            _imageShoe.gameObject.SetActive(false);
                            _currentSlot.OnActive(true);
                            _isCompletingPress = false;
                            _currentSlot = null;
                            _cachedSlot = null;
                        });
                    }
                }
                else
                {
                    ShoeSlot availableSlot = slot.GetSlotNull;
                    if (availableSlot != null)
                    {
                        _cachedSlot?.OnHideShoe();
                        _cachedSlot = availableSlot;

                        _imageShoe.transform.DOKill();
                        _imageShoe.transform.DOScale(1, 0.2f).SetLink(_imageShoe.gameObject);
                        _imageShoe.transform.DOMove(_cachedSlot.transform.position, 0.2f)
                            .SetLink(_imageShoe.gameObject)
                            .OnComplete(() =>
                            {
                                if (_imageShoe != null && _cachedSlot != null && _currentSlot != null)
                                {
                                    _imageShoe.gameObject.SetActive(false);
                                    _cachedSlot.OnSetSlot(_currentSlot.ShoeSprite);
                                    _cachedSlot.OnActive(true);
                                    _cachedSlot.OnCheckMerge();
                                    _currentSlot.OnPrepareShelf();
                                }
                                _cachedSlot = null;
                                _currentSlot = null;
                                _isCompletingPress = false;
                            });
                    }
                }

            }
            else
            {
                _imageShoe.transform.DOKill();
                _imageShoe.transform.DOScale(1, 0.2f).SetLink(_imageShoe.gameObject).OnComplete(() =>
                {
                    _imageShoe.gameObject.SetActive(false);
                    _currentSlot.OnActive(true);
                    _isCompletingPress = false;
                    _currentSlot = null;
                    _cachedSlot = null;
                });
            }
            _hasPress = false;
            return; // Dừng Update ngay tại đây để không chạy nhầm vào Drag!
        }

        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.Instance.Move();
            if (!_hasDrag && !_hasPress)
            {
                _currentSlot = Utils.GetRayCastWorld2D<ShoeSlot>(Input.mousePosition);
                if (_currentSlot != null && _currentSlot.HasShoe)
                {
                    GameManager.Instance.StartTimer();
                    
                    _startPosition = Input.mousePosition;
                    _startTimePress = Time.time;
                    // _hasDrag = true;
                    _cachedSlot = _currentSlot;
                    _imageShoe.gameObject.SetActive(true);
                    _imageShoe.sprite = _currentSlot.ShoeSprite;
                    Vector3 worldPos = _currentSlot.transform.position;
                    // worldPos.z = 0;
                    _imageShoe.transform.position = worldPos;
                    _imageShoe.DOKill();
                    _imageShoe.transform.DOScale(1.4f, 0.15f).SetLink(_imageShoe.gameObject);

                    _lineShoe.sprite = Resources.LoadAll<Sprite>("Line Shoe").FirstOrDefault(s => s.name == "Line " + _imageShoe.sprite.name);
                    if (_lineShoe.sprite == null)
                    {
                        Debug.Log("Line not found");
                    }
                    _currentSlot.OnHideShoe();

                    return;
                }
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (_cachedSlot != null && !_hasPress)
            {
                float distance = Vector3.Distance(_startPosition, Input.mousePosition);
                float time = Time.time - _startTimePress;
                if (distance > 10 || time > 0.4)
                    _hasDrag = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Nếu nhả chuột ra mà KHÔNG PHẢI Drag -> Đây đích thị là 1 cú chạm (Press)
            if (!_hasDrag && _cachedSlot != null && !_hasPress)
            {
                _hasPress = true;
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

            if (Input.GetMouseButtonUp(0) && _hasDrag)
            {
                AudioManager.Instance.Move();
                _isCompletingDrag = true;

                if (_cachedSlot != null)
                {
                    _imageShoe.transform.DOKill();
                    _imageShoe.transform.DOScale(1, 0.2f).SetLink(_imageShoe.gameObject);
                    _imageShoe.transform.DOMove(_cachedSlot.transform.position, 0.2f)
                        .SetLink(_imageShoe.gameObject)
                        .OnComplete(() =>
                        {
                            if (_imageShoe != null && _cachedSlot != null && _currentSlot != null)
                            {
                                _imageShoe.gameObject.SetActive(false);
                                _cachedSlot.OnSetSlot(_currentSlot.ShoeSprite);
                                _cachedSlot.OnActive(true);
                                _cachedSlot.OnCheckMerge();
                                _currentSlot.OnPrepareShelf();
                            }
                            _cachedSlot = null;
                            _currentSlot = null;
                            _isCompletingDrag = false;
                        });
                }
                else
                {
                    _imageShoe.transform.DOKill();
                    _imageShoe.transform.DOScale(1, 0.2f).SetLink(_imageShoe.gameObject);
                    _imageShoe.transform.DOMove(_currentSlot.transform.position, 0.2f)
                        .SetLink(_imageShoe.gameObject)
                        .OnComplete(() =>
                        {
                            if (_imageShoe != null && _currentSlot != null)
                            {
                                _imageShoe.gameObject.SetActive(false);
                                _currentSlot.OnActive(true);
                            }
                            _isCompletingDrag = false;
                        });
                }

                _hasDrag = false;
            }
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
        _isCompletingDrag = false;
    }
}
