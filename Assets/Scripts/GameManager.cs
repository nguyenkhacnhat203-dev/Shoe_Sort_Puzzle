using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

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
    private float _avgShelf, _cellWidth, _cellHeight, _scale, _startY;
    private List<Sprite> _totalSpriteShoe;
    void Awake()
    {
        OnSpawnBox(_spaceBox, _maxRowBox, _maxColBox);
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
                bool forceNotFull = (i == _totalBox - 1);
                _listBox[i].OnInitBox(shelfPerBox[i], listShoe, forceNotFull);
            }
        }
    }

    private void OnSpawnBox(float spacing, int maxRow = 3, int maxCol = 3)
    {
        Vector2 size = _prefabBox.GetComponent<ShoeBox>().BoxCollider.size;
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        float availableWidth = cameraWidth - spacing * (maxCol - 1);
        float availableHeight = cameraHeight - spacing * (maxRow - 1);
        float cellWidth = availableWidth / maxCol;
        float cellHeight = availableHeight / maxRow;
        float scaleX = cellWidth / size.x;
        float scaleY = cellHeight / size.y;
        float scale = Mathf.Min(scaleX, scaleY);

        cellWidth = size.x * scale;
        cellHeight = size.y * scale;
        float gridWidth = maxCol * cellWidth + (maxCol - 1) * spacing;
        float gridHeight = maxRow * cellHeight + (maxRow - 1) * spacing;

        int total = _totalBox;
        int fullRows = total / maxCol;
        int lastRowCount = total % maxCol;
        for (int i = 0; i < total; i++)
        {
            int row = i / maxCol;
            int col = i % maxCol;

            int currentCols = maxCol;
            if (row == fullRows && lastRowCount != 0)
            {
                currentCols = lastRowCount;
            }
            float currentRowWidth = currentCols * cellWidth + (currentCols - 1) * spacing;
            float startX = -currentRowWidth / 2 + cellWidth / 2;
            float startY = gridHeight / 2 - cellHeight / 2;
            Vector3 pos = new Vector3(startX + col * (cellWidth + spacing), startY - row * (cellHeight + spacing), 0);
            GameObject obj = Instantiate(_prefabBox, pos, Quaternion.identity, _gridBox);
            obj.transform.localScale = Vector3.one * scale;
        }

        _cellWidth = cellWidth;
        _cellHeight = cellHeight;
        _scale = scale;
        _startY = gridHeight / 2 - cellHeight / 2;
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
                    imageMagnet.transform.localEulerAngles = imageShoe.transform.localEulerAngles;

                    imageMagnet.transform.DOKill();

                    imageMagnet.transform.DOMove(posMagnet, 0.7f)
                        .SetLink(imageMagnet.gameObject)
                        .OnComplete(() =>
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
        _totalBox++;
        if (_totalBox > 9)
            return;

        // spawn object mới
        GameObject obj = Instantiate(_prefabBox, _gridBox);
        obj.transform.localScale = Vector3.one * _scale;

        ShoeBox box = obj.GetComponent<ShoeBox>();
        _listBox.Add(box);

        // layout lại toàn bộ
        Relayout();
    }
    private void Relayout()
    {
        int total = _listBox.Count;
        int fullRows = total / _maxColBox;
        int lastRowCount = total % _maxColBox;

        for (int i = 0; i < total; i++)
        {
            int row = i / _maxColBox;
            int col = i % _maxColBox;

            int currentCols = _maxColBox;

            if (row == fullRows && lastRowCount != 0)
            {
                currentCols = lastRowCount;
            }

            float currentRowWidth = currentCols * _cellWidth + (currentCols - 1) * _spaceBox;
            float startX = -currentRowWidth / 2 + _cellWidth / 2;

            _listBox[i].transform.localPosition = new Vector3(
                startX + col * (_cellWidth + _spaceBox),
                _startY - row * (_cellHeight + _spaceBox),
                0
            );
        }
    }
}
