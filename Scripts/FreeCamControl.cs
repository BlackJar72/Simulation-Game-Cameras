using UnityEngine;
namespace SimCam {

    /**
    This is for this is for first person flying that always moves relative to the
    current heading of the camera.  This is the simplest, and (in my opinion) least
    useful.
    */
    public class FreeCamControl : ACameraControl
    {
        [SerializeField]
        private float rotationSpeed = 5f;
        [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField]
        private float minMoveSpeed = 0.5f;
        [SerializeField]
        private float maxMoveSpeed = 50f;
        [SerializeField]
        private float fullAccelTime = 30;

        private float moveSpeedLog;
        private float minSpeedLog;
        private float maxSpeedLog;
        private float moveSpeedFactor;
        private float moveSpeedIncrement;

        [SerializeField] protected KeyCode flyUp =  KeyCode.Space;
        [SerializeField] protected KeyCode flyDown = KeyCode.LeftShift;


        //*
        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }//*/


        void Awake() {
            moveSpeedFactor = moveSpeed;
            moveSpeedLog = 0;
            minSpeedLog = (Mathf.Log(minMoveSpeed / moveSpeedFactor));
            maxSpeedLog = (Mathf.Log(maxMoveSpeed / moveSpeedFactor));
            moveSpeedIncrement = (maxSpeedLog - minSpeedLog) / fullAccelTime;
        }


        protected override void OnEnable()
         {
             Cursor.lockState = CursorLockMode.Locked;
             Cursor.visible = false;
             transform.position = playerEye.transform.position;
             playerEye.transform.localPosition = Vector3.zero;
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
            Accelerate();
            Move();
            CheckClicks();
        }


        void Accelerate() {
            float change = Input.mouseScrollDelta.y;
            moveSpeedLog += change * moveSpeedIncrement * Time.deltaTime;
            if(moveSpeedLog > maxSpeedLog) moveSpeedLog = maxSpeedLog;
            else if(moveSpeedLog < minSpeedLog) moveSpeedLog = minSpeedLog;
            moveSpeed = Mathf.Pow(10, moveSpeedLog) * moveSpeedFactor;
        }


        void AdjustHeading() {
            float rotation = Input.GetAxis("Mouse X") * rotationSpeed;// * Time.deltaTime;
            headingAngle += rotation;
            if(headingAngle> 360) headingAngle-= 360;
            else if(headingAngle < 0) headingAngle += 360;
            heading = Quaternion.Euler(0, headingAngle, 0);
        }


        void AdjustPitch() {
            float rotation = Input.GetAxis("Mouse Y") * rotationSpeed;// * Time.deltaTime;
            tiltAngle -= rotation;
            tiltAngle = Mathf.Clamp(tiltAngle, -80, 80);
            tilt = Quaternion.Euler(tiltAngle, 0, 0);
        }


        void SetRotation() {
            transform.rotation = heading * tilt;
        }


        void Move() {
            movement = Vector3.zero;
            movement.z = Input.GetAxis("Vertical");
            movement.x = Input.GetAxis("Horizontal");
            if(Input.GetKey(flyDown)) movement.y -= 1;
            if(Input.GetKey(flyUp)) movement.y += 1;
            movement.Normalize();
            movement *= moveSpeed ;
            movement *= Time.deltaTime;
            transform.Translate(movement);
        }


        void CheckClicks() {
            if(Input.GetMouseButtonUp(0)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                        out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnLeftUpCam(hit);
                }
            } else if(Input.GetMouseButtonDown(0)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                        out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnLeftDownCam(hit);
                }
            }
            if(Input.GetMouseButtonUp(1)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                        out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnRightUpCam(hit);
                }
            } else if(Input.GetMouseButtonDown(1)) {
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                        out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                    OnRightDownCam(hit);
                }
            }
        }


        public override Vector3? GetCursorLocation() {
            // Being off the screen or or over the UI should not be a possibility when aiming through
            // the center of the screen; if so you need to fix your UI!
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                    out RaycastHit hit, playerEye.farClipPlane, groundPlainMask)) {
                return hit.point;
            }
            return null;
        }


        public override GameObject GetCursorObject() {
            // Being off the screen or or over the UI should not be a possibility when aiming through
            // the center of the screen; if so you need to fix your UI!
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                    out RaycastHit hit, playerEye.farClipPlane, objectLayerMask)) {
                return hit.collider.gameObject;
            }
            return null;
        }

    }

}
