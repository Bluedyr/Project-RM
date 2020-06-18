using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerClimb : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask ground;
    public LayerMask objects;

    PlayerMovement pm;
    CharacterController cc;

    
    
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


    RaycastHit g_Hit;
    bool g_HitDetect;

    RaycastHit o_Hit;
    bool o_HitDetect;
    [Tooltip("The max distance of the boxcast")]
    public float rayDist = 5f;



    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        climbing = false;
        cancelLean = false;
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update(){


        

        if (!climbing&&cancelLean == true) {
            //if not climbing, reset the angle
            float curLeanAngle = Mathf.MoveTowardsAngle(transform.localEulerAngles.x, 0, rotateSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(curLeanAngle, transform.localEulerAngles.y, 0);
            if (curLeanAngle == 0) {
                transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
                cancelLean = false;
            }
        }
        if (climbing) {
            float curLeanAngle = Mathf.MoveTowardsAngle(transform.localEulerAngles.x, climbRotate, rotateSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(curLeanAngle, transform.localEulerAngles.y, 0);
            transform.Translate(Vector3.MoveTowards(transform.position, moveto, climbspeed*pm.movementSpeed*Time.deltaTime)-transform.position,Space.World);

            if (transform.position == moveto) {//if the player has finished movement, return control
                cc.enabled = true;
                pm.enabled = true;
                pm.velocity.y = -10f;//cancels the jump of the player
                climbing = false;
                cancelLean = true;
            }
        }
        else if (Input.GetKey(KeyCode.W)&&Input.GetAxisRaw("Jump")==1) {
            //check for collisions with layers
            Collider[] ledgeobj = Physics.OverlapBox(transform.position + transform.up * wallPos.y + transform.forward * wallPos.z, wallSize, Quaternion.LookRotation(transform.forward), objects);
            Collider[] ledgegrd = Physics.OverlapBox(transform.position + transform.up * wallPos.y + transform.forward * wallPos.z, wallSize, Quaternion.LookRotation(transform.forward), ground);
            
            if (ledgeobj.Length != 0 || ledgegrd.Length != 0) {
                
                //checks if the space to move into is free
                Collider[] freeobj = Physics.OverlapBox(transform.position + transform.up * freePos.y + transform.forward * freePos.z, freeSize, Quaternion.LookRotation(transform.forward), objects);
                Collider[] freegrd = Physics.OverlapBox(transform.position + transform.up * freePos.y + transform.forward * freePos.z, freeSize, Quaternion.LookRotation(transform.forward), ground);
                
                
                //TODO

                if (freeobj.Length==0&&freegrd.Length==0) {

                    //DO THE BOXCAST HERE
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    Vector3 rayStart = transform.position + transform.up * freePos.y + transform.forward * freePos.z;
                    g_HitDetect = Physics.BoxCast(rayStart, freeSize, -transform.up, out g_Hit, Quaternion.LookRotation(transform.forward), rayDist, ground);
                    o_HitDetect = Physics.BoxCast(rayStart, freeSize, -transform.up, out o_Hit, Quaternion.LookRotation(transform.forward), rayDist, objects);

                    if (g_HitDetect) {
                        if (o_HitDetect) {
                            if (g_Hit.distance < o_Hit.distance) {
                                moveto = rayStart + -transform.up * g_Hit.distance;//TODO THIS IS CAUSING AN ISSUE
                               
                            }
                            else {
                                moveto = rayStart + -transform.up * o_Hit.distance;
                            }
                        }
                        else {
                            moveto = rayStart + -transform.up * g_Hit.distance;
                        }
                    }
                    else if (o_HitDetect) {
                        moveto = rayStart + -transform.up * o_Hit.distance;
                    }
                    else {
                        moveto = transform.position + transform.up * freePos.y + transform.forward * freePos.z;
                    }
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    
                    
                    climbing = true;
                    //moveto = transform.position + transform.up * freePos.y + transform.forward * freePos.z;
                    
                    cc.enabled = false;
                    pm.canJump = false;
                    pm.enabled = false;
                }
                else {
                    print("no space");
                }
            }
        }

        //UNDERNEATH THIS COMMENT IS JUST TO UPDATE THE GIZMO CONSTANTLY INSTEAD OF JUST WHEN IT CLIMBS
        Vector3 rayStartDraw = transform.position + transform.up * freePos.y + transform.forward * freePos.z;
        g_HitDetect = Physics.BoxCast(rayStartDraw, freeSize, -transform.up, out g_Hit, Quaternion.LookRotation(transform.forward), rayDist, ground);
        o_HitDetect = Physics.BoxCast(rayStartDraw, freeSize, -transform.up, out o_Hit, Quaternion.LookRotation(transform.forward), rayDist, objects);

        //if (g_HitDetect) {
        //    if (o_HitDetect) {
        //        if (g_Hit.distance < o_Hit.distance) { Debug.Log("Ground Hit : " + g_Hit.collider.name); }
        //        else { Debug.Log("Object Hit : " + o_Hit.collider.name); }
        //    }
        //    else { Debug.Log("Ground Hit : " + g_Hit.collider.name); }
        //}
        //else if (o_HitDetect) {
        //    Debug.Log("Object Hit : " + o_Hit.collider.name);
        //}
        //else {
        //    Debug.Log("No Hit");
        //}

    }

    private void OnDrawGizmos() {
        

        Gizmos.matrix = this.transform.localToWorldMatrix;
        //Wall check
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(0,wallPos.y,wallPos.z), wallSize*2);
        //free space check
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(0, freePos.y,freePos.z), freeSize*2);
        //moveto check
        

        
        //Drawing Boxcast (best result of 2)
        Gizmos.color = Color.magenta;
        if (g_HitDetect) {
            if (o_HitDetect) {
                if (g_Hit.distance < o_Hit.distance) {
                    Gizmos.DrawRay(freePos, -transform.up * g_Hit.distance);
                    Gizmos.DrawWireCube(freePos + -transform.up * g_Hit.distance, freeSize * 2);
                }
                else {
                    Gizmos.DrawRay(freePos, -transform.up * o_Hit.distance);
                    Gizmos.DrawWireCube(freePos + -transform.up * o_Hit.distance, freeSize * 2);
                }
            }
            else {
                Gizmos.DrawRay(freePos, -transform.up * g_Hit.distance);
                Gizmos.DrawWireCube(freePos + -transform.up * g_Hit.distance, freeSize * 2);
            }
        }
        else if (o_HitDetect) {
            Gizmos.DrawRay(freePos, -transform.up * o_Hit.distance);
            Gizmos.DrawWireCube(freePos + -transform.up * o_Hit.distance, freeSize * 2);
        }
        else {
            Gizmos.DrawRay(freePos, -transform.up * rayDist );
            Gizmos.DrawWireCube(freePos + -transform.up * rayDist, freeSize * 2);
        }

        

    }
}
