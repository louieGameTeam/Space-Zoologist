using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AnimalPathfinding
{
    /// <summary>
    /// Queues up requests instead of attempting to handle them all at once.
    /// </summary>
    public class PathRequestManager : MonoBehaviour
    {
        Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        PathRequest currentPathRequest;
        Pathfinding pathfinding;

        bool isProcessingPath;

        static PathRequestManager instance;

        void Awake()
        {
            instance = this;
            pathfinding = GetComponent<Pathfinding>();
        }

        public static void RequestPath(Node start, Node end, Action<Node, bool> callback, Grid grid)
        {
            PathRequest newRequest = new PathRequest(start, end, callback, grid);
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

        public void FinishedProcessPath(Node path, bool success)
        {
            currentPathRequest.callback(path, success);
            isProcessingPath = false;
            TryProcessNext();
        }

        struct PathRequest
        {
            public Node pathStart;
            public Node pathEnd;
            public Action<Node, bool> callback;
            public Grid grid;

            public PathRequest(Node pathStart, Node pathEnd, Action<Node, bool> callback, Grid grid)
            {
                this.pathStart = pathStart;
                this.pathEnd = pathEnd;
                this.callback = callback;
                this.grid = grid;
            }

        }

    }
}