using System;
using UnityEngine;

namespace SimCam
{

    // This started as planned as a an interfaced, but for various reasons was changed to
    // and abstract class, though the name has not been changed.
    public abstract class ACameraControl : MonoBehaviour
    {
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

    }

}