using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCam
{

    /**
    This is for moving in a way typical of flying in a first person game 
    (i.e., a "fly-cam"), similar to creative and spectator modes in one 
    well know game.  This version uses the old input system; a version
    using the new input system will come later.
    */
    public class FirstPersonControl : ACameraControl
    {
        [SerializeField]
        private float rotationSpeed = 5;
        [SerializeField]
        private float moveSpeed = 1;

        private float headingAngle = 0;
        private float tiltAngle = 0;

        private Vector3 movement;

        private Quaternion heading = Quaternion.identity;
        private Quaternion tilt    = Quaternion.identity;



        //*
        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }//*/

       /*
       protected override void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            base.OnEnable();
        }


        protected override void OnDisable() 
        {
            // Will this be needed here?  IDK.            
        }
        */

        // Update is called once per frame
        void Update()
        {
            AdjustHeading();
            AdjustPitch();
            SetRotation();
            Move();
        }


        void AdjustHeading() {
            float rotation = Input.GetAxis("Mouse X") * rotationSpeed;// * Time.deltaTime;
            headingAngle += rotation;
            if(headingAngle> 360) headingAngle-= 360;
            else if(headingAngle < 0) headingAngle += 360;
            heading = Quaternion.Euler(0, headingAngle, 0);
        }


        void AdjustPitch() {
            float rotation = Input.GetAxis("Mouse Y") * rotationSpeed;// * Time.deltaTime;
            tiltAngle -= rotation;
            tiltAngle = Mathf.Clamp(tiltAngle, -80, 80);
            tilt = Quaternion.Euler(tiltAngle, 0, 0);
        }


        void SetRotation() {
            transform.rotation = heading * tilt;
        }


        void Move() {
            movement = Vector3.zero;
            movement.z = Input.GetAxis("Vertical");
            movement.x = Input.GetAxis("Horizontal");
            movement = heading * movement;
            if(Input.GetKey(KeyCode.LeftShift)) movement.y -= 1;
            if(Input.GetKey(KeyCode.Space)) movement.y += 1;
            movement.Normalize();
            movement *= moveSpeed ;
            movement *= Time.deltaTime;
            transform.Translate(movement, Space.World);
        }


        void CheckClicks() {
            if(Input.GetMouseButtonUp(0)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                            out RaycastHit hit, playerEye.farClipPlane)) {
                    OnLeftUpCam(hit);
                }
            } else if(Input.GetMouseButtonDown(0)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                        out RaycastHit hit, playerEye.farClipPlane)) {
                    OnLeftDownCam(hit);
                }
            }
            if(Input.GetMouseButton(1)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                            out RaycastHit hit, playerEye.farClipPlane)) {
                    OnRightUpCam(hit);
                }
            } else if(Input.GetMouseButtonDown(1)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                        out RaycastHit hit, playerEye.farClipPlane)) {
                    OnRightDownCam(hit);
                }
            }
        }

    }

}
