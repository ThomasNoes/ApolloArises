// Put this script on the main camera!
namespace Assets.Scripts.Camera
{
    using UnityEngine;

    public class CamPosSwitcher : MonoBehaviour
    {
        public GameObject followCamLeft, followCamRight;
        private GameObject player, currentNextPortal, currentPrevPortal;
        private GameObject[] prevRenderQuadArray, nextRenderQuadArray;
        private Renderer currentNextPortalRenderer, currentPrevPortalRenderer;

        private FollowCam followCamScriptLeft, followCamScriptRight;
        public PortalRenderController pRController;

        private LayerMask layerMask;

        public bool distanceCheck = true, rendererInViewCheck = true, useCameraAngle = true;
        private bool prevInCamFrustum, nextInCamFrustum, currentDirection;
        private int currentMaze = -1, mazeCount, prevScore, nextScore;
        public float loopRepeatRate = 0.7f;

        #region Start
        private void Start()
        {
            layerMask = LayerMask.GetMask("Head");
            layerMask |= LayerMask.GetMask("Ignore Raycast");
            layerMask = ~layerMask;

            player = GameObject.FindGameObjectWithTag("Player");

            followCamScriptLeft = followCamLeft?.GetComponent<FollowCam>();
            followCamScriptRight = followCamRight?.GetComponent<FollowCam>();

            if (player != null && followCamScriptLeft != null && pRController != null)
            {
                InvokeRepeating("CheckerLoop", 2.5f, loopRepeatRate);
                Invoke("DelayedStart", 0.7f);
            }
        }

        private void DelayedStart()
        {
            nextRenderQuadArray = pRController.nextRenderQuadArray;
            prevRenderQuadArray = pRController.prevRenderQuadArray;

            //currentMaze = pRController.currentMaze;
            mazeCount = pRController.sequenceLength;
        }
        #endregion

        #region PositionSwitch
        /// <summary>
        /// Switches camera position between previous and next maze. This can be controlled remotely (public)
        /// </summary>
        /// <param name="dir">0 = previous maze, 1 = next maze</param>
        public void PositionSwitch(bool isForward)
        {
            if (isForward == currentDirection) // should it continue looking in the same direction
            {
                ResetValues();
                return;
            }

            if (isForward) //looking at prev maze
            {
                //followCamScriptLeft.offset = -1 * followCamScriptLeft.offset;
                //followCamScriptRight.offset = -1 * followCamScriptRight.offset;
                followCamScriptLeft.SetToNext();
                followCamScriptRight.SetToNext();
                pRController.SetProjectionQuads(true);
            }
            else
            {
                //followCamScriptLeft.offset = -followCamScriptLeft.offset;
                //followCamScriptRight.offset = -followCamScriptRight.offset;
                followCamScriptLeft.SetToPrev();
                followCamScriptRight.SetToPrev();
                pRController.SetProjectionQuads(false);
            }

            currentDirection = isForward;
            ResetValues();
        }

        public void PositionSwitchOverride(bool dir)
        {
            if (dir)
            {
                followCamScriptLeft.SetToPrev();
                followCamScriptRight.SetToPrev();
                pRController.SetProjectionQuads(false);
            }
            else
            {
                followCamScriptLeft.SetToNext();
                followCamScriptRight.SetToNext();
                pRController.SetProjectionQuads(true);
            }

        }
        #endregion

        #region AngleCheck
        private void AngleCheck()
        {
            Vector3 dirNext = currentNextPortal.transform.position - transform.position;
            Vector3 dirPrev = currentPrevPortal.transform.position - transform.position;

            if (Vector3.Angle(transform.forward, dirNext) <
                Vector3.Angle(transform.forward, dirPrev))
            {
                nextScore -= 1;
            }
            else
                prevScore -= 1;
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
                    followCamScriptLeft.SetToPrev();
                    followCamScriptRight.SetToPrev();
                    PositionSwitch(true);
                }
                else if (currentMaze == mazeCount - 1)
                {
                    currentPrevPortal = prevRenderQuadArray[currentMaze - 1];
                    followCamScriptLeft.SetToNext();
                    followCamScriptRight.SetToNext();
                    PositionSwitch(false);
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
                    prevScore = tempTile.prevDistance;
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
                PositionSwitch(true); //maybe opposite
            else if (prevInCamFrustum)
                PositionSwitch(false);//maybe opposite

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

            PositionSwitch(nextScore < prevScore);
        }
    }
}