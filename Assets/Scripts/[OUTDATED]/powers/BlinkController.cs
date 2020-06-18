using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkController : MonoBehaviour{

    public KeyCode Blink;
    PlayerController pc;

    public Transform cameraTransform;

    RaycastHit hit;
    bool hitDetect;

    public float blinkDistance;
    public LayerMask excludePlayer;

    Vector3 blinkPoint;

    float playerHeight = 2f;
    float radius = 0.5f;
    public float lerpSpeed = 0.01f;

    bool failBlink;//If this is true, it was unable to find a good blink spot so it fails

    Vector3 currentPoint;

    // Start is called before the first frame update
    void Start() {
        pc = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKey(Blink)) {


            //hitDetect = Physics.BoxCast(transform.position, blinkBox, camera.forward, out hit, Quaternion.LookRotation(camera.forward), blinkDistance, excludePlayer);
            //Physics.capsuleCast//TODO CHANGE TO CAPSULE CAST SO  I DONT NEED TO WORRY ABOUT THE DIRECTION OF THE BOX. ALSO CAPSULE CAST IS A LITTLE CHEAPER
            hitDetect = Physics.Raycast(cameraTransform.position,cameraTransform.forward,out hit,blinkDistance,excludePlayer);
            
            if (hitDetect) {
                currentPoint = cameraTransform.position + cameraTransform.forward * (hit.distance);
            }
            else {
                currentPoint = cameraTransform.position + cameraTransform.forward * (blinkDistance);
            }
            //capsuleCol.center + transform.up * (capsuleCol.height / 2 - capsuleCol.radius))
            Collider[] freeSpace = Physics.OverlapCapsule(currentPoint +Vector3.up*(playerHeight/2-radius), currentPoint - Vector3.up * (playerHeight / 2 - radius),radius,excludePlayer);
            
            if (freeSpace.Length==0) {
                blinkPoint = currentPoint;
                failBlink = false;
            }
            else {
                int count = 0;
                while (freeSpace.Length!=0) {
                    
                   currentPoint= Vector3.Lerp(currentPoint, cameraTransform.position, lerpSpeed);
                    
                    if (Vector3.Distance(cameraTransform.position, currentPoint) <= radius|| count==100) {
                        failBlink = true;
                        break;
                    }
                    freeSpace = Physics.OverlapCapsule(currentPoint + Vector3.up * (playerHeight / 2 - radius), currentPoint - Vector3.up * (playerHeight / 2 - radius), radius, excludePlayer);

                    
                }
                if (freeSpace.Length==0) {
                    failBlink = false;
                }
                if (!failBlink) {
                    blinkPoint = currentPoint;
                }
                
                
            }
        }
        else if (Input.GetKeyUp(Blink)) {
            if (!failBlink) {
                if (hitDetect) {
                    transform.position = blinkPoint;
                }
                else {
                    transform.position = blinkPoint;
                }
            }
            
            
        }

       
    }





    private void OnDrawGizmos() {
        
        if (Input.GetKey(Blink)) {

            
                Gizmos.color = Color.green;
                Vector3.Distance(cameraTransform.position,blinkPoint);
                Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward* Vector3.Distance(cameraTransform.position, blinkPoint));
                Gizmos.DrawSphere(currentPoint + Vector3.up * (playerHeight / 2 - radius),radius);
             Gizmos.DrawSphere(currentPoint - Vector3.up * (playerHeight / 2 - radius), radius);
            



        }

    }
}
