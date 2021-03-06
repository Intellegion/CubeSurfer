using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelProgress : MonoBehaviour
{
    // Event triggered when the level ends
    public UnityEvent<bool> OnGameOverEvent = new UnityEvent<bool>();

    // Components of other managers
    public LevelGenerator LevelGeneratorComponent;
    public UIManager UIManagerComponent;
    public AudioManager AudioManagerComponent;
    public GameObject MainCamera;

    private PlayerMovement playerMovement;

    private int curentTotal;

    [SerializeField]
    private Material[] SkyBoxes;

    [SerializeField]
    private AudioClip[] LevelMusic;

    [SerializeField]
    private AudioClip MenuMusic;
    [SerializeField]
    private AudioClip VictoryMusic;

    void Start()
    {
        AudioManagerComponent.PlayMusic(MenuMusic);
        curentTotal = PlayerPrefs.GetInt("TotalScore");
        playerMovement = LevelGeneratorComponent.Player.GetComponent<PlayerMovement>();
        OnGameOverEvent.AddListener(GameOver);

        // Used when clicking next level
        if (Constants.CONTINUE)
        {
            StartGame(Constants.LEVEL);
        }
    }

    // Save progress if the player clears a level
    private void GameOver(bool didWin)
    {
        UIManagerComponent.ShowPanel(UIManagerComponent.GameOverPanel);

        if (didWin)
        {
            int currentLevel = Constants.LEVEL;
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

    // Configuring the start of a level
    public void StartGame(int level = 0)
    {
        if (level == 0)
        {
            Constants.LEVEL = 1;
        }

        MainCamera.GetComponent<Skybox>().material = SkyBoxes[Constants.LEVEL - 1];
        AudioManagerComponent.PlayMusic(LevelMusic[Constants.LEVEL - 1]);
        UIManagerComponent.NextButton.gameObject.SetActive(false);
        UIManagerComponent.HidePanel(UIManagerComponent.StartPanel);
        UIManagerComponent.ShowPanel(UIManagerComponent.HUDPanel);
        playerMovement.enabled = true;
        playerMovement.Respawn();
        Constants.CONTINUE = false;
    }

    // Reloading scene when clicking next level to reset all values
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
