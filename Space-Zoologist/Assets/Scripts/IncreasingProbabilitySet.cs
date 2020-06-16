using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Returns a random number from 0 to n (referred to as options). The most recently returned options will have 
/// the least likelyhood of being returned next.
/// </summary>
public class IncreasingProbabilitySet
{
    private List<int> options = new List<int>();
    private readonly Random random = new Random();

    public IncreasingProbabilitySet(int numOptions)
    {
        for (var i = 0; i < numOptions; i++)
        {
            options.Add(1);
        }
    }

    /// <summary>
    /// Gets a random number according to the current probability of each of the options, resets the probability of 
    /// the option that was given, and raises the probability of the rest of the options.
    /// </summary>
    /// <returns>The randomly chosen option.</returns>
    public int GetOption()
    {
        int option = -1;
        int sum = options.Sum();
        int hit = this.random.Next(sum) + 1;
        int runningTotal = 0;
        for (var i = 0; i < options.Count; i++)
        {
            runningTotal += options[i];

            if (hit <= runningTotal)
            {
                option = i;
                break;
            }
        }

        for (var i = 0; i < options.Count; i++)
        {
            if (i != option) options[i]++;
            else options[i] = 1;
        }

        return option;
    }

    /// <summary>
    /// Resets the current probabilities of the options.
    /// </summary>
    public void Reset()
    {
        for(var i = 0; i < options.Count; i++)
        {
            options[i] = 1;
        }
    }
}
