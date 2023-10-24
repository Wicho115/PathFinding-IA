using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileMapExtensions
{
    public static List<Vector3Int> GetNeighbours(this Tilemap tileMap, Vector3Int origin, bool includeEmpty = false)
    {
        var neighbours = new List<Vector3Int>();
        var first = origin;
        first.x += 1;
        var second = origin;
        second.x -= 1;
        var third = origin;
        third.y += 1;
        var fourth = origin;
        fourth.y -= 1;

        neighbours.Add(first);
        neighbours.Add(second);
        neighbours.Add(third);
        neighbours.Add(fourth);

        if(!includeEmpty)
            neighbours = neighbours.Where(pos => tileMap.GetTile(pos) != null).ToList();

        return neighbours;
    }


}
