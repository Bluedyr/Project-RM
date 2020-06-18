using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Title: ClimbController
 * Author: Marc Askew
 */

public class ClimbController : MonoBehaviour {

    #region Variables
    [Header("Layers")]
    public LayerMask climbables;

    PlayerController pc;
    



    [Header("Movement")]
    public float climbspeed = 0.1f;
    [Tooltip("How far the player will rotate on the x axis")]
    public float climbRotate = 20f;
    bool cancelLean = false;
    public float rotateSpeed = 20f;
    bool climbing;
    Vector3 moveto;

    [Header("Wall Check")]
    public Vector3 wallPos = new Vector3(0, 0.4f, (0.5f + (0.25f / 2f)));
    public Vector3 wallSize = new Vector3(0.25f, 0.55f, 0.5f + (0.25f / 2f));
    [Header("Free Space Check")]
    public Vector3 freePos = new Vector3(0, 1.8f, 0.75f);
    public Vector3 freeSize = new Vector3(0.5f, 0.9f, 0.5f);


    RaycastHit hit;
    bool hitDetect;


    [Tooltip("The max distance of the boxcast")]
    public float rayDist = 5f;
    #endregion


    void Start() {
        pc = GetComponent<PlayerController>();
        climbing = false;
        cancelLean = false;
    }

    // Update is called once per frame
    void Update() {
        if (!climbing && cancelLean == true) {
            ResetClimb();
        }
        if (climbing) {
            Climb();
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetAxisRaw("Jump") == 1) {
            CheckSpace();
        }
        //UNDERNEATH THIS COMMENT IS JUST TO UPDATE THE GIZMO CONSTANTLY INSTEAD OF JUST WHEN IT CLIMBS
        Vector3 rayStartDraw = transform.position + transform.up * freePos.y + transform.forward * freePos.z;
        hitDetect = Physics.BoxCast(rayStartDraw, freeSize, -transform.up, out hit, Quaternion.LookRotation(transform.forward), rayDist, climbables);
    }

    void Climb() {
        //Does the rotation and movement of the climb

        //Interpolate the angle towards the max climbinb rotation
        float curLeanAngle = Mathf.MoveTowardsAngle(transform.localEulerAngles.x, climbRotate, rotateSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(curLeanAngle, transform.localEulerAngles.y, 0);
        //Interpolate towards where we decided to climb to
        transform.Translate(Vector3.MoveTowards(transform.position, moveto, climbspeed * pc.movementSpeed * Time.deltaTime) - transform.position, Space.World);

        //if the player has finished movement, return control and cancel the rotation
        if (transform.position == moveto) {
            pc.enabled = true;
            pc.currentGravity = 0f;//cancels the jump of the player (for if they jumped into the climb)
            climbing = false;
            cancelLean = true;
            pc.canJump = true;
        }
    }

    void ResetClimb() {
        //if not climbing, reset the angle
        float curLeanAngle = Mathf.MoveTowardsAngle(transform.localEulerAngles.x, 0, rotateSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(curLeanAngle, transform.localEulerAngles.y, 0);
        if (curLeanAngle == 0) {
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
            cancelLean = false;
        }
    }

    void CheckSpace() {
        //check for collisions with layers
        Collider[] ledge = Physics.OverlapBox(transform.position + transform.up * wallPos.y + transform.forward * wallPos.z, wallSize, Quaternion.LookRotation(transform.forward), climbables);

        //if we found a potential object to climb
        if (ledge.Length != 0) {
            //checks if the space to move into is free
            Collider[] free = Physics.OverlapBox(transform.position + transform.up * freePos.y + transform.forward * freePos.z, freeSize, Quaternion.LookRotation(transform.forward), climbables);
            
            if (free.Length == 0) {

                //if there is free space, check lower to see if there is a more suitable space to move to
                Vector3 rayStart = transform.position + transform.up * freePos.y + transform.forward * freePos.z;
                hitDetect = Physics.BoxCast(rayStart, freeSize, -transform.up, out hit, Quaternion.LookRotation(transform.forward), rayDist, climbables);


                if (hitDetect) {
                    moveto = rayStart + -transform.up * hit.distance;
                }
                else {
                    moveto = transform.position + transform.up * freePos.y + transform.forward * freePos.z;
                }


                climbing = true;
                pc.enabled = false;
                //pc.canJump = false;
            }
        }
    }


    private void OnDrawGizmos() {
        Gizmos.matrix = this.transform.localToWorldMatrix;
        //Wall check
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(0, wallPos.y, wallPos.z), wallSize * 2);
        //free space check
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(0, freePos.y, freePos.z), freeSize * 2);
        //moveto check

        //Drawing Boxcast (best result of 2)
        Gizmos.color = Color.magenta;
        if (hitDetect) {
            Gizmos.DrawRay(freePos, -transform.up * hit.distance);
            Gizmos.DrawWireCube(freePos + -transform.up * hit.distance, freeSize * 2);
        }
        else {
            Gizmos.DrawRay(freePos, -transform.up * rayDist);
            Gizmos.DrawWireCube(freePos + -transform.up * rayDist, freeSize * 2);
        }
    }
}
