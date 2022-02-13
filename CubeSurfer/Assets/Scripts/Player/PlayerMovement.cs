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
    private float xInput;

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
            else
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

            transform.position = movementVector;

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
