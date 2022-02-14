using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelProgress : MonoBehaviour
{
    public UnityEvent<bool> OnGameOverEvent = new UnityEvent<bool>();
    public LevelGenerator LevelGeneratorComponent;
    public UIManager UIManagerComponent;
    public AudioManager AudioManagerComponent;
    public GameObject MainCamera;

    private PlayerMovement playerMovement;

    private int curentTotal;

    public Material[] SkyBoxes;
    public AudioClip[] LevelMusic;

    public AudioClip MenuMusic;
    public AudioClip VictoryMusic;
    // Start is called before the first frame update
    void Start()
    {
        AudioManagerComponent.PlayMusic(MenuMusic);
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
            UIManagerComponent.ResultText.text = "VICTORY";
            PlayerPrefs.SetInt("TotalScore", curentTotal + playerMovement.Score);

            if (PlayerPrefs.GetInt("Progress") < currentLevel)
            {
                if (currentLevel > UIManagerComponent.LevelButtons.Length - 1)
                {
                    AudioManagerComponent.PlayMusic(VictoryMusic);
                    return;
                }

                PlayerPrefs.SetInt("Progress", currentLevel);


                UIManagerComponent.LevelButtons[currentLevel].interactable = true;
            }

            UIManagerComponent.NextButton.gameObject.SetActive(true);
        }
        else
        {
            UIManagerComponent.ResultText.text = "DEFEAT";
        }
    }

    public void StartGame()
    {
        MainCamera.GetComponent<Skybox>().material = SkyBoxes[Constants.LEVEL - 1];
        AudioManagerComponent.PlayMusic(LevelMusic[Constants.LEVEL - 1]);
        UIManagerComponent.NextButton.gameObject.SetActive(false);
        UIManagerComponent.HidePanel(UIManagerComponent.StartPanel);
        UIManagerComponent.ShowPanel(UIManagerComponent.HUDPanel);
        playerMovement.enabled = true;
        playerMovement.Respawn();
        Constants.CONTINUE = false;
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
