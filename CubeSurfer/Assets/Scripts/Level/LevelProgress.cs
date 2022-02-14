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
        int currentLevel = Constants.LEVEL;

        UIManagerComponent.ShowPanel(UIManagerComponent.GameOverPanel);

        if (didWin)
        {
            PlayerPrefs.SetInt("TotalScore", curentTotal + playerMovement.Score);

            if (PlayerPrefs.GetInt("Progress") < currentLevel)
            {
                if (currentLevel > UIManagerComponent.LevelButtons.Length - 1)
                {
                    return;
                }

                PlayerPrefs.SetInt("Progress", currentLevel);


                UIManagerComponent.LevelButtons[currentLevel].interactable = true;
            }

            UIManagerComponent.NextButton.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        Constants.CONTINUE = false;
        UIManagerComponent.NextButton.gameObject.SetActive(false);
        UIManagerComponent.HidePanel(UIManagerComponent.StartPanel);
        UIManagerComponent.ShowPanel(UIManagerComponent.HUDPanel);
        playerMovement.enabled = true;
    }

    public void NextLevel(int currentLevel = 0)
    {
        if (currentLevel == 0)
        {
            Constants.LEVEL++;
        }
        else
        {
            Constants.LEVEL = currentLevel;
        }

        Constants.CONTINUE = true;
        SceneManager.LoadScene(0);
    }
}
