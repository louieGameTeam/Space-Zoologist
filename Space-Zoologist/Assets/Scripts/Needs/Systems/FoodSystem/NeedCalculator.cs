using System.Collections;
using System.Collections.Generic;

public interface NeedCalculator
{
    /// <summary>
    /// Marks the calculator dirty
    /// </summary>
    void MarkDirty();

    /// <summary>
    /// Add a new source to the list
    /// </summary>
    /// <param name="source"> The new source to be added to the calculator </param>
    void AddSource(Life source);

    /// <summary>
    /// Remove a source from the list and return the removal status
    /// </summary>
    /// <param name="source"> The Source to be removed </param>
    /// <returns> Status of the removal </returns>
    bool RemoveSource(Life source);

    /// <summary>
    /// Add a new consumer to the list
    /// </summary>
    /// <param name="consumer"> The new consumer to be add to the calculator </param>
    /// <remarks>
    /// Limiting the consumer to be only of the type `Populaion`,
    /// from simplicity. Hainv a `FoodSource` ot also be a consumer will add unnecessary
    /// complexity and it is not in the original design. It should be easy to revert this
    /// if needed in the future.
    /// </remarks>
    void AddConsumer(Population consumer);

    /// <summary>
    /// Remove a consumer from the list and return the removal status
    /// </summary>
    /// <param name="consumer"> The consumer to be removed </param>
    /// <returns> Status of the removal </returns>
    bool RemoveConsumer(Population consumer);

    /// <summary>
    /// Calculate and return the distribution results
    /// </summary>
    /// <returns> The distribution results </returns>
    Dictionary<Population, float> CalculateDistribution();
}
