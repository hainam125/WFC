using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public bool collapsed;
    public readonly List<Tile> tileOptions;
    public readonly Vector3 pos;

    public Cell(Vector3 position, bool collapseState, Tile[] tiles)
    {
        pos = position;
        collapsed = collapseState;
        tileOptions = new List<Tile>(tiles);
    }
}
