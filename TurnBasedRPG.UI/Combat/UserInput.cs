using System;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat
{
    /// <summary>
    /// Class that handles user input for the user interface in combat.
    /// </summary>
    public class UserInput
    {
        private readonly IUIStateTracker _defaultsHandler;
        private readonly IUIContainer _uiContainer;
        private readonly IUICharacterManager _uiCharacterManager;
        private readonly GameUIConstants _gameUIConstants;

        public UserInput(IUIStateTracker defaultsHandler,
                         IUIContainer uiContainer,
                         IUICharacterManager uiCharacterManager,
                         GameUIConstants gameUIConstants)
        {
            _defaultsHandler = defaultsHandler;
            _uiContainer = uiContainer;
            _uiCharacterManager = uiCharacterManager;
            _gameUIConstants = gameUIConstants;

            BindEvents();
        }
        
        private void BindEvents()
        {
            KeyPressEvent += _uiContainer.OnKeyPressed;
        }

        /// <summary>
        /// Event called whenever the player presses any key.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressEvent;

        /// <summary>
        /// Starts an infinite loop to listen for key presses.
        /// </summary>
        public void ListenForKeyPress()
        {
            while (true)
            {
                if (Console.KeyAvailable && !_uiContainer.IsUIUpdating)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    KeyPressEvent?.Invoke(this, new KeyPressedEventArgs()
                    {
                        PressedKey = keyInfo
                    });
                    ClearInputBuffer();
                }
            }
        }

        /// <summary>
        /// Loops and listens for the player to skip UI updating. If the UI updating finishes
        /// without the user skipping, then exit the loop.
        /// </summary>
        internal void ListenForUISkip()
        {
            while (!_uiContainer.SkipUIUpdating && !_uiContainer.IsUIUpdatingFinished)
            {
                bool consumedInput = false;
                if (Console.KeyAvailable && !consumedInput)
                {
                    consumedInput = true;
                    ConsoleKeyInfo keyinfo = Console.ReadKey(false);
                    if (keyinfo.Key == ConsoleKey.Enter)
                    {
                        _uiContainer.SkipUIUpdating = true;
                    }
                }
            }
            ClearInputBuffer();
        }

        /// <summary>
        /// Clears the input buffer, preventing the Console from reading spammed keys.
        /// </summary>
        internal void ClearInputBuffer()
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
        }
    }
}
