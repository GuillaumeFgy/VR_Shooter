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

        // Consume ammo immediately to prevent over-firing
        bullets--;

        // Play sound/anim
        if (shootClip) audioSrc.PlayOneShot(shootClip);
        if (anim) anim.SetTrigger("Shoot");

        // Decide ray origin & direction
        Transform originT = refNozzle ? refNozzle.transform : (cam ? cam.transform : transform);
        Vector3 origin = originT.position;
        Vector3 direction = originT.forward;

        // Raycast
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore))
        {
            // Draw tracer
            if (lineRenderer)
            {
                lineRenderer.SetPosition(0, origin);
                lineRenderer.SetPosition(1, hit.point);
            }

            // Damage enemy (prefer CompareTag for perf/safety)
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                var eh = hit.collider.GetComponentInParent<EnemyHealth>();
                if (eh != null) 
                {
                    eh.TakeDamage(damagePerShot);
                }
                else
                {
                    Debug.LogError("NO ENEMY HEALTH");
                }
                   
            }
        }
        else
        {
            Debug.LogError("NO HIT");
            // Draw tracer straight ahead to max distance
            if (lineRenderer)
            {
                lineRenderer.SetPosition(0, origin);
                lineRenderer.SetPosition(1, origin + direction * maxDistance);
            }
        }

        // Fire FX
        if (gunLight) gunLight.enabled = true;
        if (lineRenderer) lineRenderer.enabled = true;
        StartCoroutine(DisableEffectsAfter(effectsDisplayTime));

        // Reload if we just spent the last round
        if (bullets <= 0)
        {
            StartCoroutine(Reload());
        }
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
