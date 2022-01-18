using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName="NewLevel", menuName="Scene Data/New Level")]
public class Level : ScriptableObject
{
    public LevelID ID => LevelID.FromSceneName(SceneName);

    [SerializeField] public string SceneName = default;
    [SerializeField] public string Description = default;
    [SerializeField] public string Name = default;
    [SerializeField] public Sprite Sprite = default;
    [Expandable] public LevelData Data = default;
}
