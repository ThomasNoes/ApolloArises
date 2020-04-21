using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleRobot : MonoBehaviour
{
    public float rotateSpeed = 1.0f, blinkDuration = 0.5f, blinkFrequency = 5.0f;
    public bool enableBlinking = true;
    public GameObject headObj, mainScreenObj, keyPrefab;
    public Image faceImage;
    public Sprite openEyes, closedEyes;
    public int uniqueId = 0;

    private GameObject mainCamObj, visualGenObj;
    private WaitForSeconds blinkDur, blinkFreq;

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
        if (mainCamObj == null)
            return;

        Vector3 mainCamPosNoY = new Vector3(mainCamObj.transform.position.x, headObj.transform.position.y, mainCamObj.transform.position.z);
        Vector3 targetDirection = mainCamPosNoY - headObj.transform.position;
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

    public void SpawnKey()
    {
        if (keyPrefab == null)
            return;

        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
        GameObject tempKey = Instantiate(keyPrefab, spawnPos, Quaternion.identity, visualGenObj.transform);
        tempKey.transform.Translate(new Vector3(0, 0, 0.4f), gameObject.transform);

        if (tempKey.GetComponent<Key>() != null)
            tempKey.GetComponent<Key>().uniqueId = uniqueId;
    }

    public void SetVisualGeneratorObject(GameObject obj)
    {
        visualGenObj = obj;
    }
}