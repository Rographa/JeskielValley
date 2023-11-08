using System;

public static class Enums
{
    [Serializable]
    public enum PlayerAction
    {
        None,
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        Sprint,
        Interact,
        Jump
    }

    [Serializable]
    public enum ItemType
    {
        None,
        Hair,
        Outfit,
        Hat
    }

    [Serializable]
    public enum AnimationStates
    {
        None,
        Idle,
        Walk,
        Run
    }
    
}