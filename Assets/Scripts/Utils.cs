using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utils
{
    public static List<T> GetComponentChildren<T>(Transform parent)
    {
        List<T> result = new List<T>();
        for (int i = 0; i < parent.childCount; i++)
        {
            var component = parent.GetChild(i).GetComponent<T>();
            if (component != null)
                result.Add(component);
        }
        return result;
    }

    public static T GetRayCastUI<T>(Vector2 position) where T : Component
    {
        var pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = position
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (var raycastResult in raycastResults)
            {
                T component = raycastResult.gameObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
        }

        return null;
    }

    public static T GetRayCastWorld2D<T>(Vector2 screenPosition) where T : Component
    {
        var cam = Camera.main;
        if (cam == null)
            return null;

        Vector2 worldPoint = cam.ScreenToWorldPoint(screenPosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                T component = hit.collider.gameObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
        }

        return null;
    }

    public static List<T> TakeAndRemoveRandom<T>(List<T> source, int n)
    {
        List<T> result = new List<T>();
        n = Mathf.Min(n, source.Count);
        for (int i = 0; i < n; i++)
        {
            int randIndex = Random.Range(0, source.Count);
            
            while (i != 0 && result[i - 1].Equals(source[randIndex]) && source.Count > 1)
            {
                randIndex = Random.Range(0, source.Count);
            }

            result.Add(source[randIndex]);
            source.RemoveAt(randIndex);
        }
        return result;
    }
}
