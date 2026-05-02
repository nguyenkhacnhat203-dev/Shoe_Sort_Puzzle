using UnityEngine;

public class Camerafix : MonoBehaviour
{
    private float defaultAspect = 9f / 16f;
    private float defaultSize;
    private bool isTablet;

    void Start()
    {
        defaultSize = Camera.main.orthographicSize;

        isTablet = IsTabletDevice();

        if (!isTablet)
        {
            AdjustCamera();
        }
    }




    void AdjustCamera()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        Camera.main.orthographicSize = defaultSize * (defaultAspect / currentAspect);
    }

    bool IsTabletDevice()
    {
        float aspect = (float)Screen.height / Screen.width;

        return Mathf.Abs(aspect - 4f / 3f) < 0.1f;
    }

    //#if UNITY_EDITOR
    //    void Update()
    //    {
    //        AdjustCamera();
    //    }
    //#endif
}
