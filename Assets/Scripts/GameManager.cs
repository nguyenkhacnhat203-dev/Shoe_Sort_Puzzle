using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] private int _lvId;
    [SerializeField] private GameObject _prefabBox;
    [SerializeField] private Transform _gridBox;
    [SerializeField] private List<SpriteRenderer> _magnetList;
    [SerializeField] private RectTransform _magnetTarget;
    [SerializeField] private TextMeshProUGUI _textTime, _textLevelHome, _textLevelGame;
    [SerializeField] private DragDropController _dragAndDrop;

    private int _totalShoe, _totalShoeModel, _totalBox, _timeCountdown;
    private const string LEVEL_KEY = "CURRENT_LEVEL";
    private List<ShoeBox> _listBox;
    private float _avgShelf;
    private List<Sprite> _totalSpriteShoe;
    private bool _isWin = false;
    private bool _isTimerStarted = false;

    void Start()
    {
        // PlayerPrefs.SetInt(LEVEL_KEY, 1);
        _dragAndDrop.enabled = false;
        //UiManager.Instance.ShowMenu();
        this.SetLevelTextHome();
    }

    public void OnPlay()
    {
        UiManager.Instance.ShowGame();
        UiManager.Instance.popup_Next.SetActive(false);

        this.ClearChildren(_gridBox);
        this.LoadLevel();
        this.SetLevelTextGame();
        this.OnInitLevel();
        _gridBox.GetComponent<GridArranger>().OnTransformChildrenChanged();
        _dragAndDrop.enabled = true;

        _isTimerStarted = false;
        _isWin = false;
        int minutes = _timeCountdown / 60;
        int second = _timeCountdown % 60;
        _textTime.text = string.Format("{0:00}:{1:00}", minutes, second);

        AudioManager.Instance.PlayBackgroundMusic();
    }

    public void StartTimer()
    {
        if (!_isTimerStarted)
        {
            _isTimerStarted = true;
            StartCoroutine(StartCountdown(_timeCountdown));
        }
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }

    void SetLevelTextHome()
    {
        int level = PlayerPrefs.GetInt(LEVEL_KEY, 1);
        _textLevelHome.SetText("LEVEL " + level);
    }
    void SetLevelTextGame()
    {
        int level = PlayerPrefs.GetInt(LEVEL_KEY, 1);
        _textLevelGame.SetText("Level " + level);
    }

    private void ReadJsonLv(string lvText)
    {
        LevelData levelData = JsonUtility.FromJson<LevelData>(lvText);

        _totalBox = levelData.totalBox;
        _totalShoe = levelData.totalShoe;
        _totalShoeModel = levelData.totalShoeModel;
        _timeCountdown = levelData.timeCountdown;
    }

    private void LoadLevel()
    {
        int level = PlayerPrefs.GetInt(LEVEL_KEY, 1);
        TextAsset jsonFile = Resources.Load<TextAsset>($"Levels/level{level}");
        if (jsonFile != null)
        {
            ReadJsonLv(jsonFile.text);
        }
        else
        {
            Debug.LogError($"Level {level} not found!");
        }
    }

    private void OnInitLevel()
    {
        for (int i = 0; i < _totalBox; i++)
            Instantiate(_prefabBox, _gridBox);
        _listBox = _gridBox.GetComponentsInChildren<ShoeBox>().ToList();
        _totalSpriteShoe = Resources.LoadAll<Sprite>("Items").ToList();

        if (_totalShoe < _totalShoeModel || _totalShoe % 3 != 0)
        {
            Debug.LogError("Total shoe must be greater than total shoe model and divisible by 3");
            return;
        }

        List<Sprite> takeShoe = _totalSpriteShoe.Take(_totalShoeModel).ToList();
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

    IEnumerator StartCountdown(int seconds)
    {
        int remaining = seconds;
        while (remaining > 0)
        {
            if (_isWin)
                yield break;
            int minutes = remaining / 60;
            int second = remaining % 60;
            _textTime.text = string.Format("{0:00}:{1:00}", minutes, second);
            yield return new WaitForSeconds(1);
            remaining--;
        }
        _textTime.text = string.Format("{0:00}:{1:00}", 0, 0);
        this.OnTimeFinish();
    }

    private void OnTimeFinish()
    {
        if (_totalShoe > 0)
        {
            OnLose();
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
            Debug.Log("Win"); //Show popup win
            StartCoroutine(OnWin());
        }
    }

    public IEnumerator OnWin()
    {
        _isWin = true;
        yield return new WaitForSeconds(1f);
        // this.ClearChildren(_gridBox);
        AudioManager.Instance.GameWin();
        int nextLevel = PlayerPrefs.GetInt(LEVEL_KEY, 1) + 1;
        PlayerPrefs.SetInt(LEVEL_KEY, nextLevel);
        this.SetLevelTextHome();
        _dragAndDrop.enabled = false;
        UiManager.Instance.popup_Next.SetActive(true);
        //UiManager.Instance.ShowMenu();
    }

    public void OnLose()
    {
        Debug.Log("Lose");
        _dragAndDrop.enabled = false;
        AudioManager.Instance.backgroundMusicSource.Pause();
        AudioManager.Instance.GameOver();
        //UiManager.Instance.ShowMenu();
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

    #region Booster
    public void OnMagnet()
    {
        this.StartTimer();
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

                ShoeShelf shelf = box.Shelf;

                if (shelf != null)
                {
                    foreach (var img in shelf.ShoeSlot)
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

        // Vector3 posMagnet = Camera.main.ScreenToWorldPoint(_magnetTarget.position);
        Vector3 posMagnet = _magnetTarget.position;
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
                    imageMagnet.transform.DORotate(Vector3.zero, 0.7f);
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
        this.StartTimer();
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

                ShoeShelf shelf = box.Shelf;

                if (shelf != null)
                {
                    foreach (var img in shelf.ShoeSlot)
                    {
                        if (img.gameObject.activeInHierarchy)
                        {
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
        this.StartTimer();
        _totalBox++;
        if (_totalBox > 9)
            return;

        GameObject obj = Instantiate(_prefabBox, _gridBox);
        ShoeBox box = obj.GetComponent<ShoeBox>();
        _listBox.Add(box);
        _gridBox.GetComponent<GridArranger>().OnTransformChildrenChanged();
    }
    #endregion
}
