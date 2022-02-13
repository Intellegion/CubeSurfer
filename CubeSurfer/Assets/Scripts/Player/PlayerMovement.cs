using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private float Velocity;

    [SerializeField]
    private float Sensitivity;

    public GameObject PlayerCube;

    public int Score;

    public Direction currentDirection;

    public bool CanControl;

    public float XClampMin;
    public float XClampMax;
    public float ZClampMin;
    public float ZClampMax;

    private int numberOfCubes;

    private Vector3 movementVector;
    private float xInput;

    private List<GameObject> cubes = new List<GameObject>();

    void Start()
    {
        currentDirection = Direction.Straight;
        CanControl = true;
        numberOfCubes = 1;
        movementVector = new Vector2();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Velocity * Time.deltaTime);

        if (CanControl)
        {
            xInput = Input.GetAxisRaw("Horizontal") * Sensitivity * Time.deltaTime;
            movementVector = transform.position;

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
    }

    public void IncrementCube()
    {
        GameObject go = Instantiate(PlayerCube, transform, false);
        go.transform.localPosition = Vector3.up * 0.5f * numberOfCubes;
        cubes.Add(go);
        numberOfCubes++;
    }

    public void DecrementCube(GameObject cube = null)
    {
        if (cube != null)
        {
            if (cubes.Contains(cube))
            {
                cubes.Remove(cube);
            }
            else
            {
                return;
            }
        }

        else
        {
            if (cubes.Count > 0)
            {
                Destroy(cubes[cubes.Count - 1]);
                cubes.RemoveAt(cubes.Count - 1);
            }
            else
            {
                return;
            }
        }

        numberOfCubes--;
    }
}
