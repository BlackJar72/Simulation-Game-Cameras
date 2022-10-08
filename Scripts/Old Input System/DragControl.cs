using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCam {

    public class DragControl : ACameraControl {
        [SerializeField]
        private float rotationSpeed = 60;
        [SerializeField]
        private float moveSpeed = 1;
        [SerializeField]
        private int windowBoundarySize = 10;
        [SerializeField]
        private float minZoomDist = 10, maxZoomDist = -50;

        private float headingAngle = 0;
        private float tiltAngle = 0;
        private float zoomDist = 0;

        private Vector3 mousePos, movement;

        private Quaternion heading = Quaternion.identity;
        private Quaternion tilt    = Quaternion.identity;
        private Quaternion angle   = Quaternion.identity;

        // TODO:  This should be based on a (game or lot specific) array of 1 or more descrete heights;
        //        that is, for a classic city-builder or strategy game one high above the world, or
        //        for a Sims style like game one for each floor of the lots highest building little
        //        above those floors -- etc, as other possibilities are hypothetically possible.


        // Start is called before the first frame update
        void Start()
        {
            // TODO/FIXME: This needs to be changed for platforms other than Windows and Linux
            Cursor.lockState = CursorLockMode.Confined;
        }


        protected override void OnEnable()
        {
            // TODO/FIXME: This needs to be changed for platforms other than Windows and Linux
            Cursor.lockState = CursorLockMode.Confined;
            playerEye.transform.position = Vector3.zero;
            zoomDist = 0f;
            base.OnEnable();
        }


        protected override void OnDisable()
        {
            // Will this be needed here?  IDK.
        }


        // Update is called once per frame
        void Update()
        {
            AdjustHeading();
            AdjustPitch();
            SetRotation();
            Move();
            Zoom();
            CheckClicks();
        }


        void AdjustHeading() {
            if(RMouseDown) {
                Vector3 mpos = Input.mousePosition;
                mpos.x -= Screen.width  / 2f;
                mpos.y -= Screen.height / 2f;
                Vector3 mvel = Vector3.zero;
                if(mpos.magnitude > 0) mpos.Normalize();
                mvel.x = Input.GetAxis("Mouse X");
                mvel.y = Input.GetAxis("Mouse Y");
                float rotation = Vector3.Cross(mpos, mvel).z;
                headingAngle += rotation * 2;
                if (headingAngle > 360) headingAngle -= 360;
                else if (headingAngle < 0) headingAngle += 360;
                heading = Quaternion.Euler(0, headingAngle, 0);
            }
        }


        void AdjustPitch() {
            float rotation = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;
            tiltAngle -= rotation;
            tiltAngle = Mathf.Clamp(tiltAngle, 0, 90);
            tilt = Quaternion.Euler(tiltAngle, 0, 0);
        }


        void SetRotation() {
            angle = heading * tilt;
            transform.rotation = angle;
        }


        void Move() {
            movement = Vector3.zero;
            if(LMouseDown) {
                movement.x =  -Input.GetAxis("Mouse X");
                movement.z =  -Input.GetAxis("Mouse Y");
                movement.y = 0;
                movement = heading * movement;
                if (Input.GetKey(KeyCode.LeftShift)) movement.y -= Time.deltaTime;
                if (Input.GetKey(KeyCode.Space)) movement.y += Time.deltaTime;
                //movement.Normalize();
                //movement *= moveSpeed;
                //movement *= Time.deltaTime;
                transform.Translate(movement, Space.World);
            }
        }


        void Zoom() {
            float change = Input.mouseScrollDelta.y;
            zoomDist += change;
            // Due to the direction of movement, max and min are swapped here
            zoomDist = Mathf.Clamp(zoomDist, maxZoomDist, minZoomDist);
            playerEye.transform.localPosition = Vector3.forward * zoomDist;
        }


        void CheckClicks() {
            if(Input.GetMouseButtonUp(0)) {
                Ray ray = playerEye.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnLeftUpCam(hit);
                }
            } else if(Input.GetMouseButtonDown(0)) {
                Ray ray = playerEye.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnLeftDownCam(hit);
                }
            }
            if(Input.GetMouseButtonUp(1)) {
                Ray ray = playerEye.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnRightUpCam(hit);
                }
            } else if(Input.GetMouseButtonDown(1)) {
                Ray ray = playerEye.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnRightDownCam(hit);
                }
            }
            if(Input.GetMouseButtonUp(2)) {
                zoomDist = 0f;
                playerEye.transform.localPosition = Vector3.zero;
            }
        }
    }

}