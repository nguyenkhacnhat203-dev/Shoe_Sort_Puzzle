using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResponsiveTextAuto : MonoBehaviour
{
    private const float REF_WIDTH = 1080f;

    public float smallScale = 0.8f;
    public float bigScale = 0.85f;

    void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;

        float scale = 1f;

        if (canvasWidth < REF_WIDTH)
            scale = smallScale;
        else if (canvasWidth > REF_WIDTH)
            scale = bigScale;

        ApplyScale(scale);
    }

    void ApplyScale(float scale)
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (var txt in texts)
        {
            txt.fontSize *= scale;
        }
    }
}