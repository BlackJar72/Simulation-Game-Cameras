using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



namespace SimCam {


    /*
    A new approach, WIP>
    TODO: THIS!
    *
    /// <summary>
    /// A single class camera controller with all functionality and the ability to
    /// build custom behavior combinations through delegate arrays (populated through
    /// matching enum arrays that can be accessed in the inspector.
    /// </summary>
    public class SimulationCameraController : MonoBehaviour {

        private char[] delimiterChars = { ' ', ',', '+', ':', '\t' };

        [SerializeField]
        protected Camera playerEye;

        [SerializeField]
        protected string layerString;
        protected int layerMask;

        public delegate void CamclickHandler(RaycastHit hit);
        public static event CamclickHandler LeftholdCam;
        public static event CamclickHandler RightholdCam;
        public static event CamclickHandler LeftclickCam;
        public static event CamclickHandler RightclickCam;

        public delegate void LevelChangeHandler(int level);
        public static event LevelChangeHandler LevelChanged;

        public Camera GetPlayerEye => GetPlayerEye;

        protected bool  LMouseDown;
        protected float LMouseDownAt;
        protected bool  RMouseDown;
        protected float RMouseDownAt;

        // Former member of derived classes made static here
        // so the data will be shared between them;
        protected static float headingAngle = 0;
        protected static float tiltAngle = 0;
        protected static float zoomDist = 0;

        protected static Vector3 mousePos, movement;

        protected static Quaternion heading = Quaternion.identity;
        protected static Quaternion tilt = Quaternion.identity;
        protected static Quaternion angle = Quaternion.identity;

        private float moveSpeedLog;
        private float minSpeedLog;
        private float maxSpeedLog;
        private float moveSpeedFactor;
        private float moveSpeedIncrement;


        [SerializeField]
        private float rotationSpeed = 5;
        [SerializeField]
        private float moveSpeed = 1;

        [SerializeField]
        private float minMoveSpeed = 0.5f;
        [SerializeField]
        private float maxMoveSpeed = 50f;
        [SerializeField]
        private float accellerationFactor = 1;

        [SerializeField]
        private float rotationSpeed = 60;
        [SerializeField]
        private float moveSpeed = 1;
        [SerializeField]
        private int windowBoundarySize = 10;
        [SerializeField]
        private float minZoomDist = 10, maxZoomDist = -50;

        [SerializeField][Tooltip("Altitude (Y coordinates) for levels; must have at least one valid value.")]
        private float[] levelHeights;
        private int level = 0;



        // Start is called before the first frame update
        void Start() {

        }


        // Update is called once per frame
        void Update() {

        }


        #region Angle Controls


        void SetRotation() {
            transform.rotation = heading * tilt;
        }


        void AdjustHeading_Mouse() {
            float rotation = Input.GetAxis("Mouse X") * rotationSpeed;// * Time.deltaTime;
            headingAngle += rotation;
            if(headingAngle> 360) headingAngle-= 360;
            else if(headingAngle < 0) headingAngle += 360;
            heading = Quaternion.Euler(0, headingAngle, 0);
        }


        void AdjustPitch_Mouse() {
            float rotation = Input.GetAxis("Mouse Y") * rotationSpeed;// * Time.deltaTime;
            tiltAngle -= rotation;
            tiltAngle = Mathf.Clamp(tiltAngle, -80, 80);
            tilt = Quaternion.Euler(tiltAngle, 0, 0);
        }


        #endregion

        #region Move Controls


        void Move_WASD_FirstPerson() {
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


        void Move_WASD_Free() {
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


        void Move_Classic() {
            movement = Vector3.zero;
            mousePos = Input.mousePosition;
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


        void Move_ClassicDiscrete() {
            movement = Vector3.zero;
            mousePos = Input.mousePosition;
            if (mousePos.y <= windowBoundarySize) {
                movement.z = -1;
            } else if (mousePos.y >= (Screen.height - windowBoundarySize)) {
                movement.z = 1;
            }
            if (mousePos.x <= windowBoundarySize) {
                movement.x = -1;
            } else if (mousePos.x >= (Screen.width - windowBoundarySize)) {
                movement.x = 1;
            }
            movement = heading * movement;
            if (Input.GetKeyUp(KeyCode.Q)) ChangeLevel(-1);
            if (Input.GetKeyUp(KeyCode.E)) ChangeLevel(1);
            movement.Normalize();
            movement *= moveSpeed;
            movement *= Time.deltaTime;
            transform.Translate(movement, Space.World);
        }




        #endregion
    }

}