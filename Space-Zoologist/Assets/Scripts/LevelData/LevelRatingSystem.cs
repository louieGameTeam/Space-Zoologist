using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelRatingSystem
{
    #region Public Typedefs
    [System.Serializable]
    public class SpeciesStability
    {
        public SpeciesType species;
        public bool isStable;
    }
    [System.Serializable]
    public class SpeciesCount
    {
        public SpeciesType species;
        public int count;
    }
    #endregion

    #region Public Fields
    public static readonly string noRatingText = "No rating - enclosure not yet designed";
    public static readonly string[] ratingText = new string[]
    {
        "Needs redesign. Species’ populations are unstable",
        "Almost there! Some species are self sustaining",
        "Congratulations, all species are self sustaining in this enclosure!"
    };
    #endregion

    #region Public Methods
    public static string GetRatingText(int rating)
    {
        if (rating >= 0 && rating < ratingText.Length)
        {
            return ratingText[rating];
        }
        else return noRatingText;
    }
    public static int RateCurrentLevel()
    {
        GameManager gameManager = GameManager.Instance;

        // If a game manager was found then rate the level it is currently managing
        if (gameManager)
        {
            return RateLevel(gameManager.m_populationManager, gameManager.LevelData.LevelObjectiveData);
        }
        else throw new MissingReferenceException($"{nameof(LevelRatingSystem)}: " +
            $"cannot rate the current level because no game manager could be found " +
            $"in the current scene");
    }
    public static int RateLevel(PopulationManager populationManager, LevelObjectiveData objective)
    {
        // Get an array of species stability data
        SpeciesStability[] speciesStabilities = ComputeSpeciesStability(populationManager, objective);

        // Return the best score if all species are stable
        if (speciesStabilities.All(species => species.isStable)) return 2;
        // Return medium score if some species are stable
        else if (speciesStabilities.Any(species => species.isStable)) return 1;
        // Return worst score if no species are stable
        else return 0;
    }
    public static SpeciesStability[] ComputeSpeciesStability(PopulationManager populationManager, LevelObjectiveData objective)
    {
        // Project the minimums of the species out several days
        SpeciesCount[] speciesCounts = ProjectNextSpeciesCount(populationManager);
        // Create the array to return
        SpeciesStability[] speciesStabilities = new SpeciesStability[speciesCounts.Length];

        // Compute the stability of each species based on the projected minimum
        for (int i = 0; i < speciesCounts.Length; i++)
        {
            SpeciesType species = speciesCounts[i].species;

            // Add up the target population for every data that has the same species
            int targetAmount = objective.survivalObjectiveDatas
                .Where(data => data.targetSpecies.Species == species)
                .Sum(data => data.targetPopulationSize);

            // Species is stable if the projected minimum
            // is bigger than or equal to the target amount
            speciesStabilities[i] = new SpeciesStability()
            {
                species = species,
                isStable = speciesCounts[i].count >= targetAmount
            };
        }

        return speciesStabilities;
    }
    public static SpeciesCount[] ProjectNextSpeciesCount(PopulationManager populationManager)
    {
        // Use a dictionary to easily store and look up the counts
        Dictionary<SpeciesType, SpeciesCount> counts = new Dictionary<SpeciesType, SpeciesCount>();

        // Update the counts for each population in the population manager
        foreach (Population population in populationManager.Populations)
        {
            // Assign the species for convenience
            SpeciesType species = population.species.Species;

            // If the dictionary does not yet contain the key
            // then add the key to the dictionary 
            if (!counts.ContainsKey(species))
            {
                counts.Add(species, new SpeciesCount()
                {
                    species = population.species.Species,
                    count = 0
                });
            }

            // Calculate the population's next size 
            GrowthCalculator calculator = population.GrowthCalculator;
            counts[species].count += calculator.CalculateNextPopulationSize();
        }

        return counts.Values.ToArray();
    }
    #endregion
}
