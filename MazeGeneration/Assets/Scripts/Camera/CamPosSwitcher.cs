namespace Assets.Scripts.Camera
{
    using UnityEngine;

    public class CamPosSwitcher : MonoBehaviour
    {
        public float rayMaxDist = 20.0f;
        private GameObject thisCamera, player, currentNextPortal, currentPrevPortal;    // TODO find and assign current portal pairs for each maze
        private FollowCam followCam;
        private LayerMask layerMask;
        private bool prevCollision = false, nextCollision = false;

        private void Start()
        {
            layerMask = LayerMask.GetMask("Player");
            layerMask |= LayerMask.GetMask("Ignore Raycast");
            layerMask = ~layerMask;

            thisCamera = gameObject;
            followCam = thisCamera.GetComponent<FollowCam>();
            player = GameObject.FindGameObjectWithTag("Player");

            if (player != null && followCam != null)
                InvokeRepeating("CheckerLoop", 2.0f, 0.3f);
        }

        /// <summary>
        /// Switches camera position between previous and next maze
        /// </summary>
        /// <param name="dir">0 = previous maze, 1 = next maze</param>
        public void PositionSwitch(int dir)
        {
            if (dir == 0)
            {
                followCam.offset = -followCam.offset;   // TODO: Find a proper way to do this
            }
            else if (dir == 1)
            {
                followCam.offset = +followCam.offset;   // TODO: Find a proper way to do this
            }
        }

        private void ResetBools()
        {
            nextCollision = false;
            prevCollision = false;
        }

        private void CheckerLoop()
        {
            Vector3 dirPrevPortal =  - thisCamera.transform.position;
            Vector3 dirNextPortal = -thisCamera.transform.position; // TODO find portal edge positions (+- center?)

            RaycastHit hit;

            if (Physics.Raycast(thisCamera.transform.position, dirNextPortal, out hit, rayMaxDist, layerMask))
            {
                if (hit.collider.tag == currentNextPortal.tag) // TODO: Make sure portals are tagged
                {
                    nextCollision = true;
                }
            }
            if (Physics.Raycast(thisCamera.transform.position, dirPrevPortal, out hit, rayMaxDist, layerMask))
            {
                if (hit.collider.tag == currentPrevPortal.tag) // TODO: Make sure portals are tagged
                {
                    prevCollision = true;
                }
            }

            if (nextCollision && prevCollision)
            {
                if (Vector3.Distance(thisCamera.transform.position, currentNextPortal.transform.position) <
                    Vector3.Distance(thisCamera.transform.position, currentPrevPortal.transform.position))  // TODO: find a better solution than distance
                {
                    PositionSwitch(1);
                }
                else
                {
                    PositionSwitch(0);
                }
                ResetBools();
            }
            else if (nextCollision)
            {
                PositionSwitch(1);
                ResetBools();
            }
            else if (prevCollision)
            {
                PositionSwitch(0);
                ResetBools();
            }

            // TODO: Make sure that first and end maze behaviour is different YYY
        }
    }
}