using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace simcam 
{

    /**
    A kind of master-control for switching between camera control modes 
    in game.
    */
    public class ModeSwitch : MonoBehaviour
    {
        [SerializeField] ICameraControl[] camModes;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

}