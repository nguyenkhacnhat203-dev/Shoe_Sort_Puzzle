using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    [SerializeField] private int _totalShoe;
    [SerializeField] private int _totalShoeModel;
    [SerializeField] private int _totalBox;
    [SerializeField] private GameObject _prefabBox;
    [SerializeField] private Transform _gridBox;
    [SerializeField] private float _spaceBox;
    [SerializeField] private int _maxRowBox, _maxColBox;
    [SerializeField] private List<SpriteRenderer> _magnetList;
    [SerializeField] private RectTransform _magnetTarget;

    private List<ShoeBox> _listBox;
    private float _avgShelf;
    private List<Sprite> _totalSpriteShoe;
    void Awake()
    {
        OnSpawnBox(_totalBox, _spaceBox, _maxRowBox, _maxColBox);
        _listBox = _gridBox.GetComponentsInChildren<ShoeBox>().ToList();
        _totalSpriteShoe = Resources.LoadAll<Sprite>("Items").ToList();
        _instance = this;
    }
    void Start()
    {
        OnInitLevel();
    }

    private void OnInitLevel()
    {
        if (_totalShoe < _totalShoeModel || _totalShoe % 3 != 0)
        {
            Debug.LogError("Total shoe must be greater than total shoe model and divisible by 3");
            return;
        }

        List<Sprite> takeShoe = _totalSpriteShoe.OrderBy(x => Random.value).Take(_totalShoeModel).ToList();
        List<Sprite> useShoe = new List<Sprite>();

        this.FillUseShoe(takeShoe, useShoe, _totalShoe);

        for (int i = 0; i < useShoe.Count; i++)
        {
            int rand = Random.Range(i, useShoe.Count);
            (useShoe[i], useShoe[rand]) = (useShoe[rand], useShoe[i]);
        }

        _avgShelf = Random.Range(1.5f, 2f);
        int totalShelf = Mathf.RoundToInt(useShoe.Count / _avgShelf);

        List<int> shelfPerBox = this.DistributeEvelyn(_totalBox, totalShelf);
        List<int> shoePerBox = this.DistributeEvelyn(_totalBox, useShoe.Count);

        for (int i = 0; i < _listBox.Count; i++)
        {
            bool activeBox = i < _totalBox;
            _listBox[i].gameObject.SetActive(activeBox);

            if (activeBox)
            {
                List<Sprite> listShoe = Utils.TakeAndRemoveRandom<Sprite>(useShoe, shoePerBox[i]);
                _listBox[i].OnInitBox(shelfPerBox[i], listShoe);
            }
        }
    }

    private void OnSpawnBox(int totalBox, float space, int maxRow = 3, int maxCol = 3)
    {
        int boxCount = 0;
        float stepX = _prefabBox.GetComponent<ShoeBox>().BoxCollider.size.x + space;
        float stepY = _prefabBox.GetComponent<ShoeBox>().BoxCollider.size.y + space;
        int midCol = Mathf.CeilToInt((float)maxCol / 2) - 1;

        for (int i = 0; i < maxRow; i++)
        {
            if (boxCount == totalBox)
                break;
            Vector3 spawnPos = new Vector3(-stepX * midCol, -stepY * i, 0);
            for (int j = 0; j < maxCol; j++)
            {
                if (boxCount == totalBox)
                    break;
                Instantiate(_prefabBox, spawnPos, Quaternion.identity, _gridBox);
                spawnPos += new Vector3(stepX, 0, 0);
                boxCount++;
            }
        }

    }

    private void FillUseShoe(List<Sprite> takeShoe, List<Sprite> useShoe, int target, int indexShoe = 0)
    {
        if (useShoe.Count >= target)
            return;

        if (indexShoe >= takeShoe.Count)
            indexShoe = 0;

        for (int i = 0; i < 3; i++)
        {
            useShoe.Add(takeShoe[indexShoe]);
        }

        FillUseShoe(takeShoe, useShoe, target, indexShoe + 1);
    }

    private List<int> DistributeEvelyn(int boxCount, int totalShelf)
    {
        List<int> result = new List<int>();

        float agv = (float)totalShelf / boxCount;
        int low = Mathf.FloorToInt(agv);
        int high = Mathf.CeilToInt(agv);

        int highCount = totalShelf - low * boxCount;
        int lowCount = boxCount - highCount;

        for (int i = 0; i < lowCount; i++)
            result.Add(low);
        for (int i = 0; i < highCount; i++)
            result.Add(high);

        for (int i = 0; i < result.Count; i++)
        {
            int rand = Random.Range(i, result.Count);
            (result[i], result[rand]) = (result[rand], result[i]);
        }

        return result;
    }

    public void OnMinusShoe()
    {
        _totalShoe -= 3;
        if (_totalShoe <= 0)
        {
            Debug.Log("Win");
        }
    }

    public void OnCheckAndShake()
    {
        Dictionary<string, List<ShoeSlot>> groups = new Dictionary<string, List<ShoeSlot>>();

        foreach (var box in _listBox)
        {
            if (box.gameObject.activeInHierarchy)
            {
                foreach (var slot in box.TotalSlots)
                {
                    if (slot.HasShoe)
                    {
                        string name = slot.ShoeSprite.name;
                        if (!groups.ContainsKey(name))
                            groups.Add(name, new List<ShoeSlot>());
                        groups[name].Add(slot);
                    }
                }
            }
        }

        foreach (var group in groups)
        {
            if (group.Value.Count >= 3)
            {
                foreach (var slot in group.Value)
                {
                    slot.OnShake();
                }
                return;
            }
        }
    }

    public void OnMagnet()
    {
        Dictionary<string, List<SpriteRenderer>> groups = new Dictionary<string, List<SpriteRenderer>>();

        foreach (var box in _listBox)
        {
            if (box.gameObject.activeInHierarchy)
            {
                foreach (var slot in box.TotalSlots)
                {
                    if (slot.HasShoe)
                    {
                        string name = slot.ShoeSprite.name;
                        if (!groups.ContainsKey(name))
                            groups.Add(name, new List<SpriteRenderer>());
                        groups[name].Add(slot.ImageShoe);
                    }
                }

                ShoeShelf shelf = box.GetFirstShelf();

                if (shelf != null)
                {
                    foreach (var img in shelf.ShoeList)
                    {
                        if (img.gameObject.activeInHierarchy)
                        {
                            string name = img.sprite.name;
                            if (!groups.ContainsKey(name))
                                groups.Add(name, new List<SpriteRenderer>());
                            groups[name].Add(img);
                        }
                    }
                }
            }
        }

        Vector3 posMagnet = Camera.main.ScreenToWorldPoint(_magnetTarget.position);
        posMagnet.z = 0;

        foreach (var group in groups)
        {
            if (group.Value.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {

                    SpriteRenderer imageMagnet = _magnetList[i];
                    SpriteRenderer imageShoe = group.Value[i];
                    imageShoe.gameObject.SetActive(false);
                    imageMagnet.gameObject.SetActive(true);
                    imageMagnet.sprite = imageShoe.sprite;
                    imageMagnet.transform.position = imageShoe.transform.position;
                    // imageMagnet.rectTransform.sizeDelta = imageShoe.rectTransform.sizeDelta;
                    imageMagnet.transform.localEulerAngles = imageShoe.transform.localEulerAngles;

                    imageMagnet.transform.DOMove(posMagnet, 0.7f).OnComplete(() =>
                    {
                        imageMagnet.gameObject.SetActive(false);
                    });
                }
                foreach (var box in _listBox)
                {
                    if (box.gameObject.activeInHierarchy)
                    {
                        if (box.HasBoxEmpty())
                        {
                            box.OnCheckPrepareShelf();
                        }
                        box.OnCheckShelfEmpty();
                    }
                }
                this.OnMinusShoe();
                break;
            }
        }
    }

    public void OnShuffle()
    {
        List<SpriteRenderer> imageShoe = new List<SpriteRenderer>();
        foreach (var box in _listBox)
        {
            if (box.gameObject.activeInHierarchy)
            {
                foreach (var slot in box.TotalSlots)
                {
                    if (slot.HasShoe)
                    {
                        imageShoe.Add(slot.ImageShoe);
                    }
                }

                ShoeShelf shelf = box.GetFirstShelf();

                if (shelf != null)
                {
                    foreach (var img in shelf.ShoeList)
                    {
                        if (img.gameObject.activeInHierarchy)
                        {
                            string name = img.sprite.name;
                            imageShoe.Add(img);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < imageShoe.Count; i++)
        {
            int rand = Random.Range(i, imageShoe.Count);
            (imageShoe[i].sprite, imageShoe[rand].sprite) = (imageShoe[rand].sprite, imageShoe[i].sprite);
        }
    }

    public void OnMoreBox()
    {
        int currentBoxCount = _listBox.Count;
        int midCol = Mathf.CeilToInt((float)_maxColBox / 2) - 1;
        float stepX = _prefabBox.GetComponent<ShoeBox>().BoxCollider.size.x + _spaceBox;
        float stepY = _prefabBox.GetComponent<ShoeBox>().BoxCollider.size.y + _spaceBox;

        int row = currentBoxCount / _maxColBox;
        int col = currentBoxCount % _maxColBox;

        GameObject box = Instantiate(_prefabBox, new Vector3(-stepX * (midCol - col), -stepY * row, 0), Quaternion.identity, _gridBox);

        _listBox.Add(box.GetComponent<ShoeBox>());
    }
}
