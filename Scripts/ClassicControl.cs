using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCam
{

    /**
    This is for moving the camera in a way typical of most classic and many
    modern simulation, god, and rts games.  Basically, hover and a hieght and 
    use keys or on-screen buttons to rotate and possible raise/lower the camera.
    */
    public class ClassicControl : ACameraControl
    {
        [SerializeField]
        private float rotationSpeed = 60;
        [SerializeField]
        private float moveSpeed = 1;
        [SerializeField]
        private int windowBoundarySize = 10;
        [SerializeField]
        private float minZoomDist = 10, maxZoomDist = -50;
        [SerializeField]
        private float floorY = 0;


        private Vector3 pivot, camproj, twod, newpos;


        // TODO:  This should be based on a (game or lot specific) array of 1 or more descrete heights;
        //        that is, for a classic city-builder or strategy game one high above the world, or
        //        for a Sims style like game one for each floor of the lots highest building little
        //        above those floors -- etc, as other possibilities are hypothetically possible.


        void Awake() {
            pivot   = new Vector3(0, floorY, 0);
            camproj = new Vector3(0, floorY, 0);
            twod    = new Vector3(0, floorY, 0);
            newpos  = new Vector3(0, floorY, 0);
        }


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
            playerEye.transform.localPosition = Vector3.zero;
            zoomDist = 0f;
            base.OnEnable();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
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


        protected void FindPivot() {
            float a = transform.forward.x / transform.forward.y;
            float b = transform.forward.z / transform.forward.y;
            pivot.x = transform.position.x + (a * (pivot.y - transform.position.y));
            pivot.z = transform.position.z + (b * (pivot.y - transform.position.y));
            camproj.x = transform.position.x - pivot.x;
            camproj.y = pivot.y;
            camproj.z = transform.position.z - pivot.z;
        }



        void AdjustHeading() {
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