using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [System.Serializable]
    private class MouseSettings
    {
        public float sensitivity = 1f;
        public float minRotationY = -90f;
        public float maxRotationY = 90f;
    }

    [System.Serializable]
    private class MovementSettings
    {
        public float runSpeed = 4f;
        public float sprintMultiplier = 2f;
        public float airSpeed = 1f;
        public float jumpSpeed = 7.5f;
        public float maxAirSpeed = 3f;
        public float runAcceleration = 10f;
        public float airAcceleration = 100f;
        public float friction = 10f;
    }

    [System.Serializable]
    private class PhysicsSettings
    {
        public float gravity = 20f;
    }

    [SerializeField]
    private MouseSettings mouseSettings = new MouseSettings();

    [SerializeField]
    private MovementSettings movementSettings = new MovementSettings();

    [SerializeField]
    private PhysicsSettings physicsSettings = new PhysicsSettings();

    [HideInInspector]
    public CharacterController controller;

    [HideInInspector]
    public float mouseX = 0f;
    [HideInInspector]
    public float mouseY = 0f;
    private bool cursorIsLocked = false;

    private Transform cameraTransform;
    private Vector3 velocity = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;

    private float forwardMove = 0f;
    private float rightMove = 0f;
    private float runSpeed = 0f;
    private bool wishJump = false;

    private float gravity;

    void OnEnable()
    {
        EventManager.Instance.AddListener<OpenInventoryEvent>(OnOpenInventory);
        EventManager.Instance.AddListener<CloseInventoryEvent>(OnCloseInventory);
    }

    void OnDisable()
    {
        EventManager.Instance.RemoveListener<OpenInventoryEvent>(OnOpenInventory);
        EventManager.Instance.RemoveListener<CloseInventoryEvent>(OnCloseInventory);
    }

    // Use this for initialization
    void Start()
    {
        cameraTransform = GetComponentInChildren<Camera>().transform;
        if (!cameraTransform) Debug.Log("Needs Camera as child object");
        controller = GetComponent<CharacterController>();
        gravity = physicsSettings.gravity;
        runSpeed = movementSettings.runSpeed;
        LockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseInput();
        UpdateMovementInput();
        SetMovementDirection();
        QueueJump();

        if (controller.isGrounded)
        {
            GroundMove();
        }
        else
        {
            AirMove();
        }

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // LateUpdate is called once per frame after the regular Update
    void LateUpdate()
    {
        UpdateCursorLock();
        RotatePlayer();
        RotateCamera();
    }

    void UpdateMouseInput()
    {
        if (cursorIsLocked)
        {
            mouseX += Input.GetAxis("Mouse X") * mouseSettings.sensitivity;
            mouseY += Input.GetAxis("Mouse Y") * mouseSettings.sensitivity;
            mouseY = ClampAngle(mouseY, mouseSettings.minRotationY, mouseSettings.maxRotationY);
        }
    }

    void RotatePlayer()
    {
        if (Input.GetKey(KeyCode.LeftAlt)) return;
        Quaternion rotation = Quaternion.Euler(0, mouseX, 0);
        transform.rotation = rotation;
    }

    void RotateCamera()
    {
        Quaternion rotation = Quaternion.Euler(-mouseY, mouseX, 0);
        cameraTransform.rotation = rotation;
    }

    void UpdateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!cursorIsLocked)
            {
                LockCursor();
            }
            else
            {
                UnlockCursor();
            }
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cursorIsLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        cursorIsLocked = false;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    void QueueJump()
    {
        if (InputManager.GetKeyDown(Actions.JUMP) && controller.isGrounded)
        {
            wishJump = true;
        }
        if (InputManager.GetKeyUp(Actions.JUMP))
        {
            wishJump = false;
        }
    }

    void UpdateMovementInput()
    {
        rightMove = 0f;
        if (InputManager.GetKey(Actions.RIGHT)) rightMove += 1f;
        if (InputManager.GetKey(Actions.LEFT)) rightMove -= 1f;

        forwardMove = 0f;
        if (InputManager.GetKey(Actions.FORWARD)) forwardMove += 1f;
        if (InputManager.GetKey(Actions.BACK)) forwardMove -= 1f;

        if (InputManager.GetKeyDown(Actions.SPRINT))
        {
            runSpeed = movementSettings.runSpeed * movementSettings.sprintMultiplier;
        }
        if (InputManager.GetKeyUp(Actions.SPRINT))
        {
            runSpeed = movementSettings.runSpeed;
        }
    }

    void SetMovementDirection()
    {
        moveDirection = new Vector3(rightMove, 0f, forwardMove);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection.Normalize();
    }

    void GroundMove()
    {
        if (!wishJump)
        {
            ApplyFriction();
        }

        float wishSpeed = moveDirection.magnitude;
        float newSpeed = wishSpeed * runSpeed;

        Accelerate(moveDirection, newSpeed, movementSettings.runAcceleration);

        if (wishJump)
        {
            velocity.y = 0;
            velocity.y += movementSettings.jumpSpeed;
            wishJump = false;
        }
    }

    void AirMove()
    {
        float wishSpeed = moveDirection.magnitude;
        float newSpeed = wishSpeed * movementSettings.airSpeed;

        if (newSpeed > movementSettings.maxAirSpeed)
        {
            newSpeed = movementSettings.maxAirSpeed;
        }

        Accelerate(moveDirection, newSpeed, movementSettings.airAcceleration);
        AirControl(moveDirection, wishSpeed);
    }

    private void AirControl(Vector3 wishdir, float wishspeed)
    {
        float zspeed;
        float speed;
        float dot;
        float k;

        // Can't control movement if not moving forward or backward
        if (Mathf.Abs(forwardMove) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
            return;
        zspeed = velocity.y;
        velocity.y = 0;
        /* Next two lines are equivalent to idTech's VectorNormalize() */
        speed = velocity.magnitude;
        velocity.Normalize();

        dot = Vector3.Dot(velocity, wishdir);
        k = 32;
        k *= 1f * dot * dot * Time.deltaTime;

        // Change direction while slowing down
        if (dot > 0)
        {
            velocity.x = velocity.x * speed + wishdir.x * k;
            velocity.y = velocity.y * speed + wishdir.y * k;
            velocity.z = velocity.z * speed + wishdir.z * k;

            velocity.Normalize();
        }

        velocity.x *= speed;
        velocity.y = zspeed;
        velocity.z *= speed;
    }

    void Accelerate(Vector3 direction, float newSpeed, float acceleration)
    {
        float currentSpeed;
        float addSpeed;
        float accelSpeed;

        currentSpeed = Vector3.Dot(velocity, direction);

        addSpeed = newSpeed - currentSpeed;

        if (addSpeed <= 0) return;

        accelSpeed = acceleration * newSpeed;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        velocity.x += accelSpeed * direction.x;
        velocity.z += accelSpeed * direction.z;
    }

    void ApplyFriction()
    {
        float currentSpeed = velocity.magnitude;

        // Apply friction
        if (currentSpeed != 0) // To avoid divide by zero errors
        {
            float drop = currentSpeed * movementSettings.friction * Time.deltaTime;
            velocity *= Mathf.Max(currentSpeed - drop, 0) / currentSpeed; // Scale the velocity based on friction.
        }
    }

    void OnOpenInventory(OpenInventoryEvent e)
    {
        UnlockCursor();
    }

    void OnCloseInventory(CloseInventoryEvent e)
    {
        LockCursor();
    }
}
