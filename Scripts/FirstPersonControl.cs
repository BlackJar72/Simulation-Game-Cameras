using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simcam 
{

    /**
    This is for moving in a way typical of flying in a first person game 
    (i.e., a "fly-cam"), similar to creative and spectator modes in one 
    well know game.  This version uses the old input system; a version
    using the new input system will come later.
    */
    public class FirstPersonControl : ICameraControl
    {
        [SerializeField]
        private float rotationSpeed = 1;
        [SerializeField]
        private float moveSpeed = 1;

        public float headingAngle = 0;
        public float tiltAngle = 0;

        public Vector3 movement;

        public Quaternion heading = Quaternion.identity;
        public Quaternion tilt    = Quaternion.identity;


        //*
        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }//*/

       /*
       protected override void OnEnable()
        {
            // Will this be needed here?  IDK.
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
            float rotation = Input.GetAxis("Mouse X") * rotationSpeed / Time.deltaTime;
            headingAngle += rotation;
            if(headingAngle> 360) headingAngle-= 360;
            else if(headingAngle < 0) headingAngle += 360;
            heading = Quaternion.Euler(0, headingAngle, 0);
        }


        void AdjustPitch() {
            float rotation = Input.GetAxis("Mouse Y") * rotationSpeed / Time.deltaTime;
            tiltAngle += rotation;
            tiltAngle = Mathf.Clamp(tiltAngle, -80, 80);
            tilt = Quaternion.Euler(tiltAngle, 0, 0);
        }


        void SetRotation() {
            transform.rotation = heading * tilt;
        }


        void Move() {
            /*Vector3*/ movement = Vector3.zero;
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

    }

}
