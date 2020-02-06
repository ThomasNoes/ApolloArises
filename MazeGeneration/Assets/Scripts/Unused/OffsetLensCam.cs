using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetLensCam : MonoBehaviour
{
    public Transform leftController;
    public Transform forwardVec;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - leftController.position;
        Vector3 initCamHeight = new Vector3(0f, leftController.position.y,0f);
        transform.position = initCamHeight;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = offset + leftController.position;
        transform.forward = forwardVec.forward;
    }
}
