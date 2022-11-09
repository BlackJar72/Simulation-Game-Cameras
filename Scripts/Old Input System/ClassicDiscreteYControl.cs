using UnityEngine;


namespace SimCam {

    /**
    This is for moving the camera in a way typical of most classic and many
    modern simulation, god, and rts games.  Basically, hover and a hieght and
    use keys or on-screen buttons to rotate and possible raise/lower the camera.
    */
    public class ClassicDiscreteYControl : ACameraControl {
        [SerializeField]
        private float rotationSpeed = 60;
        [SerializeField]
        private float moveSpeed = 1;
        [SerializeField]
        private int windowBoundarySize = 10;
        [SerializeField]
        private float minZoomDist = 10, maxZoomDist = -50;

        [SerializeField][Tooltip("Camera holder Y coordinates for levels; must have at least one valid value.")]
        private float[] levelHeights;
        private int level = 0;
        [SerializeField][Tooltip("Floor Y coordinates for levels; should be the same size as level heights.")]
        private float[] floorHeights;


        private Vector3 pivot, camproj, twod, newpos;



        // TODO:  This should be based on a (game or lot specific) array of 1 or more descrete heights;
        //        that is, for a classic city-builder or strategy game one high above the world, or
        //        for a Sims style like game one for each floor of the lots highest building little
        //        above those floors -- etc, as other possibilities are hypothetically possible.


        void Awake() {
            if((floorHeights == null) || (floorHeights.Length != levelHeights.Length)) {
                float[] tmpFloorYs = floorHeights;
                floorHeights = new float[levelHeights.Length];
                for(int i = 0; i <= levelHeights.Length; i++) {
                    floorHeights[i] = tmpFloorYs[i];
                }
            }
            for(int i = 0; i < floorHeights.Length; i++) {
                if(floorHeights[i] >= (levelHeights[i] - minZoomDist)) {
                    floorHeights[i] =  levelHeights[i] - minZoomDist;
                }
            }
            pivot   = new Vector3(0, floorHeights[level], 0);
            camproj = new Vector3(0, floorHeights[level], 0);
            twod    = new Vector3(0, floorHeights[level], 0);
            newpos  = new Vector3(0, floorHeights[level], 0);
        }


        // Start is called before the first frame update
        void Start() {
            // TODO/FIXME: This needs to be changed for platforms other than Windows and Linux
            Cursor.lockState = CursorLockMode.Confined;
            ChangeLevel(0);
        }


        protected override void OnEnable()
        {
            // TODO/FIXME: This needs to be changed for platforms other than Windows and Linux
            Cursor.lockState = CursorLockMode.Confined;
            playerEye.transform.localPosition = Vector3.zero;
            ChangeLevel(0);
            zoomDist = 0f;
            base.OnEnable();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            // Will this be needed here?  IDK.
        }


        // Update is called once per frame
        void Update() {
            AdjustHeading();
            AdjustPitch();
            SetRotation();
            Move();
            Zoom();
            CheckClicks();
        }


        protected void FindPivot() {
            if(transform.forward.y != 0f) {
                float a = transform.forward.x / transform.forward.y;
                float b = transform.forward.z / transform.forward.y;
                pivot.x = transform.position.x + (a * (pivot.y - transform.position.y));
                pivot.y = camproj.y = twod.y = newpos.y = floorHeights[level];
                pivot.z = transform.position.z + (b * (pivot.y - transform.position.y));
            } else {
                pivot = playerEye.transform.position;
                camproj.y = twod.y = newpos.y = pivot.y;
            }
            camproj.x = transform.position.x - pivot.x;
            camproj.y = pivot.y;
            camproj.z = transform.position.z - pivot.z;
        }



        void AdjustHeading() {
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            FindPivot();
            if(transform.forward.y != 0f) {
                rotation = -rotation;
            }
            Quaternion q = Quaternion.Euler(0, rotation, 0);
            camproj = q * camproj;
            newpos = pivot + camproj;
            newpos.y = transform.position.y;
            transform.position = newpos;
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


        void ChangeLevel(int change) {
            level += change;
            if(level < 0) level = 0;
            else if(level >= levelHeights.Length) level = levelHeights.Length - 1;
            Vector3 pos = transform.position;
            pos.y = levelHeights[level];
            transform.position = pos;
            pivot.y = camproj.y = twod.y = newpos.y = floorHeights[level];
            OnLevelChanged(level);
        }


        void Zoom() {
            float change = Input.mouseScrollDelta.y;
            zoomDist += change;
            // Due to the direction of movement, max and min are swapped here
            zoomDist = Mathf.Clamp(zoomDist, maxZoomDist, minZoomDist);
            playerEye.transform.localPosition = Vector3.forward * zoomDist;
        }


        void CheckClicks() {
            if (Input.GetMouseButtonUp(0)) {
                Ray ray = playerEye.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnLeftUpCam(hit);
                }
            } else if (Input.GetMouseButtonDown(0)) {
                Ray ray = playerEye.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnLeftDownCam(hit);
                }
            }
            if (Input.GetMouseButtonUp(1)) {
                Ray ray = playerEye.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnRightUpCam(hit);
                }
            } else if (Input.GetMouseButtonDown(1)) {
                Ray ray = playerEye.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnRightDownCam(hit);
                }
            }
            if (Input.GetMouseButtonUp(2)) {
                zoomDist = 0f;
                playerEye.transform.localPosition = Vector3.zero;
            }
        }


        public void SetLevels(float[] levels) {
            levelHeights = levels;
            ChangeLevel(0);
        }
    }
}