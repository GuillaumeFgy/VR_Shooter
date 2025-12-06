using UnityEngine;

public class BonusPickup : MonoBehaviour
{
    public enum BonusType
    {
        Heart,
        Star,
        Arrows,
        Bullets,
        Mystery
    }

    [Header("Setup")]
    public BonusType bonusType;
    public BonusSpawner ownerSpawner;

    [Header("Heart (life)")]
    public int heartAmount = 1;              // +1 life, clamped to max

    [Header("Star (score)")]
    public int starScore = 200;

    [Header("Arrows (speed boost)")]
    public float speedMultiplier = 1.7f;
    public float speedDuration = 3f;

    [Header("Bullets (burst)")]
    public int bonusMaxBullets = 5;         // temporary max shots
    public int bonusShots = 5;              // how many shots to fire
    public float burstInterval = 0.05f;     // time between shots in burst

    void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        var movement = other.GetComponentInParent<PlayerMovement>();
        var shooting = other.GetComponentInParent<PlayerShooting>();

        ApplyBonus(health, movement, shooting);

        if (ownerSpawner != null)
        {
            ownerSpawner.OnBonusCollected(this);
        }
        Destroy(transform.root.gameObject);
    }


    void ApplyBonus(PlayerHealth health, PlayerMovement movement, PlayerShooting shooting)
    {
        // Mystery = random other bonus
        BonusType resolved = bonusType;
        if (bonusType == BonusType.Mystery)
        {
            // Random between Heart, Star, Arrows, Bullets
            resolved = (BonusType)Random.Range(0, (int)BonusType.Mystery);
        }

        switch (resolved)
        {
            case BonusType.Heart:
                health.AddLife(heartAmount);
                break;

            case BonusType.Star:
                if (ScoreManager.Instance != null)
                    ScoreManager.Instance.AddScore(starScore);
                break;

            case BonusType.Arrows:
                if (movement != null)
                    movement.ApplySpeedBoost(speedMultiplier, speedDuration);
                break;

            case BonusType.Bullets:
                if (shooting != null)
                    shooting.TriggerBulletBonus(bonusMaxBullets, bonusShots, burstInterval);
                break;
        }
    }
}
