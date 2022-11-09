using System;
using UnityEngine;

namespace SimCamNI {

    // This started as planned as a an interfaced, but for various reasons was changed to
    // and abstract class, though the name has not been changed.
    public abstract class ACameraControl : MonoBehaviour {
        private char[] delimiterChars = { ' ', ',', '+', ':', '\t' };

        [SerializeField]
        protected Camera playerEye;

        [SerializeField]
        private float rotationSpeed = 60;
        [SerializeField]
        private float moveSpeed = 1;

        [SerializeField]
        protected string layerString;
        protected int layerMask;

        // Former member of derived classes made static here
        // so the data will be shared between them;
        protected static float headingAngle = 0;
        protected static float tiltAngle = 0;
        protected static float zoomDist = 0;

        protected static Vector3 mousePos, movement;

        protected static Quaternion heading = Quaternion.identity;
        protected static Quaternion tilt = Quaternion.identity;
        protected static Quaternion angle = Quaternion.identity;

        private Vector3 pivot, camproj, twod, newpos;

#region Event definitions
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
#endregion


        void Awake() {
            layerString.Trim();
            if(layerString.Length > 0) {
                layerMask = 0;
                string[] layers = layerString.Split(delimiterChars);
                for(int i = 0; i < layers.Length; i++) {
                    try {
                        int l = int.Parse(layers[i]);
                        layerMask ^= 0x1 << l;
                    } catch(Exception) {/*Ignore what is almost certainly a non-integer*/}
                }
            } else {
                layerMask = 0x1;
            }
        }


        protected virtual void OnEnable()
        {

        }


        protected virtual void OnDisable()
        {
            if(RMouseDown) RMouseDownAt -= 0.5f;
            if(LMouseDown) LMouseDownAt -= 0.5f;
            RMouseDown = LMouseDown = false;
        }


        void Start() {

        }


        #region Event Publishing

        protected virtual void OnLeftDownCam(RaycastHit hit) {
            //Debug.Log("Left button down!");
            if(!LMouseDown) {
                LMouseDown = true;
                LMouseDownAt = Time.time;
            }
            LeftholdCam?.Invoke(hit);
        }


        protected virtual void OnRightDownCam(RaycastHit hit) {
            if(!RMouseDown) {
                RMouseDown = true;
                RMouseDownAt = Time.time;
            }
            RightholdCam?.Invoke(hit);
        }


        protected virtual void OnLeftUpCam(RaycastHit hit) {
            //Debug.Log("Left button up!");
            if(LMouseDown && (((LMouseDownAt + 0.5f) > Time.time))) {
                LMouseDownAt -= 0.5f;
                LeftclickCam?.Invoke(hit);
            }
            LMouseDown = false;
        }


        protected virtual void OnRightUpCam(RaycastHit hit) {
            if(RMouseDown && (((RMouseDownAt + 0.5f) > Time.time))) {
                RMouseDownAt -= 0.5f;
                RightclickCam?.Invoke(hit);
            }
            RMouseDown = false;
        }


        protected virtual void OnLevelChanged(int level) {
            LevelChanged?.Invoke(level);
        }

        #endregion

        #region Headding Controls

        /// <summary>
        /// Sets the full rotation of the camera holder, combining horizontal heading (yaw)
        /// and verticle view angle (pitch).  This is used by all cameras.
        /// </summary>
        /// <returns></returns>
        protected void SetRotation() {
            angle = heading * tilt;
            transform.rotation = angle;
        }


        /// <summary>
        /// A utility method to find the pivot point around with to orbid the camera in classic mode.
        /// </summary>
        protected void FindPivot() {
            float a = transform.forward.x / transform.forward.y;
            float b = transform.forward.z / transform.forward.y;
            pivot.x = transform.position.x + (a * (pivot.y - transform.position.y));
            pivot.z = transform.position.z + (b * (pivot.y - transform.position.y));
            camproj.x = transform.position.x - pivot.x;
            camproj.y = pivot.y;
            camproj.z = transform.position.z - pivot.z;
        }


        /// <summary>
        /// Rotation the camera around a pivot point for camera orbiting as in class controls
        /// </summary>
        void AdjustHeading_Orbit() {
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            FindPivot();
            Quaternion q = Quaternion.Euler(0, -rotation, 0);
            camproj = q * camproj;
            newpos = pivot + camproj;
            newpos.y = transform.position.y;
            transform.position = newpos;
            transform.LookAt(pivot);
            heading = heading * q;
            headingAngle = Quaternion.Angle(heading, Quaternion.identity);
        }




        #endregion


        #region Movement

        #endregion


        #region Scroll Effects

        #endregion




    }

}