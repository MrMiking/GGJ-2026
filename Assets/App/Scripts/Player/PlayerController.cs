using GGJ2026;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CharacterStats))]
public class PlayerController : RegularSingleton<PlayerController>
{
    [Header("References")]
    [SerializeField] Transform attackPoint;
    [SerializeField] Rigidbody2D bulletPrefabRb;
    [SerializeField] Transform m_AimTarget;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveIA;
    [SerializeField] private InputActionReference dashIA;
    [SerializeField] private InputActionReference aimInputRef;

    [Header("Player Settings")]
    public float dashForce = 50f;

    [Header("Shoot Settings")]
    public float bulletLifetime = 2f;

    private Vector2 m_AimInput;
    private Vector2 m_MoveInput;
    private float m_LastGamepadInputTime = float.NegativeInfinity;

    private CharacterStats m_CharacterStats;
    private Rigidbody2D m_Rigidbody;

    private Vector2 m_TargetVelocity = Vector2.zero;

    public bool canShoot = true;

    protected override void Awake()
    {
        base.Awake();
        m_CharacterStats = GetComponent<CharacterStats>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        BindInputs();
    }

    private void OnDestroy()
    {
        UnbindInputs();
    }

    private void OnEnable()
    {
        dashIA.action.performed += OnDash;
    }

    private void OnDisable()
    {
        dashIA.action.performed -= OnDash;
    }

    private void Start()
    {
        StartCoroutine(ShootCooldown());
    }

    public void AddForce(Vector2 force)
    {
        m_Rigidbody.linearVelocity += force;
    }

    private void Update()
    {
        // Compute the target velocity
        if (m_MoveInput != Vector2.zero)
        {
            var characterSpeed = m_CharacterStats.MovementSpeed.Value;
            m_TargetVelocity = m_MoveInput * characterSpeed;
        }
        else
        {
            m_TargetVelocity = Vector2.zero;
        }

        // Make the acceleration of the player bound to it's movement speed stat.
        var acceleration = 2.0f * m_CharacterStats.MovementSpeed.Value;
        var maxVelocityChange = acceleration * Time.deltaTime;
        m_Rigidbody.linearVelocity = Vector3.MoveTowards(
            m_Rigidbody.linearVelocity, 
            m_TargetVelocity, 
            maxVelocityChange
        );

        if (m_AimInput != Vector2.zero)
        {
            var maxAngleChance = 1080f * Mathf.Deg2Rad * Time.deltaTime;
            m_AimTarget.up = Vector3.RotateTowards(m_AimTarget.up, m_AimInput, maxAngleChance, 0.0f);
        }
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(1.0f / m_CharacterStats.FireRate.Value);
        yield return new WaitUntil(() => canShoot == true);
        Shoot();
    }

    private void Shoot()
    {
        StartCoroutine(ShootCooldown());
        Rigidbody2D rb = Instantiate(bulletPrefabRb, attackPoint.position, attackPoint.rotation);
        rb.AddForce(rb.transform.up * m_CharacterStats.BulletSpeed.Value, ForceMode2D.Impulse);
        Destroy(rb.gameObject, bulletLifetime);
    }

    private void Dash()
    {
        m_Rigidbody.linearVelocity = m_MoveInput * dashForce;
    }

    #region Inputs

    private void BindInputs()
    {
        moveIA.action.Enable();
        aimInputRef.action.Enable();
        dashIA.action.Enable();

        moveIA.action.performed += OnMove;
        moveIA.action.started += OnMove;
        moveIA.action.canceled += OnMove;
        aimInputRef.action.performed += OnAim;
        aimInputRef.action.started += OnAim;
        aimInputRef.action.canceled += OnAim;
    }

    private void UnbindInputs()
    {
        moveIA.action.performed -= OnMove;
        moveIA.action.started -= OnMove;
        moveIA.action.canceled -= OnMove;
        aimInputRef.action.performed -= OnAim;
        aimInputRef.action.started -= OnAim;
        aimInputRef.action.canceled -= OnAim;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // Should be normalized in the InputAction asset
        m_MoveInput = context.ReadValue<Vector2>();
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        if (context.control.device is Gamepad)
        {
            m_AimInput = context.ReadValue<Vector2>();
            m_LastGamepadInputTime = Time.time;
        }
        else if (context.control.device is Mouse)
        {
            // Wait at least 1 second before giving control back to mouse input.
            if (Time.time - m_LastGamepadInputTime < 0.75f)
                return;

            var mouseScreenPos = context.ReadValue<Vector2>();
            var mouseWorldPos = (Vector2) Camera.main.ScreenToWorldPoint(mouseScreenPos);
            var playerPos = (Vector2) transform.position;
            m_AimInput = (mouseWorldPos - playerPos).normalized;
        }
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        Dash();
    }
    #endregion
}