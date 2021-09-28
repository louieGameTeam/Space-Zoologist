using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A data class containing information about a given set from a level.
[System.Serializable]
public class SetTrace
{
    private enum Result { PASS, FAIL };
    private enum Failure { TIME, MONEY };
    // A string representing the player ID associated with this set trace.
    [SerializeField] private string playerID;
    // An enumeration representing status of set -- PASS/FAIL.
    [SerializeField] private Result result;
    // An integer representing number of days taken at time of pass/fail.
    [SerializeField] private int numDays;
    // An integer representing number of attempts of the set.
    [SerializeField] private int numAttempts;
    // An integer representing currency at time of pass/fail.
    [SerializeField] private int currency;
    // An enumeration representing reason for set failure, if applicable.
    [SerializeField] private Failure failure;
    // An integer representing the score received on the report back for food related questions.
    [SerializeField] private int foodScore;
    // An integer representing the score received on the report back for terrain related questions.
    [SerializeField] private int terrainScore;
}
