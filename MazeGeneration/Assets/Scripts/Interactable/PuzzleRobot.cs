using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleRobot : MonoBehaviour
{
    public float rotateSpeed = 1.0f, blinkDuration = 0.5f, blinkFrequency = 5.0f;
    public bool enableBlinking = true, turnedOff;
    public GameObject headObj, mainScreenObj, keyPrefab, puzzleStandObj, puzzleDoor;
    public GameObject mainScreenCanvas;
    public Image faceImage;
    public Sprite openEyes, closedEyes, turnedOffFace;
    public int uniqueId = 0;
    [HideInInspector] public ItemSpawner itemSpawner;

    private GameObject mainCamObj, visualGenObj;
    private WaitForSeconds blinkDur, blinkFreq;
    private bool animatePuzzleDoor;
    private Vector3 startPos, endPos;
    private float fraction;

    private void Start()
    {
        if (headObj == null)
            return;

        mainCamObj = Camera.main?.gameObject;

        if (faceImage != null && openEyes != null && closedEyes != null)
        {
            blinkDur = new WaitForSeconds(blinkDuration);
            blinkFreq = new WaitForSeconds(blinkFrequency);

            if (puzzleDoor != null)
            {
                startPos = puzzleDoor.transform.position;
                endPos = new Vector3(puzzleDoor.transform.position.x, puzzleDoor.transform.position.y - (puzzleDoor.transform.localScale.y), puzzleDoor.transform.position.z);
            }

            if (!turnedOff)
                StartCoroutine(FaceBehaviour());
            else if (turnedOffFace != null)
            {
                faceImage.sprite = turnedOffFace;
                if (mainScreenCanvas != null)
                    mainScreenCanvas.SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        if (animatePuzzleDoor)
            if (fraction < 1)
            {
                fraction += Time.deltaTime * 1.0f;
                puzzleDoor.transform.position = Vector3.Lerp(startPos, endPos, fraction);
            }

        if (mainCamObj == null || turnedOff)
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

            if (turnedOff)
                break;
        }
    }

    public void SpawnKey()
    {
        if (keyPrefab == null)
            return;

        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);

        if (puzzleStandObj != null)
            spawnPos = puzzleStandObj.transform.position;

        GameObject tempKey = Instantiate(keyPrefab, spawnPos, Quaternion.identity, visualGenObj.transform);
        Key tempKeyScript = tempKey.GetComponent<Key>();

        if (tempKeyScript != null)
        {
            tempKeyScript.uniqueId = uniqueId;

            if (itemSpawner != null)
            {
                tempKeyScript.colourMaterial = itemSpawner.GetMaterialFromId(uniqueId);
            }
        }

        animatePuzzleDoor = true;
    }

    public void SetVisualGeneratorObject(GameObject obj)
    {
        visualGenObj = obj;
    }

    public void TurnOn()
    {
        turnedOff = false;

        if (mainScreenCanvas != null)
            mainScreenCanvas.SetActive(true);

        StartCoroutine(FaceBehaviour());
    }

    public void TurnOff()
    {
        turnedOff = true;

        if (turnedOffFace != null)
            faceImage.sprite = turnedOffFace;

        if (mainScreenCanvas != null)
            mainScreenCanvas.SetActive(false);

        StopAllCoroutines();
    }
}