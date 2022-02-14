using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CollisionDetection : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private int finalRotation;

    [SerializeField]
    private GameObject splashObject;

    private GameObject splash;

    private Vector3 surfacePosition;
    private void Start()
    {
        surfacePosition = new Vector3(0, 0.5f, 0);
        playerMovement = transform.parent.GetComponent<PlayerMovement>();
        StartCoroutine(ClearSplashes());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("obstacle"))
        {
            transform.parent = null;
            playerMovement.DecrementCube(gameObject);
            foreach (GameObject child in playerMovement.cubes)
            {
                child.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePositionY;
            }
        }

        else if (collision.collider.tag.Equals("path"))
        {
            foreach (GameObject child in playerMovement.cubes)
            {
                child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }

            if (playerMovement.currentDirection == Direction.Straight)
            {
                playerMovement.XClampMin = collision.collider.transform.position.x - collision.collider.transform.localScale.x / 2 + 0.5f;
                playerMovement.XClampMax = collision.collider.transform.position.x + collision.collider.transform.localScale.x / 2 - 0.5f;
            }

            else
            {
                playerMovement.ZClampMin = collision.collider.transform.position.z - collision.collider.transform.localScale.x / 2 + 0.5f;
                playerMovement.ZClampMax = collision.collider.transform.position.z + collision.collider.transform.localScale.x / 2 - 0.5f;
            }

        }

        else if (collision.collider.tag.Equals("turning"))
        {
            playerMovement.CanControl = false;
            if (collision.collider.transform.rotation.eulerAngles.y == 0 || collision.collider.transform.rotation.eulerAngles.y == 90)
            {
                finalRotation = (int)transform.parent.rotation.eulerAngles.y - 90;
                SetParentDirection();
                transform.parent.DORotate(new Vector3(0, finalRotation, 0), 1).OnComplete(delegate
                    {
                        playerMovement.CanControl = true;           
                    });
            }

            else
            {
                finalRotation = (int)transform.parent.rotation.eulerAngles.y + 90;
                SetParentDirection();
                transform.parent.DORotate(new Vector3(0, finalRotation, 0), 1).OnComplete(delegate
                {
                    playerMovement.CanControl = true;
                });
            }
        }  

        else if (collision.collider.tag.Equals("slime"))
        {
            StartCoroutine(DecrementCubes());
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer == 6)
        {
            splash = Instantiate(splashObject, (transform.position - surfacePosition) + Vector3.up * 0.01f, Quaternion.identity);
            playerMovement.splashes.Add(splash);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.collider.tag.Equals("slime"))
        {
            StopAllCoroutines();
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
        if (other.tag.Equals("pickup"))
        {
            other.gameObject.SetActive(false);
            playerMovement.IncrementCube();
        }

        else if (other.tag.Equals("coin"))
        {
            other.gameObject.SetActive(false);
            playerMovement.Score++;
        }
    }

    private IEnumerator DecrementCubes()
    {
        while(true)
        {
            playerMovement.DecrementCube();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator ClearSplashes()
    {
        while (true)
        {
            if (playerMovement.splashes.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    Destroy(playerMovement.splashes[i]);
                    playerMovement.splashes.RemoveAt(i);
                }

                yield return new WaitForSeconds(0.3f);
            }

            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }
}
