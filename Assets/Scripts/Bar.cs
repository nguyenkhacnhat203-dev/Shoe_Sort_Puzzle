using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [Header("Shadow")]
    public RectTransform Shadow;

  
    private float[] shadowTargetAnchorX = new float[] { 0.1f, 0.3f, 0.5f, 0.7f, 0.9f };
    private float currentShadowAnchorX;

    [Header("Texts")]
    public TextMeshProUGUI Shop_Text;
    public TextMeshProUGUI Ranking_Text;
    public TextMeshProUGUI Home_Text;
    public TextMeshProUGUI Teams_Text;
    public TextMeshProUGUI Outfit_Text;

    [Header("Jambs")]
    public RectTransform[] jambs;

    [Header("Buttons")]
    public Button btn_Shop;
    public Button btn_Ranking;
    public Button btn_Home;
    public Button btn_Teams;
    public Button btn_Outfit;

    [Header("Panels")]
    public RectTransform panel_Shop;
    public RectTransform panel_Ranking;
    public RectTransform panel_Home;
    public RectTransform panel_Teams;
    public RectTransform panel_Outfit;

    [Header("Settings")]
    public float shadowSpeed = 10f;
    public float transitionSpeed = 10f;
    public float panelTransitionSpeed = 8f;
    public float elevatedY = 100f;
    public Vector3 selectedScale = new Vector3(2f, 2f, 2f);
    public Vector2 panelOffscreenOffset = new Vector2(0, -1000f);

    private float[] weights = new float[] { 1f, 1f, 2f, 1f, 1f };
    private float[] targetXPositions = new float[4];
    private float[] targetBtnXPositions = new float[5];
    private float[] targetBtnYPositions = new float[5];
    private Vector3[] targetScales = new Vector3[5];

    private RectTransform[] btnRects;
    private RectTransform[] allPanels;
    private RectTransform parentRect;


    //private void OnEnable()
    //{
    //    parentRect = (RectTransform)transform;

    //    btnRects = new RectTransform[] {
    //        btn_Shop?.GetComponent<RectTransform>(),
    //        btn_Ranking?.GetComponent<RectTransform>(),
    //        btn_Home?.GetComponent<RectTransform>(),
    //        btn_Teams?.GetComponent<RectTransform>(),
    //        btn_Outfit?.GetComponent<RectTransform>()
    //    };

    //    allPanels = new RectTransform[] {
    //        panel_Shop,
    //        panel_Ranking,
    //        panel_Home,
    //        panel_Teams,
    //        panel_Outfit
    //    };

    //    if (jambs != null) foreach (var jamb in jambs) if (jamb != null) jamb.gameObject.SetActive(true);
    //    foreach (var btn in btnRects) if (btn != null) btn.gameObject.SetActive(true);

    //    btn_Shop?.onClick.AddListener(() => ChangeFocus(0));
    //    btn_Ranking?.onClick.AddListener(() => ChangeFocus(1));
    //    btn_Home?.onClick.AddListener(() => ChangeFocus(2));
    //    btn_Teams?.onClick.AddListener(() => ChangeFocus(3));
    //    btn_Outfit?.onClick.AddListener(() => ChangeFocus(4));

    //    if (Shadow != null) Shadow.gameObject.SetActive(true);

    //    ChangeFocus(2);
    //    SnapToTargets();
    //}

  


    private void Start()
    {
        parentRect = (RectTransform)transform;

        btnRects = new RectTransform[] {
            btn_Shop?.GetComponent<RectTransform>(),
            btn_Ranking?.GetComponent<RectTransform>(),
            btn_Home?.GetComponent<RectTransform>(),
            btn_Teams?.GetComponent<RectTransform>(),
            btn_Outfit?.GetComponent<RectTransform>()
        };

        allPanels = new RectTransform[] {
            panel_Shop,
            panel_Ranking,
            panel_Home,
            panel_Teams,
            panel_Outfit
        };

        if (jambs != null) foreach (var jamb in jambs) if (jamb != null) jamb.gameObject.SetActive(true);
        foreach (var btn in btnRects) if (btn != null) btn.gameObject.SetActive(true);

        btn_Shop?.onClick.AddListener(() => ChangeFocus(0));
        btn_Ranking?.onClick.AddListener(() => ChangeFocus(1));
        btn_Home?.onClick.AddListener(() => ChangeFocus(2));
        btn_Teams?.onClick.AddListener(() => ChangeFocus(3));
        btn_Outfit?.onClick.AddListener(() => ChangeFocus(4));

        if (Shadow != null) Shadow.gameObject.SetActive(true);

        ChangeFocus(2);
        SnapToTargets();
    }

    private void Update()
    {
        SetPo();
    }

    private void SetPo()
    {
        for (int i = 0; i < jambs.Length; i++)
        {
            if (jambs[i] == null) continue;
            float newX = Mathf.Lerp(jambs[i].anchoredPosition.x, targetXPositions[i], Time.deltaTime * transitionSpeed);
            jambs[i].anchoredPosition = new Vector2(newX, jambs[i].anchoredPosition.y);
        }

        for (int i = 0; i < btnRects.Length; i++)
        {
            if (btnRects[i] == null) continue;
            Vector2 currentPos = btnRects[i].anchoredPosition;
            float newX = Mathf.Lerp(currentPos.x, targetBtnXPositions[i], Time.deltaTime * transitionSpeed);
            float newY = Mathf.Lerp(currentPos.y, targetBtnYPositions[i], Time.deltaTime * transitionSpeed);

            btnRects[i].anchoredPosition = new Vector2(newX, newY);
            btnRects[i].localScale = Vector3.Lerp(btnRects[i].localScale, targetScales[i], Time.deltaTime * transitionSpeed);
        }

        foreach (var panel in allPanels)
        {
            if (panel != null && panel.gameObject.activeInHierarchy)
            {
                panel.anchoredPosition = Vector2.Lerp(panel.anchoredPosition, Vector2.zero, Time.deltaTime * panelTransitionSpeed);
            }
        }

        if (Shadow != null)
        {
            float shadowWidthHalf = (Shadow.anchorMax.x - Shadow.anchorMin.x) / 2f;

            float targetMinX = currentShadowAnchorX - shadowWidthHalf;
            float targetMaxX = currentShadowAnchorX + shadowWidthHalf;

            float newMinX = Mathf.Lerp(Shadow.anchorMin.x, targetMinX, Time.deltaTime * shadowSpeed);
            float newMaxX = Mathf.Lerp(Shadow.anchorMax.x, targetMaxX, Time.deltaTime * shadowSpeed);

            Shadow.anchorMin = new Vector2(newMinX, Shadow.anchorMin.y);
            Shadow.anchorMax = new Vector2(newMaxX, Shadow.anchorMax.y);

            Shadow.offsetMin = new Vector2(0, Shadow.offsetMin.y);
            Shadow.offsetMax = new Vector2(0, Shadow.offsetMax.y);
        }
    }

    public void ChangeFocus(int index)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            bool isSelected = (i == index);
            weights[i] = isSelected ? 2f : 1f;
            targetBtnYPositions[i] = isSelected ? elevatedY : 0f;
            targetScales[i] = isSelected ? selectedScale : Vector3.one;

            if (allPanels[i] != null)
            {
                if (isSelected)
                {
                    if (!allPanels[i].gameObject.activeSelf)
                    {
                        allPanels[i].anchoredPosition = panelOffscreenOffset;
                        allPanels[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    allPanels[i].gameObject.SetActive(false);
                }
            }
        }

        if (Shop_Text != null) Shop_Text.gameObject.SetActive(index == 0);
        if (Ranking_Text != null) Ranking_Text.gameObject.SetActive(index == 1);
        if (Home_Text != null) Home_Text.gameObject.SetActive(index == 2);
        if (Teams_Text != null) Teams_Text.gameObject.SetActive(index == 3);
        if (Outfit_Text != null) Outfit_Text.gameObject.SetActive(index == 4);

        CalculateTargets();


        float width = parentRect.rect.width;
        currentShadowAnchorX = (targetBtnXPositions[index] + (width / 2f)) / width;
    }

    private void CalculateTargets()
    {
        float width = parentRect.rect.width;
        float totalWeight = 0;
        foreach (float w in weights) totalWeight += w;

        float currentLeftEdge = -width / 2f;

        for (int i = 0; i < weights.Length; i++)
        {
            float cellWidth = (weights[i] / totalWeight) * width;
            targetBtnXPositions[i] = currentLeftEdge + (cellWidth / 2f);

            if (i < jambs.Length)
                targetXPositions[i] = currentLeftEdge + cellWidth;

            currentLeftEdge += cellWidth;
        }
    }

    private void SnapToTargets()
    {
        CalculateTargets();

        for (int i = 0; i < jambs.Length; i++)
        {
            if (jambs[i] != null)
                jambs[i].anchoredPosition = new Vector2(targetXPositions[i], jambs[i].anchoredPosition.y);
        }

        for (int i = 0; i < btnRects.Length; i++)
        {
            if (btnRects[i] == null) continue;
            btnRects[i].anchoredPosition = new Vector2(targetBtnXPositions[i], targetBtnYPositions[i]);
            btnRects[i].localScale = targetScales[i];
        }

        if (Shadow != null)
        {
            float width = parentRect.rect.width;
            int selectedIndex = 2;
            for (int i = 0; i < weights.Length; i++) if (weights[i] > 1.1f) selectedIndex = i;

            currentShadowAnchorX = (targetBtnXPositions[selectedIndex] + (width / 2f)) / width;

            float shadowWidthHalf = (Shadow.anchorMax.x - Shadow.anchorMin.x) / 2f;
            Shadow.anchorMin = new Vector2(currentShadowAnchorX - shadowWidthHalf, Shadow.anchorMin.y);
            Shadow.anchorMax = new Vector2(currentShadowAnchorX + shadowWidthHalf, Shadow.anchorMax.y);
            Shadow.offsetMin = new Vector2(0, Shadow.offsetMin.y);
            Shadow.offsetMax = new Vector2(0, Shadow.offsetMax.y);
        }
    }
}