using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider), typeof(AudioSource))]
public class Door : MonoBehaviour
{
    public bool colourWholeDoor = true;
    public GameObject doorMainObj, keyHoleObj;
    public int uniqueId, inMaze;
    public bool useDelay, isPowered;
    public float delay = 1.0f, moveSpeed = 1.0f, height;
    public AudioClip unlockSound, openSound;
    [HideInInspector] public Material colourMaterial;
    [HideInInspector] public Tile thisTile;

    private bool doorControlBool;
    private Vector3 startPos, endPos;
    private float fraction;
    private AudioSource audioSource;
    private BeaconManager beaconManager;

    private void Start()
    {
        beaconManager = FindObjectOfType<BeaconManager>();

        if (keyHoleObj != null)
        {
            BoxCollider thisCollider = GetComponent<BoxCollider>();
            thisCollider.size = keyHoleObj.transform.localScale;
            thisCollider.center = keyHoleObj.transform.localPosition;
        }

        if (doorMainObj != null)
            Invoke("DelayedStart", 1.0f);

        if (beaconManager != null && uniqueId != 0)
        {
            if (beaconManager.beacons.Count != 0)
            {
                isPowered = beaconManager.beacons[inMaze].isActive;
                beaconManager.beacons[inMaze].doorRef = this;
            }
        }

        startPos = transform.position;
        endPos = new Vector3(transform.position.x, transform.position.y - (height - 0.01f), transform.position.z);
        audioSource = GetComponent<AudioSource>();
    }

    private void DelayedStart()
    {
        if (colourWholeDoor)
            doorMainObj.GetComponent<Renderer>().material = colourMaterial;
        else
            keyHoleObj.GetComponent<Renderer>().material = colourMaterial;
    }

    public void OpenDoor()
    {
        StartCoroutine(PlayDoorSoundWithDelay());
        //gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (!isPowered)
            return;

            if (col.gameObject.GetComponent<Key>() != null)
        {
            int tempId = col.gameObject.GetComponent<Key>().uniqueId;
            col.gameObject.GetComponent<SVGrabbable>()?.ClearActiveController();

            if (tempId == uniqueId)
                if (!useDelay)
                {
                    Destroy(col.gameObject);
                    CompanionBehaviour.instance?.OnOpenDoor();
                    OpenDoor();
                }
                else
                {
                    Destroy(col.gameObject);
                    Invoke("OpenDoor", delay);
                }
        }
    }

    private void LateUpdate()
    {
        if (doorControlBool)
        {
            if (fraction < 1)
            {
                fraction += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPos, endPos, fraction);
            }
        }
    }

    private IEnumerator PlayDoorSoundWithDelay()
    {
        PlaySound(unlockSound);
        yield return new WaitForSeconds(0.6f);
        doorControlBool = true;

        if (thisTile != null)
            thisTile.blocked = false;

        PlaySound(openSound);
    }

    private void PlaySound(AudioClip audioClip)
    {
        if (audioClip == null || audioSource == null)
            return;

        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void PowerDoor() => isPowered = true;
}

#region CustomInspector
#if UNITY_EDITOR
[CustomEditor(typeof(Door))]
public class Door_Editor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var script = target as Door;

        DrawDefaultInspector();

        if (GUILayout.Button("Open Door"))
        {
            script.OpenDoor();
        }
    }
}
#endif
#endregion