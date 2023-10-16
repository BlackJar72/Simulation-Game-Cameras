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
        [SerializeField][Min(0f)]
        private float minZoomDist = 10;
        [SerializeField][Min(0f)]
        private float baseZoomDist = 10;
        [SerializeField][Min(0f)]
        private float maxZoomDist = 50;
        [SerializeField]
        private float floorY = 0;
        [SerializeField][Min(0f)]
        private float minPitch = 10;
        [SerializeField][Min(0f)]
        private float maxPitch = 90;


        private Vector3 pivot, tpos;

        [SerializeField]
        private LayerMask UILayer;


        // TODO:  This should be based on a (game or lot specific) array of 1 or more descrete heights;
        //        that is, for a classic city-builder or strategy game one high above the world, or
        //        for a Sims style like game one for each floor of the lots highest building little
        //        above those floors -- etc, as other possibilities are hypothetically possible.


        void Awake() {
            pivot   = new Vector3(transform.position.x, floorY, transform.position.z);
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
            tiltAngle = Mathf.Clamp(tiltAngle, minPitch, maxPitch);
            tilt = Quaternion.Euler(tiltAngle, 0, 0);
            SetRotation();
            FindPivot();
            zoomDist = (transform.position - pivot).magnitude;
            transform.position = pivot;
            Zoom();
            //playerEye.transform.localPosition = Vector3.zero;
            //zoomDist = 0f;
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
        }



        void AdjustHeading() {
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            headingAngle -= rotation;
            if(headingAngle > 360) headingAngle-= 360;
            else if(headingAngle < 0) headingAngle += 360;
            heading = Quaternion.Euler(0, headingAngle, 0);
        }


        void AdjustPitch() {
            float rotation = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;
            tiltAngle += rotation;
            tiltAngle = Mathf.Clamp(tiltAngle, minPitch, maxPitch);
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
            if(transform.position.y < floorY) {
                tpos = transform.position;
                tpos.y = floorY;
                transform.position = tpos;
            }
        }


        void Zoom() {
            float change = Input.mouseScrollDelta.y;
            zoomDist -= change;
            zoomDist = Mathf.Clamp(zoomDist, minZoomDist, maxZoomDist);
            playerEye.transform.localPosition = -(Vector3.forward * zoomDist);
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
                zoomDist = baseZoomDist;
                Zoom();
            }
        }


        public override Vector3? GetCursorLocation() {
            Ray ray = playerEye.ScreenPointToRay(Input.mousePosition);
            // Don't give a world position if on the UI
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, float.PositiveInfinity, UILayer)) {
                return null;
            }
            if(Physics.Raycast(ray, out hit, playerEye.farClipPlane, groundPlainMask)) {
                return hit.point;
            }
            // Don't give a world position if somehow off screen or not pointing a ground plain
            return null;
        }


        public override GameObject GetCursorObject() {
            Ray ray = playerEye.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, float.PositiveInfinity, UILayer)) {
                return null;
            }
            if(Physics.Raycast(ray, out hit, playerEye.farClipPlane, layerMask)) {
                return hit.collider.gameObject;
            }
            return null;
        }



    }

}
