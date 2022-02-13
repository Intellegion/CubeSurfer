using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CollisionDetection : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private int finalRotation;

    private void Start()
    {
        playerMovement = transform.parent.GetComponent<PlayerMovement>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("obstacle"))
        {
            transform.parent = null;
        }

        else if (collision.collider.tag.Equals("turning"))
        {
            playerMovement.CanControl = false;
            if (collision.collider.transform.rotation.eulerAngles.y == 0 || collision.collider.transform.rotation.eulerAngles.y == 90)
            {
                finalRotation = (int)transform.parent.rotation.eulerAngles.y - 90;
                SetParentDirection();
                transform.parent.DORotate(new Vector3(0, finalRotation, 0), 0.75f).OnComplete(delegate
                    {
                        playerMovement.CanControl = true;           
                    });
            }

            else
            {
                finalRotation = (int)transform.parent.rotation.eulerAngles.y + 90;
                SetParentDirection();
                transform.parent.DORotate(new Vector3(0, finalRotation, 0), 0.75f).OnComplete(delegate
                {
                    playerMovement.CanControl = true;
                });
            }
        }

        else if (collision.collider.tag.Equals("path"))
        {
            if (playerMovement.currentDirection == Direction.Straight)
            {
                playerMovement.XClampMin = collision.collider.transform.position.x - collision.collider.transform.localScale.x / 2;
                playerMovement.XClampMax = collision.collider.transform.position.x + collision.collider.transform.localScale.x / 2;
            }

            else
            {
                playerMovement.ZClampMin = collision.collider.transform.position.z - collision.collider.transform.localScale.x / 2;
                playerMovement.ZClampMax = collision.collider.transform.position.z + collision.collider.transform.localScale.x / 2f;
            }

        }
    }

    private void SetParentDirection()
    {
        if (finalRotation == 0 || finalRotation == 360)
        {
            playerMovement.currentDirection = Direction.Straight;
        }
        else if (finalRotation == 90)
        {
            playerMovement.currentDirection = Direction.Right;
        }
        else
        {
            playerMovement.currentDirection = Direction.Left;
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("obstacle"))
        {
            playerMovement.IncrementCube();
        }
    }

}
