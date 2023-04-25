using System;
using System.Collections.Generic;
using UnityEngine;

// Modify as needed
[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [System.Serializable]
    private class NotebookUnlockConfig
    {
        public bool unlockByDefault = true;
        [Header("If Not UnlockByDefault")]
        public LevelID unlockLevelID;
    }
    
    public ItemID ID => ItemRegistry.FindShopItem(this);
    public ItemRegistry.Category Type => ID.Category;
    public string ItemName => itemName;
    public int Price => price;
    public string Description => description;
    public Sprite Icon => icon;
    public List<AudioClip> AudioClips => audio;
    public int buildTime => BuildTime;

    public LevelID UnlockLevelID => notebookUnlockConfig.unlockLevelID;
    public bool unlockByDefault => notebookUnlockConfig.unlockByDefault;

    [SerializeField] private string itemName = default;
    [SerializeField] private int price = default;
    [SerializeField] private string description = default;
    [SerializeField] private Sprite icon = default;
    [SerializeField] private List<AudioClip> audio = default;
    [SerializeField] private int BuildTime = default;
    [SerializeField] private NotebookUnlockConfig notebookUnlockConfig;

    public void SetupData(string name, int price)
    {
        this.itemName = name;
        this.price = price;
    }
}
