﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Key : MonoBehaviour
{
    public int uniqueId;
    public Material colourMaterial;
    public ItemSpawner itemSpawner;
    private Renderer renderer;
    private bool pickedUp;

    private void Start()
    {
        renderer = GetComponent<Renderer>();

        if (renderer != null)
            Invoke("DelayedStart", 0.8f);
    }

    private void DelayedStart()
    {
        if (itemSpawner == null)
            return;

        colourMaterial = itemSpawner.GetMaterialFromId(uniqueId);
        renderer.material = colourMaterial; // TODO, make work with new key texture (when it is done)
    }

    private void OnCollisionEnter(Collision col)
    {
        if (pickedUp)
            return;

        if (col.gameObject.tag == "Hands")
        {
            pickedUp = true;
            CompanionBehaviour.instance?.OnPickUpKey();
        }
    }
}