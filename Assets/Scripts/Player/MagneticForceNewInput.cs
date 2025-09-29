using UnityEngine;
using UnityEngine.InputSystem;

public class MagneticForceNewInput : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float radius = 5f;
    public float maxForce = 20f;
    public float maxTimer = 3f;
    public float forceMultiplier = 10f;

    private float attractTimer = 0f;
    private float repelTimer = 0f;

    private PlayerControls controls;
    private bool isAttracting = false;
    private bool isRepelling = false;

    private void Awake()
    {
        controls = new PlayerControls();

        // Input Events 등록
        controls.Player.Attract.started += _ => isAttracting = true;
        controls.Player.Attract.canceled += _ => { isAttracting = false; attractTimer = 0f; };

        controls.Player.Repel.started += _ => isRepelling = true;
        controls.Player.Repel.canceled += _ => { isRepelling = false; repelTimer = 0f; };
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        // ✅ 척력이 우선
        if (isRepelling)
        {
            repelTimer += Time.deltaTime;
            repelTimer = Mathf.Min(repelTimer, maxTimer);

            ApplyForce(isAttract: false, repelTimer);
        }
        else if (isAttracting)
        {
            attractTimer += Time.deltaTime;
            attractTimer = Mathf.Min(attractTimer, maxTimer);

            ApplyForce(isAttract: true, attractTimer);
        }
    }

    private void ApplyForce(bool isAttract, float timer)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.attachedRigidbody != null && hit.gameObject != gameObject)
            {
                Vector2 direction = (hit.transform.position - transform.position).normalized;
                if (isAttract) direction = -direction; // 인력: 나에게 당김

                float strength = Mathf.Lerp(0, maxForce, timer / maxTimer);
                hit.attachedRigidbody.AddForce(direction * strength * forceMultiplier * Time.deltaTime, ForceMode2D.Force);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}