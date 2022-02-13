using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    private float Velocity;

    [SerializeField]
    private float Sensitivity;

    public GameObject PlayerCube;

    private LevelGenerator levelGenerator;

    public Direction currentDirection;

    public bool CanControl;

    public float XClampMin;
    public float XClampMax;
    public float ZClampMin;
    public float ZClampMax;

    private int numberOfCubes;

    private Vector3 movementVector;

    void Start()
    {
        currentDirection = Direction.Straight;
        CanControl = true;
        numberOfCubes = 1;
        rb = GetComponent<Rigidbody>();
        movementVector = new Vector2();
        levelGenerator = FindObjectOfType<LevelGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Velocity * Time.deltaTime);

        if (CanControl)
        {
            movementVector.x = Input.GetAxis("Horizontal");

            if (currentDirection == Direction.Straight)
            {
                if (transform.position.x + movementVector.x > XClampMax || transform.position.x + movementVector.x < XClampMin)
                {
                    return;
                }
            }
            else
            {
                if (transform.position.z + movementVector.x > ZClampMax || transform.position.z + movementVector.x < XClampMin)
                {
                    return;
                }
            }

            transform.Translate(movementVector * Sensitivity * Time.deltaTime);

            Debug.Log(XClampMin + ", X " + XClampMax);
            Debug.Log(ZClampMin + ", Z " + ZClampMax);
        }
    }

    public void IncrementCube()
    {
        GameObject go = Instantiate(PlayerCube, transform, false);
        go.transform.localPosition = Vector3.up * numberOfCubes;
        numberOfCubes++;
    }

    public void DecrementCube()
    {
        numberOfCubes--;
    }
}
