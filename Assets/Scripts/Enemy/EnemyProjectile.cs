// EnemyProjectile.cs
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyProjectile : MonoBehaviour
{
    public int damage = 5;
    public float speed = 12f;
    public float lifetime = 5f;
    public EnemyController owner;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other) return;
        // Ignore hitting the shooter itself
        if (owner && other.transform.IsChildOf(owner.transform)) return;

        // Example player damage hook:
        // var ph = other.GetComponentInParent<PlayerHealth>();
        // if (ph) ph.TakeDamage(damage);

        Destroy(gameObject);
    }
}
