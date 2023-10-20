using UnityEngine;

namespace SimCam
{
    public class FirstPersonDiscreteYControl : FirstPersonControl {

        [SerializeField][Tooltip("Camera holder Y coordinates for levels; must have at least one valid value.")]
        private float[] levelHeights;
        private int level = 0;


        protected override void Move() {
            base.Move();
            AdjustLevel();
        }


        private void AdjustLevel() {
            int oldLevel = level;
            while((level > 0) && (transform.position.y <= levelHeights[level])) {
                level--;
            }
            while((level < levelHeights.Length) && (transform.position.y > levelHeights[level + 1])) {
                level++;
            }
            level = Mathf.Clamp(level, 0, levelHeights.Length - 1); // Simple Sanity check
            if(level != oldLevel) {
                Vector3 newGroundPos = groundPlain.transform.position;
                newGroundPos.y = levelHeights[level];
                groundPlain.transform.position = newGroundPos;
            }
        }

    }
}
