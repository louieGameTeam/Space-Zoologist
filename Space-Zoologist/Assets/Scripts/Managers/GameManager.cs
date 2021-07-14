using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Initialize the instance of this GameManager to null.
    private static GameManager instance = null;
    private PlayTrace currentPlayTrace;

    // On Awake, check the status of the instance. If the instance is null, replace it with the current GameManager.
    // Else, destroy the gameObject this script is attached to. There can only be one.
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
    		DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // Initialize the current PlayTrace object.
        currentPlayTrace = new PlayTrace();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Public method for accessing PlayTrace object.
    public PlayTrace CurrentPlayTrace
    {
        get { return currentPlayTrace; }
        set { currentPlayTrace = value; }
    }
}
