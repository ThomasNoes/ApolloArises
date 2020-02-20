// Put this script on the main camera!
namespace Assets.Scripts.Camera
{
    using UnityEngine;

    public class CamPosSwitcher : MonoBehaviour
    {
        public GameObject followCamLeft, followCamRight;
        private GameObject thisCamera, player, currentNextPortal, currentPrevPortal;
        private GameObject[] prevRenderQuadArray, nextRenderQuadArray;
        private Renderer currentNextPortalRenderer, currentPrevPortalRenderer;

        private FollowCam followCamScriptLeft, followCamScriptRight;
        public PortalRenderController pRController;

        private Vector3[] portalDirs;
        private LayerMask layerMask;

        public bool isDebugPlayer, distanceCheck = true, rendererInViewCheck = true, useCameraAngle = true;
        private bool prevInCamFrustum, nextInCamFrustum;
        private int currentMaze = -1, mazeCount, currentDir = -1, prevScore, nextScore, prevDistance, nextDistance;
        private float portalWidth;
        public float loopRepeatRate = 0.2f;

        #region Start
        private void Start()
        {
            layerMask = LayerMask.GetMask("Head");
            layerMask |= LayerMask.GetMask("Ignore Raycast");
            layerMask = ~layerMask;

            portalDirs = new Vector3[4];

            thisCamera = Camera.main.gameObject;
            player = GameObject.FindGameObjectWithTag("Player");

            followCamScriptLeft = followCamLeft?.GetComponent<FollowCam>();
            followCamScriptRight = followCamRight?.GetComponent<FollowCam>();


            if (player != null && followCamScriptLeft != null && pRController != null)
            {
                InvokeRepeating("CheckerLoop", 2.5f, loopRepeatRate);
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
        #endregion

        #region PositionSwitch
        /// <summary>
        /// Switches camera position between previous and next maze. This can be controlled remotely (public)
        /// </summary>
        /// <param name="dir">0 = previous maze, 1 = next maze</param>
        public void PositionSwitch(int dir)
        {
            if (dir == currentDir)
            {
                ResetValues();
                return;
            }

            if (dir == 0)
            {
                if (followCamScriptLeft.offset > 0)
                {
                    followCamScriptLeft.offset = -followCamScriptLeft.offset;
                    followCamScriptRight.offset = -followCamScriptRight.offset;
                    pRController.SetProjectionQuads(false);
                }
            }
            else if (dir == 1)
            {
                if (followCamScriptLeft.offset < 0)
                {
                    followCamScriptLeft.offset = -1 * followCamScriptLeft.offset;
                    followCamScriptRight.offset = -1 * followCamScriptRight.offset;
                    pRController.SetProjectionQuads(true);
                }
            }

            currentDir = dir;
            ResetValues();
        }
        #endregion

        #region AngleCheck
        private void AngleCheck()
        {
            if (useCameraAngle)
            {
                if (Vector3.Angle(thisCamera.transform.forward, currentNextPortal.transform.position) <
                    Vector3.Angle(thisCamera.transform.forward, currentPrevPortal.transform.position))
                {
                    nextScore -= 1;
                }
                else
                    prevScore -= 1;

            }
            else
            {
                if (Vector3.Angle(player.transform.forward, currentNextPortal.transform.position) <
                    Vector3.Angle(player.transform.forward, currentPrevPortal.transform.position))
                {
                    nextScore -= 1;
                }
                else
                    prevScore -= 1;
            }
        }
        #endregion

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
                    currentPrevPortal = prevRenderQuadArray[currentMaze - 1];
                    PositionSwitch(0);
                }
                else
                {
                    currentNextPortal = nextRenderQuadArray[currentMaze];
                    currentPrevPortal = prevRenderQuadArray[currentMaze - 1];
                    currentNextPortalRenderer = currentNextPortal.GetComponent<Renderer>();
                    currentPrevPortalRenderer = currentPrevPortal.GetComponent<Renderer>();
                }
            }
        }

        private void DistanceCheck()
        {
            //nextScore = nextDistance;
            //prevScore = prevDistance;

            RaycastHit hit;

            if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 10.0f, layerMask))
            {
                Tile tempTile = hit.collider.gameObject.GetComponentInParent<Tile>();

                if (tempTile != null)
                {
                    nextScore = tempTile.nextDistance;
                    prevScore = tempTile.prevdistance;
                }
            }

            //nextScore += Vector3.Distance(thisCamera.transform.position, currentNextPortal.transform.position) * 2.0f;
            //prevScore += Vector3.Distance(thisCamera.transform.position, currentPrevPortal.transform.position) * 2.0f;
        }


        #region InCamFrustumCheck
        static bool VisibleFromCamera(Renderer renderer, Camera camera)
        {
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
        }

        private void InCamFrustumCheck()
        {
            if (VisibleFromCamera(currentNextPortalRenderer, Camera.main))
                nextInCamFrustum = true;
            if (VisibleFromCamera(currentPrevPortalRenderer, Camera.main))
                prevInCamFrustum = true;

            if (nextInCamFrustum && prevInCamFrustum)
                FinalChecks();
            else if (nextInCamFrustum)
                PositionSwitch(1);
            else if (prevInCamFrustum)
                PositionSwitch(0);

            ResetValues();
        }
        #endregion

        private void ResetValues()
        {
            nextInCamFrustum = false;
            prevInCamFrustum = false;
            nextScore = 0;
            prevScore = 0;
        }

        private void CheckerLoop()  // This is a loop invoked in Start()
        {
            CurrentMazeCheck();

            if (currentMaze == 0 || currentMaze == mazeCount - 1)
                return;

            if (rendererInViewCheck)
                InCamFrustumCheck();
            else
                FinalChecks();
        }

        private void FinalChecks()
        {
            if (distanceCheck)
                DistanceCheck();

            if (nextScore == prevScore)
                AngleCheck();

            PositionSwitch(nextScore < prevScore ? 1 : 0);
        }

        public void SetDistanceVariables(int prev, int next)
        {
            prevDistance = prev;
            nextDistance = next;
        }

        //// NOT IN USE ANY LONGER: ////
        #region RaycastCheck
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

                if (Physics.Raycast(thisCamera.transform.position, portalDirs[i], out hit, 15.0f, layerMask))
                {
                    if (hit.collider.tag == currentNextPortal.tag)
                    {
                        prevScore += 1000;
                    }
                    else if (hit.collider.tag == currentPrevPortal.tag)
                    {
                        nextScore += 1000;
                    }
                }
            }
        }

        #region PortalEdgeFinder
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
        #endregion
        #endregion
    }
}