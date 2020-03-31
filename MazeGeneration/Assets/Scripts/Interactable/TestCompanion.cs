using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TestCompanion : MonoBehaviour
{
    public float rotateSpeed = 1.0f, blinkDuration = 0.5f, blinkFrequency = 5.0f;
    public bool enableBlinking = true;
    public GameObject companionHead;
    public Image faceImage;
    public Sprite openEyes, closedEyes;

    private GameObject mainCamObj;
    private WaitForSeconds blinkDur, blinkFreq;

    private void Start()
    {
        if (companionHead == null)
            return;

        mainCamObj = Camera.main.gameObject;

        if (faceImage != null && openEyes != null && closedEyes != null)
        {
            StartCoroutine(FaceBehaviour());
            blinkDur = new WaitForSeconds(blinkDuration);
            blinkFreq = new WaitForSeconds(blinkFrequency);
        }
    }

    private void LateUpdate()
    {
        if (mainCamObj == null)
            return;

        Vector3 mainCamPosNoY = new Vector3(mainCamObj.transform.position.x, companionHead.transform.position.y, mainCamObj.transform.position.z);
        Vector3 targetDirection = mainCamPosNoY - companionHead.transform.position;
        float singleStep = rotateSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(companionHead.transform.forward, targetDirection, singleStep, 0.0f);

        companionHead.transform.rotation = Quaternion.LookRotation(newDirection);
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