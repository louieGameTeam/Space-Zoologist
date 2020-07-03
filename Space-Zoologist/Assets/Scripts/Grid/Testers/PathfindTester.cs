using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

public class PathfindTester : MonoBehaviour
{
    /// <summary>
    /// Mask for showing the demo.
    /// </summary>
    public Tilemap mask;
    public Population pop;
    public Transform goal;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Graph", 0.3f);
    }


    /// <summary>
    /// Graphing for demo purposes, may be worked into the game as a sort of inspection mode?
    /// </summary>
    public void Graph()
    {
        ReservePartitionManager rpm = ReservePartitionManager.ins;

        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        int id = rpm.PopulationToID[pop];
        Vector3Int end = FindObjectOfType<TileSystem>().WorldToCell(goal.position);
        Node node = new Node(FindObjectOfType<TileSystem>().WorldToCell(pop.transform.position), end, 0, null);
        List<Node> queue = new List<Node>();
        queue.Add(node);
        bool found = false;
        print(node.pos);
        print(end);


        while (!found && queue.Count <= 1000 && queue.Count > 0) {
            queue.Sort();
            node = queue[0];
            Vector3Int pos = node.pos;
            queue.Remove(node);
            visited.Add(node.pos);

            // remove any node 
            LinkedList<Node> toRemove = new LinkedList<Node>();
            foreach (Node n in queue) {
                if (n.pos == pos && n != node) {
                    toRemove.AddLast(n);
                }
            }
            foreach (Node n in toRemove) {
                queue.Remove(n);
            }

            if (pos != end)
            {
                // if adjacent tiles are not visited and can be accessed, add it to queue
                // new cost of node is 'original cost' + 'preference of tile'
                if (!visited.Contains(pos+Vector3Int.up) && rpm.CanAccess(pop, pos + Vector3Int.up))
                    queue.Add(new Node(pos + Vector3Int.up, end, node.cost + rpm.PopulationPreference[pos+Vector3Int.up][id], node));
                if (!visited.Contains(pos + Vector3Int.left) && rpm.CanAccess(pop, pos + Vector3Int.left))
                    queue.Add(new Node(pos + Vector3Int.left, end, node.cost + rpm.PopulationPreference[pos + Vector3Int.left][id], node));
                if (!visited.Contains(pos + Vector3Int.down) && rpm.CanAccess(pop, pos + Vector3Int.down))
                    queue.Add(new Node(pos + Vector3Int.down, end, node.cost + rpm.PopulationPreference[pos + Vector3Int.down][id], node));
                if (!visited.Contains(pos + Vector3Int.right) && rpm.CanAccess(pop, pos + Vector3Int.right))
                    queue.Add(new Node(pos + Vector3Int.right, end, node.cost + rpm.PopulationPreference[pos + Vector3Int.right][id], node));
            }
            else {
                found = true;
            }
        }
        print(found);
        print(queue.Count);

        // set color based on the fraction density/maxdensity
        while (node != null)
        {
            // By default the flag is TileFlags.LockColor
            mask.SetTileFlags(node.pos, TileFlags.None);

            // set color of tile, close to maxDensity = red, close to 0 = green, in the middle = orange
            mask.SetColor(node.pos, new Color(0, 1f, 0, 1f));
            node = node.prev;
        }

    }
}
class Node: IComparable<Node> {
    public Vector3Int pos;
    public Node prev;
    public Vector3Int goal;
    public float dist;
    public float cost;
    public static Node operator +(Node a, Vector3Int b) {
        return new Node(a.pos + b, a.goal, a.cost, a);
    }
    public Node(Vector3Int position, Vector3Int cur_goal, float cur_cost, Node previous) {
        pos = position;
        prev = previous;
        goal = cur_goal;
        cost = cur_cost;
        dist = Vector3Int.Distance(position, goal)*10;
    }
    public int CompareTo(Node a) {
        if (cost + dist < a.cost + a.dist)
        {
            return -1;
        }
        else if (cost + dist > a.cost + a.dist)
        {
            return 1;
        }
        else {
            return 0;
        }
    }
}
