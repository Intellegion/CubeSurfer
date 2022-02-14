using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public GameObject initialCube;

    [SerializeField]
    private LevelProgress levelProgress;

    [SerializeField]
    private GameObject Head;

    public float Velocity;
    public float Sensitivity;

    public AudioManager AudioManagerComponent;
    public AudioClip SlimeEffect;
    public AudioClip ObstacleEffect;
    public AudioClip VictoryEffect;
    public AudioClip DefeatEffect;
    public AudioClip CoinEffect;
    public AudioClip StackEffect;

    public Transform SplashContainer;

    public GameObject PlayerCube;

    public Slider ProgressSlider;

    public TextMeshProUGUI ScoreText;

    public int Score;

    public Direction CurrentDirection;

    public bool CanControl;

    public float XClampMin;
    public float XClampMax;
    public float ZClampMin;
    public float ZClampMax;

    public int progress;

    public Vector3 StartPosition;

    private Vector3 movementVector;
    private float xInput;
    

    public List<GameObject> cubes = new List<GameObject>();
    public List<GameObject> splashes = new List<GameObject>();
    public List<GameObject> pickups = new List<GameObject>();

    private List<GameObject> removedCubes = new List<GameObject>();
    private Touch touch;

    void Start()
    {
        cubes.Add(initialCube);
        Initialize();
    }

    void Initialize()
    {
        XClampMax = 2;
        XClampMin = -2;

        ScoreText.text = "0";

        CurrentDirection = Direction.Straight;
        CanControl = true;
        movementVector = new Vector2();

        progress = 0;
        ProgressSlider.value = 0;

        Velocity = Constants.VELOCITY;
        Sensitivity = Constants.SENSITIVITY;

        StartCoroutine(ClearSplashes());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Velocity * Time.deltaTime);

        if (CanControl)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    xInput = touch.deltaPosition.x * Sensitivity * Time.deltaTime;
                }
            }

            xInput = Input.GetAxisRaw("Horizontal") * Sensitivity * 14 * Time.deltaTime;
            movementVector = transform.position;

            if (CurrentDirection == Direction.Straight)
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
            else if (CurrentDirection == Direction.Left)
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
    }

    public void IncrementCube()
    {
        GameObject go = Instantiate(PlayerCube, transform, false);
        go.transform.localPosition = Vector3.up * cubes.Count;
        Head.transform.localPosition = go.transform.localPosition + Vector3.up;
        cubes.Add(go);
    }

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

    private IEnumerator DestroyCube(GameObject cube)
    {
        yield return new WaitForSeconds(1);

        Destroy(cube);
    }

    private IEnumerator ClearSplashes()
    {
        while (true)
        {
            if (splashes.Count > 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    Destroy(splashes[i]);
                    splashes.RemoveAt(i);
                }

                yield return new WaitForSeconds(0.3f);
            }

            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }

    public void StopMovement()
    {
        bool victory = progress >= Constants.MAX_CHUNKS;
        Velocity = Sensitivity = 0;

        if (victory)
        {
            AudioManagerComponent.PlayClip(VictoryEffect);
        }
        else
        {
            AudioManagerComponent.PlayClip(DefeatEffect);
        }

        levelProgress.OnGameOverEvent.Invoke(victory);
    }

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

    public void IncreaseProgress()
    {
        progress++;
        ProgressSlider.value = (float) progress / Constants.MAX_CHUNKS;
    }

    public void CubePickUp()
    {
        AudioManagerComponent.PlayClip(StackEffect);
        IncrementCube();
    }

    public void CoinPickUp()
    {
        AudioManagerComponent.PlayClip(CoinEffect);
        Score++;
        ScoreText.text = Score.ToString();
    }
}
