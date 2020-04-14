﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Implements the A Star algorithm without edge weights, so each tile is equally easy to move to. */
public class MapAStar : Map
{
    // Tiles with information needed to run the A* algorithm.
    private AStarTile[,] aStarTiles;

    // Overriden so the base class can be called.
    protected override void Start()
    {
        base.Start();

        aStarTiles = new AStarTile[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                aStarTiles[x, y] = new AStarTile(tileGrid[x, y]);
            }
        }
    }

    // Overriden so the base class can be called.
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.P))
        {
            CalculateAStar();
            ClearDisplay();
        }
    }

    // Find the path to the destination using the A* algorithm.
    private void CalculateAStar()
    {
        AStarTile start = aStarTiles[startPos.x, startPos.y];
        AStarTile end = aStarTiles[destPos.x, destPos.y];
        List<AStarTile> open = new List<AStarTile>(); // List of tiles we need to check.
        List<AStarTile> closed = new List<AStarTile>(); // List of tiles we have already checked.
        AStarTile current = start;

        // Add the starting tile to the open list.
        open.Add(start);
        start.prev = null;

        // Repeat until we have either checked all tiles or found the end.
        while (open.Count > 0 && !open.Contains(end))
        {
            // Find the tile on the open list with the least cost to move to.
            int minCost = int.MaxValue; 
            int lowestIndex = 0;

            for (int t = 0; t < open.Count; t++)
            {
                if (open[t].Cost < minCost)
                {
                    minCost = open[t].Cost;
                    lowestIndex = t;
                }
            }

            current = open[lowestIndex]; // Move to the tile with least cost.
            open.Remove(current); // Remove it from the open list.
            closed.Add(current); // Add it to the closed list.


            // Find all valid adjacent tiles.
            List<AStarTile> adjacent = new List<AStarTile>();

            foreach (Vector2Int dir in allDirections)
            {
                Vector2Int pos = current.Pos + dir;
                if (IsValidTile(pos)) // Check that it is possible to move to the tile.
                {
                    if (!closed.Contains(aStarTiles[pos.x, pos.y])) // Make sure the tile hasn't been already checked.
                    {
                        adjacent.Add(aStarTiles[pos.x, pos.y]);
                    }
                }
            }

            // Add the best a
            foreach (AStarTile tile in adjacent)
            {
                if (open.Contains(tile))
                {
                    // If the adjacent tile is already in the open list, and the distance is shorter via this route,
                    // set the current tile to be its "parent." 
                    if (current.distFromStart + 1 < tile.distFromStart)
                    {
                        tile.distFromStart = current.distFromStart + 1;
                        tile.prev = current;
                    }
                }
                else
                {
                    // If the adjacent tile is not in the open list, add it, and set the current tile to be its "parent." 
                    open.Add(tile);
                    tile.prev = current;
                    tile.distFromStart = current.distFromStart + 1;
                    tile.distFromEnd = Mathf.Abs(tile.Pos.x - end.Pos.x) + Mathf.Abs(tile.Pos.y - end.Pos.y);
                }
            }
        }

        // Build display path.
        pathTiles = new List<Tile>();
        current = end;
        while (current.prev != null)
        {
            pathTiles.Add(current.displayTile);
            current = current.prev;
        }

        pathTiles.Reverse(); // Reverse display path as it is built from the destination to the start.
    }



    // Local class for containing extra details about tiles that the A* algorithm needs.
    // (only accessible to the MapAStar class)
    private class AStarTile
    {
        public Tile displayTile; // Link to the actual Monobehaviour tile object.

        public AStarTile prev; // The previous tile in the path.
        public int distFromStart; // How far have we come from the start?
        public int distFromEnd; // A guess at how far we are from the destination.
        public int Cost
        {
            get
            {
                return distFromStart + distFromEnd;
            }
        }

        // Constructor
        public AStarTile(Tile linkedTile)
        {
            displayTile = linkedTile;
            prev = null;
            distFromStart = 0;
            distFromEnd = 0;
        }

        // Get the display tile grid position.
        public Vector2Int Pos
        {
            get
            {
                return displayTile.Pos;
            }

            set
            {
                displayTile.Pos = value;
            }
        }
    }
}

