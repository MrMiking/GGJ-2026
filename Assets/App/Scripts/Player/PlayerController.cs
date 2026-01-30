using GGJ2026;
using MVsToolkit.Utilities;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterStats))]
public class PlayerController : RegularSingleton<PlayerController>
{
    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform attackPoint;
    [SerializeField] Rigidbody2D bulletPrefabRb;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveIA;
    [SerializeField] private InputActionReference dashIA;
    [SerializeField] private InputActionReference mousePointerIA;

    [Header("Player Settings")]
    public float health = 100f;
    public float dashForce = 50f;

    [Header("Shoot Settings")]
    public float bulletLifetime = 2f;

    private Vector2 mousePointer;
    private Vector2 moveInput;

    private CharacterStats characterStats;

    public bool canShoot = true;

    protected override void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
        base.Awake();
        moveIA.action.Enable();
        mousePointerIA.action.Enable();
        dashIA.action.Enable();
    }

    private void OnEnable()
    {
        dashIA.action.performed += Dash;
    }

    private void OnDisable()
    {
        dashIA.action.performed -= Dash;
    }

    private void Start()
    {
        StartCoroutine(ShootCooldown());
    }

    private void Update()
    {
        moveInput = moveIA.action.ReadValue<Vector2>().normalized;
        rb.AddForce(moveInput * characterStats.MovementSpeed.Value);

        mousePointer = mousePointerIA.action.ReadValue<Vector2>();

        Rotate();
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(1.0f / characterStats.FireRate.Value);
        yield return new WaitUntil(() => canShoot == true);
        Shoot();
    }

    private void Shoot()
    {
        StartCoroutine(ShootCooldown());
        Rigidbody2D rb = Instantiate(bulletPrefabRb, attackPoint.position, attackPoint.rotation);
        rb.AddForce(rb.transform.up * characterStats.BulletSpeed.Value, ForceMode2D.Impulse);
        Destroy(rb.gameObject, bulletLifetime);
    }

    private void Dash(InputAction.CallbackContext ctx)
    {
        Debug.Log("Dash!");
        rb.AddForce(moveInput * dashForce, ForceMode2D.Impulse);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Handle player death
        }
    }

    void Rotate()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(mousePointer);
        Vector2 lookDir = mousePos - rb.position;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}