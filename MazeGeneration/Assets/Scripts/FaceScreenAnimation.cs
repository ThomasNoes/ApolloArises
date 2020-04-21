using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceScreenAnimation : MonoBehaviour
{
    public float rotateSpeed = 1.0f, blinkDuration = 0.5f, blinkFrequency = 5.0f;
    public bool enableBlinking = true, lookAtPlayer;
    public GameObject headObj, frameObj;
    public Image faceImage;
    public Sprite openEyes, closedEyes;

    private GameObject mainCamObj;
    private WaitForSeconds blinkDur, blinkFreq;
    private bool isMoving;

    private void Start()
    {
        if (headObj == null)
            return;

        mainCamObj = Camera.main?.gameObject;

        if (faceImage != null && openEyes != null && closedEyes != null)
        {
            StartCoroutine(FaceBehaviour());
            blinkDur = new WaitForSeconds(blinkDuration);
            blinkFreq = new WaitForSeconds(blinkFrequency);
        }
    }

    private void LateUpdate()
    {
        if (mainCamObj == null || !lookAtPlayer || isMoving)
            return;

        Vector3 targetDirection = mainCamObj.transform.position - headObj.transform.position;
        float singleStep = rotateSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(headObj.transform.forward, targetDirection, singleStep, 0.0f);

        headObj.transform.rotation = Quaternion.LookRotation(newDirection);
    }

    private IEnumerator FaceBehaviour()
    {
        while (enableBlinking)
        {
            faceImage.sprite = closedEyes;

            yield return blinkDur;

            faceImage.sprite = openEyes;

            yield return blinkFreq;
        }
    }
}
