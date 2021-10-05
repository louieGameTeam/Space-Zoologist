using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

/// <summary>
/// Contains all the starting data of a particular level.
/// </summary>
[CreateAssetMenu(fileName="LevelData", menuName="Scene Data/LevelData")]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class ItemData
    {
        [Expandable] public Item itemObject;
        public int initialAmount;

        public ItemData(Item item)
        {
            itemObject = item;
        }
    }

    public Level Level = default;
    public float StartingBalance => startingBalance;
    public List<FoodSourceSpecies> FoodSourceSpecies => foodSources;
    public List<AnimalSpecies> AnimalSpecies => animalSpecies;
    public List<ItemData> ItemQuantities => itemQuantities;
    public NPCConversation StartingConversation => startingConversation;
    public NPCConversation DefaultConversation => defaultConversation;
    public NormalOrQuizConversation PassedConversation => passedConversation;
    public NPCConversation RestartConversation => restertEnclosureConversation;
    public List<Vector3Int> StartinPositions => startingPositions;
    public AudioClip LevelMusic => levelMusic;

    [SerializeField] public float startingBalance = default;
    [SerializeField] public int MapWidth = default;
    [SerializeField] public int MapHeight = default;
    [Expandable] public LevelObjectiveData LevelObjectiveData = default;
    [Expandable] public List<FoodSourceSpecies> foodSources = default;
    [Expandable] public List<AnimalSpecies> animalSpecies = default;
    [SerializeField] public List<ItemData> itemQuantities = default;

    [SerializeField] private AudioClip levelMusic = default;
    [SerializeField] private List<Vector3Int> startingPositions = default;
    [SerializeField] private NPCConversation startingConversation = default;
    [SerializeField] private NPCConversation defaultConversation = default;
    [Header("After level completed")]
    [SerializeField] NormalOrQuizConversation passedConversation = default;
    [SerializeField] NPCConversation restertEnclosureConversation = default;
}
