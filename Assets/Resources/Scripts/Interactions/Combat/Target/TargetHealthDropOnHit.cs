using System.Collections;
using TMPro;
using UnityEngine;

public class TargetHealthDropOnHit : MonoBehaviour, IWeaponHitReceiver
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 5f;

    [Tooltip("Kalau ON, setiap kena peluru damage-nya tetap sesuai Damage Per Hit.")]
    [SerializeField] private bool useFixedDamagePerHit = true;

    [SerializeField] private float damagePerHit = 1f;

    [Header("UI HP")]
    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private bool showAsInteger = true;

    [Header("Drop Motion")]
    [SerializeField] private Transform dropRoot;
    [SerializeField] private Vector3 standingLocalEuler = Vector3.zero;
    [SerializeField] private Vector3 downLocalEuler = new Vector3(-90f, 0f, 0f);
    [SerializeField] private float dropDuration = 0.25f;

    [Header("Reset")]
    [SerializeField] private bool autoReset = true;
    [SerializeField] private float resetDelay = 3f;

    [Header("Debug")]
    [SerializeField] private bool debugLog = true;

    private float currentHealth;
    private bool isDown;
    private Coroutine motionRoutine;

    private void Awake()
    {
        if (dropRoot == null)
            dropRoot = transform;

        currentHealth = maxHealth;
        UpdateHpText();
        dropRoot.localRotation = Quaternion.Euler(standingLocalEuler);
    }

    private void OnEnable()
    {
        ResetTargetInstant();
    }

    public void ReceiveWeaponHit(WeaponHitInfo hitInfo)
    {
        if (isDown)
            return;

        float damage = useFixedDamagePerHit
            ? damagePerHit
            : Mathf.Max(0f, hitInfo.damage);

        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        UpdateHpText();

        Debug.Log(
            "[TargetHealthDropOnHit] Hit damage: " +
            damage.ToString("0.0") +
            " | HP: " +
            currentHealth.ToString("0.0") +
            "/" +
            maxHealth.ToString("0.0"),
            this
        );

        if (currentHealth <= 0f)
        {
            DropTarget();
        }
    }

    private void DropTarget()
    {
        isDown = true;

        if (motionRoutine != null)
            StopCoroutine(motionRoutine);

        motionRoutine = StartCoroutine(RotateTarget(
            Quaternion.Euler(downLocalEuler)
        ));

        if (autoReset)
            StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(resetDelay);

        ResetTargetInstant();

        if (motionRoutine != null)
            StopCoroutine(motionRoutine);

        motionRoutine = StartCoroutine(RotateTarget(
            Quaternion.Euler(standingLocalEuler)
        ));
    }

    private IEnumerator RotateTarget(Quaternion targetRotation)
    {
        Quaternion startRotation = dropRoot.localRotation;
        float timer = 0f;

        while (timer < dropDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / dropDuration);

            dropRoot.localRotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                t
            );

            yield return null;
        }

        dropRoot.localRotation = targetRotation;
    }

    private void ResetTargetInstant()
    {
        currentHealth = maxHealth;
        isDown = false;

        if (dropRoot == null)
            dropRoot = transform;

        dropRoot.localRotation = Quaternion.Euler(standingLocalEuler);
        UpdateHpText();
    }

    private void UpdateHpText()
    {
        if (hpText == null)
            return;

        if (showAsInteger)
            hpText.text = Mathf.CeilToInt(currentHealth).ToString();
        else
            hpText.text = currentHealth.ToString("0.0");
    }
}