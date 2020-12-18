using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BaseManager : MonoBehaviour
{
    LineRenderer line;
    float[] lineBoundry;

    public GameObject ActorObject;
    float summonTImer;

    public TextMeshProUGUI startGameText;

    public Animator tutorAnim;
    public GameObject TutorialPanel;
    public GameObject skipIcon;
    public TextMeshProUGUI tutorTitleText;
    public TextMeshProUGUI tutorBodyText;
    string[] tutorBody = new string[] { Keyword.TUTORIAL_1_BODY, Keyword.TUTORIAL_2_BODY };
    int tutorialClipCode;
    int skipTutorial;

    public bool gameOver { get; private set; }
    public GameObject GameOverPanel;
    public TextMeshProUGUI GameoverInfoText;

    public TextMeshProUGUI comboText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    bool isPlayed;
    float gameTimeLeft;
    float comboTimer;
    float maxComboTimer = 2f;
    int combo;
    int score;
    int maxCombo;

    private void Awake() => FirstSetup();
    
    private void Update()
    {

        // SUMMONING
        if (summonTImer > 0) summonTImer -= Time.deltaTime;
        if (Input.GetMouseButton(0) && summonTImer <= 0 && !gameOver && isPlayed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            if (InsideLineBoundry(mousePos)) 
            {
                MainManager.Instance.SummonSSound.Play();
                Instantiate(ActorObject).transform.position = mousePos;
                summonTImer = Mathf.Lerp(0.4f, 0.1f, combo / 8f);
            }
        }

        // CHANGE TIMER COLOR
        if (comboTimer > 0f) comboTimer -= Time.deltaTime;
        else timerText.color = Keyword.DARK_YELLOW;

        // SET COMBO
        comboText.alpha = Mathf.Lerp(0f, 1f, comboTimer / maxComboTimer);
        comboText.fontSize = Mathf.Lerp(25, 33, comboTimer / maxComboTimer);
        if (comboTimer > 0) comboText.SetText("Combo " + combo + "x");
        else combo = 0;

        // COUNTDOWN TIMER
        if (startGameText.alpha > 0)
        {
            startGameText.alpha -= Time.deltaTime/2;
            startGameText.fontSize -= Time.deltaTime*5f;
        }

        if (gameTimeLeft > 0 && isPlayed)
        {
            gameTimeLeft -= Time.deltaTime;
            timerText.SetText("Time Left: \n" + gameTimeLeft.ToString("0.00"));
        }
        else if (!isPlayed && !gameOver) timerText.SetText("Time Left: N/A");
        else if (!gameOver)
        {
            gameOver = true;
            timerText.SetText("Game Over");
            MainManager.Instance.GameBacksound.Stop();
            MainManager.Instance.GameoverSound.Play();

            int currentHighscore = PlayerPrefs.GetInt(Keyword.HIGHEST_SCORE_PREF);
            int currentMaxCombo = PlayerPrefs.GetInt(Keyword.MAX_COMBO_PREF);
            string infoText = "";

            if (score > currentHighscore)
            {
                PlayerPrefs.SetInt(Keyword.HIGHEST_SCORE_PREF, score);
                infoText += "\nNew Highscore!";
            }
            if (maxCombo > currentMaxCombo)
            {
                PlayerPrefs.SetInt(Keyword.MAX_COMBO_PREF, maxCombo);
                infoText += "\nNew Max Combo!";
            }
            GameoverInfoText.SetText(infoText);
            GameOverPanel.SetActive(true);
        }
    }

    void FirstSetup()
    {
        startGameText.alpha = 0f;
        UpdateTutorialClip();
        UpdateSkipTutorialIcon();

        GameOverPanel.SetActive(false);
        MainManager.Instance.GameBacksound.Play();
        gameTimeLeft = 30f;
        comboText.alpha = 0;
        SetLineBoundry();
        UpdateScoreText();
    }

    public void StartGame()
    {
        skipTutorial = PlayerPrefs.GetInt(Keyword.SKIP_TUTORIAL);
        if (skipTutorial == 1) CloseTutorialButton();
    }

    public void PlayAgainButton()
    {
        MainManager.Instance.ButtonSound.Play();
        MainManager.Instance.StartGame();
    }

    public void MainMenuButton()
    {
        MainManager.Instance.ButtonSound.Play();
        MainManager.Instance.GoToMenu();
    }

    public void NextTutoriaButton()
    {
        tutorialClipCode += 1;
        UpdateTutorialClip();
    }

    public void CloseTutorialButton()
    {
        MainManager.Instance.StartGameSound.Play();
        PlayerPrefs.SetInt(Keyword.SKIP_TUTORIAL, skipTutorial);
        TutorialPanel.SetActive(false);
        startGameText.alpha = 1f;
        isPlayed = true;
    }

    public void AlwaysSkipTutorialButton()
    {
        if (skipTutorial == 1) skipTutorial = 0;
        else skipTutorial = 1;
        UpdateSkipTutorialIcon();
    }

    void UpdateTutorialClip()
    {
        int clipCode = 0;
        if (tutorialClipCode % 2 == 0) clipCode = 0;
        else clipCode = 1;

        tutorAnim.SetInteger(Keyword.CLIP_CODE, clipCode);
        tutorTitleText.SetText("Tutorial " + (clipCode + 1) + "/2");
        tutorBodyText.SetText(tutorBody[clipCode]);
    }

    void UpdateSkipTutorialIcon()
    {
        if (skipTutorial == 1) skipIcon.SetActive(true);
        else skipIcon.SetActive(false);
    }

    void SetLineBoundry()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 4;
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;

        Vector3 leftBottomLimit = Camera.main.ViewportToWorldPoint(new Vector3(0.4f, 0.4f, 1f));
        Vector3 rightBottomLimit = Camera.main.ViewportToWorldPoint(new Vector3(0.6f, 0.4f, 1f));
        Vector3 rightUpperLimit = Camera.main.ViewportToWorldPoint(new Vector3(0.6f, 0.6f, 1f));
        Vector3 leftUpperLimit = Camera.main.ViewportToWorldPoint(new Vector3(0.4f, 0.6f, 1f));
        
        Vector3[] lineVectorBoundry = new Vector3[] {leftBottomLimit, rightBottomLimit, rightUpperLimit, leftUpperLimit};
        lineBoundry = new float[] {leftBottomLimit.x, rightBottomLimit.x, leftBottomLimit.y, leftUpperLimit.y};

        line.SetPositions(lineVectorBoundry);
    }

    bool InsideLineBoundry(Vector3 mousePos)
    {
        return mousePos.x > lineBoundry[0] && mousePos.x < lineBoundry[1] &&
            mousePos.y > lineBoundry[2] && mousePos.y< lineBoundry[3];
    }

    public void IncreaseTime()
    {
        combo += 1;
        score += combo;
        gameTimeLeft += 0.75f;
        comboTimer = maxComboTimer;
        timerText.color = Color.green;
        UpdateScoreText();

        if (combo > maxCombo) maxCombo = combo;
    }

    void UpdateScoreText() => scoreText.SetText("Score: " + score);
    
}
