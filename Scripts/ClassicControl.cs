using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simcam 
{

    /**
    This is for moving the camera in a way typical of most classic and many
    modern simulation, god, and rts games.  Basically, hover and a hieght and 
    use keys or on-screen buttons to rotate and possible raise/lower the camera.
    */
    public class ClassicControl : ICameraControl
    
    {
        /*// Start is called before the first frame update
        void Start()
        {
            
        }*/

        protected override void OnEnable() 
        {
            // Will this be needed here?  IDK.
        }


        protected override void OnDisable() 
        {
            // Will this be needed here?  IDK.
        }


        // Update is called once per frame
        void Update()
        {
            
        }
    }

}