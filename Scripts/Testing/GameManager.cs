using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCam;


namespace simcamTest {

    public class GameManager : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            //ACameraControl.LeftclickCam  += ProcessLeftClick;
            //ACameraControl.RightclickCam += ProcessRightClick;
        }

        // Update is called once per frame
        void Update() {

        }


        public void ProcessLeftClick(RaycastHit hit) {
            Debug.Log("Left clicked!");
            hit.collider.gameObject.SetActive(false);
        }


        public void ProcessRightClick(RaycastHit hit) {
            Debug.Log("Right clicked!");
            hit.collider.gameObject.SetActive(false);
        }
    }

}