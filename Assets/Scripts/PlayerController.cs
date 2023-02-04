using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.iOS;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Camera myCam;
    public CinemachineFreeLook virtualCam;
    public Transform camFollow;
    public Transform originalCamPos;
    public Transform shoulderCamPos;
    private float playerAimRotSpeed = 10f;
    private Vector3 shoulderCamVelocity;
    private float fovSpeed;
    private float normalFOV = 40f;
    private float aimFOV = 20f;

    public float rayDist = 2.5f;
    public bool _rayDidHit = false;
    public LayerMask ground;
    private RaycastHit _rayHit;
    public float rideHeight = 2.5f;

    public float rideSpringStrength = 10.0f;
    public float rideSpringDamper = 2.0f;
    public float _uprightJointSpringStrength = 10.0f;
    public float _uprightJointSpringDamper = 2.0f;

    private Quaternion _uprightJointTargetRot = Quaternion.identity;

    public float maxSpeed = 10.0f;
    public float speedFactor = 1.0f;
    public float acceleration = 4.0f;
    public float maxAccelForce = 10.0f;
    public float maxAccelForceFactor = 1.0f;
    public AnimationCurve accelerationFactorFromDot;
    public Vector3 forceScale = new Vector3(1.0f, 1.0f, 1.0f);

    private Vector3 m_UnitGoal = Vector3.zero;
    private Vector3 m_GoalVel = Vector3.zero;
    private Vector3 groundVel = Vector3.zero;

    public bool isGrounded = false;
    public float jumpForce = 100.0f;

    public float coyoteTime = 0.2f;
    private float coyoteCounter;
    public float jumpBufferLength = 0.1f;
    private float jumpBufferCount;

    // inputs
    private Vector2 inputXZ;
    private bool isJumping;
    private bool isSprinting;
    private bool isCrouching;
    private bool isShooting;
    private bool isAimingDown;
    private bool isLighting;
    private bool melee;

    [Header("Player Animations")]
    public FaceAnimator faceAnim;
    public Animator playerAnimator;

    [SerializeField] private bool enableHeadFollow = false;
    public GameObject head;
    [SerializeField] private float headSpeed = 1;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        myCam = Camera.main;
        _uprightJointTargetRot = transform.rotation;
        camFollow.position = originalCamPos.position;

        SubscribeInputs();
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, rayDist, ground))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * rayDist, Color.red);
            _rayHit = hit;
            _rayDidHit = true;
            if (hit.rigidbody != null)
            {
                groundVel = hit.rigidbody.velocity;
            }
        }
        else
        {
            groundVel = Vector3.zero;
            _rayDidHit = false;
        }

        isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - (rideHeight - 0.05f), transform.position.z), 0.1f, ground);

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        Movement();
        Aim();
        Jumping();
        AnimateMouth();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (_rayDidHit)
        {
            Vector3 vel = rb.velocity;
            Vector3 rayDir = transform.TransformDirection(Vector3.down);

            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = _rayHit.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.velocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;

            float x = _rayHit.distance - rideHeight;

            float springForce = (x * rideSpringStrength) - (relVel * rideSpringDamper);

            rb.AddForce(rayDir * springForce);

            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(rayDir * -springForce, _rayHit.point);
            }
        }
        GroundMovement();
        UpdateUprightForce();
    }

    private void LateUpdate()
    {
        if (enableHeadFollow)
            AimHead();
    }

    public static Quaternion ShortestRotation(Quaternion to, Quaternion from)
    {
        if (Quaternion.Dot(to, from) < 0)
        {
            return to * Quaternion.Inverse(Multiply(from, -1));
        }

        else return to * Quaternion.Inverse(from);
    }

    public static Quaternion Multiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    void UpdateUprightForce()
    {
        Quaternion characterCurrent = transform.rotation;
        Quaternion toGoal = ShortestRotation(_uprightJointTargetRot, characterCurrent);

        Vector3 rotAxis;
        float rotDegrees;

        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        rb.AddTorque((new Vector3(rotAxis.x, 0.0f, rotAxis.z) * (rotRadians * _uprightJointSpringStrength)) - (rb.angularVelocity * _uprightJointSpringDamper));
    }

    void Movement()
    {
        if (myCam == null)
            return;

        Vector3 move = new Vector3(inputXZ.x, 0, inputXZ.y);

        move = myCam.transform.TransformDirection(move);

        if (move.sqrMagnitude > 1.0f)
            move.Normalize();

        float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + myCam.transform.rotation.y;
        Quaternion rotation = Quaternion.Euler(transform.rotation.x, targetAngle, transform.rotation.z);

        if (move.sqrMagnitude > 0.0f && !isAimingDown)
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10.0f);

        m_UnitGoal = move;
    }

    void Aim()
    {
        if (camFollow == null || myCam == null || shoulderCamPos == null || originalCamPos == null || virtualCam == null)
        {
            Debug.LogWarning("One of the camera transforms / virtual camera is not assigned");
            return;
        }

        camFollow.eulerAngles = new Vector3(camFollow.eulerAngles.x, myCam.transform.eulerAngles.y, camFollow.eulerAngles.z);

        if (isAimingDown)
        {
            camFollow.position = Vector3.SmoothDamp(camFollow.position, shoulderCamPos.position, ref shoulderCamVelocity, 0.1f);
            transform.rotation = Quaternion.Slerp(transform.rotation, camFollow.rotation, Time.deltaTime * playerAimRotSpeed);
            virtualCam.m_Lens.FieldOfView = Mathf.SmoothDamp(virtualCam.m_Lens.FieldOfView, aimFOV, ref fovSpeed, 0.1f);
        }
        else
        {
            camFollow.position = Vector3.SmoothDamp(camFollow.position, originalCamPos.position, ref shoulderCamVelocity, 0.1f);
            virtualCam.m_Lens.FieldOfView = Mathf.SmoothDamp(virtualCam.m_Lens.FieldOfView, normalFOV, ref fovSpeed, 0.1f);
        }
    }

    void GroundMovement()
    {
        Vector3 unitVel = m_GoalVel.normalized;
        float velDot = Vector3.Dot(m_UnitGoal, unitVel);
        float accel = acceleration * accelerationFactorFromDot.Evaluate(velDot);

        Vector3 goalVel = m_UnitGoal * maxSpeed * speedFactor;
        m_GoalVel = Vector3.MoveTowards(m_GoalVel, (goalVel) + (groundVel), accel * Time.fixedDeltaTime);

        Vector3 neededAccel = (m_GoalVel - rb.velocity) / Time.fixedDeltaTime;
        float maxAccel = maxAccelForce * maxAccelForceFactor;
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        rb.AddForce(Vector3.Scale(neededAccel * rb.mass, forceScale));
    }

    void Jumping()
    {
        if (isJumping)
            jumpBufferCount = jumpBufferLength;
        else
            jumpBufferCount -= Time.deltaTime;

        if (coyoteCounter > 0.0f && jumpBufferCount >= 0.0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            jumpBufferCount = 0.0f;
        }

        if (isJumping && rb.velocity.y > 0.0f)
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z);
    }

    private void MoveInput(InputAction.CallbackContext context)
    {
        inputXZ = context.ReadValue<Vector2>();
    }

    private void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            isJumping = true;
        else if (context.canceled)
            isJumping = false;
    }

    private void ADSInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            isAimingDown = true;
        else if (context.canceled)
            isAimingDown = false;
    }

    private void CrouchInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            isCrouching = true;
        else if (context.canceled)
            isCrouching = false;
    }

    private void SprintInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            isSprinting = true;
        else if (context.canceled)
            isSprinting = false;
    }

    private void ShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            isShooting = true;
        else if (context.canceled)
            isShooting = false;
    }

    private void LightInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            isLighting = true;
        else if (context.canceled)
            isLighting = false;
    }

    private void MeleeInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            melee = true;
        else if (context.canceled)
            melee = false;
    }

    private void SubscribeInputs()
    {
        InputManager.onMove += MoveInput;
        InputManager.onJump += JumpInput;
        InputManager.onAimSight += ADSInput;
        InputManager.onCrouch += CrouchInput;
        InputManager.onSprint += SprintInput;
        InputManager.onShoot += ShootInput;
        InputManager.onLight += LightInput;
        InputManager.onMelee += MeleeInput;
    }

    private void UnsubscribeInputs()
    {
        InputManager.onMove -= MoveInput;
        InputManager.onJump -= JumpInput;
        InputManager.onAimSight -= ADSInput;
        InputManager.onCrouch -= CrouchInput;
        InputManager.onSprint -= SprintInput;
        InputManager.onShoot -= ShootInput;
        InputManager.onLight -= LightInput;
        InputManager.onMelee -= MeleeInput;
    }

    private void OnDestroy()
    {
        UnsubscribeInputs();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - (rideHeight - 0.05f), transform.position.z), 0.1f);
    }

    private void AimHead()
    {
        var lookRot = Quaternion.LookRotation(myCam.transform.position - head.transform.position);
        head.transform.rotation = lookRot;
    }

    private void AnimateMouth()
    {
        if (isGrounded)
        {
            faceAnim.SetAnimationState(FaceAnimator.State.NORMAL);
        }
        else
        {
            faceAnim.SetAnimationState(FaceAnimator.State.OPEN);
        }
    }

    private void UpdateAnimator()
    {
        float speed = Mathf.InverseLerp(0, maxSpeed, rb.velocity.magnitude);
        playerAnimator.SetFloat("Speed", speed);
    }
}
