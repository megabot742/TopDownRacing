using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Setting")]
    [SerializeField] int carIndex = 0; // Index để lấy CarSO từ CarDatabase
    [SerializeField] CarDatabase carDatabase;

    // [SerializeField] float driftFactor = 0.95f;
    // [SerializeField] float accelerationFactor = 30f; //default 30f
    // [SerializeField] float turnFactor = 3.5f; //default 3.5f 
    // [SerializeField] public float maxSpeed = 20f;

    private float driftFactor;
    private float accelerationFactor;
    private float turnFactor;
    public float maxSpeed;

    [Header("Sprite")]
    [SerializeField] SpriteRenderer carRenderer;
    [SerializeField] SpriteRenderer carShadowRenderer;

    [Header("Jumping")]
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] ParticleSystem landingEffect;
    //[SerializeField] ParticleSystem landingParticleSystem;

    private float accelerationInput = 0;
    private float steeringInput = 0;
    private float rotationAngle = 0;
    private float velocityVsUp = 0;

    private bool isJumping = false;

    [SerializeField] public int currentLap = 1; //default = 1
    [SerializeField] float lapTime;
    [SerializeField] float bestLapTime;
    [SerializeField] public int nextCheckPoint;


    //Component
    public Rigidbody2D carRb2D;
    CapsuleCollider2D capsuleCollider2D;
    CarSFXHandler carSFXHandler;
    CarInputHandler carInputHandler;

    void Awake()
    {
        carRb2D = GetComponent<Rigidbody2D>();
        carSFXHandler = GetComponent<CarSFXHandler>();
        capsuleCollider2D = GetComponentInChildren<CapsuleCollider2D>();
        carInputHandler = GetComponent<CarInputHandler>();

        InitializeCarSettings();
    }
    void Start()
    {
        rotationAngle = transform.rotation.eulerAngles.z;
        if (!carInputHandler.isAICar)
        {
            DisplayLap();
            DisplayTime();
            DisplaySpeed();
        }
    }
    void Update()
    {
        if (!RaceController.Instance.isStarting)
        {
            lapTime += Time.deltaTime;
            if (!carInputHandler.isAICar)
            {
                DisplayTime();
                DisplaySpeed();
            }
        }

    }
    void FixedUpdate()
    {
        ApplyEngineForce();
        KillOrThogonalVelocity();
        ApplySteering();

    }
    void InitializeCarSettings()
    {
        if (carDatabase == null)
        {
            Debug.LogError("CarDatabase is not assigned in the Inspector!");
            return;
        }

        if (carIndex < 0 || carIndex >= carDatabase.CarCount)
        {   
            SetDefaultSettings();
            return;
        }

        CarSO carSO = carDatabase.GetCarSO(carIndex);
        if (carSO != null)
        {
            driftFactor = carSO.driftFactor;
            accelerationFactor = carSO.accelerationFactor;
            turnFactor = carSO.turnFactor;
            maxSpeed = carSO.maxSpeed;
            //Debug.Log($"Car {carIndex} ({carSO.carName}) loaded: driftFactor={driftFactor}, accelerationFactor={accelerationFactor}, turnFactor={turnFactor}, maxSpeed={maxSpeed}");
        }
        else
        {
            //Debug.LogError($"No CarSO found at index: {carIndex}. Using default values.");
            SetDefaultSettings();
        }
    }
    private void SetDefaultSettings()
    {
        driftFactor = 0.95f;
        accelerationFactor = 30f;
        turnFactor = 3.5f;
        maxSpeed = 20f;
    }
    void ApplyEngineForce()
    {
        //Don't let the player brake while in the air, but we still allow some drag so it acan be slowed slightly
        if (isJumping && accelerationInput < 0)
        {
            accelerationInput = 0;
        }
        //Unity 6: drag = linearDamping, apply drag if there is no accelerationInput so the car stops when the player lets go of the accelerator
        if (accelerationInput == 0)
        {
            carRb2D.linearDamping = Mathf.Lerp(carRb2D.linearDamping, 3.0f, Time.fixedDeltaTime * 3);
        }
        else
        {
            carRb2D.linearDamping = 0;
        }

        //Caculate how muck "forward" we are going in terms of the direction of out velocity
        velocityVsUp = Vector2.Dot(transform.up, carRb2D.linearVelocity);

        //Limit spped forward direction
        if (velocityVsUp > maxSpeed && accelerationInput > 0) return;

        //Limit speed reverse direction with 50% max speed
        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0) return;


        //Limit speed cannot go faster in any direction while accelerating
        if (carRb2D.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0 && !isJumping) return;

        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor; //Create a force
        carRb2D.AddForce(engineForceVector, ForceMode2D.Force); //Apply force for car
    }
    void ApplySteering()
    {
        float minSpeedBeforeAllowTurningFactor = carRb2D.linearVelocity.magnitude / 2; //Limit the cars ability to turn when moving slowly
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor; //Update rotation angle base on input
        carRb2D.MoveRotation(rotationAngle); //Apply steering by rotating the car object
    }
    void KillOrThogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRb2D.linearVelocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRb2D.linearVelocity, transform.right);

        carRb2D.linearVelocity = forwardVelocity + rightVelocity * driftFactor;
    }
    float GetSidewaysVelocity()
    {
        //Return how fast car is moving sideways
        return Vector2.Dot(transform.right, carRb2D.linearVelocity);
    }
    public void SetAISetup(bool isAI)
    {
        if (carInputHandler != null)
        {
            carInputHandler.isAICar = isAI;
        }
    }
    public bool IsTireScreeching(out float sidewaysVelocity, out bool isBraking)
    {
        sidewaysVelocity = GetSidewaysVelocity();
        isBraking = false;

        if (isJumping) return false;

        //Check if we are moving forward and if the player is hitting the brakes. In that case the tires should screech.
        if (accelerationInput < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }

        //If we have a lot of side movenment then the tires should be screenching
        if (Mathf.Abs(GetSidewaysVelocity()) > 4.0f)
        {
            return true;
        }

        return false;
    }
    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }
    public float GetVelocityMagnitude()
    {
        return carRb2D.linearVelocity.magnitude;
    }
    public bool GetJumping()
    {
        return isJumping;
    }
    public void Jump(float jumpHeightScale, float jumpPushScale)
    {
        if (!isJumping)
        {
            StartCoroutine(JumpCO(jumpHeightScale, jumpPushScale));
        }
    }

    IEnumerator JumpCO(float jumpHeightScale, float jumpPushScale)
    {
        isJumping = true;



        float jumpStarTime = Time.time;
        float jumpDuration = carRb2D.linearVelocity.magnitude * 0.05f;

        jumpHeightScale = jumpHeightScale * carRb2D.linearVelocity.magnitude * 0.05f;
        jumpHeightScale = Mathf.Clamp01(jumpHeightScale);

        //Disable collisiions
        capsuleCollider2D.enabled = false;

        carSFXHandler.JumpSFX();

        //Change sorting layer to flying
        carRenderer.sortingLayerName = "Flying";
        carShadowRenderer.sortingLayerName = "Flying";

        //Push  the object forward as we passed a jump
        carRb2D.AddForce(carRb2D.linearVelocity.normalized * jumpPushScale * 10, ForceMode2D.Impulse);

        while (isJumping)
        {

            float jumpCompletedPercentage = (Time.time - jumpStarTime) / jumpDuration;
            jumpCompletedPercentage = Mathf.Clamp01(jumpCompletedPercentage);

            //Take the base scale of 1 and add how much we should increase the scale with.
            carRenderer.transform.localScale = Vector3.one + Vector3.one * jumpCurve.Evaluate(jumpCompletedPercentage) * jumpHeightScale;

            //Change the shadow scale
            carShadowRenderer.transform.localScale = carRenderer.transform.localScale * 0.75f;

            //Offset shadow a bit.
            carShadowRenderer.transform.localPosition = new Vector3(1, -1, 0.0f) * 3 * jumpCurve.Evaluate(jumpCompletedPercentage) * jumpHeightScale;
            //When we reach 100% we are done
            if (jumpCompletedPercentage == 1.0f)
            {
                break;
            }


            yield return null;
        }
        //Do not check for collisions with triggers
        capsuleCollider2D.enabled = false;

        //Do not check for collisions with triggers
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useTriggers = false;

        Collider2D[] hitResults = new Collider2D[2];

        int numberOfHitObjects = Physics2D.OverlapCircle(transform.position, 1.5f, contactFilter2D, hitResults);

        //Enable the car collider again so we can detect things with the trigger. 
        capsuleCollider2D.enabled = true;
        //Check if landing is ok or not
        if (numberOfHitObjects != 0)
        {
            //Add small jump to push car forward a bit
            isJumping = false;
            Jump(0.2f, 0.6f);
        }
        else
        {
            //Handle landing, scale back the object
            carRenderer.transform.localScale = Vector3.one;

            //Reset shadow position and scale
            carShadowRenderer.transform.localPosition = Vector3.zero;
            carShadowRenderer.transform.localScale = carRenderer.transform.localScale;


            //We are safe to lane, so enable collider
            capsuleCollider2D.enabled = true;

            //Change sorting layer to regular layer
            carRenderer.sortingLayerName = "Player";
            carShadowRenderer.sortingLayerName = "Player";


            //Play the landing effect for bigger jump
            if (jumpHeightScale > 0.2f)
            {
                if (landingEffect != null)
                {
                    landingEffect.Play();
                    carSFXHandler.LandingSFX();
                }
            }

            //Change state
            isJumping = false;

        }

    }
    //Detact Jump Trigger
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Jump"))
        {
            //Get the jump data form the jump
            JumpData jumpData = collision.GetComponent<JumpData>();
            Jump(jumpData.jumpHeightScale, jumpData.jumpPushScale);
        }
    }

    public void CheckPointHit(int checkPointNumber)
    {
        if (checkPointNumber == nextCheckPoint)
        {
            nextCheckPoint++;

            if (nextCheckPoint == RaceController.Instance.allCheckPoints.Length)
            {
                nextCheckPoint = 0;
                LapCompleted();
            }

        }
    }
    void LapCompleted()
    {
        currentLap++; //When completed lap
        if (lapTime < bestLapTime || bestLapTime == 0f)
        {
            bestLapTime = lapTime;
        }

        if (currentLap <= RaceController.Instance.totalLaps)
        {
            lapTime = 0f; //Reset value for new lap
                          //Show best lap time

            if (!carInputHandler.isAICar)
            {
                var bestTime = System.TimeSpan.FromSeconds(bestLapTime);
                if (UIManager.HasInstance)
                {
                    UIManager.Instance.hUDPanel.bestLapTime_Text.text = string.Format("{0:00}m{1:00}.{2:00}s", bestTime.Minutes, bestTime.Seconds, bestTime.Milliseconds);
                }
                //Show current lap
                DisplayLap();
            }
        }
        else
        {
            if (!carInputHandler.isAICar)
            {
                carInputHandler.isAICar = true; //Change Player to AI when complete race
                                                //Not update Pos, Time, Lap anymore
                RaceController.Instance.FinishRace();
            }
        }



    }
    void DisplayLap()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.hUDPanel.lapCounter_Text.text = "Lap: " + currentLap + "/" + RaceController.Instance.totalLaps;
        }
    }
    void DisplayTime()
    {
        var time = System.TimeSpan.FromSeconds(lapTime);
        if (UIManager.HasInstance)
        {
            UIManager.Instance.hUDPanel.currentLapTime_Text.text = string.Format("{0:00}m{1:00}.{2:00}s", time.Minutes, time.Seconds, time.Milliseconds);
        }
    }
    void DisplaySpeed()
    {
        if (UIManager.HasInstance)
        {
            int speed = (int)(carRb2D.linearVelocity.magnitude * 15f);
            UIManager.Instance.hUDPanel.speedDometer_Text.text = speed.ToString();
        }
    }

}
