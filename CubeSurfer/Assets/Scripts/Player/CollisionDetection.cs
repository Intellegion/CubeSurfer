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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("obstacle"))
        {
            playerMovement.AudioManagerComponent.PlayClip(playerMovement.ObstacleEffect);
            this.enabled = false;
            transform.parent = null;
            playerMovement.DecrementCube(gameObject);

            foreach (GameObject cubes in playerMovement.cubes)
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll | ~RigidbodyConstraints.FreezePositionY;
            }
        }
        else if (collision.collider.tag.Equals("player"))
        {
            foreach (GameObject cubes in playerMovement.cubes)
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        else if (collision.collider.tag.Equals("path"))
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll | ~RigidbodyConstraints.FreezePositionY;

            if (playerMovement.CurrentDirection == Direction.Straight)
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
            playerMovement.AudioManagerComponent.PlayClip(playerMovement.SlimeEffect);
            StartCoroutine(DecrementCubes());
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer == 6)
        {
            splash = Instantiate(splashObject, (transform.position - surfacePosition) + Vector3.up * 0.01f, Quaternion.identity);
            splash.transform.SetParent(playerMovement.SplashContainer);
            playerMovement.splashes.Add(splash);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.collider.tag.Equals("slime"))
        {
            playerMovement.AudioManagerComponent.StopSoundFX();
            StopAllCoroutines();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("pickup"))
        {
            playerMovement.pickups.Add(other.gameObject);
            other.gameObject.SetActive(false);
            playerMovement.CubePickUp();
        }

        else if (other.tag.Equals("coin"))
        {
            playerMovement.pickups.Add(other.gameObject);
            other.gameObject.SetActive(false);
            playerMovement.CoinPickUp();
        }

        else if (other.tag.Equals("waypoint"))
        {
            playerMovement.IncreaseProgress();
        }
    }

    private IEnumerator DecrementCubes()
    {
        while(true)
        {
            playerMovement.DecrementCube();
            yield return new WaitForSeconds(0.15f);
        }
    }

    private void SetParentDirection()
    {
        if (finalRotation == 0 || finalRotation == 360)
        {
            playerMovement.CurrentDirection = Direction.Straight;
        }
        else if (finalRotation == 90)
        {
            playerMovement.CurrentDirection = Direction.Right;
        }
        else
        {
            playerMovement.CurrentDirection = Direction.Left;
        }
    }
}
