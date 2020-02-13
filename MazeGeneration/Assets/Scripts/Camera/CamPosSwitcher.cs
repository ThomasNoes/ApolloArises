﻿// Put this script on the main camera!
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

        public bool useDistance = true, useCameraAngle = true;
        private bool prevCollision = false, nextCollision = false;
        private int currentMaze = -1, mazeCount, currentDir = -1;
        private float portalWidth, prevScore, nextScore;
        public float rayMaxDist = 15.0f, loopRepeatRate = 0.3f, scoreModifier = 3.0f;

        #region Start
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

            currentDir = dir;
            ResetValues();
        }
        #endregion

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

                if (Physics.Raycast(thisCamera.transform.position, portalDirs[i], out hit, rayMaxDist, layerMask))
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
        #endregion

        #region AngleCheck
        private void AngleCheck()
        {
            if (useCameraAngle)
            {
                nextScore = Vector3.Angle(thisCamera.transform.forward, currentNextPortal.transform.position);
                prevScore = Vector3.Angle(thisCamera.transform.forward, currentPrevPortal.transform.position);
            }
            else
            {
                nextScore = Vector3.Angle(player.transform.forward, currentNextPortal.transform.position);
                prevScore = Vector3.Angle(player.transform.forward, currentPrevPortal.transform.position);
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
                }
            }
        }

        private void DistanceCheck()
        {
            nextScore += Vector3.Distance(thisCamera.transform.position, currentNextPortal.transform.position) * scoreModifier;
            prevScore += Vector3.Distance(thisCamera.transform.position, currentPrevPortal.transform.position) * scoreModifier;
        }

        private void ResetValues()
        {
            nextCollision = false;
            prevCollision = false;
            nextScore = 0;
            prevScore = 0;
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

        private void CheckerLoop()  // This is a loop invoked in Start()
        {
            CurrentMazeCheck();

            if (currentMaze == 0 || currentMaze == mazeCount - 1)
                return;

            AngleCheck();

            if (useDistance)
                DistanceCheck();
            else
                RaycastCheck();

            PositionSwitch(nextScore < prevScore ? 1 : 0);
        }

    }
}