/* using UnityEngine;
using UnityEngine.Rendering;

public class TwinCamController : MonoBehaviour
{
    [SerializeField]
    private Camera _activeCamera;
    [SerializeField]
    private Camera _hiddenLeft;
    [SerializeField]
    private Camera _hiddenRight;




    /// <summary>
    /// Set up our RT and 
    /// </summary>
    private void Awake()
    {
        //var rt = new RenderTexture(Screen.width, Screen.height, 24);
        //TimeCrackTexture.mainTexture = rt;
        _hiddenLeft.fieldOfView = _activeCamera.fieldOfView;
        _hiddenRight.fieldOfView = _activeCamera.fieldOfView;
        //Shader.SetGlobalTexture("_TimeCrackTexture", rt);
        //_hiddenCamera.targetTexture = rt;

    }

} */