/**
 * Provide simple path-finding algorithm with tile weight support.
 * Based on code and tutorial by Sebastian Lague (https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ).
 *
 * Author: Ronen Ness.
 * Since: 2016.
*/
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

// TODO use hashset to mark visited vector locations on get neighbor, remove grid
namespace AnimalPathfinding
{
    /// <summary>
    /// Main class to find the best path to walk from A to B.
    ///
    /// Usage example:
    /// Grid grid = new Grid(width, height, tiles_costs);
    /// </summary>
    public class Pathfinding : MonoBehaviour
    {
        /// <summary>
        /// Different ways to calculate path distance.
        /// </summary>
		public enum DistanceType
		{
            /// <summary>
            /// The "ordinary" straight-line distance between two points.
            /// </summary>
			Euclidean,

            /// <summary>
            /// Distance without diagonals, only horizontal and/or vertical path lines.
            /// </summary>
			Manhattan
        }

        private bool ignoreWeights = true;

        public void StartPathFind(Node start, Node targetPos, Grid grid)
        {
            StartCoroutine(FindPath(start, targetPos, grid));
        }

        /// <summary>
        /// Internal function that implements the path-finding algorithm.
        /// </summary>
        /// <param name="grid">Grid to search.</param>
        /// <param name="startPos">Starting position.</param>
        /// <param name="targetPos">Ending position.</param>
        /// <param name="distance">The type of distance, Euclidean or Manhattan.</param>
        /// <returns>List of grid nodes that represent the path to walk.</returns>
        IEnumerator FindPath(Node startPos, Node targetPos, Grid grid)
        {
            Node startNode = startPos;
            Node targetNode = targetPos;
            Node path = null;
            if (!startNode.walkable && targetNode.walkable)
            {
                yield return null;
                PathRequestManager.instance.FinishedProcessPath(path, path==null);
            }

            Heap<Node> openSet = new Heap<Node>(grid.MaxGridSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.currentItemCount > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
                if (currentNode == targetNode)
                {
                    path = SetupPathFound(currentNode);
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode, DistanceType.Euclidean))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) * (ignoreWeights ? 1 : (int)(10.0f * neighbour.price));
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            yield return null;
            bool pathSuccessfullyFound = (path != null) ? true : false;
            PathRequestManager.instance.FinishedProcessPath(path, pathSuccessfullyFound);
        }

        // Need to reverse linked list so the animals can follow it from their current location
        private Node SetupPathFound(Node pathFound)
        {
            Node prev = null, curr = pathFound, next = null;

            // Simplify path by skipping nodes that don't change the direction
            Vector2 directionOld = Vector2.zero;
            while(curr.parent != null)
            {
                // Vector2 directionNew = new Vector2(curr.gridX - curr.parent.gridX, curr.gridY - curr.parent.gridY);
                // if (directionNew != directionOld) {
                //     next = curr.parent;
                //     curr.parent = prev;
                //     prev = curr;
                //     curr = next;
                //     // prev.gridX += MapToGridUtil.ins.map.origin.x;
                //     // prev.gridY += MapToGridUtil.ins.map.origin.y;
                // }
                // else
                // {
                //     curr = curr.parent;
                // }
                // directionOld = directionNew;

                next = curr.parent;
                curr.parent = prev;
                prev = curr;
                curr = next;
            }
            return prev;
        }

        /// <summary>
        /// Get distance between two nodes.
        /// </summary>
        /// <param name="nodeA">First node.</param>
        /// <param name="nodeB">Second node.</param>
        /// <returns>Distance between nodes.</returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = System.Math.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = System.Math.Abs(nodeA.gridY - nodeB.gridY);
            return (dstX > dstY) ?
                14 * dstY + 10 * (dstX - dstY) :
                14 * dstX + 10 * (dstY - dstX);
        }
    }

}