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
        [SerializeField][Min(0f)]
        private float minZoomDist = 10;
        [SerializeField][Min(0f)]
        private float baseZoomDist = 10;
        [SerializeField][Min(0f)]
        private float maxZoomDist = 50;
        [SerializeField][Min(0f)]
        private float minPitch = 10;
        [SerializeField][Min(0f)]
        private float maxPitch = 90;

        [SerializeField][Tooltip("Camera holder Y coordinates for levels; must have at least one valid value.")]
        private float[] levelHeights;
        private int level = 0;


        private Vector3 pivot;



        // TODO:  This should be based on a (game or lot specific) array of 1 or more descrete heights;
        //        that is, for a classic city-builder or strategy game one high above the world, or
        //        for a Sims style like game one for each floor of the lots highest building little
        //        above those floors -- etc, as other possibilities are hypothetically possible.


        void Awake() {
            pivot   = new Vector3(transform.position.x, levelHeights[level], transform.position.z);
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

            tiltAngle = Mathf.Clamp(tiltAngle, minPitch, maxPitch);
            tilt = Quaternion.Euler(tiltAngle, 0, 0);
            SetRotation();

            zoomDist = baseZoomDist;
            Zoom();

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
            float a = transform.forward.x / transform.forward.y;
            float b = transform.forward.z / transform.forward.y;
            pivot.x = transform.position.x + (a * (pivot.y - transform.position.y));
            pivot.z = transform.position.z + (b * (pivot.y - transform.position.y));
        }


        void AdjustHeading() {
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            headingAngle += rotation;
            if (headingAngle > 360) headingAngle -= 360;
            else if (headingAngle < 0) headingAngle += 360;
            heading = Quaternion.Euler(0, headingAngle, 0);
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
            if (Input.GetKeyUp(KeyCode.Q)) ChangeLevel(-1);
            if (Input.GetKeyUp(KeyCode.E)) ChangeLevel(1);
            movement.Normalize();
            movement *= moveSpeed ;
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
            pivot.y = levelHeights[level];
            Zoom();
            OnLevelChanged(level);
        }


        void Zoom() {
            float change = Input.mouseScrollDelta.y;
            zoomDist -= change;
            zoomDist = Mathf.Clamp(zoomDist, minZoomDist, maxZoomDist);
            playerEye.transform.localPosition = -(Vector3.forward * zoomDist);
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
                zoomDist = baseZoomDist;
                Zoom();
            }
        }


        public void SetLevels(float[] levels) {
            levelHeights = levels;
            ChangeLevel(0);
        }
    }
}