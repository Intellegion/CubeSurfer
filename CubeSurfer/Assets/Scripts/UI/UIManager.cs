using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public CanvasGroup StartPanel;
    public CanvasGroup HUDPanel;
    public CanvasGroup GameOverPanel;

    public Button RetryButton;
    public Button NextButton;

    public TextMeshProUGUI TotalScoreText;
    public TextMeshProUGUI ResultText;

    public Button[] LevelButtons;

    public Slider ProgressSlider;
    private void Start()
    {
        TotalScoreText.text = PlayerPrefs.GetInt("TotalScore").ToString();
        EnableLevels();
    }

    public void ShowPanel(CanvasGroup panel)
    {
        panel.alpha = 1;
        panel.blocksRaycasts = true;
        panel.interactable = true;
    }

    public void HidePanel(CanvasGroup panel)
    {
        panel.alpha = 0;
        panel.blocksRaycasts = false;
        panel.interactable = false;
    }

    public void EnableLevels()
    {
        int progress = PlayerPrefs.GetInt("Progress") + 1;
        for (int i = 0; i < progress; i++)
        {
            LevelButtons[i].interactable = true;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
