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
    
}