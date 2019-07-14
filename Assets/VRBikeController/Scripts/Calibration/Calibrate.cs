using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrate : MonoBehaviour {

    public Animation bikeAnim;

    [Header("Important Transforms")]
    public Transform frontTracker;
    public Transform leftMarker;
    public Transform centerMarker;
    public Transform rightMarker;
    public Transform rootMarker;

    [Header("Calibration Checks")]
    public GameObject calibrationCanvas;
    public bool calibrateStarted = false;
    public bool calibrateCompleted = false;
    public bool calibrateOnStart = true;
    public BikeMovement bikeMovementComponent;
    public CalculateBikeModelPositionAndForward bikeAlignment;
    public Timer durationTimer;
    public float centerTrackingCompletion, rightTrackingCompletion, leftTrackingCompletion;
    public bool calibrationOverride = false;
    [TextArea]
    public string GuideText;

    [Header("Config")]
    public float neglibleDelta = 0.01f;
    public float continuousHeldTime = 0f;
    public float calibrationStartedThreshold = 0.05f;
    public float handlebarTrackerHeight = 0.5f;
    
    private Vector3 frontPoint;

    void Start() 
    {
        if (!calibrateStarted && calibrateOnStart)
        {
            StartCoroutine(BeginCalibration());
        }
    }

    void Update() 
    {
        if (!calibrateStarted && calibrateOnStart) 
        {
            StartCoroutine(BeginCalibration());
        }
    }

    public void BeginCalibrating() 
    {
        StartCoroutine(BeginCalibration());
    }


    private IEnumerator BeginCalibration()
    {
        calibrateStarted = true;
        frontPoint = frontTracker.position;
        Vector3 previousFrontPoint = frontPoint;

        if(!calibrationOverride)
        {
            Debug.Log("Dont move keep the front at the center");
            GuideText = "KEEP THE BIKE STILL IN THE CENTER";
            continuousHeldTime = 0f;
            centerTrackingCompletion = 0f;

            while (continuousHeldTime < 1f) 
            {
                if (Vector3.Distance(previousFrontPoint, frontTracker.position) < neglibleDelta)
                {
                    continuousHeldTime += Time.deltaTime / 5f;
                } 
                else 
                {
                    continuousHeldTime = 0f;
                }

                centerTrackingCompletion = Mathf.Clamp01(continuousHeldTime);
                previousFrontPoint = frontTracker.position;
                yield return new WaitForEndOfFrame();
            }

            centerMarker.position = previousFrontPoint;
            GuideText = "LEAN ALL THE WAY TO THE RIGHT";
            
            Debug.Log("Lean all the way to the right");
            while (Vector3.Distance(frontTracker.position, previousFrontPoint) < calibrationStartedThreshold)
            {
                yield return new WaitForEndOfFrame();
            }
            continuousHeldTime = 0f;
            rightTrackingCompletion = 0f;
            while (continuousHeldTime < 1f) {
                if (Vector3.Distance(previousFrontPoint, frontTracker.position) < neglibleDelta) {
                        continuousHeldTime += Time.deltaTime / 5f;
                } else {
                        continuousHeldTime = 0f;
                }
                rightTrackingCompletion = Mathf.Clamp01(continuousHeldTime);
                previousFrontPoint = frontTracker.position;
                yield return new WaitForEndOfFrame();
            }
            rightMarker.position = previousFrontPoint;
            GuideText = "LEAN ALL THE WAY TO THE LEFT";
            Debug.Log("Now lean all the way to the left");
            while (Vector3.Distance(frontTracker.position, previousFrontPoint) < calibrationStartedThreshold) {
                yield return new WaitForEndOfFrame();
            }
            continuousHeldTime = 0f;
            leftTrackingCompletion = 0f;
            while (continuousHeldTime < 1f) {
                if (Vector2.Distance(previousFrontPoint, frontTracker.position) < neglibleDelta) {
                    continuousHeldTime += Time.deltaTime / 5f;
                } else {
                    continuousHeldTime = 0f;
                }
                leftTrackingCompletion = Mathf.Clamp01(continuousHeldTime);
                previousFrontPoint = frontTracker.position;
                yield return new WaitForEndOfFrame();
            }
            leftMarker.position = previousFrontPoint;
            Debug.Log("Okay chill");
        }
        else
        {
            centerMarker.position = frontPoint;
            rightMarker.position = frontPoint + new Vector3(1,0,0);
            leftMarker.position = frontPoint + new Vector3(1,0,0);         
        }

        Vector3 rootPosition = centerMarker.position;
        rootPosition.y = rootPosition.y - handlebarTrackerHeight;
        rootMarker.position = rootPosition;
        calibrateCompleted = true;
        
        // RJ STUFF ADDED ************************************
        int count_down = 3;
        float time = Time.time;
        while(count_down > 0)
        {
            GuideText = "CENTER YOURSELF, GET READY " + count_down;
            yield return new WaitForSeconds(1);
            count_down -= 1;
            yield return null;
        }
        //*************************** */

        calibrationCanvas.SetActive(false);
        // Turn on animator
        bikeAnim.Play();
        bikeMovementComponent.enabled = true;
        durationTimer.bikeCalibrated = true;
       // bikeMovementComponent.gameObject.GetComponent<Rigidbody>().velocity = bikeMovementComponent.transform.forward * 20f;
        bikeAlignment.Align();
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(frontTracker.position, frontTracker.position + calibrationStartedThreshold * transform.right);

        Gizmos.DrawLine(frontTracker.position, frontTracker.position - handlebarTrackerHeight * Vector3.up);
    }
}
