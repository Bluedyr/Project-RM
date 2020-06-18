using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Title: Blink
 * Author: Marc Askew
 */

[CreateAssetMenu(menuName = "Abilities/Blink")]
public class Blink : Ability{

    public GameObject test;
    GameObject testspawn;

    Transform playerTransform;
    Transform cameraTransform;

    RaycastHit hit;
    bool hitDetect;

    public float blinkDistance;
    public LayerMask excludePlayer;

    Vector3 blinkPoint;

    float playerHeight = 2f;
    float radius = 0.5f;
    public float lerpSpeed = 0.01f;//used for checking for free space

    bool failBlink;//If this is true, it was unable to find a good blink spot so it fails

    Vector3 currentPoint;

    

    public override void Initialise(GameObject obj) {
        playerTransform = obj.transform;
        cameraTransform = obj.transform.GetChild(0);//not sure if this works
        onCooldown = false;
    }

    public override void Update() {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0) {
            onCooldown = false;
        }
    }

    public override void OnDown() {
        
        
    }

    public override void OnHold() {
        if (testspawn==null) {
            testspawn = Instantiate(test);
            testspawn.transform.SetParent(cameraTransform);
        }

        
        hitDetect = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, blinkDistance, excludePlayer);

        if (hitDetect) {
            currentPoint = cameraTransform.position + cameraTransform.forward * (hit.distance);
        }
        else {
            currentPoint = cameraTransform.position + cameraTransform.forward * (blinkDistance);
        }
        //capsuleCol.center + transform.up * (capsuleCol.height / 2 - capsuleCol.radius))
        Collider[] freeSpace = Physics.OverlapCapsule(currentPoint + Vector3.up * (playerHeight / 2 - radius), currentPoint - Vector3.up * (playerHeight / 2 - radius), radius, excludePlayer);

        if (freeSpace.Length == 0) {
            blinkPoint = currentPoint;
            failBlink = false;
        }
        else {
            int count = 0;
            while (freeSpace.Length != 0) {

                currentPoint = Vector3.Lerp(currentPoint, cameraTransform.position, lerpSpeed);

                if (Vector3.Distance(cameraTransform.position, currentPoint) <= radius || count == 100) {
                    failBlink = true;
                    break;
                }
                freeSpace = Physics.OverlapCapsule(currentPoint + Vector3.up * (playerHeight / 2 - radius), currentPoint - Vector3.up * (playerHeight / 2 - radius), radius, excludePlayer);


            }
            if (freeSpace.Length == 0) {
                failBlink = false;
            }
            if (!failBlink) {
                blinkPoint = currentPoint;
            }

            
        }
        testspawn.transform.position = blinkPoint;
        testspawn.transform.eulerAngles=Vector3.zero;

    }

    public override void OnUp() {
        Destroy(testspawn);
        if (!failBlink) {
            if (hitDetect) {
                playerTransform.position = blinkPoint;
            }
            else {
                playerTransform.position = blinkPoint;
            }
        }
        onCooldown = true;
        currentTime = cooldown;
        
    }

}
