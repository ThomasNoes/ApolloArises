/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGeneratorCaller : MonoBehaviour
{

    private MapManager mapManager;
    private PortalGenerator portalGenerator;
    private EventCallbacks.TerrainGenerator tg;

    TileInfo[] portalInfos;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LateStart());
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>(); 
        portalGenerator = GameObject.Find("PortalGenerator").GetComponent<PortalGenerator>();
        tg = GameObject.Find("TileGenerator").GetComponent<EventCallbacks.TerrainGenerator>();
    }

    IEnumerator LateStart()
    {
        //returning 0 will make it wait 1 frame
        //that way portalInfo in mapGenerator contain all dead ends for portals
        yield return 50;

        portalInfos = mapManager.portalInfo;
        portalGenerator.GeneratePortals(portalInfos,mapManager.tileWidth,tg.wallOffset);

    }
}
 */