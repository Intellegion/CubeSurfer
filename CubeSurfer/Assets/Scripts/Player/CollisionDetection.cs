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
        // Unparenting objects on collision to only allow the upper objects to pass through
        if (collision.collider.tag.Equals("obstacle"))
        {
            playerMovement.AudioManagerComponent.PlayClip(playerMovement.ObstacleEffect);
            this.enabled = false;
            transform.parent = null;
            playerMovement.DecrementCube(gameObject);

            // To avoid unintended physics
            foreach (GameObject cubes in playerMovement.cubes)
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll | ~RigidbodyConstraints.FreezePositionY;
            }
        }
        else if (collision.collider.tag.Equals("player"))
        {
            // Stopping individual cubes to move on top of each other
            foreach (GameObject cubes in playerMovement.cubes)
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        else if (collision.collider.tag.Equals("path"))
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll | ~RigidbodyConstraints.FreezePositionY;

            float xClampMin, xClampMax, zClampMin, zClampMax;
            // Calculating the bounds on each path to avoid letting the player fall out of the map
            if (playerMovement.GetDirection() == Direction.Straight)
            {
                xClampMin = collision.collider.transform.position.x - collision.collider.transform.localScale.x / 2 + 0.5f;
                xClampMax = collision.collider.transform.position.x + collision.collider.transform.localScale.x / 2 - 0.5f;

                playerMovement.SetXClamps(xClampMin, xClampMax);
            }

            else
            {
                zClampMin = collision.collider.transform.position.z - collision.collider.transform.localScale.x / 2 + 0.5f;
                zClampMax = collision.collider.transform.position.z + collision.collider.transform.localScale.x / 2 - 0.5f;

                playerMovement.SetZClamps(zClampMin, zClampMax);
            }
        }

        // Automatically turn the player in curves
        // Horizontal movement disabled temporarily in order to keep the player on track
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

        // Decrease cubes gradually on slimy floor
        else if (collision.collider.tag.Equals("slime"))
        {
            playerMovement.AudioManagerComponent.PlayClip(playerMovement.SlimeEffect);
            StartCoroutine(DecrementCubes());
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Create a trail of splashes
        if (collision.collider.gameObject.layer == 6)
        {
            splash = Instantiate(splashObject, (transform.position - surfacePosition) + Vector3.up * 0.01f, Quaternion.identity);
            splash.transform.SetParent(playerMovement.SplashContainer);
            playerMovement.splashes.Add(splash);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Stop removing cubes on exiting slimy floor
        if(collision.collider.tag.Equals("slime"))
        {
            playerMovement.AudioManagerComponent.StopSoundFX();
            StopAllCoroutines();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Stack cubes on pickup
        if (other.tag.Equals("pickup"))
        {
            playerMovement.pickups.Add(other.gameObject);
            other.gameObject.SetActive(false);
            playerMovement.CubePickUp();
        }

        // Collecting coins
        else if (other.tag.Equals("coin"))
        {
            playerMovement.pickups.Add(other.gameObject);
            other.gameObject.SetActive(false);
            playerMovement.CoinPickUp();
        }

        // Used to measure level progress
        else if (other.tag.Equals("waypoint"))
        {
            playerMovement.IncreaseProgress();
        }
    }

    // Gradual decrease in cubes on slimy ground
    private IEnumerator DecrementCubes()
    {
        while(true)
        {
            playerMovement.DecrementCube();
            yield return new WaitForSeconds(0.15f);
        }
    }

    // Setting player direction based on turnings at curves;
    private void SetParentDirection()
    {
        if (finalRotation == 0 || finalRotation == 360)
        {
            playerMovement.SetDirection(Direction.Straight);
        }
        else if (finalRotation == 90)
        {
            playerMovement.SetDirection(Direction.Right);
        }
        else
        {
            playerMovement.SetDirection(Direction.Left);
        }
    }
}
