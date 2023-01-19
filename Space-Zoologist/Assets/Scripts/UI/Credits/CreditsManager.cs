using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsManager : MonoBehaviour
{
    private enum RolePriority
    {
        Faculty,
        Project_Manager,
        Tech_Lead,
        Design,
        Art,
        Audio_Designer,
        Writing,
        Backend_Programmer,
        Gameplay_Programmer,
        UI_UX
    }

    /// <summary>
    /// Constant modifier to scroll speed to make it reasonable, simplifies the range of values ScrollSpeedMultiplier can take
    /// </summary>
    private const int SCROLL_SPEED_CONSTANT = 50;


#region Private Fields
    private Regex Pattern = new Regex(@"^(?<FirstName>[a-zA-Z\s]*)[,](?<MiddleName>[a-zA-Z\s]*)[,](?<LastName>[a-zA-Z\s]*)[,](?>[""]?(?<JobTitle>[a-zA-Z\s\/]+)[""]?[,]?){1,}$");

    /// <summary>
    /// Unity reference to the .csv containing employee names and roles. File is assumed to be sorted by last name in alphabetical order after being sorted by role
    /// </summary>
    [SerializeField] private TextAsset EmployeeList = null;
    [SerializeField] private CanvasGroup CreditsCanvasGroup = null;
    [SerializeField] private TMP_Text CreditsContent = null;

    [SerializeField] private Canvas CanvasReference = null;

    /// <summary>
    /// Number of lines of spacing between each role's members
    /// </summary>
    [SerializeField] private int RoleSpacing = 3;
    [SerializeField] private int JobTitleTextSize = 82;

    [Range(1f, 10f)]
    [SerializeField] private float ScrollSpeedMultiplier = 1f;

    private Vector3 CreditsTargetEndPosition = Vector3.zero;
    private bool IsScrolling = false;

    /// <summary>
    /// Maps role priorities to their strings containing formatted employee names for display in a RoleList
    /// </summary>
    private Dictionary<RolePriority, string> RoleDict = null;

    /// <summary>
    /// Maps role names as strings to their priority levels
    /// </summary>
    private readonly Dictionary<string, RolePriority> PriorityDict = new Dictionary<string, RolePriority>
    {
        { "Faculty", RolePriority.Faculty},
        { "Project Manager", RolePriority.Project_Manager},
        { "Tech Lead", RolePriority.Tech_Lead},
        { "Design", RolePriority.Design},
        { "Art", RolePriority.Art},
        { "Audio Designer", RolePriority.Audio_Designer},
        { "Writing", RolePriority.Writing},
        { "Backend Programmer", RolePriority.Backend_Programmer},
        { "Gameplay Programmer", RolePriority.Gameplay_Programmer},
        { "UI/UX", RolePriority.UI_UX},
    }; 
#endregion


#region Monobehaviour Callbacks
    private void Awake()
    {
        if (RoleDict == null)
        {
            LoadCredits();
        }

        SetupCreditsContent();
    }

    private void LateUpdate()
    {
        if (IsScrolling && CreditsContent.rectTransform.position.y < CreditsTargetEndPosition.y)
        {
            CreditsContent.rectTransform.Translate(0, Time.deltaTime * SCROLL_SPEED_CONSTANT * ScrollSpeedMultiplier, 0);
        }
    }
#endregion


#region Private Functions
    /// <summary>
    /// Loads data from EmployeeList into RoleDict
    /// </summary>
    private void LoadCredits()
    {
        RoleDict = new Dictionary<RolePriority, string>();
        string[] employeeListData = EmployeeList.ToString().Split('\n', System.StringSplitOptions.RemoveEmptyEntries);

        // Assumes there are only four columns containing First, Middle, and Last names along with job titles separated by commas
        // Skips first line since first line is reserved for column titles
        for (int i = 1; i < employeeListData.Length; i++)
        {
            Match match = Pattern.Match(employeeListData[i]);
            if (match.Success)
            {
                // Construct full name from regex groups
                string fullName = match.Groups[1].Value;

                if (!string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    fullName += $" {match.Groups[2]}.";
                }

                fullName += $" {match.Groups[3]}";

                CaptureCollection roleNames = match.Groups[4].Captures;
                foreach (Capture roleName in roleNames)
                {
                    string trimmedRoleName = roleName.Value.Trim(new char[] { ' ', '\n', '\r', '\0', '\t' });
                    if (!string.IsNullOrWhiteSpace(trimmedRoleName))
                    {
                        if (!RoleDict.ContainsKey(PriorityDict[trimmedRoleName]))
                            RoleDict.Add(PriorityDict[trimmedRoleName], "");

                        RoleDict[PriorityDict[trimmedRoleName]] += $"{fullName}\n";
                    }
                }
            }
        }
    }

    /// <summary>
    /// Places content parsed from LoadCredits into the CreditsContent text, adding formatting
    /// </summary>
    private void SetupCreditsContent()
    {
        string creditsText = "";

        foreach (KeyValuePair<string, RolePriority> role in PriorityDict)
        {
            creditsText += $"<size={JobTitleTextSize}><b><u>{role.Key}</u></b></size>\n\n";
            creditsText += RoleDict[role.Value];

            for (int i = 0; i < RoleSpacing; i++)
            {
                creditsText += "\n";
            }
        }

        creditsText = creditsText.TrimEnd();

        CreditsContent.text = creditsText;

        // Set the target position for when the CreditsContent has left the top of the canvas
        CreditsTargetEndPosition = new Vector3(CreditsContent.rectTransform.position.x, CanvasReference.GetComponent<RectTransform>().rect.height);
    }

    /// <summary>
    /// Starts the credits panel
    /// </summary>
    public void StartCredits()
    {
        CreditsCanvasGroup.gameObject.SetActive(true);

        // Force rebuild of layout to update height of RectTransform
        LayoutRebuilder.ForceRebuildLayoutImmediate(CreditsContent.rectTransform);

        ResetCreditsContentPosition();
        IsScrolling = true;
    }

    /// <summary>
    /// Stops the credits panel
    /// </summary>
    public void StopCredits()
    {
        CreditsCanvasGroup.gameObject.SetActive(false);
        IsScrolling = false;
    }

    /// <summary>
    /// Sets CreditsContent to its starting position (out of view below the Canvas)
    /// </summary>
    private void ResetCreditsContentPosition()
    {
        CreditsContent.rectTransform.anchoredPosition = new Vector2(0, -CreditsContent.rectTransform.rect.height);
    }
#endregion
    

#region Debug Functions
    /// <summary>
    /// Prints the contents of RoleDict to Debug.Log
    /// </summary>
    private void DebugLogRoleDict()
    {
        foreach (RolePriority role in Enum.GetValues(typeof(RolePriority)))
        {
            Debug.Log(Enum.GetName(typeof(RolePriority), role));

            if (RoleDict.ContainsKey(role))
                Debug.Log(RoleDict[role]);
            else
                Debug.Log("NO MEMBERS");
        }
    }
#endregion
}