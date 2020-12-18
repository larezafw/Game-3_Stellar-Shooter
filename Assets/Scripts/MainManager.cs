using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public AudioSource ButtonSound;
    public AudioSource PointUpSound;
    public AudioSource GameoverSound;
    public AudioSource MenuBacksound;
    public AudioSource GameBacksound;
    public AudioSource SummonSSound;
    public AudioSource StartGameSound;

    public GameObject LoadingPage;
    float loadingTimer;
    bool loading;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != null && Instance != this) Destroy(gameObject);

        LoadingPage.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (loadingTimer > 0 && loading) loadingTimer -= Time.deltaTime;
        else if (loading)
        {
            LoadingPage.SetActive(false);
            loading = false;

            if (BaseManager() != null) BaseManager().GetComponent<BaseManager>().StartGame();
        }
    }

    public void StartGame()
    {
        LoadingPage.SetActive(true);
        loadingTimer = 2f;
        loading = true;
        SceneManager.LoadScene(Keyword.iNGAME_SCENE);
    }

    public void GoToMenu()
    {
        LoadingPage.SetActive(true);
        loadingTimer = 2f;
        loading = true;
        SceneManager.LoadScene(Keyword.MAINMENU_SCENE);
    }

    GameObject BaseManager()
    {
        return GameObject.FindGameObjectWithTag(Keyword.BASE_MANAGER);
    }
}
