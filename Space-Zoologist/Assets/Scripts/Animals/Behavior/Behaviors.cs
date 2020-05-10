using System.Collections.Generic;

public enum AnimalBehavior {ChangeMovementSpeed, ChangeMovementPattern, ChangeSpriteColor, AddDustParticles, PlaySoundWhenClicked}
[System.Serializable]
public class Behaviors
{
    public NeedType BehaviorSet;
    public List<AnimalBehavior> behaviors; 
}
