using System;
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

    private GameState _currentState;
    private int _totalShoe, _totalShoeModel, _totalBox, _timeCountdown;
    private List<ShoeBox> _listBox;
    private float _avgShelf;
    private List<Sprite> _totalSpriteShoe;
    private bool _isTimerStarted = false;
    private Coroutine _countdownCoroutine;

    #region Properties & Events
    public GameState CurrentState => _currentState;
    public static event Action<GameState> OnGameStateChanged;
    #endregion

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();
        ResourceManager.Instance.InitResource();
    }

    void Start()
    {
        // PlayerPrefs.SetInt(GameKeys.LEVEL_KEY, 1);
        //UiManager.Instance.ShowMenu();
        ChangeState(GameState.OnMenu);
        this.SetLevelTextHome();
    }

    private void OnApplicationQuit()
    {
        if (_currentState == GameState.OnGame || _currentState == GameState.Pause)
        {
            ResourceManager.Instance.SetHeart(-1);
            PlayerPrefs.Save();
        }
    }
    #endregion

    #region Game State Management

    public void ChangeState(GameState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        switch (newState)
        {
            case GameState.OnMenu:
            case GameState.OnGame:
                Time.timeScale = 1f;
                break;
            case GameState.Pause:
            case GameState.Lose:
            case GameState.Win:
                Time.timeScale = 0f;
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    public void OnPlay()
    {
        if (ResourceManager.Instance.GetHeart() == 0)
        {
            UiManager.Instance.Show_Popup_Heart(() =>
            {
                this.OnPlay();
            });
            return;
        }
        ChangeState(GameState.OnGame);
        UiManager.Instance.Show_Menu_Game();

        this.ClearChildren(_gridBox);
        this.LoadLevel();
        this.SetLevelTextGame();
        this.OnInitLevel();
        _gridBox.GetComponent<GridArranger>().OnTransformChildrenChanged();
        _dragAndDrop.Reset();

        _isTimerStarted = false;
        int minutes = _timeCountdown / 60;
        int second = _timeCountdown % 60;
        _textTime.text = string.Format("{0:00}:{1:00}", minutes, second);

        AudioManager.Instance.PlayBackgroundMusic();
    }

    public void ResetGame()
    {
        _isTimerStarted = false;
        this.ClearChildren(_gridBox);
        if (_countdownCoroutine != null)
        {
            StopCoroutine(_countdownCoroutine);
            _countdownCoroutine = null;
        }
    }
    #endregion

    #region Level Initialization
    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }

    private void LoadLevel()
    {
        int level = ResourceManager.Instance.GetLevel();
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

    private void ReadJsonLv(string lvText)
    {
        LevelData levelData = JsonUtility.FromJson<LevelData>(lvText);

        _totalBox = levelData.totalBox;
        _totalShoe = levelData.totalShoe;
        _totalShoeModel = levelData.totalShoeModel;
        _timeCountdown = levelData.timeCountdown;
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
            int rand = UnityEngine.Random.Range(i, useShoe.Count);
            (useShoe[i], useShoe[rand]) = (useShoe[rand], useShoe[i]);
        }

        _avgShelf = UnityEngine.Random.Range(1.5f, 2f);
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
            int rand = UnityEngine.Random.Range(i, result.Count);
            (result[i], result[rand]) = (result[rand], result[i]);
        }

        return result;
    }
    #endregion

    #region Timer
    public void StartTimer()
    {
        if (!_isTimerStarted)
        {
            _isTimerStarted = true;
            _countdownCoroutine = StartCoroutine(StartCountdown(_timeCountdown));
        }
    }

    IEnumerator StartCountdown(int seconds)
    {
        int remaining = seconds;
        Debug.Log("time");
        while (remaining > 0)
        {
            if (_currentState == GameState.Win)
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
    #endregion

    #region UI Setup
    void SetLevelTextHome()
    {
        int level = ResourceManager.Instance.GetLevel();
        _textLevelHome.SetText("LEVEL " + level);
    }

    void SetLevelTextGame()
    {
        int level = ResourceManager.Instance.GetLevel();
        _textLevelGame.SetText("Level " + level);
    }
    #endregion

    #region Game Logic
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
        yield return new WaitForSeconds(1f);
        // this.ClearChildren(_gridBox);
        AudioManager.Instance.GameWin();
        int nextLevel = ResourceManager.Instance.GetLevel() + 1;
        ResourceManager.Instance.SetLevel(nextLevel);
        this.SetLevelTextHome();
        ChangeState(GameState.Win);
        UiManager.Instance.Show_Win_Lose();
        //UiManager.Instance.ShowMenu();
    }

    public void OnLose()
    {
        Debug.Log("Lose");
        ResourceManager.Instance.SetHeart(-1);
        AudioManager.Instance.backgroundMusicSource.Pause();
        AudioManager.Instance.GameOver();
        ChangeState(GameState.Lose);
        UiManager.Instance.Show_Win_Lose();
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
    #endregion

    #region Buy Pack
    public void OnBuyPack(GameObject clickedButton)
    {
        string txtPrice = clickedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        int price = int.Parse(txtPrice);
        if (ResourceManager.Instance.CanBuy(price))
        {
            Transform packTransform = clickedButton.transform.parent;
            for (int i = 0; i < packTransform.childCount; i++)
            {
                Transform child = packTransform.GetChild(i);

                if (child.name.Contains("_Booster"))
                {
                    TextMeshProUGUI txtAmount = child.GetComponentInChildren<TextMeshProUGUI>();
                    int amount = ExtractNumber(txtAmount.text);

                    ResourceManager.Instance.ChangeCountBooster(child.name, amount);
                    ResourceManager.Instance.ChangeCoin(-price);
                }
            }
        }
    }

    private int ExtractNumber(string input)
    {
        // Loại bỏ chữ 'X' hoặc 'x', sau đó ép kiểu sang int
        string cleanString = input.Replace("X", "").Replace("x", "").Trim();

        if (int.TryParse(cleanString, out int result))
        {
            return result;
        }
        return 0;
    }
    #endregion

    #region Booster
    public void OnMagnet()
    {
        if (!ResourceManager.Instance.CanUseBooster("Magnet_Booster"))
            return;
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
                ResourceManager.Instance.ChangeCountBooster("Magnet_Booster", -1);
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
        if (!ResourceManager.Instance.CanUseBooster("Shuffle_Booster"))
            return;
        ResourceManager.Instance.ChangeCountBooster("Shuffle_Booster", -1);
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
            int rand = UnityEngine.Random.Range(i, imageShoe.Count);
            (imageShoe[i].sprite, imageShoe[rand].sprite) = (imageShoe[rand].sprite, imageShoe[i].sprite);
        }
    }

    public void OnMoreBox()
    {
        if (!ResourceManager.Instance.CanUseBooster("More_Box_Booster"))
            return;
        _totalBox++;
        if (_totalBox > 9)
            return;
        ResourceManager.Instance.ChangeCountBooster("More_Box_Booster", -1);
        GameObject obj = Instantiate(_prefabBox, _gridBox);
        ShoeBox box = obj.GetComponent<ShoeBox>();
        _listBox.Add(box);
        _gridBox.GetComponent<GridArranger>().OnTransformChildrenChanged();
    }
    #endregion
}
public enum GameState
{
    OnMenu,
    OnGame,
    Win,
    Lose,
    Pause
}