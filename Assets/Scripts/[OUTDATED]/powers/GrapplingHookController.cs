using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookController : MonoBehaviour{

    public Transform cameraTransform;
    public LayerMask excludePlayer;

    PlayerController pc;

    public Vector3 ropeOrigin;//where the rope comes from in relation to the player
    public KeyCode grapple;

    RaycastHit hit;
    bool hitDetect;
    Vector3 hitPoint;//this is its own thing just because we need a vector3 for when it doesnt hit anything

    public float ropeDist=10f;
    public float grappleSpeed=10f;

    [HideInInspector] public bool isGrappling;

   

    //TODO we might eventually want to make it so it doesn't work against some surfaces. e.g. enemies


    //So the idea is to try and mix Widowmakers grappling hook with Emily Kaldwin's Far Reach
    // Start is called before the first frame update
    void Start(){
        pc = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(grapple)) {//pressed
            hitDetect = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, ropeDist, excludePlayer);
            if (hitDetect) {
                hitPoint = cameraTransform.position + cameraTransform.forward * (hit.distance);
            }
            else {
                hitPoint = cameraTransform.position + cameraTransform.forward * (ropeDist);
            }

            //raycast towards where user is looking
            //if not bad spot set grapple position
        }
        else if (Input.GetKey(grapple)) {//held
            isGrappling = true;
            //dont care about animations just yet. get functionality first
            if (hitDetect) {//by changing velocity, we can make the player faster during the grapple
                pc.enableGravity = false;
               
                
                if (Vector3.Distance(cameraTransform.position,hit.point)>1) {
                    pc.velocity =  Vector3.Normalize(hit.point - cameraTransform.position)  * grappleSpeed;
                }
            }
            else {
                //nothing
            }

        }
        else if (Input.GetKeyUp(grapple)) {//let go
            //cancel grapple
            pc.enableGravity = true;
            pc.currentGravity = pc.velocity.y;//maintain vertical velocity for smooth transition to falling
            
            hitDetect = false;
            isGrappling = false;
        }

    }
}
