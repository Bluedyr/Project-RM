using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour{


    public Transform playerCameraTransform;

    public float fov = 45f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        



    }

    void seePlayer() {
        if (Vector3.Angle(transform.forward, playerCameraTransform.position) <= fov) {


            //TODO
            //Add "bones" for the enemy to see
            //raycast to each player "bone"
            //We only raycast the camera is in view. My thinking is that it will seem more fair to the player (i.e poking from behind corner slightly won't get you caught unless you can see the enemy)
        }
    }
}
