using UnityEngine;


namespace SimCam
{

    /// <summary>
    /// A kind of master-control for switching between camera control modes
    /// in game.
    /// </summary>
    public class ModeSwitch : MonoBehaviour
    {
        public ACameraControl[] camModes;

        [SerializeField] uint defaultMode; // The mode that will be selected when first created
        [SerializeField] KeyCode nextMode = KeyCode.F; // The key to switch controllers

        private int mode;

        public delegate void modeSwitchHandler(ACameraControl controlMode);
        public static event modeSwitchHandler modeChanged;


        public ACameraControl CurrentMode => camModes[mode];


        // Start is called before the first frame update
        void Awake()
        {
            camModes = gameObject.GetComponents<ACameraControl>();
            mode = (int)defaultMode % (int)camModes.Length;
        }


        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(nextMode)) {
                Increment();
            }
        }


        void OnEnable() {
            camModes[mode].enabled = true;
        }


        /// <summary>
        /// For updating which camera mode are actually active, since otherwise this would be copied
        /// identically in the next several methods.
        /// </summary>
        /// <param name="previous"></param>
        private void UpdateMode(int previous) {
            if(mode != previous) {
                camModes[previous].enabled = false;
                camModes[mode].enabled = true;
                modeChanged?.Invoke(camModes[mode]);
            }
        }


        /// <summary>
        /// Set the camera controller by ID (i.e., its index in the camModes array); this for setting the controller
        /// either in code or through Unity events.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public ACameraControl SetControll(int controller) {
            int previous = mode;
            mode = (int)controller % (int)camModes.Length;
            UpdateMode(previous);
            // Return the current camera mode so it can be accessed elsewhere
            return camModes[mode];
        }


        /// <summary>
        /// Switch the next camera controller
        /// </summary>
        /// <returns></returns>
        public ACameraControl Increment() {
            int previous = mode;
            mode++;
            if(mode >= camModes.Length) mode = 0;
            UpdateMode(previous);
            // Return the current camera mode so it can be accessed elsewhere
            return camModes[mode];
        }


        /// <summary>
        /// Switch to the previous camera controller
        /// </summary>
        /// <returns></returns>
        public ACameraControl Decriment() {
            int previous = mode;
            mode--;
            if(mode < 0) mode = camModes.Length - 1;
            UpdateMode(previous);
            // Return the current camera mode so it can be accessed elsewhere
            return camModes[mode];
        }


        /// <summary>
        /// Change / configure the ground plain for all camera controllers
        /// </summary>
        /// <param name="plain"></param>
        public void SetGroundPlain(GameObject plain) {
            foreach(ACameraControl control in camModes) {
                control.SetGroundPlain(plain);
            }
        }


        /// <summary>
        /// Set the ground plain for the currently active camera controller (only)
        /// </summary>
        /// <param name="plain"></param>
        public void SetCurrentGroundPlain(GameObject plain) {
            camModes[mode].SetGroundPlain(plain);
        }


    }

}
