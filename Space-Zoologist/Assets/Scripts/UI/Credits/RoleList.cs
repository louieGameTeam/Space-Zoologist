using UnityEngine;
using TMPro;

public class RoleList : MonoBehaviour
{
    #region Private Fields
    [SerializeField] private TMP_Text RoleName;
    [SerializeField] private TMP_Text RoleMembers;
    #endregion

    #region Private Methods
    /// <summary>
    /// Scrolls this RoleList upwards
    /// </summary>
    public void Scroll()
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + (Time.deltaTime * 60), this.transform.position.z);
    }

    /// <summary>
    /// Sets text of RoleList
    /// </summary>
    /// <param name="roleName"> Name of the roles that are shown in this RoleList </param>
    /// <param name="roleMembers"> Members of the role shown in this RoleList </param>
    public void SetupText(string roleName, string roleMembers)
    {
        RoleName.text = roleName;
        RoleMembers.text = roleMembers;
    }

    /// <summary>
    /// Calculates the current world position of the top edge of this RoleList, for use in determining if
    /// this RoleList is no longer visible on the canvas it resides in
    /// </summary>
    /// <returns> Returns the y-coordinate of the top edge of this RoleList in worldspace </returns>
    public float TopEdgeWorldPosition()
    {
        Vector3[] worldPositionCorners = new Vector3[4];
        this.gameObject.GetComponent<RectTransform>().GetWorldCorners(worldPositionCorners);

        return worldPositionCorners[1].y;
    }
    #endregion
}