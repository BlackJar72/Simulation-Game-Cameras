using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is for converting C# events from the cameras to
/// Unity events that can be connected through the Unity editor.
/// Its use is entirely optional, adding this is an alterantive to
/// coding the connections directly from the ACameraControl's
/// static C# events.
///
/// Most typically, these events would go to a manager that
/// can decide what to do based on what is hit.
/// </summary>
namespace SimCam {

    public class EventConverter : MonoBehaviour {

        public UnityEvent<RaycastHit> LeftClickUnity;
        public UnityEvent<RaycastHit> LeftHoldUnity;
        public UnityEvent<RaycastHit> RightClickUnity;
        public UnityEvent<RaycastHit> RightHoldUnity;

        public UnityEvent<int> LevelChange;


        void OnEnable() {
            ACameraControl.LeftMouseUp += ForwardLeftClick;
            ACameraControl.LeftMouseDown += ForwardLeftHold;
            ACameraControl.RightMouseUp += ForwardRightClick;
            ACameraControl.RightMouseDown += ForwardRightHold;
            ACameraControl.LevelChanged  += ForwardLevelChange;
        }


        void OnDisable() {
            ACameraControl.LeftMouseUp -= ForwardLeftClick;
            ACameraControl.LeftMouseDown -= ForwardLeftHold;
            ACameraControl.RightMouseUp -= ForwardRightClick;
            ACameraControl.RightMouseDown -= ForwardRightHold;
            ACameraControl.LevelChanged  -= ForwardLevelChange;
        }


        public void ForwardLeftClick(RaycastHit hit) {
            LeftClickUnity?.Invoke(hit);
        }


        public void ForwardRightClick(RaycastHit hit) {
            RightClickUnity?.Invoke(hit);
        }


        public void ForwardLeftHold(RaycastHit hit) {
            LeftHoldUnity?.Invoke(hit);
        }


        public void ForwardRightHold(RaycastHit hit) {
            RightHoldUnity?.Invoke(hit);
        }


        public void ForwardLevelChange(int level) {
            LevelChange?.Invoke(level);
        }
    }

}