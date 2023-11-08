using System;
using System.Collections;
using Config;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class InputManager : MonoSingleton<InputManager>
    {
        public static event Action<Enums.PlayerAction> OnInputDown;
        public static event Action<Enums.PlayerAction> OnInputUp;
        public static event Action<Enums.PlayerAction> OnInput;

        [SerializeField] private UserInputConfig inputConfig;
        [SerializeField] private bool autoInit = true;
        
        public bool ShouldCheckInput { get; set; }

        public override void Init()
        {
            base.Init();
            ShouldCheckInput = autoInit;
        }

        private void Update()
        {
            if (ShouldCheckInput)
            {
                ReadInput();
            }
        }

        private void ReadInput()
        {
            foreach (var input in inputConfig.userInputs)
            {
                CheckInput(input);
            }
        }

        private static void CheckInput(UserInput input)
        {
            var actionKey = input.mainKey;
            var alternativeKey = input.alternativeKey;
        
            if (Input.GetKeyDown(actionKey) || Input.GetKeyDown(alternativeKey))
            {
                OnInputDown?.Invoke(input.playerAction);
            } else if (Input.GetKeyUp(actionKey) || Input.GetKeyUp(alternativeKey))
            {
                OnInputUp?.Invoke(input.playerAction);
            } else if (Input.GetKey(actionKey) || Input.GetKey(alternativeKey))
            {
                OnInput?.Invoke(input.playerAction);
            }
        }
    }
}
