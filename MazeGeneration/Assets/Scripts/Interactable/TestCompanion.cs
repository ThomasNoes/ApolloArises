using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCompanion : MonoBehaviour
{
    public float rotateSpeed = 1.0f;
    public GameObject companionHead;
    private GameObject mainCamObj;

    private void Start()
    {
        if (companionHead != null)
            mainCamObj = Camera.main.gameObject;
    }

    private void LateUpdate()
    {
        if (mainCamObj == null)
            return;

        Vector3 targetDirection = mainCamObj.transform.position - companionHead.transform.position;
        float singleStep = rotateSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(companionHead.transform.right, targetDirection, singleStep, 0.0f);

        companionHead.transform.rotation = Quaternion.LookRotation(newDirection);
    }
}