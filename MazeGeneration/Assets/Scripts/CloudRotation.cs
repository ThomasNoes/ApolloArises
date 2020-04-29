using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRotation : MonoBehaviour
{

    public float Speed =1;

    MapManager mm;
    
    // Start is called before the first frame update
    void Start()
    {
        mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        transform.rotation = mm.transform.rotation;
        transform.Translate(mm.offset);
        MakeChildrenFaceCenter();
        if (Speed == 0)
        {
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, Speed * Time.deltaTime);
    }

    void MakeChildrenFaceCenter()
    {
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            Vector3 pos = transform.GetChild(i).position;
            pos.y = transform.position.y;

            Vector3 toCenter = pos - transform.position;

            transform.GetChild(i).forward = toCenter; 
        }

    }
}
