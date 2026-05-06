using System.Collections.Generic;
using UnityEngine;

public class GridArranger : MonoBehaviour
{
    public int maxColumn = 3;
    public float spacingX = 2f;
    public float spacingY = 2f;

    private List<GameObject> items = new List<GameObject>();

    private void Awake()
    {
        GetAllItems();
        Arrange();
    }

    void GetAllItems()
    {
        items = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            items.Add(child.gameObject);
        }
    }

    public void Arrange()
    {
        List<GameObject> activeItems = new List<GameObject>();

        foreach (var item in items)
        {
            if (item != null && item.activeSelf)
                activeItems.Add(item);
        }

        int total = activeItems.Count;
        if (total == 0) return;

        int rows = Mathf.CeilToInt((float)total / maxColumn);

        float totalHeight = (rows - 1) * spacingY;

        for (int row = 0; row < rows; row++)
        {
            int itemInRow = Mathf.Min(maxColumn, total - row * maxColumn);

            float rowWidth = (itemInRow - 1) * spacingX;

            for (int col = 0; col < itemInRow; col++)
            {
                int index = row * maxColumn + col;

                float posX = col * spacingX - rowWidth / 2f;
                float posY = -row * spacingY + totalHeight / 2f;

                activeItems[index].transform.localPosition =
                    new Vector3(posX, posY, 0);
            }
        }
    }

    public void OnTransformChildrenChanged()
    {
        GetAllItems();
        Arrange();
    }


#if UNITY_EDITOR
    void Update()
    {
        GetAllItems();
        Arrange();
    }
#endif
}