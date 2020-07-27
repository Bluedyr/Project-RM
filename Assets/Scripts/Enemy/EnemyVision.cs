using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour{


    public Transform playerCameraTransform;

    public float sideFOV = 45f;
    public float upFOV = 20f;
    public float viewDistance= 20f;

    public float eyePos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        seePlayer();



    }

    void seePlayer() {
        if (Vector3.Distance(transform.position, playerCameraTransform.position) <= viewDistance) {
            //right now it is going to look I think from the center of the body. might change that later


            //not sure if order of x y z matters, shouldnt as long as I keep it consistent I think
            //this is the angles to set as 0
            Vector2 npcHorizontal = new Vector2(transform.forward.x, transform.forward.z);//xz
            Vector2 npcUp = new Vector2(transform.forward.z, transform.forward.y);//zy

            //what we are measuring
            Vector2 playerHorizontal = new Vector2(playerCameraTransform.position.x, playerCameraTransform.position.z) - new Vector2(transform.position.x, transform.position.z);//xz
            Vector3 playerUp = new Vector2(playerCameraTransform.position.z, playerCameraTransform.position.y) - new Vector2(transform.position.z, transform.position.y);//zy

            if (Vector2.Angle(npcHorizontal, playerHorizontal) <= sideFOV) {
                //if(Vector2.Angle(npcUp,playerUp)<=upFOV){
                //    print("Found u");
                //}
                //else {
                //    print("lost");
                //}

                //print("Found u");

                //TODO
                //Add "bones" for the enemy to see
                //raycast to each player "bone"
                //We only raycast the camera is in view. My thinking is that it will seem more fair to the player (i.e poking from behind corner slightly won't get you caught unless you can see the enemy)
                if (Vector2.Angle(npcUp, playerUp) <= upFOV) {
                    //print("Found u");
                }
                else {
                    //print("lost");
                }

            }
            else {
                //print("lost");
            }
        }
        else {
            //print("player too far");
        }

        

        


    }

    private void OnDrawGizmos() {

        

        //Gizmos.DrawSphere(transform.position +transform.forward,0.1f);
        Gizmos.DrawLine(transform.position,transform.position+transform.forward*viewDistance);
        Gizmos.color = Color.blue;
        Vector3 s1 = (transform.forward * viewDistance);
        Vector3 s2 = s1;
        Vector3 u1 = s1;//for some reason multivariable declartion wouldnt work. this is just for testing purposes so it doesnt really matter
        Vector3 u2 = s1;

     

        s1 = Quaternion.AngleAxis(sideFOV, transform.up) * s1;
        Gizmos.DrawLine(transform.position, transform.position + s1);

        s2 = Quaternion.AngleAxis(-sideFOV, transform.up) * s2;
        Gizmos.DrawLine(transform.position, transform.position + s2);

        u1 = Quaternion.AngleAxis(upFOV, transform.right) * u1;
        Gizmos.DrawLine(transform.position, transform.position + u1);

        u2 = Quaternion.AngleAxis(-upFOV, transform.right) * u2;
        Gizmos.DrawLine(transform.position, transform.position + u2);

        Gizmos.DrawLine(transform.position, transform.position + (u1 + s1));
        Gizmos.DrawLine(transform.position, transform.position + (u1 + s2));
        Gizmos.DrawLine(transform.position, transform.position + (u2 + s1));
        Gizmos.DrawLine(transform.position, transform.position + (u2 + s2));

    }
}
