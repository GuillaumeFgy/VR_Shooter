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
    [SerializeField] string pSpeed = "Speed";
    [SerializeField] string pInMelee = "InMelee";
    [SerializeField] string tAttack = "Attack";
    [SerializeField] string tHit = "Hit";
    [SerializeField] string bDead = "Dead";

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
        if (anim) anim.SetFloat(pSpeed, vel.magnitude);
    }




    void TryAttack(bool ranged)
    {
        if (attackTimer < (1f / Mathf.Max(0.01f, config.attackRate))) return;
        attackTimer = 0f;

        if (anim) anim.SetTrigger(tAttack);
        if (ranged) Shoot(); // melee damage via OnMeleeHit() event
    }



    void Shoot()
    {
        if (!projectilePrefab || !target) return;

        Transform spawnT = projectileSpawn ? projectileSpawn : transform;

        Vector3 aimPos = target.position;
        // If the target (e.g., the VR camera rig) has a collider, use its center
        var col = target.GetComponentInParent<Collider>();
        if (col) aimPos = col.bounds.center;

        Vector3 dir = (aimPos - spawnT.position);
        if (dir.sqrMagnitude < 0.0001f) dir = spawnT.forward; // fallback

        Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);

        var go = Instantiate(projectilePrefab, spawnT.position, rot);
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
        if (!target || !config) return;
        LookAtTargetFlat();

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist > config.attackRangeMelee + 0.05f) { MoveForward(1f); return; }

        if (anim) anim.SetFloat(pSpeed, 0f);
        TryAttack(false);
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

        if (Vector3.Distance(transform.position, target.position) <= config.attackRangeDistance)
            TryAttack(true);
    }

    void DoZigZagRush()
    {
        LookAtTargetFlat();
        // forward + side sine
        float side = Mathf.Sin((Time.time + zigZagPhase.x) * config.zigZagFrequency) * config.zigZagAmplitude;
        transform.position += (transform.forward * config.moveSpeed + transform.right * side) * Time.deltaTime;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= config.attackRangeMelee) TryAttack(false);
    }

    void DoTurret()
    {
        LookAtTargetFlat();
        if (Vector3.Distance(transform.position, target.position) <= config.attackRangeDistance)
            TryAttack(true);
    }

    void DoEvader()
    {
        LookAtTargetFlat();
        // small wandering with occasional side dashes
        float side = Mathf.PerlinNoise(Time.time * 0.8f, zigZagPhase.x) - 0.5f;
        transform.position += (transform.forward * (config.moveSpeed * 0.6f) + transform.right * side * config.moveSpeed) * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) <= config.attackRangeDistance + 1f)
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
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, config.attackRangeMelee);
        Gizmos.color = Color.magenta; Gizmos.DrawWireSphere(transform.position, config.attackRangeDistance);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, config.strafeRadius);
    }
}
