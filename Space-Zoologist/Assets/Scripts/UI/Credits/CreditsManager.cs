using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    enum RolePriority
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

    #region Private Fields
    private Regex Pattern = new Regex(@"^(?<FirstName>[a-zA-Z\s]*)[,](?<MiddleName>[a-zA-Z\s]*)[,](?<LastName>[a-zA-Z\s]*)[,](?>[""]?(?<JobTitle>[a-zA-Z\s\/]+)[""]?[,]?){1,}$");

    /// <summary>
    /// Unity reference to the .csv containing employee names and roles. File is assumed to be sorted by last name in alphabetical order after being sorted by role
    /// </summary>
    [SerializeField] private TextAsset EmployeeList;

    /// <summary>
    /// Maps role priorities to their strings containing formatted employee names for display in a RoleList
    /// </summary>
    private Dictionary<RolePriority, string> RoleDict;

    /// <summary>
    /// Maps role names as strings to their priority levels
    /// </summary>
    private Dictionary<string, RolePriority> PriorityDict = new Dictionary<string, RolePriority>
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
        LoadCredits();
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
                    fullName += $" {match.Groups[2]}.";

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

    // TODO: FINISH IMPLEMENTING CREATION OF ROLELIST OBJECTS AND SETTING THEIR VALUES AND SCROLLING/BOUNDCHECKING/CLEANUP

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