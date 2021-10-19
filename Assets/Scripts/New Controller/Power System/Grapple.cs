using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Title: Grapple
 * Author: Marc Askew
 */

[CreateAssetMenu(menuName = "Abilities/Grapple")]
public class Grapple : Ability{

    public LineRenderer ropeRenderer;
    
    Transform cameraTransform;
    public LayerMask excludePlayer;

    PlayerController pc;

    Transform abilityOrigin;//where the rope comes
    
    

    RaycastHit hit;
    bool hitDetect;
    Vector3 hitPoint;//this is its own thing just because we need a vector3 for when it doesnt hit anything

    public float ropeDist = 10f;
    public float grappleSpeed = 10f;
    public float minSpeed = 1.5f;
    public float maxSpeed = 15;

    public float wallOffset = 1f;

    AbilityController ac;

    public override void Initialise(GameObject obj) {
       
        ac = obj.GetComponent<AbilityController>();
        pc = obj.GetComponent<PlayerController>();
        ropeRenderer = pc.GetComponent<LineRenderer>();
        cameraTransform = obj.transform.GetChild(0);//not sure if this works
        abilityOrigin = cameraTransform.GetChild(0);
        onCooldown = false;
        ropeRenderer.enabled = false;
    }

    public override void Update() {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0) {
            onCooldown = false;
        }
        //do rope animation here

    }

   

    public override void OnDown() {
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

    public override void OnHold() {

        //dont care about animations just yet. get functionality first
        if (hitDetect) {//by changing velocity, we can make the player faster during the grapple


            //do shooting rope here
            ropeRenderer.enabled = true;
            

            //if rope reached hit point do below

            ac.isGrappling = true;
            pc.enableGravity = false;

            float distance = Vector3.Distance(hit.point, cameraTransform.position);


            ropeRenderer.SetPosition(1, pc.transform.InverseTransformPoint(hit.point));

            if (distance > wallOffset) {
                //speed is going to be equal to the inverse distance or min speed
                Vector3 direction = Vector3.Normalize(hit.point - cameraTransform.position);
               
                float speed = Mathf.Min(Mathf.Max((distance * grappleSpeed), minSpeed), maxSpeed);

                pc.velocity = direction * speed;

                //pc.velocity = Vector3.Normalize(hit.point - cameraTransform.position) * grappleSpeed;
                
            }
            else {
                
                pc.velocity = Vector3.zero;
            }
        }
        else {
            //nothing
        }
        if (ac.isGrappling) {
            Debug.Log("hi");
            ropeRenderer.SetPosition(0, pc.transform.InverseTransformPoint(abilityOrigin.position));
        }

    }


    public override void OnUp() {

        

        if (hitDetect) {//could be replaced with ac.isgrappling but didnt want it to be circular. might be a better way for this to be done
            //cancel grapple
            pc.enableGravity = true;
            pc.currentGravity = pc.velocity.y;//maintain vertical velocity for smooth transition to falling

            hitDetect = false;
            ac.isGrappling = false;
            onCooldown = true;
            currentTime = cooldown;
            ropeRenderer.enabled = false;
        }

    }



    
}
