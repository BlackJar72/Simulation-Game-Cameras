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
        [SerializeField] ACameraControl[] camModes;
        [SerializeField] uint defaultMode;

        private int mode;

        // Start is called before the first frame update
        void Awake()
        {
            mode = (int)defaultMode % (int)camModes.Length;
        }


        // Update is called once per frame
        void Update()
        {
            
        }


        public void Increment() {
            int previous = mode;
            mode++;
            if(mode > camModes.Length) mode = 0;
            camModes[previous].enabled = false;
            camModes[mode].enabled = true;
        }


        public void Decriment() {
            int previous = mode;
            mode--;
            if(mode < 0) mode = camModes.Length;
            camModes[previous].enabled = false;
            camModes[mode].enabled = true;
        }
    }

}