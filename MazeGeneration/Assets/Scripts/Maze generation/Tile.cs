using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is for

public class Tile : MonoBehaviour {

    [Header("A Star information")]
    //A star information
    public bool isAStarTile = false;
    public bool isMarked = false;
    public int nextDistance;
    public int prevdistance;

    [Header("Tile information")]
    //tile information
    public int tileID;
    public int[] wallArray; //[0] = north, [1] = east, [2]= south, [3] = west. 1 = traversable / no wall, 0 = not traversable / wall
    public float tileWidth;

    //for A Star algorithm
    private int row;
    private int column;
    private int gCost = 0;
    private int hCost = 0;
    private Tile parent;

    void Awake () {
        wallArray = new int[] { 0, 0, 0, 0 };
        tileID = 0;
    }

    public void SetRowAndColumn(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public void SetWidth (float width) {
        tileWidth = width;
        transform.localScale *= tileWidth;
    }

    private void SetIDFromArray () {
        tileID = 8 * wallArray[3] + 4 * wallArray[2] + 2 * wallArray[1] + wallArray[0];
        //SetMaterial(tileID, 'p');
    }

    // Direction specifies which side we want to traverse through,
    // the opposite side of that on the "to" tile is always dir-2,
    // and to prevent out of bounds we take the mod(4) of it.
    public static bool IsTraversable (Tile from, Tile to, int direction) {
        int oppositeDirection = (direction + 2) % 4;
        return (from.wallArray[direction] == to.wallArray[oppositeDirection]);
    }

    //Sets the value of the wallArray at position "direction" to val (0 to close 1 to open) then sets the ID
    public void SetWall (int direction, int val) {
        wallArray[direction] = val;
        SetIDFromArray ();
    }

    public void OpenWall (int direction) {
        SetWall (direction, 1);
    }

    public void CloseWall (int direction) {
        SetWall (direction, 0);
    }

    //Calls openWall on the from tile towards direction, and the to tile towards oppositeDirection (0-2 1-3)
    public static void ConnectTiles (Tile from, Tile to, int direction) {
        int oppositeDirection = (direction + 2) % 4;
        from.OpenWall (direction);
        to.OpenWall (oppositeDirection);
    }

    //Same as connectTiles but closes them down.
    public static void DisconnectTiles (Tile from, Tile to, int direction) {
        int oppositeDirection = (direction + 2) % 4;
        from.CloseWall (direction);
        to.CloseWall (oppositeDirection);
    }

    // Could be done with an event and a listener so the functions themselves
    // wouldn't have to call setmaterial but it could react to changes
    public void SetMaterial (int materialID, char materialType) {
        Renderer renderer = GetComponentInChildren<Renderer> ();
        renderer.material = Resources.Load<Material> (materialType + materialID.ToString ());
    }

    public void SetArrayFromID () {
        int temp = tileID;
        for (int i = 3; i >= 0; i--) {
            wallArray[i] = temp / (int) Mathf.Pow (2, i);
            temp %= (int) Mathf.Pow (2, i);
        }
        //SetMaterial(tileID, 'p');
    }

    public void SetWallArray (int[] a) {
        wallArray = a;
        SetIDFromArray ();
    }

    public int[] GetWallArray () {
        return wallArray;
    }

    public void SetTileID (int i) {
        tileID = i;
        SetArrayFromID ();
    }

    public int GetTileID () {
        return tileID;
    }

    public int GetG()
    {
        return gCost;
    }
    public void SetG( int g)
    {
        gCost = g;
    }
    public int GetH()
    {
        return hCost;
    }
    public void SetH(int h)
    {
        hCost = h;
    }
    public int GetF()
    {
        return gCost + hCost;
    }
    public Tile GetParent()
    {
        return parent;
    }
    public void SetParent(Tile parent)
    {
        this.parent = parent;
    }
    public int GetRow()
    {
        return row;
    }
    public void SetRow(int r)
    {
        row = r;
    }
    public int GetCol()
    {
        return column;
    }
    public void SetCol(int c)
    {
        column = c;
    }
    public void SetPortalDistance(int index, int length)
    {
        nextDistance = (length - 1) - index;
        prevdistance = index;
    }
    public void SetPortalDistance(Tile t)
    {
        nextDistance = t.nextDistance+1;
        prevdistance = t.prevdistance+1;
    }
    public void SetAsAstarTile()
    {
        isAStarTile = true;
    }
    public void SetAsMarked()
    {
        isMarked = true;
    }
}