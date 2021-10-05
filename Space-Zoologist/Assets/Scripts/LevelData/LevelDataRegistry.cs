using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelDataRegistry : ScriptableObjectSingleton<LevelDataRegistry>
{
    #region Public Typedefs
    [System.Serializable]
    public class LevelDataList
    {
        public LevelData[] levelDatas;
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of all levels, organized into the registry")]
    [WrappedProperty("levelDatas")]
    private LevelDataList[] levelDatas;
    #endregion

    #region Public Methods
    public static LevelData Get(LevelID id)
    {
        if (id.LevelNumber >= 0 && id.LevelNumber < Instance.levelDatas.Length)
        {
            LevelData[] list = Instance.levelDatas[id.LevelNumber].levelDatas;

            if (id.EnclosureNumber >= 0 && id.EnclosureNumber < list.Length)
            {
                return list[id.EnclosureNumber];
            }
            else throw new System.IndexOutOfRangeException("LevelDataRegistry: level #" + id.LevelNumber +
                " does not have an enclosure at index #" + id.EnclosureNumber);
        }
        else throw new System.IndexOutOfRangeException("LevelDataRegistry: level #" + id.LevelNumber +
            " does not exist in the registry");
    }
    #endregion
}