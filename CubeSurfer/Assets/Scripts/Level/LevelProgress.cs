using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelProgress : MonoBehaviour
{
    public UnityEvent<bool> OnGameOverEvent = new UnityEvent<bool>();
    public LevelGenerator LevelGeneratorComponent;
    public UIManager UIManagerComponent;

    private PlayerMovement playerMovement;

    private int curentTotal;
    // Start is called before the first frame update
    void Start()
    {
        curentTotal = PlayerPrefs.GetInt("TotalScore");
        playerMovement = LevelGeneratorComponent.Player.GetComponent<PlayerMovement>();
        OnGameOverEvent.AddListener(GameOver);

        if (Constants.CONTINUE)
        {
            StartGame();
        }
    }

    private void GameOver(bool didWin)
    {
        int currentLevel = LevelGeneratorComponent.Level + 1;

        UIManagerComponent.ShowPanel(UIManagerComponent.GameOverPanel);

        if (didWin)
        {
            PlayerPrefs.SetInt("TotalScore", curentTotal + playerMovement.Score);

            if (PlayerPrefs.GetInt("Progress") < currentLevel)
            {
                PlayerPrefs.SetInt("Progress", currentLevel);

                if (currentLevel > UIManagerComponent.LevelButtons.Length)
                {
                    return;
                }

                UIManagerComponent.LevelButtons[currentLevel].interactable = true;
            }

            UIManagerComponent.NextButton.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        Constants.CONTINUE = false;
        UIManagerComponent.HidePanel(UIManagerComponent.StartPanel);
        UIManagerComponent.ShowPanel(UIManagerComponent.HUDPanel);
        playerMovement.enabled = true;
    }

    public void NextLevel()
    {
        Constants.LEVEL++;
        Constants.CONTINUE = true;
        SceneManager.LoadScene(0);
    }
}
