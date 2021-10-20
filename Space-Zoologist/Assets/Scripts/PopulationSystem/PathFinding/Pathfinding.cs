/**
 * Provide simple path-finding algorithm with tile weight support.
 * Based on code and tutorial by Sebastian Lague (https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ).
 *
 * Author: Ronen Ness.
 * Since: 2016.
 * Improved on above design by adding heap optimization and PathRequestManager according to Sebastian tutorial series.
 * Further optimization/improvements may start here: https://www.youtube.com/watch?v=Tb-rM3wGwv4
*/
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace AnimalPathfinding
{
    /// <summary>
    /// Used by PathRequestManager to start a coroutine which calculates the path from point A to B on a grid (2d array).
    /// </summary>
    /// Improvements: Caching paths between visited nodes on the grid
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
        // Prevent infinite searches

        public void StartPathFind(Node start, Node targetPos, Grid grid)
        {
            // Coroutine causes problems when it is called on the same frame
            // StartCoroutine(FindPath(start, targetPos, grid));
            FindPathDirect(start, targetPos, grid);
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
            if (startPos == null || targetPos == null || grid == null)
            {
                Debug.Log("Issue with pathfinding a");
                yield return null;
                PathRequestManager.instance.FinishedProcessPath(null, false);
            }
            Node startNode = startPos;
            Node targetNode = targetPos;
            List<Vector3> path = new List<Vector3>();
            if ((!startNode.walkable && targetNode.walkable) || startNode.Equals(targetNode))
            {
                path.Add(new Vector3(targetNode.gridX, targetNode.gridY, 0));
                yield return null;
                PathRequestManager.instance.FinishedProcessPath(path, true);
            }

            Heap<Node> openSet = new Heap<Node>(grid.MaxGridSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);
            while (openSet.currentItemCount > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                if (currentNode == targetNode)
                {
                    path = SetupPathFound(currentNode, startPos);
                    break;
                }
                closedSet.Add(currentNode);

                foreach (Node neighbour in grid.GetNeighbours(currentNode, DistanceType.Manhattan))
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
            bool pathSuccessfullyFound = (path.Count > 0) ? true : false;
            if (!pathSuccessfullyFound)
            {
                //Debug.Log("Issue with pathfinding b");
            }
            PathRequestManager.instance.FinishedProcessPath(path, pathSuccessfullyFound);
        }

        /// <summary>
        /// Non-coroutine version of the FindPath() method
        /// </summary>
        /// <param name="grid">Grid to search.</param>
        /// <param name="startPos">Starting position.</param>
        /// <param name="targetPos">Ending position.</param>
        /// <param name="distance">The type of distance, Euclidean or Manhattan.</param>
        /// <returns>(Return through callback) List of grid nodes that represent the path to walk.</returns>
        void FindPathDirect(Node startPos, Node targetPos, Grid grid)
        {
            if (startPos == null || targetPos == null || grid == null)
            {
                Debug.Log("Issue with pathfinding a");
                PathRequestManager.instance.FinishedProcessPath(null, false);
            }
            Node startNode = startPos;
            Node targetNode = targetPos;
            List<Vector3> path = new List<Vector3>();
            if ((!startNode.walkable && targetNode.walkable) || startNode.Equals(targetNode))
            {
                path.Add(new Vector3(targetNode.gridX, targetNode.gridY, 0));
                PathRequestManager.instance.FinishedProcessPath(path, true);
            }

            Heap<Node> openSet = new Heap<Node>(grid.MaxGridSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);
            while (openSet.currentItemCount > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                if (currentNode == targetNode)
                {
                    path = SetupPathFound(currentNode, startPos);
                    break;
                }
                closedSet.Add(currentNode);

                foreach (Node neighbour in grid.GetNeighbours(currentNode, DistanceType.Manhattan))
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

            bool pathSuccessfullyFound = (path.Count > 0) ? true : false;
            if (!pathSuccessfullyFound)
            {
                //Debug.Log("Issue with pathfinding b");
            }
            PathRequestManager.instance.FinishedProcessPath(path, pathSuccessfullyFound);
        }
        private List<Vector3> SetupPathFound(Node nodePath, Node start)
        {
            List<Vector3> path = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;
            while(nodePath.parent != null && nodePath != start)
            {
                // Simplify path by skipping nodes that don't change the direction
                Vector2 directionNew = new Vector2(nodePath.gridX - nodePath.parent.gridX, nodePath.gridY - nodePath.parent.gridY);
                if (directionNew != directionOld) {
                    path.Add(new Vector3(nodePath.gridX, nodePath.gridY, 0));
                    //TilemapUtil.ins.map.SetColor(TilemapUtil.ins.GridToWorld(new Vector3(nodePath.gridX, nodePath.gridY, 0)), Color.green);
                }
                directionOld = directionNew;
                nodePath = nodePath.parent;
            }
            // Could potentially optimize from O(2n) to O(n) if linked list kept track of length. Could then add paths backwards to predefined list<vector3> length
            path.Reverse();
            return path;
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