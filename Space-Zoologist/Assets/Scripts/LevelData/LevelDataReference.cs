using UnityEngine;

public class LevelDataReference : MonoBehaviour
{
    [Expandable] public LevelData LevelData = default;

    public static LevelDataReference instance;

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
    }
}
