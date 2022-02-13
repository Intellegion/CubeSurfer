using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CollisionDetection : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private int finalRotation;
    int layerMask;

    [SerializeField]
    private GameObject splashObject;

    private GameObject splash;

    private void Start()
    {
        layerMask = 1 << 6;

        playerMovement = transform.parent.GetComponent<PlayerMovement>();
        StartCoroutine(ClearSplash());
    }

    private void Update()
    {
        RaycastHit hit;
        layerMask = ~layerMask;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            splash = Instantiate(splashObject, hit.point, Quaternion.identity);
            playerMovement.splashes.Add(splash);
        }
    }

    private IEnumerator ClearSplash()
    {
        while(playerMovement.splashes.Count == 0)
        {
            yield return new WaitForSeconds(0.3f);
        }

        while (playerMovement.splashes.Count != 0)
        {
            Destroy(playerMovement.splashes[0]);
            playerMovement.splashes.RemoveAt(0);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("obstacle"))
        {
            transform.parent = null;
            playerMovement.DecrementCube(collision.collider.gameObject);
        }

        else if (collision.collider.tag.Equals("path"))
        {
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
}
