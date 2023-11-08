using System;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "Config/User Input", fileName = "New User Input")]
    public class UserInputConfig : ScriptableObject
    {
        public List<UserInput> userInputs = new();
    }

    [Serializable]
    public class UserInput
    {
        public Enums.PlayerAction playerAction = Enums.PlayerAction.None;
        public KeyCode mainKey = KeyCode.None;
        public KeyCode alternativeKey = KeyCode.None;
    }
}