using System.Collections.Generic;

// Behavior that will be displayed when a need is not being met
public enum AnimalBehavior {ChangeMovementSpeed, ChangeMovementPattern, ChangeSpriteColor, AddDustParticles, PlaySoundWhenClicked, None}
// Core behaviors that most animals will display
public enum CoreBehavior {Eat, Drink, Mate, Sleep, MoveRandomly}

[System.Serializable]
public class Behavior
{
    public NeedType NeedType;
    public AnimalBehavior behavior; 
}
