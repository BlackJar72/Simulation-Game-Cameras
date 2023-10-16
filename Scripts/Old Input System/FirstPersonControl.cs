using UnityEngine;

namespace SimCam
{

    /**
    This is for moving in a way typical of flying in a first person game 
    (i.e., a "fly-cam"), similar to creative and spectator modes in one 
    well know game.  This version uses the old input system; a version
    using the new input system will come later.
    */
    public class FirstPersonControl : ACameraControl
    {
        [SerializeField]
        protected float rotationSpeed = 5;
        [SerializeField]
        protected float moveSpeed = 1;

        [SerializeField]
        protected float minMoveSpeed = 0.5f;
        [SerializeField]
        protected float maxMoveSpeed = 50f;
        [SerializeField]
        protected float accellerationFactor = 1;

        protected float moveSpeedLog;
        protected float minSpeedLog;
        protected float maxSpeedLog;
        protected float moveSpeedFactor;
        protected float moveSpeedIncrement;


        //*
        // Start is called before the first frame update
        protected void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }//*/


        protected void Awake() {
            moveSpeedFactor = moveSpeed;
            moveSpeedLog = 0;
            minSpeedLog = (Mathf.Log10(minMoveSpeed / moveSpeedFactor));
            maxSpeedLog = (Mathf.Log10(maxMoveSpeed / moveSpeedFactor));
            moveSpeedIncrement = (maxSpeedLog - minSpeedLog) / accellerationFactor;
        }


       protected override void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
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
        protected void Update()
        {
            AdjustHeading();
            AdjustPitch();
            SetRotation();
            Accelerate();
            Move();
            CheckClicks();
        }


        protected void Accelerate() {
            float change = Input.mouseScrollDelta.y;
            moveSpeedLog += change * moveSpeedIncrement * Time.deltaTime;
            if(moveSpeedLog > maxSpeedLog) moveSpeedLog = maxSpeedLog;
            else if(moveSpeedLog < minSpeedLog) moveSpeedLog = minSpeedLog;
            moveSpeed = Mathf.Pow(10, moveSpeedLog) * moveSpeedFactor;
        }


        protected void AdjustHeading() {
            float rotation = Input.GetAxis("Mouse X") * rotationSpeed;// * Time.deltaTime;
            headingAngle += rotation;
            if(headingAngle> 360) headingAngle-= 360;
            else if(headingAngle < 0) headingAngle += 360;
            heading = Quaternion.Euler(0, headingAngle, 0);
        }


        protected void AdjustPitch() {
            float rotation = Input.GetAxis("Mouse Y") * rotationSpeed;// * Time.deltaTime;
            tiltAngle -= rotation;
            tiltAngle = Mathf.Clamp(tiltAngle, -80, 80);
            tilt = Quaternion.Euler(tiltAngle, 0, 0);
        }


        protected void SetRotation() {
            transform.rotation = heading * tilt;
        }


        protected virtual void Move() {
            movement = Vector3.zero;
            movement.z = Input.GetAxis("Vertical");
            movement.x = Input.GetAxis("Horizontal");
            movement = heading * movement;
            if(Input.GetKey(KeyCode.LeftShift)) movement.y -= 1;
            if(Input.GetKey(KeyCode.Space)) movement.y += 1;
            movement.Normalize();
            movement *= moveSpeed ;
            movement *= Time.deltaTime;
            transform.Translate(movement, Space.World);
        }


        protected void CheckClicks() {
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
                    out RaycastHit hit, playerEye.farClipPlane, layerMask)) {
                return hit.collider.gameObject;
            }
            return null;
        }




    }

}
