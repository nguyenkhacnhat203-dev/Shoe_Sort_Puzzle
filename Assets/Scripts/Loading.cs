using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [Header("UI")]
    public Image loadingImage;

    [Header("Config")]
    public float loadTime = 2f;
    public string sceneName = "MainGame";

    private void Start()
    {
        loadingImage.fillAmount = 0f;
        StartCoroutine(LoadProgress());
    }

    IEnumerator LoadProgress()
    {
        float timer = 0f;

        while (timer < loadTime)
        {
            timer += Time.deltaTime;
            loadingImage.fillAmount = timer / loadTime;
            yield return null;
        }

        loadingImage.fillAmount = 1f;

        SceneManager.LoadScene(sceneName);
    }
}
