// Put this script on the main camera!
namespace Assets.Scripts.Camera
{
    using UnityEngine;

    public class CamPosSwitcher : MonoBehaviour
    {
        public GameObject followCamLeft, followCamRight;
        private GameObject thisCamera, player, currentNextPortal, currentPrevPortal;
        private GameObject[] prevRenderQuadArray, nextRenderQuadArray;

        private FollowCam followCamScriptLeft, followCamScriptRight;
        public PortalRenderController pRController;

        private Vector3[] portalDirs;
        private LayerMask layerMask;

        private bool prevCollision = false, nextCollision = false;
        private int currentMaze, mazeCount;
        public float rayMaxDist = 15.0f, portalWidth;

        private void Start()
        {
            layerMask = LayerMask.GetMask("Player");
            layerMask |= LayerMask.GetMask("Ignore Raycast");
            layerMask = ~layerMask;

            portalDirs = new Vector3[4];

            thisCamera = gameObject;
            player = GameObject.FindGameObjectWithTag("Player");

            followCamScriptLeft = followCamLeft?.GetComponent<FollowCam>();
            followCamScriptRight = followCamRight?.GetComponent<FollowCam>();


            if (player != null && followCamScriptLeft != null && pRController != null)
            {
                InvokeRepeating("CheckerLoop", 2.5f, 0.3f);
                Invoke("DelayedStart", 1.0f);
            }
        }

        private void DelayedStart()
        {
            nextRenderQuadArray = pRController.nextRenderQuadArray;
            prevRenderQuadArray = pRController.prevRenderQuadArray;

            currentMaze = pRController.currentMaze;
            mazeCount = pRController.mazeCount;
            portalWidth = pRController.portalWidth;
        }

        /// <summary>
        /// Switches camera position between previous and next maze. This can be controlled remotely (public)
        /// </summary>
        /// <param name="dir">0 = previous maze, 1 = next maze</param>
        public void PositionSwitch(int dir)
        {
            if (dir == 0)
            {
                if (followCamScriptLeft.offset > 0)
                {
                    followCamScriptLeft.offset = -followCamScriptLeft.offset;
                    followCamScriptRight.offset = -followCamScriptRight.offset;
                }
            }
            else if (dir == 1)
            {
                if (followCamScriptLeft.offset < 0)
                {
                    followCamScriptLeft.offset = -1 * followCamScriptLeft.offset;
                    followCamScriptRight.offset = -1 * followCamScriptRight.offset;
                }
            }
        }

        private void RaycastCheck()
        {
            if (currentPrevPortal == null || currentNextPortal == null)
                return;

            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        portalDirs[i] = PortalEdgeFinder(false, true) - thisCamera.transform.position;
                        break;
                    case 1:
                        portalDirs[i] = PortalEdgeFinder(false, false) - thisCamera.transform.position;
                        break;
                    case 2:
                        portalDirs[i] = PortalEdgeFinder(true, true) - thisCamera.transform.position;
                        break;
                    case 3:
                        portalDirs[i] = PortalEdgeFinder(true, false) - thisCamera.transform.position;
                        break;
                }

                RaycastHit hit;

                if (Physics.Raycast(thisCamera.transform.position, portalDirs[i], out hit, rayMaxDist, layerMask))
                {
                    if (hit.collider.tag == currentNextPortal.tag)
                    {
                        nextCollision = true;
                    }
                    else if (hit.collider.tag == currentPrevPortal.tag)
                    {
                        prevCollision = true;
                    }
                }
            }
        }

        private void AngleCheck()
        {
            if (nextCollision && prevCollision)
            {
                PositionSwitch(Vector3.Angle(thisCamera.transform.forward, currentNextPortal.transform.position) <
                               Vector3.Angle(thisCamera.transform.forward, currentPrevPortal.transform.position)
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

        private Vector3 PortalEdgeFinder(bool next, bool right)
        {
            Vector3 tempEdgePos = new Vector3();

            if (next)   // if next portal
            {
                if (right) // if right edge
                    tempEdgePos = currentNextPortal.transform.position + currentNextPortal.transform.right * (portalWidth / 2.0f);
                else // else left edge
                    tempEdgePos = currentNextPortal.transform.position - currentNextPortal.transform.right * (portalWidth / 2.0f);
            }
            else   // else prev portal
            {
                if (right) // if right edge
                    tempEdgePos = currentPrevPortal.transform.position + currentPrevPortal.transform.right * (portalWidth / 2.0f);
                else // else left edge
                    tempEdgePos = currentPrevPortal.transform.position - currentPrevPortal.transform.right * (portalWidth / 2.0f);
            }

            return tempEdgePos;
        }

        private void CheckerLoop()  // This is a loop invoked in Start()
        {
            CurrentMazeCheck();

            if (currentMaze == 0 && currentMaze == mazeCount - 1)
                return;

            RaycastCheck();
            AngleCheck();
        }
    }
}