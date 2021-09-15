using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AnimalPathfinding
{
    /// <summary>
    /// Queues up requests to spread calculations across frames.
    /// </summary>
    public class PathRequestManager : MonoBehaviour
    {
        Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        PathRequest currentPathRequest;
        Pathfinding pathfinding;

        bool isProcessingPath;

        public static PathRequestManager instance;

        void Awake()
        {
            if (instance != null && this != instance)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
            pathfinding = GetComponent<Pathfinding>();
        }

        /// <summary>
        /// Used by behaviors to calculate a path between 2 points on a grid (2d array calculated by RPM). 
        /// Callback invoked after path has been calculated, returning the path and true if successful.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="callback">Defined in base.Behavior: PathFound</param>
        /// <param name="grid"></param>
        public static void RequestPath(Vector3Int start, Vector3Int end, Action<List<Vector3>, bool> callback, Grid grid)
        {
            if (end.x == -1 && end.y == -1)
            {
                Debug.Log("No accessible locations, pathfinding returning early");
                return;
            }
            if (instance == null)
            {
                Debug.Log("PathRequestManager not attached to GameObject, Pathfinding will not work");
                return;
            }
            // for (int x=0; x<30; x++)
            // {
            //     for (int y=0; y<20; y++)
            //     {
            //         Debug.Log("(" + x + ", " + y + ") can access: " + grid.nodes[x, y].walkable);
            //     }
            // }
            // Debug.Log("Grid Size: " + grid.nodes.GetLength(0) +" x " + grid.nodes.GetLength(1));
            // Debug.Log("Start map position: ");
            // Debug.Log("("+start.x+","+start.y+")");
            // Debug.Log("End map position: ");
            // Debug.Log("("+end.x+","+end.y+")");
            AnimalPathfinding.Node nodeStart = grid.GetNode(start.x, start.y);
            AnimalPathfinding.Node nodeEnd = grid.GetNode(end.x, end.y);
            // Debug.Log("Start grid position: ");
            // Debug.Log("("+nodeStart.gridX+","+nodeStart.gridY+")");
            // Debug.Log("End grid position: ");
            // Debug.Log("("+nodeEnd.gridX+","+nodeEnd.gridY+")");
            PathRequest newRequest = new PathRequest(nodeStart, nodeEnd, callback, grid);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (!isProcessingPath && pathRequestQueue.Count > 0)
            {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;
                pathfinding.StartPathFind(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.grid);
            }
        }

        /// <summary>
        /// Called by Pathfinding once a path has been calculated.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="success"></param>
        public void FinishedProcessPath(List<Vector3> path, bool success)
        {
            currentPathRequest.callback(path, success);
            isProcessingPath = false;
            TryProcessNext();
        }

        struct PathRequest
        {
            public Node pathStart;
            public Node pathEnd;
            public Action<List<Vector3>, bool> callback;
            public Grid grid;

            public PathRequest(Node pathStart, Node pathEnd, Action<List<Vector3>, bool> callback, Grid grid)
            {
                this.pathStart = pathStart;
                this.pathEnd = pathEnd;
                this.callback = callback;
                this.grid = grid;
            }

        }

    }
}