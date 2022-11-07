using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SimCam
{

    /**
    A kind of master-control for switching between camera control modes 
    in game.
    */
    public class ModeSwitch : MonoBehaviour
    {
        public ACameraControl[] camModes;

        [SerializeField] uint defaultMode;

        private int mode;

        // Start is called before the first frame update
        void Awake()
        {
            camModes = gameObject.GetComponents<ACameraControl>();
            Debug.Log(camModes);
            mode = (int)defaultMode % (int)camModes.Length;
        }


        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F)) {
                Increment();
            }
        }


        void OnEnable() {
            camModes[mode].enabled = true;
        }


        public void Increment() {
            int previous = mode;
            mode++;
            if(mode >= camModes.Length) mode = 0;
            camModes[previous].enabled = false;
            camModes[mode].enabled = true;
        }


        public void Decriment() {
            int previous = mode;
            mode--;
            if(mode < 0) mode = camModes.Length - 1;
            camModes[previous].enabled = false;
            camModes[mode].enabled = true;
        }
    }

}