using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayTeleporter : MonoBehaviour
{
    public float mazeWidth;
    public float padding;
    public float tileWidth;
    public Transform player;
    public Transform sender;
    public Transform receiver;
    public Camera dummy;
    public TileInfo entranceTeleporterPosition;
    public Vector3 portalToPlayer;
    // Start is called before the first frame update

    public void Initialize()
    {

        sender.localPosition = new Vector3(-mazeWidth / 2f + tileWidth / 2f + (float)entranceTeleporterPosition.column * tileWidth, 0f, mazeWidth / 2f - tileWidth / 2f - (float)entranceTeleporterPosition.row * tileWidth);
        receiver.position = new Vector3(sender.position.x + mazeWidth + padding, sender.position.y, sender.position.z);

        sender.Rotate(0f, 90f * entranceTeleporterPosition.direction, 0f, Space.Self);
        receiver.Rotate(0f, 90f * ((2 + entranceTeleporterPosition.direction) % 4), 0f, Space.Self);

        dummy.enabled = true;
        dummy.transform.parent = receiver;
    }

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        portalToPlayer = player.position - sender.position;
        if (dummy.enabled)
        {
            dummy.transform.localPosition = -portalToPlayer;
            dummy.transform.rotation = player.localRotation;
        }
    }
}
