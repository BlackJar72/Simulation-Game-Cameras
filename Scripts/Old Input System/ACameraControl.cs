using UnityEngine;

namespace SimCam
{

    // This started as planned as a an interfaced, but for various reasons was changed to
    // and abstract class, though the name has not been changed.
    public abstract class ACameraControl : MonoBehaviour
    {
        private char[] delimiterChars = { ' ', ',', '+', ':', '\t' };

        [SerializeField] protected Camera playerEye;
        [SerializeField] protected GameObject groundPlain;
        [SerializeField] protected LayerMask groundPlainMask;

        [SerializeField] protected LayerMask layerMask = 0x1;
        [SerializeField] protected LayerMask objectLayerMask = 0x1;

        protected LayerMask defaultLayerMask;
        protected LayerMask defaultObjectLayerMask;

        public delegate void CamclickHandler(RaycastHit hit);
        public static event CamclickHandler LeftMouseDown;
        public static event CamclickHandler RightMouseDown;
        public static event CamclickHandler LeftMouseUp;
        public static event CamclickHandler RightMouseUp;
        public static event CamclickHandler LeftMouseClick;
        public static event CamclickHandler RightMouseClick;

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


        void Awake() {
            defaultLayerMask = layerMask;
            defaultObjectLayerMask = objectLayerMask;
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


        protected virtual void OnLeftDownCam(RaycastHit hit) {
            //Debug.Log("Left button down!");
            if(!LMouseDown) {
                LMouseDown = true;
                LMouseDownAt = Time.time;
            }
            LeftMouseDown?.Invoke(hit);
        }


        protected virtual void OnRightDownCam(RaycastHit hit) {
            if(!RMouseDown) {
                RMouseDown = true;
                RMouseDownAt = Time.time;
            }
            RightMouseDown.Invoke(hit);
        }


        protected virtual void OnLeftUpCam(RaycastHit hit) {
            //Debug.Log("Left button up!");
            if(LMouseDown && (((LMouseDownAt + 0.5f) > Time.time))) {
                LMouseDownAt -= 0.5f;
                LeftMouseClick?.Invoke(hit);
            } else {
                LeftMouseUp?.Invoke(hit);
            }
            LMouseDown = false;
        }


        protected virtual void OnRightUpCam(RaycastHit hit) {
            if(RMouseDown && (((RMouseDownAt + 0.5f) > Time.time))) {
                RMouseDownAt -= 0.5f;
                RightMouseClick?.Invoke(hit);
            } else {
                RightMouseUp?.Invoke(hit);
            }
            RMouseDown = false;
        }


        protected virtual void OnLevelChanged(int level) {
            LevelChanged?.Invoke(level);
        }


        /// <summary>
        /// This gets the position currently under under the cursor (center screen for first-person
        /// views).  This is intended for situations when some element most be moved constantly,
        /// such as an effected spot marker.  It needs to be nullable in case moved off the area.
        /// </summary>
        /// <returns></returns>
        public abstract Vector3? GetCursorLocation();


        /// <summary>
        /// This gets the game object currently under under the cursor (center screen for first-person
        /// views).
        /// </summary>
        /// <returns></returns>
        public abstract GameObject GetCursorObject();


        public virtual void SetLayerMask(LayerMask mask) {
            layerMask = mask;
        }


        public virtual void ResetLayerMask() {
            layerMask = defaultLayerMask;
        }


        public virtual void SetObjectLayerMask(LayerMask mask) {
            objectLayerMask = mask;
        }


        public virtual void ResetObjectLayerMask() {
            objectLayerMask = defaultObjectLayerMask;
        }

    }

}
