using System;


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
    Jump,
    Inventory,
    MainMenu
}

[Serializable]
public enum ItemType
{
    None,
    Hair,
    Outfit,
    Hat,
    Crop
}

[Serializable]
public enum AnimationStates
{
    None,
    Idle,
    Walk,
    Run
}

[Serializable]
public enum CropType
{
    None,
    Potato,
    Tomato
}
    
