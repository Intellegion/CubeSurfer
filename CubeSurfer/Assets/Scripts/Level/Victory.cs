using UnityEngine;

public class Victory : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem victoryEffect;
    private void OnCollisionEnter(Collision collision)
    {
        // Bonus victory on reaching the top most platform
        if (collision.collider.tag.Equals("Player"))
        {
            collision.collider.transform.parent.GetComponent<PlayerMovement>().StopMovement(true);
            victoryEffect.Play();
        }
    }
}
