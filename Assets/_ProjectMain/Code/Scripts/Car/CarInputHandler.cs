using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    public enum AIMode
    {
        followPlayer,
        followMouse,
        followWayPoint //defaullt
    }

    [Header("AI Setting")]
    [SerializeField] public AIMode aIMode;
    [SerializeField] public bool isAICar = true;
    [SerializeField][Range(0, 1)] float skillLevel = 1.0f;
    [SerializeField] bool isAvoidingCars = true;

    Vector3 targetPosition = Vector3.zero;
    Transform targetTransform = null;

    //Avoidace
    Vector2 avoidanceVectorLerped = Vector3.zero;

    [Header("WayPoint")]
    [SerializeField] public WaypointNode currentWaypoint = null;
    [SerializeField] public WaypointNode previousWaypoint = null;
    [SerializeField] WaypointNode[] allWayPoints;



    [Header("Player Setting")]
    [SerializeField] float maxSpeedWithLevel;
    [SerializeField] float maxSpeed; //Speed from car controller
    [SerializeField] float resetCoolDown = 2f;
    private float resetCounter;


    CarController carController;
    BoxCollider2D rayCastBoxCollider2D;
    Rigidbody2D carRb2D;

    void Awake()
    {
        carController = GetComponent<CarController>();
        rayCastBoxCollider2D = GetComponentInChildren<BoxCollider2D>();
        carRb2D = GetComponent<Rigidbody2D>();

        maxSpeed = carController.maxSpeed;
        maxSpeedWithLevel = maxSpeed;

        //Replace by sort with LINQ
        allWayPoints = FindObjectsByType<WaypointNode>(FindObjectsSortMode.None);

        //Manual sorting for simple and run 1 time 
        if (allWayPoints.Length > 1)
        {
            System.Array.Sort(allWayPoints, (a, b) =>
            {
                if (a.name == "WaypointNode") return -1;
                if (b.name == "WaypointNode") return 1;

                string aName = a.name.Replace("WaypointNode", "").Replace("(", "").Replace(")", "");
                string bName = b.name.Replace("WaypointNode", "").Replace("(", "").Replace(")", "");

                if (int.TryParse(aName, out int indexA) && int.TryParse(bName, out int indexB))
                    return indexA.CompareTo(indexB);

                return string.Compare(a.name, b.name, System.StringComparison.Ordinal);
            });
        }
    }
    private void Start()
    {
        SetMaxSpeedBaseOnSkillLevel(maxSpeed);
        resetCounter = resetCoolDown;

        //Start with waypoint
        if (allWayPoints != null && allWayPoints.Length > 0)
        {
            currentWaypoint = allWayPoints[0];
            previousWaypoint = currentWaypoint;
        }
        else
        {
            Debug.LogError("[CarInputHandler] Không tìm thấy WaypointNode nào trong scene!", this);
            enabled = false;
        }
    }
    void FixedUpdate()
    {

        if (!RaceController.Instance.isStarting)
        {
            InputValue();
        }

    }

    void InputValue()
    {
        Vector2 inputVector = Vector2.zero;
        if (isAICar)
        {
            switch (aIMode)
            {
                case AIMode.followPlayer:
                    FollowPlayer();
                    break;

                case AIMode.followMouse:
                    FollowMouse();
                    break;

                case AIMode.followWayPoint:
                    FollowWayPoint();
                    break;
            }

            inputVector.x = TurnTowardTarget();
            inputVector.y = ApplyThorttleOrBrake(inputVector.x);
        }
        //Player
        else if (!isAICar)
        {
            inputVector.x = Input.GetAxis("Horizontal");
            inputVector.y = Input.GetAxis("Vertical");

            //Reset car in track with coolDown Time
            if (resetCounter > 0)
            {
                resetCounter -= Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.R) && resetCounter <= 0)
            {
                ResetToTrack();
                Debug.Log("Reset to track");
            }


        }
        //Send Input to the car controller;
        carController.SetInputVector(inputVector);
    }
    void FollowPlayer()
    {
        if (targetTransform == null)
        {
            //Fine all CarInputHandler in scene
            CarInputHandler[] allCars = FindObjectsByType<CarInputHandler>(FindObjectsSortMode.InstanceID);
            foreach (CarInputHandler car in allCars)
            {
                if (!car.isAICar) // If not AI, is a Player
                {
                    targetTransform = car.transform;
                    break;
                }
            }
        }
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
        }
    }
    void FollowMouse()
    {
        //Take the mouse position in screen space and convert it to world space
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Set the target position of for the AI
        targetPosition = worldPosition;
    }
    void FollowWayPoint()
    {
        //Find the nearest waypoint
        if (currentWaypoint == null)
        {
            currentWaypoint = FindClosestWayPoint();
            if (currentWaypoint == null)
            {
                targetPosition = transform.position;
                return;
            }
            previousWaypoint = currentWaypoint;
        }

        targetPosition = currentWaypoint.transform.position;
        float distanceToWayPoint = Vector3.Distance(transform.position, targetPosition);

        // Shortcut khi quá xa: kéo về đường thẳng nối 2 waypoint
        if (distanceToWayPoint > 20f && previousWaypoint != null)
        {
            Vector3 nearest = FindNearestPointOnLine(
                previousWaypoint.transform.position,
                currentWaypoint.transform.position,
                transform.position
            );

            float segments = distanceToWayPoint / 20f;
            targetPosition = (targetPosition + nearest * segments) / (segments + 1);
        }

        // Move to current waypoint → move to next waypoint
        if (distanceToWayPoint <= currentWaypoint.minDistanceToReachWaypoint)
        {
            //Max speed to waypint
            if (currentWaypoint.maxSpeed > 0f)
                maxSpeed = currentWaypoint.maxSpeed;
            else
                SetMaxSpeedBaseOnSkillLevel(carController.maxSpeed);

            previousWaypoint = currentWaypoint;

            //Get next waypoint (use random if more than 2 waypoint)
            var nextNodes = currentWaypoint.nextWaypointNode;
            if (nextNodes != null && nextNodes.Length > 0)
            {
                currentWaypoint = nextNodes[Random.Range(0, nextNodes.Length)];
            }
            else
            {
                //There is no link → find the nearest one
                currentWaypoint = FindClosestWayPoint() ?? currentWaypoint;
            }
        }
    }

    WaypointNode FindClosestWayPoint()
    {
        if (allWayPoints == null || allWayPoints.Length == 0) return null;

        WaypointNode closest = null;
        float closestDist = float.MaxValue;
        Vector3 myPos = transform.position;

        for (int i = 0; i < allWayPoints.Length; i++)
        {
            if (allWayPoints[i] == null) continue; //Check if no waypoint

            float dist = Vector3.SqrMagnitude(allWayPoints[i].transform.position - myPos);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = allWayPoints[i];
            }
        }

        return closest;
    }

    float TurnTowardTarget()
    {
        Vector2 vectorToTarget = targetPosition - transform.position;
        vectorToTarget.Normalize();

        //Apply avoidance to steering
        if (isAvoidingCars && !carController.GetJumping())
        {
            AvoidCars(vectorToTarget, out vectorToTarget);
        }

        //Calculate an angle towards the target 
        float angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
        angleToTarget *= -1;

        //We want the car to turn as much as possible if the angle is greater than 45 degrees and we wan't it to smooth out so if the angle is small we want the AI to make smaller corrections. 
        float steerAmount = angleToTarget / 45.0f;

        //Clamp steering to between -1 and 1.
        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);

        return steerAmount;
    }
    float ApplyThorttleOrBrake(float inputX)
    {
        //If we are going too fast then do not accelerate further.
        if (carController.GetVelocityMagnitude() > maxSpeed)
        {
            return 0;
        }

        //Apply throttle forward based on how much the car wants to turn. If it's a sharp turn this will cause the car to apply less speed forward
        float reduceSeedToCornering = Mathf.Abs(inputX) / 1.0f;

        //Apply throttle based on cornering and skill.
        return 1.05f - reduceSeedToCornering * skillLevel;
    }

    //Check for car ahead of the car
    bool IsCarsInFrontOfAICar(out Vector3 position, out Vector3 otherCarRightVector)
    {
        //Disable the cars own collider to avoid having the AI car detect itself. 
        rayCastBoxCollider2D.enabled = false;
        Physics2D.SyncTransforms();

        //Perform the circle cast in front of the car with a slight offset forward and only in the Car layer
        RaycastHit2D raycastHit2d = Physics2D.CircleCast(transform.position + transform.up * 0.5f, 1.2f, transform.up, 12, 1 << LayerMask.NameToLayer("Car"));
        //Debug.Log(raycastHit2d.collider);
        //Enable the colliders again so the car can collide and other cars can detect it.  
        rayCastBoxCollider2D.enabled = true;
        Physics2D.SyncTransforms();

        if (raycastHit2d.collider != null)
        {
            //Draw a red line showing how long the detection is, make it red since we have detected another car
            Debug.DrawRay(transform.position, transform.up * 12, Color.red);

            position = raycastHit2d.collider.transform.position;
            otherCarRightVector = raycastHit2d.collider.transform.right;
            return true;
        }
        else
        {
            //We didn't detect any other car so draw black line with the distance that we use to check for other cars. 
            Debug.DrawRay(transform.position, transform.up * 12, Color.black);
        }

        //No car was detected but we still need assign out values so lets just return zero. 
        position = Vector3.zero;
        otherCarRightVector = Vector3.zero;

        return false;
    }

    void AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget)
    {
        if (IsCarsInFrontOfAICar(out Vector3 otherCarPosition, out Vector3 otherCarRightVector))
        {
            Vector2 avoidanceVector = Vector2.zero;

            //Calculate the reflecing vector if we would hit the other car. 
            avoidanceVector = Vector2.Reflect((otherCarPosition - transform.position).normalized, otherCarRightVector);

            float distanceToTarget = (targetPosition - transform.position).magnitude;

            //We want to be able to control how much desire the AI has to drive towards the waypoint vs avoiding the other cars. 
            //As we get closer to the waypoint the desire to reach the waypoint increases.
            float driveToTargetInfluence = 6.0f / distanceToTarget;

            //Ensure that we limit the value to between 30% and 100% as we always want the AI to desire to reach the waypoint.  
            driveToTargetInfluence = Mathf.Clamp(driveToTargetInfluence, 0.30f, 1.0f);

            //The desire to avoid the car is simply the inverse to reach the waypoint
            float avoidanceInfluence = 1.0f - driveToTargetInfluence;

            //Reduce jittering a little bit by using a lerp
            avoidanceVectorLerped = Vector2.Lerp(avoidanceVectorLerped, avoidanceVector, Time.fixedDeltaTime * 4);

            //Calculate a new vector to the target based on the avoidance vector and the desire to reach the waypoint
            newVectorToTarget = (vectorToTarget * driveToTargetInfluence + avoidanceVector * avoidanceInfluence);
            newVectorToTarget.Normalize();

            //Draw the vector which indicates the avoidance vector in green
            Debug.DrawRay(transform.position, avoidanceVector * 10, Color.green);

            //Draw the vector that the car will actually take in yellow. 
            Debug.DrawRay(transform.position, newVectorToTarget * 10, Color.yellow);

            //we are done so we can return now. 
            return;
        }

        //We need assign a default value if we didn't hit any cars before we exit the function. 
        newVectorToTarget = vectorToTarget;
    }
    void SetMaxSpeedBaseOnSkillLevel(float newSpeed)
    {
        maxSpeed = Mathf.Clamp(newSpeed, 0, maxSpeedWithLevel);

        float skillBasedMaxSpeed = Mathf.Clamp(skillLevel, 0.3f, 1.0f);
        maxSpeed = maxSpeed * skillBasedMaxSpeed;

    }
    //Finds the nearest point on a line
    Vector2 FindNearestPointOnLine(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 point)
    {
        //Get a heading as a vector
        Vector2 lineHeadingVector = (lineEndPosition - lineStartPosition);

        //Store the max distance
        float maxDistance = lineHeadingVector.magnitude;
        lineHeadingVector.Normalize();

        //Do projection from the start position on the point
        Vector2 lineVectorStartToPoint = point - lineStartPosition;
        float dotProduct = Vector2.Dot(lineVectorStartToPoint, lineHeadingVector);

        //Clamp the dot product to maxDistance
        dotProduct = Mathf.Clamp(dotProduct, 0f, maxDistance);

        return lineStartPosition + (lineHeadingVector * dotProduct);
    }

    void ResetToTrack()
    {
        //Get last check point 
        int pointToReset = carController.nextCheckPoint - 1;
        if (pointToReset < 0) //If retunr 0 checkpoint (new lap), return to final check point of list
        {

            pointToReset = RaceController.Instance.allCheckPoints.Length - 1;

        }
        //Tranform to last check point

        transform.position = RaceController.Instance.allCheckPoints[pointToReset].transform.position;
        carRb2D.transform.position = transform.position;
        carRb2D.linearVelocity = Vector3.zero;

        resetCounter = resetCoolDown;
    }
}

