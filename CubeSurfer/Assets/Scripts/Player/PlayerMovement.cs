using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    // First cube
    public GameObject initialCube;

    [SerializeField]
    private LevelProgress levelProgress;

    [SerializeField]
    private GameObject Head;

    [SerializeField]
    private GameObject PlayerCube;

    [SerializeField]
    public Transform SplashContainer;

    [SerializeField]
    private Vector3 StartPosition;

    [SerializeField]
    private CinemachineVirtualCamera vCam;

    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    private float shakeTimer;

    private float velocity;
    private float sensitivity;

    // Audio fields
    public AudioManager AudioManagerComponent;
    public AudioClip SlimeEffect;
    public AudioClip ObstacleEffect;
    public AudioClip VictoryEffect;
    public AudioClip DefeatEffect;
    public AudioClip CoinEffect;
    public AudioClip StackEffect;

    public Slider ProgressSlider;

    public TextMeshProUGUI ScoreText;

    public int Score;

    public bool CanControl;

    // Used to clamp movement on path
    private float XClampMin;
    private float XClampMax;
    private float ZClampMin;
    private float ZClampMax;

    private int progress;


    private Direction currentDirection;
    private Vector3 movementVector;
    private float xInput;
    
    // Reference of objects that need to be reset on respawn
    public List<GameObject> cubes = new List<GameObject>();
    public List<GameObject> splashes = new List<GameObject>();
    public List<GameObject> pickups = new List<GameObject>();

    private List<GameObject> removedCubes = new List<GameObject>();

    private Touch touch;

    void Start()
    {
        cubes.Add(initialCube);
        cinemachineBasicMultiChannelPerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        Initialize();
    }

    // Default values on start
    void Initialize()
    {
        XClampMax = 2;
        XClampMin = -2;

        ScoreText.text = "0";

        currentDirection = Direction.Straight;
        CanControl = true;
        movementVector = new Vector2();

        progress = 0;
        ProgressSlider.value = 0;

        velocity = Constants.VELOCITY;
        sensitivity = Constants.SENSITIVITY;

        StartCoroutine(ClearSplashes());
    }

    void Update()
    {
        // Translating the parent in order to make all cubes move at a constant speed
        transform.Translate(Vector3.forward * velocity * Time.deltaTime);

        if (CanControl)
        {
            xInput = Input.GetAxisRaw("Horizontal") * sensitivity * 14 * Time.deltaTime;
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                // Swipe to move player
                if (touch.phase == TouchPhase.Moved)
                {
                    xInput = touch.deltaPosition.x * sensitivity * Time.deltaTime;
                }
            }

            movementVector = transform.position;

            // Position clamps
            if (currentDirection == Direction.Straight)
            {
                if (transform.position.x + xInput > XClampMax)
                {
                    movementVector.x = XClampMax;
                }
                else if (transform.position.x + xInput < XClampMin)
                {
                    movementVector.x = XClampMin;
                }
                else
                {
                    movementVector.x += xInput;
                }
            }
            else if (currentDirection == Direction.Left)
            {
                if (transform.position.z + xInput > ZClampMax)
                {
                    movementVector.z = ZClampMax;
                }
                else if (transform.position.z + xInput < ZClampMin)
                {
                    movementVector.z = ZClampMin;
                }
                else
                {
                    movementVector.z += xInput;
                }
            }
            else
            {
                if (transform.position.z - xInput > ZClampMax)
                {
                    movementVector.z = ZClampMax;
                }
                else if (transform.position.z - xInput < ZClampMin)
                {
                    movementVector.z = ZClampMin;
                }
                else
                {
                    movementVector.z -= xInput;
                }
            }

            transform.position = movementVector;
        }

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <=0)
            {
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }
        }
    }

    // Stacking cubes on top
    public void IncrementCube()
    {
        GameObject go = Instantiate(PlayerCube, transform, false);
        go.transform.localPosition = Vector3.up * cubes.Count;
        Head.transform.localPosition = go.transform.localPosition + Vector3.up;
        cubes.Add(go);
    }

    // Removing cubes from stack
    public void DecrementCube(GameObject cube = null)
    {
        if (cube != null)
        {
            if (cubes.Contains(cube))
            {
                cubes.Remove(cube);
                removedCubes.Add(cube);
                StartCoroutine(DestroyCube(cube));
            }

            if (cubes.Count == 0)
            {
                initialCube = null;
                StopMovement();
                StopAllCoroutines();
            }
        }
        else
        {
            if (cubes.Count > 0)
            {
                Destroy(cubes[cubes.Count - 1]);
                cubes.RemoveAt(cubes.Count - 1);

                if (cubes.Count == 0)
                {
                    StopMovement();
                }
            }
        }
    }

    // Delay on destroying cubes
    private IEnumerator DestroyCube(GameObject cube)
    {
        yield return new WaitForSeconds(1);

        Destroy(cube);
    }

    // Clearing splashes to optimize level
    private IEnumerator ClearSplashes()
    {
        yield return new WaitForSeconds(1);

        while (true)
        {
            if (splashes.Count > 0)
            {
                Destroy(splashes[0]);
                splashes.RemoveAt(0);
            }

            yield return new WaitForSeconds(0.075f);
        }
    }

    // Game over functionality
    public void StopMovement(bool bonus = false)
    {
        bool victory = progress >= Constants.MAX_CHUNKS;
        velocity = sensitivity = 0;

        if (victory)
        {
            if (bonus)
            {
                Score *= 5;
            }

            AudioManagerComponent.PlayClip(VictoryEffect);
        }
        else
        {
            AudioManagerComponent.PlayClip(DefeatEffect);
        }

        levelProgress.OnGameOverEvent.Invoke(victory);
    }

    // Called everytime the user starts a level
    // Resets everything to normal 
    public void Respawn()
    {
        transform.position = StartPosition;
        transform.rotation = Quaternion.identity;
        Initialize();
        foreach (GameObject splash in splashes)
        {
            Destroy(splash);
        }
        splashes.Clear();

        foreach (GameObject cube in cubes)
        {
            Destroy(cube);
        }
        cubes.Clear();

        foreach (GameObject removed in removedCubes)
        {
            Destroy(removed);
        }
        removedCubes.Clear();

        foreach (GameObject pickup in pickups)
        {
            pickup.SetActive(true);
        }

        if (initialCube == null)
        {
            IncrementCube();
            initialCube = cubes[0];
            initialCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    // Calculates progress based on the number of chunks crossed
    public void IncreaseProgress()
    {
        progress++;
        ProgressSlider.value = (float) progress / Constants.MAX_CHUNKS;
    }

    // Adding cubes to the stack
    public void CubePickUp()
    {
        AudioManagerComponent.PlayClip(StackEffect);
        IncrementCube();
    }

    // Adds to the score
    public void CoinPickUp()
    {
        AudioManagerComponent.PlayClip(CoinEffect);
        Score++;
        ScoreText.text = Score.ToString();
    }

    public void ScreenShake(float intensity, float time)
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    public Direction GetDirection()
    {
        return currentDirection;
    }
    public void SetDirection(Direction direction)
    {
        currentDirection = direction;
    }

    public void SetXClamps(float min, float max)
    {
        XClampMin = min;
        XClampMax = max;
    }

    public void SetZClamps(float min, float max)
    {
        ZClampMin = min;
        ZClampMax = max;
    }
}
