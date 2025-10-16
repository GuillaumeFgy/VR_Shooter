using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{

    public enum EnemyBehavior
    {
        Charger,        // rush straight at player and bite
        StrafeShooter,  // circle the player and shoot
        ZigZagRush,     // run forward with a lateral sine wave
        Turret,         // stationary shooter
        Evader          // drifts around, occasionally dodges sideways then shoots
    }

    [Header("Setup")]
    public EnemyConfig config;
    public EnemyBehavior behavior = EnemyBehavior.Charger;
    public Transform projectileSpawn;    // optional; falls back to transform
    public GameObject projectilePrefab;  // simple rigidbody projectile (below)
    public Transform target;             // usually the VR camera/player

    [Header("Animation")]
    public Animator anim;
    public string animIntName = "animation";

    [Header("Gizmos")]
    public bool drawRanges = true;

    EnemyHealth health;
    float attackTimer;
    Vector3 zigZagPhase;  // for ZigZag
    float strafeAngle;

    void Awake()
    {
        health = GetComponent<EnemyHealth>();
        if (!config) config = health.config; // auto-pull from health if not set
        if (!target && Camera.main) target = Camera.main.transform;
        zigZagPhase = new Vector3(Random.value * 10f, 0, 0);
        strafeAngle = Random.Range(0f, 360f);
    }

    void Update()
    {
        if (!target || !config) return;

        switch (behavior)
        {
            case EnemyBehavior.Charger:
                DoCharger();
                break;
            case EnemyBehavior.StrafeShooter:
                DoStrafeShooter();
                break;
            case EnemyBehavior.ZigZagRush:
                DoZigZagRush();
                break;
            case EnemyBehavior.Turret:
                DoTurret();
                break;
            case EnemyBehavior.Evader:
                DoEvader();
                break;
        }

        attackTimer += Time.deltaTime;
    }

    void LookAtTargetFlat()
    {
        Vector3 to = (target.position - transform.position);
        to.y = 0f;
        if (to.sqrMagnitude > 0.0001f)
        {
            Quaternion want = Quaternion.LookRotation(to, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, want, config.turnSpeed * Time.deltaTime);
        }
    }

    void MoveForward(float speedMul = 1f)
    {
        Vector3 vel = transform.forward * (config.moveSpeed * speedMul);
        transform.position += vel * Time.deltaTime;

        if (!anim) return;

        if (vel.magnitude > 0.05f)
            anim.SetInteger(animIntName, 2); // moving
        else
            anim.SetInteger(animIntName, 1); // idle
    }


    void TryAttack(bool ranged)
    {
        if (attackTimer < (1f / Mathf.Max(0.01f, config.attackRate))) return;
        attackTimer = 0f;

        if (anim) anim.SetInteger(animIntName, 3); // attack animation

        if (ranged) Shoot();
        // For melee, use animation event to call OnMeleeHit()
    }

    void Shoot()
    {
        if (!projectilePrefab) return;
        Transform spawnT = projectileSpawn ? projectileSpawn : transform;
        LookAtTargetFlat();
        var go = Instantiate(projectilePrefab, spawnT.position, spawnT.rotation);
        var proj = go.GetComponent<EnemyProjectile>();
        if (proj)
        {
            proj.damage = config.projectileDamage;
            proj.speed = config.projectileSpeed;
            proj.owner = this;
        }
    }

    // --- Behaviors ---

    void DoCharger()
    {
        LookAtTargetFlat();
        MoveForward(1f);

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= 1.4f) TryAttack(false);
    }

    void DoStrafeShooter()
    {
        // orbit the player at Config.strafeRadius while facing them
        Vector3 to = (transform.position - target.position);
        to.y = 0f;
        if (to.sqrMagnitude < 0.01f)
            to = Quaternion.Euler(0, Random.Range(0, 360f), 0) * Vector3.forward * config.strafeRadius;

        float wantRadius = config.strafeRadius;
        float dist = to.magnitude;

        // radial push
        Vector3 radialDir = to.normalized;
        float radialSpeed = (dist - wantRadius) * 0.9f;

        // tangential strafe
        Vector3 tangential = Vector3.Cross(Vector3.up, radialDir);
        float tangentialSpeed = config.moveSpeed;

        Vector3 vel = (-radialDir * radialSpeed) + (tangential * tangentialSpeed);
        transform.position += vel * Time.deltaTime;

        LookAtTargetFlat();

        if (Vector3.Distance(transform.position, target.position) <= config.attackRange)
            TryAttack(true);
    }

    void DoZigZagRush()
    {
        LookAtTargetFlat();
        // forward + side sine
        float side = Mathf.Sin((Time.time + zigZagPhase.x) * config.zigZagFrequency) * config.zigZagAmplitude;
        transform.position += (transform.forward * config.moveSpeed + transform.right * side) * Time.deltaTime;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= 1.6f) TryAttack(false);
    }

    void DoTurret()
    {
        LookAtTargetFlat();
        if (Vector3.Distance(transform.position, target.position) <= config.attackRange)
            TryAttack(true);
    }

    void DoEvader()
    {
        LookAtTargetFlat();
        // small wandering with occasional side dashes
        float side = Mathf.PerlinNoise(Time.time * 0.8f, zigZagPhase.x) - 0.5f;
        transform.position += (transform.forward * (config.moveSpeed * 0.6f) + transform.right * side * config.moveSpeed) * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) <= config.attackRange + 1f)
            TryAttack(true);
    }

    // Example: called by an animation event during a melee hit frame
    public void OnMeleeHit()
    {
        // If you have a PlayerHealth component, call it here.
        // var ph = target.GetComponentInParent<PlayerHealth>();
        // if (ph) ph.TakeDamage(config.contactDamage);
    }

    void OnDrawGizmosSelected()
    {
        if (!drawRanges || !config) return;
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, config.attackRange);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, config.strafeRadius);
    }

    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter()
    {
        return;
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit()
    {
        return;
    }

    /// <summary>
    /// This method is called by the Main Camera when it is gazing at this GameObject and the screen
    /// is touched.
    /// </summary>
    public void OnPointerClick()
    {
        health.TakeDamage(1);
    }
}
