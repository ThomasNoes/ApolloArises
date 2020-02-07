namespace MazeGeneration.Camera
{
    using UnityEngine;

    public class CamPosSwitcher : MonoBehaviour
    {
        private GameObject thisCamera;
        private GameObject player;
        // private GameObject[] 

        private void Start()
        {
            thisCamera = gameObject;
            player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                InvokeRepeating("CheckerLoop", 2.0f, 0.3f);
        }

        /// <summary>
        /// Switches camera position between previous and next maze
        /// </summary>
        /// <param name="dir">0 = previous maze, 1 = next maze</param>
        public void PositionSwitch(int dir)
        {

        }

        private void CheckerLoop()
        {
            // if ()


            // TODO: Make sure that first and end maze behaviour is different YYY
        }
    }
}