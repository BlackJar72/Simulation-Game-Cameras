using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimCam {

    /**
    This is for this is for first person flying that always moves relative to the
    current heading of the camera.  This is the simplest, and (in my opinion) least
    useful.
    */
    public class FreeCamControl : ACameraControl
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
            CheckClicks();
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
            if(Input.GetKey(KeyCode.LeftShift)) movement.y -= 1;
            if(Input.GetKey(KeyCode.Space)) movement.y += 1;
            movement.Normalize();
            movement *= moveSpeed ;
            movement *= Time.deltaTime;
            transform.Translate(movement);
        }


        void CheckClicks() {
            if(Input.GetMouseButtonUp(0)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                        out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnLeftUpCam(hit);
                }
            } else if(Input.GetMouseButtonDown(0)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                        out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnLeftDownCam(hit);
                }
            }
            if(Input.GetMouseButtonUp(1)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                        out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnRightUpCam(hit);
                }
            } else if(Input.GetMouseButtonDown(1)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                        out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnRightDownCam(hit);
                }
            }
        }

    }

}