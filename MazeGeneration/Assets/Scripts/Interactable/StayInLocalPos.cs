using UnityEngine;

public class StayInLocalPos : MonoBehaviour
{
    public bool onlySetOnce, continuePosSetting;
    private Vector3 initialPos;
    private bool posTransform;
    private FixedJoint fixedJoint;
    private Vector3 anchor, axis; 

    void Awake()
    {
        initialPos = transform.localPosition;
        fixedJoint = GetComponent<FixedJoint>();

        if (fixedJoint != null)
        {
            fixedJoint.autoConfigureConnectedAnchor = true;
            anchor = fixedJoint.anchor;
            axis = fixedJoint.axis;
        }
    }

    void Start()
    {
        Invoke("DelayedStart", 0.3f);
    }

    void DelayedStart()
    {
        if (onlySetOnce)
            transform.localPosition = initialPos;
        else if (continuePosSetting)
            posTransform = true;

        if (fixedJoint != null)
        {
            fixedJoint.anchor = anchor;
            fixedJoint.axis = axis;
        }
    }

    void LateUpdate()
    {
        if (posTransform)
            transform.localPosition = initialPos;
    }
}