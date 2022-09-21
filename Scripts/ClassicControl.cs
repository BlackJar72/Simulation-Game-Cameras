using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simcam 
{

    /**
    This is for moving the camera in a way typical of most classic and many
    modern simulation, god, and rts games.  Basically, hover and a hieght and 
    use keys or on-screen buttons to rotate and possible raise/lower the camera.
    */
    public class ClassicControl : ICameraControl
    {
        [SerializeField]
        private float rotationSpeed = 60;
        [SerializeField]
        private float moveSpeed = 1;
        [SerializeField]
        private int windowBoundarySize = 10;
        [SerializeField]
        private Camera camera;

        public float headingAngle = 0;
        private float tiltAngle = 0;

        private Vector3 mousePos, movement;

        public Quaternion heading = Quaternion.identity;
        private Quaternion tilt    = Quaternion.identity;

        public float debugAxis;

        // TODO:  This should be based on a (game or lot specific) array of 1 or more descrete heights;
        //        that is, for a classic city-builder or strategy game one high above the world, or
        //        for a Sims style like game one for each floor of the lots highest building little
        //        above those floors -- etc, as other possibilities are hypothetically possible.

        // TODO:  Need to implement a zoom function, likely based slighting the camera along a line through
        //        the camera holding empty to which this is attached along the local forward direction and
        //        with a definite minimum and maximum of course (we certainly don't want them moving to though
        //        the floor or even the ceiling of a viewed room for example).

        // TODO: Need to implement ray-based camera picking so as to interact with the game invironment.


        // Start is called before the first frame update
        void Start()
        {
            // TODO/FIXME: This needs to be changed for platforms other than Windows and Linux
            Cursor.lockState = CursorLockMode.Confined;
        }

        /*
        protected override void OnEnable() 
        {
            // TODO/FIXME: This needs to be changed for platforms other than Windows and Linux
            Cursor.lockState = CursorLockMode.Confined;
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
            debugAxis = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            if(Mathf.Abs(debugAxis) > 10) Debug.Log(debugAxis);
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            headingAngle += rotation;
            if(headingAngle > 360) headingAngle-= 360;
            else if(headingAngle < 0) headingAngle += 360;
            heading = Quaternion.Euler(0, headingAngle, 0);
        }


        void AdjustPitch() {
            float rotation = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;
            tiltAngle -= rotation;
            tiltAngle = Mathf.Clamp(tiltAngle, 0, 90);
            tilt = Quaternion.Euler(tiltAngle, 0, 0);
        }


        void SetRotation() {
            transform.rotation = heading * tilt;
        }


        void Move() {
            /*Vector3*/ movement = Vector3.zero;
            /*Vector3*/ mousePos = Input.mousePosition;
            if(mousePos.y <= windowBoundarySize) {
                movement.z = -1;
            } else if(mousePos.y >= (Screen.height - windowBoundarySize)) {
                movement.z = 1;
            }
            if(mousePos.x <= windowBoundarySize) {
                movement.x = -1;
            } else if(mousePos.x >= (Screen.width - windowBoundarySize)) {
                movement.x = 1;
            }
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