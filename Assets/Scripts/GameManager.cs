using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int _totalShoe;
    [SerializeField] private int _totalBox;
    [SerializeField] private Transform _gridBox;

    private List<ShoeBox> _listBox;
    private float _avgShelf;
    private List<Sprite> _totalSpriteShoe;
    void Awake()
    {
        _listBox = _gridBox.GetComponentsInChildren<ShoeBox>().ToList();
        _totalSpriteShoe = Resources.LoadAll<Sprite>("Items").ToList();
    }
    void Start()
    {
        OnInitLevel();
    }

    private void OnInitLevel()
    {
        List<Sprite> takeShoe = _totalSpriteShoe.OrderBy(x => Random.value).Take(_totalShoe).ToList();
        List<Sprite> useShoe = new List<Sprite>();

        for (int i = 0; i < takeShoe.Count; i++)
        {
            for (int j = 0; j < 6; j++)
                useShoe.Add(takeShoe[i]);
        }
        Debug.Log("Use shoe: "+useShoe.Count);

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
                Debug.Log("Use Shoe: "+useShoe.Count);
                _listBox[i].OnInitBox(shelfPerBox[i], listShoe);
            }
        }
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
}
