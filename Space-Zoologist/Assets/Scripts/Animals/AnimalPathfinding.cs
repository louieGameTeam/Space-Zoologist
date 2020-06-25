using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// Modified https://greenday96.blogspot.com/2018/10/c-4-aa-star-simple-4-way-algorithm.html

/*
TODO use this one instead https://github.com/RonenNess/Unity-2d-pathfinding
and improve by following https://www.youtube.com/watch?v=3Dw5d7PlcTM&t=737s

*/

// Holds x y location, parent location, and algorithm scoress
public class Location
{
    public int X;
    public int Y;
    public int F;
    public int G;
    public int H;
    public Location Parent;
}

public class AnimalPathfinding : MonoBehaviour
{
    public Location FindPath(Population population, Vector3 vectorStart, Vector3 vectorTarget)
    {

        // Convert start and end to location using WorldToCell
        var start = new Location() { X = population.WorldToCell(vectorStart).x, Y = population.WorldToCell(vectorStart).y };
        var target = new Location() { X = population.WorldToCell(vectorTarget).x, Y = population.WorldToCell(vectorTarget).y };
        // Debug.Log("Start Coordinate (" + start.X + ", " + start.Y + ")");
        // Debug.Log("End Coordinate (" + target.X + ", " + target.Y + ")");

        // open and closed list for keeping track of visited/unvisited nodes, current for keeping track of path nodes
        Location current = null;
        var openList = new List<Location>();
        var closedList = new List<Location>();
        int g = 0;
        // start by adding the original position to the open list  
        openList.Add(start);
        while (openList.Count > 0)
        {
            // get the square with the lowest F score  
            var lowest = openList.Min(l => l.F);
            current = openList.First(l => l.F == lowest);

            // add the current square to the closed list  
            closedList.Add(current);

            // remove it from the open list  
            openList.Remove(current);

            // if we added the destination to the closed list, we've found a path  
            if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)
                break;

            List<Location> adjacentSquares = GetWalkableSurroundingSquares(current.X, current.Y, population, openList);
            g = current.G + 1;

            foreach (var adjacentSquare in adjacentSquares)
            {
                // if this adjacent square is already in the closed list, ignore it  
                if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                    && l.Y == adjacentSquare.Y) != null)
                    continue;

                // if it's not in the open list...  
                if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                    && l.Y == adjacentSquare.Y) == null)
                {
                    // compute its score, set the parent  
                    adjacentSquare.G = g;
                    adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                    adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                    adjacentSquare.Parent = current;

                    // and add it to the open list  
                    openList.Insert(0, adjacentSquare);
                }
                else
                {
                    // test if using the current G score makes the adjacent square's F score  
                    // lower, if yes update the parent because it means it's a better path  
                    if (g + adjacentSquare.H < adjacentSquare.F)
                    {
                        adjacentSquare.G = g;
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;
                    }
                }
            }
        }

        Location prev = null, curr = current, next = null;
        // Need to reverse linked list so the animals can actually follow it.
        // May be room for optimization here if existing algorithm can be improved 
        while(curr != null)
        {
            // Debug.Log("Should be changing color");
            // Debug.Log( "(" + curr.X + ", " + curr.Y + ")");
            next = curr.Parent;
            curr.Parent = prev;
            prev = curr;
            curr = next;    
        }
        return prev;
    }

    // See TODO for first optimization, another optimization would be setting the bounds for this bfs operation. 
    public List<Location> GetWalkableSurroundingSquares(int x, int y, Population population, List<Location> openList)
    {
        List<Location> list = new List<Location>();
        // tile below
        if (ReservePartitionManager.ins.CanAccess(population, new Vector3Int(x, y-1, 0)))
        {
            Location node = openList.Find(l => l.X == x && l.Y == y - 1);
            if (node == null) list.Add(new Location() { X = x, Y = y - 1 });
            else list.Add(node);
        }
        // tile above
        if (ReservePartitionManager.ins.CanAccess(population, new Vector3Int(x, y+1, 0)))
        {
            Location node = openList.Find(l => l.X == x && l.Y == y + 1);
            if (node == null) list.Add(new Location() { X = x, Y = y + 1 });
            else list.Add(node);
        }
        // tile left
        if (ReservePartitionManager.ins.CanAccess(population, new Vector3Int(x-1, y, 0)))
        {
            Location node = openList.Find(l => l.X == x - 1 && l.Y == y);
            if (node == null) list.Add(new Location() { X = x - 1, Y = y });
            else list.Add(node);
        }
        // tile right
        if (ReservePartitionManager.ins.CanAccess(population, new Vector3Int(x+1, y, 0)))
        {
            Location node = openList.Find(l => l.X == x + 1 && l.Y == y);
            if (node == null) list.Add(new Location() { X = x + 1, Y = y });
            else list.Add(node);
        }
        return list;
    }

    // TODO: determine how to handle negative coordinate values or modify above to ensure no negative coordinate values used
    private int ComputeHScore(int x, int y, int targetX, int targetY)
    {
        return Mathf.Abs(targetX - x) + Mathf.Abs(targetY - y);
    }
}