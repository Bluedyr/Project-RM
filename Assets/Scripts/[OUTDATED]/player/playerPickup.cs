using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerPickup : MonoBehaviour{

    public LayerMask interactables;//TODO change to pickup
    public float raycastDist = 2f;
   
    public float throwPower = 10f;
    public float holdSpeed = 100f;
 

    public KeyCode pickUp = KeyCode.F;

    private GameObject pickedUp;

    KeyCode rotate = KeyCode.R;
    public float  rotSpeed = 100f;

    public float dropDist = 0.3f;

    Vector3 prl;
    float pxl;//previous x axis (during look)
    float pyl;//previous y axis (during look)
    float pzl;

   

    float pxr;//previous x axis (during rotate)
    float pyr;//previous y axis (during rotate)

    Quaternion r;
    
    void Start()    {
       
    }

    // Update is called once per frame
    void Update()    {
        
        RaycastHit hit;

        //if we currently have something picked up
        if (pickedUp != null) {

            if (Input.GetKeyDown(pickUp)) {         //drops it
                pickedUp.GetComponent<Rigidbody>().useGravity = true;
                
                pickedUp.transform.parent = null;
                pickedUp = null;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0)) {    //throws it
                pickedUp.GetComponent<Rigidbody>().velocity = transform.forward * (throwPower/ pickedUp.GetComponent<Rigidbody>().mass);
                pickedUp.GetComponent<Rigidbody>().useGravity = true;
               
                pickedUp.transform.parent = null;
                pickedUp = null;
            }
            else if (Vector3.Distance(transform.position, pickedUp.transform.position)>dropDist) {
                pickedUp.GetComponent<Rigidbody>().useGravity = true;
                pickedUp.transform.parent = null;
                pickedUp = null;
            }

        }
        //if we dont have something picked up
        else if (Physics.Raycast(transform.position, transform.forward,out hit, raycastDist, interactables)) {
            if (Input.GetKeyDown(pickUp)) {
                hit.rigidbody.useGravity = false;
               // hit.rigidbody.freezeRotation = true;
                pickedUp = hit.rigidbody.gameObject;
                pickedUp.transform.parent = this.transform;
                
            }
            
        }

        Debug.DrawRay(transform.position, transform.forward*raycastDist,Color.blue);

    }

    private void FixedUpdate() {
        if (pickedUp!=null) {
            Rigidbody rb = pickedUp.GetComponent<Rigidbody>();
            //Move the object towards the pick up point
            Vector3 raypos = transform.position + transform.forward * raycastDist;
            Vector3 objpos = pickedUp.transform.position;

            Vector3 force = -1*(objpos - raypos) * holdSpeed * Time.deltaTime;
            
            rb.velocity = new Vector3(0,0,0);
            rb.angularVelocity = new Vector3(0, 0, 0);
            rb.AddForce(force,ForceMode.VelocityChange);
                      
            

           // handles rotation while holding R(maybe remove ?)
            if (Input.GetKeyDown(rotate)) {
                //gets the axis at the beginning of pressing r. this gets set only once when you press r
                pxr = Input.GetAxis("Mouse X");
                pyr = Input.GetAxis("Mouse Y");
            }
            if (Input.GetKey(rotate)) {
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) {
                    Vector3 rot = new Vector3(((Input.GetAxis("Mouse X")) - pxr) * rotSpeed, ((Input.GetAxis("Mouse Y")) - pyr) * rotSpeed, 0);

                    pickedUp.transform.Rotate(transform.up, -rot.x * Time.deltaTime, Space.World);
                    pickedUp.transform.Rotate(transform.right, rot.y * Time.deltaTime, Space.World);//change these to a single quarternian if I can
                    //pickedUp.transform.RotateAround(transform.position + transform.forward * raycastDist, transform.right,rot.y);
                }
            }
        }
    } 
}
