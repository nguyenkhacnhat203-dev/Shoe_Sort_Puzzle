using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Image _imageDrag;
    [SerializeField] private ShoeSlot _slotTutorial;
    [SerializeField] private ShoeBox _shoeBox, _shoeBoxCheck;

    private bool _hasPlayedTutorial = false;

    void Start()
    {
        Setup();
    }

    void Update()
    {
        if (!_hasPlayedTutorial)
        {
            StartCoroutine(PlayTutorial());
            _hasPlayedTutorial = true;
        }
    }

    private IEnumerator PlayTutorial()
    {
        yield return new WaitForSeconds(1f);
        ShoeSlot slot = _shoeBoxCheck.GetSlotNull();
        _slotTutorial.OnHideShoe();
        _imageDrag.gameObject.SetActive(true);
        _imageDrag.transform.position = _slotTutorial.transform.position;
        _imageDrag.transform.DOMove(slot.transform.position, 0.5f).OnComplete(() =>
        {
            slot.OnSetSlot(_imageDrag.sprite);
            _imageDrag.gameObject.SetActive(false);
        });
        yield return new WaitForSeconds(1);
        foreach (var slotCheck in _shoeBoxCheck.TotalSlots)
        {
            if (slotCheck.HasShoe)
            {
                slotCheck.OnActive(false);
            }
        }
        yield return new WaitForSeconds(0.5f);
        _imageDrag.transform.position = Vector3.zero;
        _imageDrag.gameObject.SetActive(false);
        this.Setup();
        _hasPlayedTutorial = false;
    }

    private void Setup()
    {
        foreach (var slot in _shoeBox.TotalSlots)
        {
            if (!slot.HasShoe)
                slot.OnActive(true);
        }

        for (int i = 1; i < _shoeBoxCheck.TotalSlots.Count; i++)
        {
            if (!_shoeBoxCheck.TotalSlots[i].HasShoe)
                _shoeBoxCheck.TotalSlots[i].OnActive(true);
        }
    }
    public void HideTutorial()
    {
        _imageDrag.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
}