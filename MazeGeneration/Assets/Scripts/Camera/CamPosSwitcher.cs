namespace Assets.Scripts.Camera
{
    using UnityEngine;

    public class CamPosSwitcher : MonoBehaviour
    {
        private GameObject thisCamera, player, currentNextPortal, currentPrevPortal;
        private GameObject[] prevRenderQuadArray;
        private GameObject[] nextRenderQuadArray;

        private FollowCam followCam;
        public PortalRenderController pRController;

        private Vector3[] portalDirs;
        private LayerMask layerMask;

        private bool prevCollision = false, nextCollision = false;
        private int currentMaze, mazeCount;
        public float rayMaxDist = 15.0f;

        private void Start()
        {
            layerMask = LayerMask.GetMask("Player");
            layerMask |= LayerMask.GetMask("Ignore Raycast");
            layerMask = ~layerMask;

            portalDirs = new Vector3[4];

            thisCamera = gameObject;
            followCam = thisCamera.GetComponent<FollowCam>();
            player = GameObject.FindGameObjectWithTag("Player");

            if (player != null && followCam != null && pRController != null)
            {
                InvokeRepeating("CheckerLoop", 2.0f, 0.3f);

                for (int i = 0; i < pRController.nextProjectionQuadArray.Length; i++)
                {
                    // TODO: find prevRenderQuadArray and nextRenderQuadArray objects;
                }

                currentMaze = pRController.currentMaze;
                mazeCount = pRController.mazeCount;
            }
        }

        /// <summary>
        /// Switches camera position between previous and next maze. This can be controlled remotely (public)
        /// </summary>
        /// <param name="dir">0 = previous maze, 1 = next maze</param>
        public void PositionSwitch(int dir)
        {
            if (dir == 0)
            {
                if (followCam.offset > 0)
                    followCam.offset = - followCam.offset;
            }
            else if (dir == 1)
            {
                if (followCam.offset < 0)
                    followCam.offset = -1 * followCam.offset;
            }
        }

        private void RaycastCheck()
        {
            if (currentPrevPortal == null || currentNextPortal == null)
                return;

            RaycastHit hit;

            for (int i = 0; i < 4; i++)
            {
                if (i <= 1)
                {
                    portalDirs[i] = currentPrevPortal.transform.position - thisCamera.transform.position;
                    // TODO: check edges instead
                }
                else
                {
                    portalDirs[i] = currentNextPortal.transform.position - thisCamera.transform.position;
                    // TODO: check edges instead
                }

                if (Physics.Raycast(thisCamera.transform.position, portalDirs[i], out hit, rayMaxDist, layerMask))
                {
                    if (hit.collider.tag == currentNextPortal.tag) // TODO: Make sure portals are tagged
                    {
                        nextCollision = true;
                    }
                    else if (hit.collider.tag == currentPrevPortal.tag) // TODO: Make sure portals are tagged
                    {
                        prevCollision = true;
                    }
                }
            }
        }

        private void DistanceCheck()
        {
            if (nextCollision && prevCollision)
            {
                PositionSwitch(Vector3.Distance(thisCamera.transform.position, currentNextPortal.transform.position) <
                               Vector3.Distance(thisCamera.transform.position, currentPrevPortal.transform.position)
                    ? 1
                    : 0);

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
        }

        private void CurrentMazeCheck()
        {
            if (currentMaze != pRController.currentMaze)
            {
                currentMaze = pRController.currentMaze;

                if (currentMaze == 0)
                {
                    currentNextPortal = nextRenderQuadArray[currentMaze];
                    PositionSwitch(1);
                }
                else if (currentMaze == mazeCount - 1)
                {
                    currentPrevPortal = prevRenderQuadArray[currentMaze];
                    PositionSwitch(0);
                }
                else
                {
                    currentNextPortal = nextRenderQuadArray[currentMaze];
                    currentPrevPortal = prevRenderQuadArray[currentMaze];
                }
            }
        }

        private void ResetBools()
        {
            nextCollision = false;
            prevCollision = false;
        }

        private void CheckerLoop()  // This is a loop invoked in Start()
        {
            CurrentMazeCheck();

            if (currentMaze == 0 && currentMaze == mazeCount - 1)
                return;

            RaycastCheck();
            DistanceCheck();
        }
    }
}