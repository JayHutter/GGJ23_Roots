using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private Rigidbody rb;
    private Camera myCam;
    public CinemachineFreeLook virtualCam;
    private CinemachineImpulseSource impulse;
    public Transform camFollow;
    public Transform originalCamPos;
    public Transform shoulderCamPos;
    public ParticleSystem waterSpray;
    public Transform muzzle;
    public GameObject flashlight;
    public GameObject landFX;
    private float waterDrainRate = 0.1f;
    public WaterTank waterTank;
    private bool blankSFX = true;
    private float playerAimRotSpeed = 10f;
    private Vector3 shoulderCamVelocity;
    private float fovSpeed;
    private float normalFOV = 40f;
    private float aimFOV = 20f;
    private float shootSFXTimer = 0.0f;

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
    public float airControl = 0.25f;
    public float walkMult = 0.5f;
    public float crawlMult = 0.25f;
    public float diveForce = 10;
    public Transform diveAngle;
    public AnimationCurve accelerationFactorFromDot;
    public Vector3 forceScale = new Vector3(1.0f, 1.0f, 1.0f);

    private Vector3 m_UnitGoal = Vector3.zero;
    private Vector3 m_GoalVel = Vector3.zero;
    private Vector3 groundVel = Vector3.zero;

    public bool isGrounded = false;
    private bool justLanded = false;
    private bool justJumped = false;
    public float jumpForce = 10.0f;

    public float coyoteTime = 0.5f;
    private float coyoteCounter;
    public float jumpBufferLength = 0.1f;
    private float jumpBufferCount;
    private bool canControl = true;
    private bool isDiving = false;
    public float crouchColScale = 0.5f;
    public CapsuleCollider col;

    // inputs
    private Vector2 inputXZ;
    private bool isJumping;
    private bool isSprinting;
    private bool isCrouching;
    private bool isShooting;
    private bool isAimingDown;
    private bool isLighting;
    private bool isAttacking;

    [Header("Player Animations")]
    public FaceAnimator faceAnim;
    public Animator playerAnimator;

    [SerializeField] private bool enableHeadFollow = false;
    public GameObject head;
    [SerializeField] private float headSpeed = 1;

    public float pullbackForce = 10;

    public Rope tether;

    public GameObject meleeTrail;

    public int maxHealth = 5;
    public int health;

    private float aimBlend = 0;
    private float blendSpeed = 10;
    [SerializeField]
    private Rig aimRig;
    private float aimRigWeight;
    [SerializeField]
    private Transform aimIKTarget;
    [SerializeField]
    private LayerMask aimLayer;

    public int deaths = 0;
    public int carrots = 0;

    public CheckPoint currentCheckPoint;

    private void Start()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        myCam = Camera.main;
        _uprightJointTargetRot = transform.rotation;
        camFollow.position = originalCamPos.position;
        speedFactor = walkMult;
        impulse = GetComponent<CinemachineImpulseSource>();

        SubscribeInputs();
        health = maxHealth;
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, rayDist, ground, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * rayDist, Color.red);
            _rayHit = hit;
            _rayDidHit = true;

            if (hit.rigidbody != null)
                groundVel = hit.rigidbody.velocity;
        }
        else
        {
            groundVel = Vector3.zero;
            _rayDidHit = false;
        }

        isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - (rideHeight - 0.05f), transform.position.z), 0.1f, ground, QueryTriggerInteraction.Ignore);

        if (isGrounded)
        {
            justJumped = false;
            coyoteCounter = coyoteTime;
            if (!justLanded)
            {
                justLanded = true;
                AudioManager.instance.PlayOneShotWithParameters("Land", transform);
                Instantiate(landFX, hit.point, Quaternion.FromToRotation(landFX.transform.up, hit.normal));
                if (rb.velocity.y < -11.0f || isDiving)
                    Shake();
            }
        }
        else
        {
            justLanded = false;
            coyoteCounter -= Time.deltaTime;
        }

        if (isGrounded && isDiving)
        {
            isDiving = false;
            playerAnimator.SetBool("Diving", false);
        }

        Aim();

        if (canControl && !isDiving)
        {
            TargetMovement();
            Jumping();
            Lighting();
            Attack();
        }

        Shooting();
        AnimateMouth();
        UpdateAnimator();
        SetAimBlend();

        CheckTetherLength();
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

    void TargetMovement()
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
        Ray ray = myCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimLayer))
        {
            aimIKTarget.position = hit.point;
            Debug.Log(hit.transform.name);
        }

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
            waterSpray.transform.forward = myCam.transform.forward;
            aimRigWeight = 1f;
        }
        else
        {
            camFollow.position = Vector3.SmoothDamp(camFollow.position, originalCamPos.position, ref shoulderCamVelocity, 0.1f);
            virtualCam.m_Lens.FieldOfView = Mathf.SmoothDamp(virtualCam.m_Lens.FieldOfView, normalFOV, ref fovSpeed, 0.1f);
            waterSpray.transform.forward = transform.forward;
            aimRigWeight = 0f;
        }

        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);
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

        rb.AddForce(Vector3.Scale(neededAccel * rb.mass, forceScale * (isGrounded ? 1 : airControl)));
    }

    void Lighting()
    {
        if(isLighting)
        {
            flashlight.transform.forward = myCam.transform.forward;
            flashlight.SetActive(true);
        }
        else
        {
            flashlight.SetActive(false);
        }
    }

    void Shooting()
    {
        shootSFXTimer += Time.deltaTime;
        if(isShooting && waterTank.amount > 0)
        {
            waterTank.amount -= waterDrainRate * Time.deltaTime;
            waterSpray.transform.position = muzzle.transform.position;
            waterSpray.transform.rotation = muzzle.transform.rotation;

            if (shootSFXTimer >= 0.05f)
            {
                AudioManager.instance.PlayOneShotWithParameters("Shoot", transform);
                shootSFXTimer = 0.0f;
            }

            waterSpray.Play();
        }
        else
        {
            waterSpray.Stop();
            if(!blankSFX)
            {
                AudioManager.instance.PlayOneShotWithParameters("EmptyGun", transform);
                blankSFX = true;
            }
        }
    }

    void Jumping()
    {
        if (isJumping)
            jumpBufferCount = jumpBufferLength;
        else
            jumpBufferCount -= Time.deltaTime;

        if (coyoteCounter > 0.0f && jumpBufferCount >= 0.0f && !justJumped)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce * (isCrouching? 0.3f:1), rb.velocity.z);
            AudioManager.instance.PlayOneShotWithParameters("Jump", transform);
            jumpBufferCount = 0.0f;
            justJumped = true;
        }
    }

    void Shake()
    {
        impulse.m_DefaultVelocity *= Mathf.Clamp(Mathf.Abs(rb.velocity.y) / 15f, 0f, 5f);
        impulse.GenerateImpulse();
        impulse.m_DefaultVelocity = new Vector3(-1f, -1f, -1f);
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
        if (context.started)
        {
            if (isGrounded)
            {
                isCrouching = true;
                speedFactor = crawlMult;
                gameObject.layer = LayerMask.NameToLayer("PlayerCrouched");
            }
            else if (!isDiving)
            {
                isDiving = true;
                Vector3 vel = diveAngle.forward * diveForce;
                rb.velocity = vel;
                playerAnimator.SetBool("Diving", true);
                AudioManager.instance.PlayOneShotWithParameters("Dive", transform);
            }
        }


        if (context.canceled)
        {
            isCrouching = false;
            speedFactor = walkMult;
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        playerAnimator.SetBool("Crouched", isCrouching);
    }

    private void SprintInput(InputAction.CallbackContext context)
    {
        if (isCrouching)
            return;

        if (context.performed)
            speedFactor = 1;
        else if (context.canceled)
            speedFactor = walkMult;
    }

    private void ShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isShooting = true;
            blankSFX = false;
        }
        else if (context.canceled)
        {
            isShooting = false;
            blankSFX = true;
        }
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
        {
            StartCoroutine(AttackFor(0.25f));
            StartCoroutine(SpinVFX());
        }
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

        InputManager.onDebug1 += IncreaseRopeLengthTest;
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

        InputManager.onDebug1 -= IncreaseRopeLengthTest;
    }

    private void OnDestroy()
    {
        UnsubscribeInputs();
        instance = null;
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
        var vel = rb.velocity;
        Vector2 vel2D = new Vector2(vel.x, vel.z);
        float speed = Mathf.InverseLerp(0, maxSpeed, vel2D.magnitude);
        playerAnimator.SetFloat("Speed", speed);
    }

    private void CheckTetherLength()
    {
        if (!tether.IsWithinDistance(transform.position))
        {
            Vector3 dir = tether.GetDirectionTowardsEnd(transform.position);
            rb.velocity = dir * pullbackForce;
        }
    }

    private void IncreaseRopeLengthTest(InputAction.CallbackContext context)
    {
        if (context.started)
            tether.AddNode();
    }

    //public void AddTetherSegments(int segments)
    //{
    //    tether.AddNode();
    //
    //    //for (int i=0; i<segments; i++)
    //    //{
    //    //    tether.AddNode();
    //    //}
    //}

    public void AddTetherSegments(int segments)
    {
        StartCoroutine(AddTetherSegmentsWithDelay(segments));
    }

    public IEnumerator AddTetherSegmentsWithDelay(int segments)
    {
        for (int i=0; i<segments; i++)
        {
            tether.AddNode();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Attack()
    {
        if (isAttacking)
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 1.0f, transform.forward, out hit))
            {
                Debug.Log("Melee Hit");
            }
        }
    }

    private IEnumerator AttackFor(float time)
    {
        isAttacking = true;
        yield return new WaitForSeconds(time);
        isAttacking = false;
    }

    private IEnumerator SpinVFX()
    {
        meleeTrail.transform.localRotation = Quaternion.identity;
        meleeTrail.SetActive(true);
        Vector3 rot = meleeTrail.transform.eulerAngles;

        float spinSpeed = 2000;
        while(isAttacking)
        {
            rot.y -= Time.deltaTime * spinSpeed;
            meleeTrail.transform.eulerAngles = rot;
            yield return null;
        }

        meleeTrail.GetComponentInChildren<TrailRenderer>().Clear();

        meleeTrail.SetActive(false);
    }

    private void SetAimBlend()
    {
        float target = (isShooting || isAimingDown ? 1 : 0);
        aimBlend = Mathf.MoveTowards(aimBlend, target, Time.deltaTime * blendSpeed);
        playerAnimator.SetLayerWeight(1, aimBlend);
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Death();
            deaths++;
        }
    }

    private void Death()
    {
        transform.position = currentCheckPoint.location.position;
        transform.rotation = currentCheckPoint.location.rotation;
        health = 5;
    }
}
