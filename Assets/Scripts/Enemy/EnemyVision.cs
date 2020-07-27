using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour{


    public Transform playerCameraTransform;

    public float sideFOV = 45f;
    public float upFOV = 20f;

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
        //right now it is going to look I think from the center of the body. might change that later


        //not sure if order of x y z matters, shouldnt as long as I keep it consistent I think
        Vector2 npcHorizontal = new Vector2(transform.forward.x,transform.forward.z);//xz
        Vector2 npcUp = new Vector2(transform.forward.z,transform.forward.y);//zy

        Vector2 playerHorizontal = new Vector2(playerCameraTransform.position.x, playerCameraTransform.position.z) - new Vector2(transform.position.x,transform.position.z);//xz
        Vector3 playerUp =  new Vector2(playerCameraTransform.position.z, playerCameraTransform.position.y) - new Vector2(transform.position.z, transform.position.y);//zy

        if (Vector2.Angle(npcHorizontal, playerHorizontal) <= sideFOV) {
            //if(Vector2.Angle(npcUp,playerUp)<=upFOV){
            //    print("Found u");
            //}
            //else {
            //    print("lost");
            //}
            print("Found u");
            //TODO
            //Add "bones" for the enemy to see
            //raycast to each player "bone"
            //We only raycast the camera is in view. My thinking is that it will seem more fair to the player (i.e poking from behind corner slightly won't get you caught unless you can see the enemy)
        }
        else {
            print("lost");
        }
    }

    private void OnDrawGizmos() {

        //Gizmos.DrawSphere(transform.position +transform.forward,0.1f);
        Gizmos.DrawLine(transform.position,transform.position+transform.forward);
        Gizmos.DrawSphere(transform.position + new Vector3(0, eyePos, 0), 0.1f);
    }
}
