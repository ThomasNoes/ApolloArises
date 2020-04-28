using UnityEngine;

public class StayInLocalPos : MonoBehaviour
{
    private Vector3 initialPos;
    private bool posTransform;

    void Start()
    {
        Invoke("DelayedStart", 0.3f);
    }

    void DelayedStart()
    {
        initialPos = transform.localPosition;
        posTransform = true;
    }

    void LateUpdate()
    {
        if (posTransform)
            transform.localPosition = initialPos;
    }
}