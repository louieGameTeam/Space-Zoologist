using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceRequest
{
    [SerializeField]
    [Tooltip("Priority of the request relative to other requests being made")]
    private int priority;
    [SerializeField]
    [Tooltip("Target of the resource request")]
    private ResearchCategory target;
    [SerializeField]
    [Tooltip("Need of the target that this resource request is supposed to improve")]
    private NeedType improvedNeed;
    [SerializeField]
    [Tooltip("Quantity of the resource requested")]
    private int quantity;
    [SerializeField]
    [Tooltip("Name of the item requested")]
    private string itemName;

    public bool SendRequest(ResourceManager resources)
    {
        // Validate this request
        // Add resources to the resource manager
        return true;
    }
}
