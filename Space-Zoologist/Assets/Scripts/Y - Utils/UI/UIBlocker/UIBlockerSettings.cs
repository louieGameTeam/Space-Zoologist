using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu]
public class UIBlockerSettings : ScriptableObjectSingleton<UIBlockerSettings>
{
    #region Public Properties
    public static string[] BlockableOperations => Instance.blockableOperations;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of operations that can be blocked by UI elements")]
    private string[] blockableOperations = null;
    #endregion

    #region Public Methods
    public static int GetTotalBlockableOperations() => Instance.blockableOperations.Length;
    public static string GetBlockableOperation(int index)
    {
        if (index >= 0 && index < Instance.blockableOperations.Length)
        {
            return Instance.blockableOperations[index];
        }
        else throw new System.IndexOutOfRangeException(
            $"No ui blockable operation found at index '{index}'. " +
            $"Total operations: {Instance.blockableOperations.Length}");
    }
    public static int IndexOf(string operation) => System.Array.IndexOf(Instance.blockableOperations, operation);
    public static bool OperationIsAvailable(string operation)
    {
        // Setup a new pointer event
        PointerEventData pointerEvent = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // Get all raycast results
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEvent, results);

        // Count the number of blockers that block this operation
        int appliedBlockers = results
            .Select(result => result.gameObject.GetComponentInParent<UIBlocker>())
            .Where(blocker => blocker != null)
            .Where(blocker => blocker.OperationIsBlocked(operation))
            .Count();

        // Advance by click only if the raycast didn't hit anything with a blocker tag
        return appliedBlockers <= 0;
    }
    #endregion
}
