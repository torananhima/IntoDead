using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject loadingScene;
    [SerializeField] Image loading_fill;
    [SerializeField] TMP_Text loading_text;
    public static PauseManager Instance;
    private GameManager gameManager;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        gameManager = GameManager.GetInstance();
        pauseMenu.SetActive(false);
        loadingScene.SetActive(false);

    }
    private void OnEnable()
    {
        GameManager.OnPaused += OnPause;
        GameManager.OnUnPaused += OnUnPause;
    }
    private void OnDisable()
    {
        GameManager.OnPaused -= OnPause;
        GameManager.OnUnPaused -= OnUnPause;
    }

    public void OnPause()
    {
        pauseMenu.SetActive(true);
    }
    public void OnUnPause()
    {
        pauseMenu.SetActive(false);
    }

    public void OpenSetting()
    {
        Debug.Log("No setting");
    }
    public void Resume()
    {
        gameManager.UnPause();
    }
    public void Restart()
    {
        StartCoroutine(LoadSceneSequence(SceneManager.GetActiveScene().buildIndex));
    }
    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadSceneSequence(0));
    }
    private IEnumerator LoadSceneSequence(int buildIndex)
    {
        loadingScene.SetActive(true);
        float progress = 0;
        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);

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

}