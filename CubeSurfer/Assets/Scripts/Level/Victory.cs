using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Player"))
        {
            collision.collider.transform.parent.GetComponent<PlayerMovement>().StopMovement();
            collision.collider.transform.parent.GetComponent<PlayerMovement>().Score *= 5;
        }
    }
}
