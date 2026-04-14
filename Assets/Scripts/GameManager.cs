using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    [SerializeField] private int _totalShoe;
    [SerializeField] private int _totalShoeModel;
    [SerializeField] private int _totalBox;
    [SerializeField] private Transform _gridBox;

    private List<ShoeBox> _listBox;
    private float _avgShelf;
    private List<Sprite> _totalSpriteShoe;
    void Awake()
    {
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
        _totalShoe-=3;
        if (_totalShoe <= 0)
        {
            Debug.Log("Win");
        }
    }
}
