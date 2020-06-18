using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Title: CameraController
 * Author: Marc Askew
 */

public class CameraController : MonoBehaviour
{

    //[HideInInspector]
    
    [Header("Variables")]
    
    public Transform player;


    public bool lockCursor;
    public bool inverted;
    public bool freezeCamera;
    public float mouseSensitivity = 10f;
    [Tooltip("The player can only look X degrees down and Y degrees up")]
    public Vector2 pitchMinMax = new Vector2(-90f,90f);

    float yaw;//rotation on the y axis  (side to side)
    float pitch;//rotation on the x axis    (up and down)

    float currentPitch;

    float invert;
    
    // Start is called before the first frame update
    void Start() {
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (inverted) {
            invert = 1;
        }
        else {
            invert = -1;
        }
    }

    // Update is called once per frame
    void Update() {

        if (!freezeCamera) {
            yaw = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            pitch = invert * Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


            currentPitch += pitch;
            currentPitch = Mathf.Clamp(currentPitch, pitchMinMax.x, pitchMinMax.y);

            transform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
            player.Rotate(Vector3.up * yaw);
        }
        
    }


}
