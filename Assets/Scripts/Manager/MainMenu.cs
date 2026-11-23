using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] AudioData bgm;
    [SerializeField] GameObject loadingScene;
    [SerializeField] Image loading_fill;
    [SerializeField] TMP_Text loading_text;

    private void Start()
    {
        SoundManager.Instance.PlayBGM(bgm);
        loadingScene.SetActive(false);
    }
    public void StartGame()
    {
        StartCoroutine(LoadSceneSequence());
    }

    private IEnumerator LoadSceneSequence()
    {
        loadingScene.SetActive(true);
        float progress = 0;
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);

        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            progress = Mathf.Clamp01(operation.progress / 0.9f);
            loading_text.text = Mathf.Round(progress * 100f) + "%";
            loading_fill.fillAmount = progress / 1;
            if (progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit");
        return;
#endif
        Application.Quit();
    }
}
