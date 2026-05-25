using UnityEngine;
using UnityEngine.Events;

public class CollisionForwarder : MonoBehaviour
{
    public UnityEvent<Vector3> onPlayerCollision;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerHub"))
        {
            onPlayerCollision?.Invoke(transform.position);
        }
    }
}