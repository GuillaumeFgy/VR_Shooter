using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Damage & Timing")]
    public int damagePerShot = 1;
    public float timeBetweenBullets = 0.15f;

    [Header("Barrel / Aiming")]
    public GameObject refNozzle;            // optional; if null, we’ll use the VR camera
    public float maxDistance = 100f;
    public LayerMask hitMask = ~0;          // set to your Enemy/World layers

    [Header("FX")]
    public Light gunLight;
    public float effectsDisplayTime = 0.1f;
    public LineRenderer lineRenderer;

    [Header("Audio")]
    public AudioClip shootClip;
    public AudioClip reloadClip;

    [Header("Animation")]
    public Animator anim;

    [Header("Ammo UI")]
    public int initialBullets = 3;

    [Header("Reload")]
    public float reloadTime = 1.6f;

    [Header("Input")]
    public SoundDetector clap;

    // --- private ---
    private int bullets;
    private bool reloading;
    private float elapsed;
    private AudioSource audioSrc;
    private Camera cam;

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
        cam = Camera.main;

        bullets = Mathf.Max(0, initialBullets);

        if (gunLight) gunLight.enabled = false;

        if (lineRenderer)
        {
            lineRenderer.useWorldSpace = true;
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        bool triggerPressed = clap && clap.ConsumeClap();

        if (triggerPressed && !reloading && elapsed >= timeBetweenBullets && bullets > 0 && Time.timeScale > 0f)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        elapsed = 0f;

        ConsumeAmmo();
        PlayShotAV();

        (Vector3 origin, Vector3 direction) = GetFireRay();

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, hitMask, QueryTriggerInteraction.Collide))
        {
            HandleHit(hit, origin);
        }
        else
        {
            HandleMiss(origin, direction);
        }

        PlayEffects();
        MaybeStartReload();
    }

    private void ConsumeAmmo()
    {
        // Consume ammo immediately to prevent over-firing
        bullets--;
    }

    private void PlayShotAV()
    {
        if (shootClip) audioSrc.PlayOneShot(shootClip);
        if (anim) anim.SetTrigger("Shoot");
    }

    /// <summary>Returns the firing ray's origin and forward direction.</summary>
    private (Vector3 origin, Vector3 direction) GetFireRay()
    {
        Transform originT = refNozzle ? refNozzle.transform : (cam ? cam.transform : transform);
        return (originT.position, originT.forward);
    }

    private void HandleHit(RaycastHit hit, Vector3 origin)
    {
        DrawTracer(origin, hit.point);

        // 1) Pop enemy projectiles with one shot
        var incoming = hit.collider ? hit.collider.GetComponentInParent<EnemyProjectile>() : null;
        if (incoming)
        {
            Destroy(incoming.gameObject);
            return; // done for this shot
        }

        // 2) Damage enemies
        if (hit.collider && hit.collider.CompareTag("Enemy"))
        {
            var eh = hit.collider.GetComponentInParent<EnemyHealth>();
            if (eh != null) eh.TakeDamage(damagePerShot);
            else Debug.LogError("NO ENEMY HEALTH");
        }
    }

    private void HandleMiss(Vector3 origin, Vector3 direction)
    {
        Debug.LogError("NO HIT");
        DrawTracer(origin, origin + direction * maxDistance);
    }

    private void DrawTracer(Vector3 from, Vector3 to)
    {
        if (!lineRenderer) return;
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
    }

    private void PlayEffects()
    {
        if (gunLight) gunLight.enabled = true;
        if (lineRenderer) lineRenderer.enabled = true;
        StartCoroutine(DisableEffectsAfter(effectsDisplayTime));
    }

    private void MaybeStartReload()
    {
        if (bullets <= 0)
            StartCoroutine(Reload());
    }


    private IEnumerator DisableEffectsAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gunLight) gunLight.enabled = false;
        if (lineRenderer) lineRenderer.enabled = false;
    }

    private IEnumerator Reload()
    {
        reloading = true;
        if (anim) anim.SetTrigger("Reload");
        if (reloadClip) audioSrc.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadTime);

        bullets = initialBullets;
        reloading = false;
    }

}
