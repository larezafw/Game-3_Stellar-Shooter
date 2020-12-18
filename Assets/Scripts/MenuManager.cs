using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject CreditPanel;
    public TextMeshProUGUI HighestScoreText;
    public TextMeshProUGUI HighestComboText;
    int HighScore;
    int HighCombo;

    private void Start()
    {
        SetHighScoreVisual();
        CreditPanel.SetActive(false);
        MainManager.Instance.MenuBacksound.Play();
    }

    void SetHighScoreVisual()
    {
        HighScore = PlayerPrefs.GetInt(Keyword.HIGHEST_SCORE_PREF,0);
        HighCombo = PlayerPrefs.GetInt(Keyword.MAX_COMBO_PREF,0);

        string boldTag = "<b>";
        string closeTag = "</b>";
        if (HighScore > 0) HighestScoreText.SetText("Highest Score: " + boldTag + HighScore + closeTag);
        else HighestScoreText.SetText("Highest Score: " + boldTag + "N/A" + closeTag);
        if(HighCombo>0) HighestComboText.SetText("Max Combo: " + boldTag + HighCombo + closeTag);
        else HighestComboText.SetText("Max Combo: " + boldTag + "N/A" + closeTag);
    }

    public void ShowCreditButton()
    {
        MainManager.Instance.ButtonSound.Play();
        CreditPanel.SetActive(true);
    }

    public void StartGameButton()
    {
        MainManager.Instance.ButtonSound.Play();
        MainManager.Instance.MenuBacksound.Stop();
        MainManager.Instance.StartGame();
    }

    public void HideAllPanel()
    {
        MainManager.Instance.ButtonSound.Play();
        CreditPanel.SetActive(false);
    }
}
