using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simcam 
{

    // This started as planned as a an interfaced, but for various reasons was changed to
    // and abstract class, though the name has not been changed.
    public abstract class ACameraControl : MonoBehaviour
    {
        [SerializeField]
        protected Camera playerEye;

        public delegate void CamclickHandler(RaycastHit hit);
        public event CamclickHandler LeftholdCam;
        public event CamclickHandler RightholdCam;
        public event CamclickHandler LeftclickCam;
        public event CamclickHandler RightclickCam;

        public Camera GetPlayerEye => GetPlayerEye;

        protected virtual void OnEnable() 
        {

        }


        protected virtual void OnDisable() 
        {
            
        }


        protected virtual void OnLeftholdCam(RaycastHit hit) {
            LeftholdCam?.Invoke(hit);
        }


        protected virtual void OnRightholdCam(RaycastHit hit) {
            RightholdCam?.Invoke(hit);
        }


        protected virtual void OnLeftclickCam(RaycastHit hit) {
            LeftclickCam?.Invoke(hit);
        }


        protected virtual void OnRightclickCam(RaycastHit hit) {
            RightclickCam?.Invoke(hit);
        }

    }

}