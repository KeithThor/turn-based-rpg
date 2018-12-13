using System;
using System.Collections.Generic;
using TurnBasedRPG.Controller.EventArgs;
using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Contains UI panel elements and responsible for printing the entire UI.
    /// </summary>
    public interface IUIContainer
    {
        bool IsUIUpdating { get; set; }
        bool IsUIUpdatingFinished { get; }
        bool SkipUIUpdating { get; set; }

        /// <summary>
        /// Event invoked whenever the player selects an action.
        /// </summary>
        event EventHandler<ActionSelectedEventArgs> ActionSelectedEvent;

        /// <summary>
        /// Event invoked whenever the player starts an action.
        /// </summary>
        event EventHandler<ActionStartedEventArgs> ActionStartedEvent;

        /// <summary>
        /// Event invoked whenever the player presses a key.
        /// </summary>
        event EventHandler<KeyPressedEventArgs> KeyPressed;

        /// <summary>
        /// Event invoked whenever the action list should be updated.
        /// </summary>
        event EventHandler<UpdateActionListEventArgs> UpdateActionListEvent;

        /// <summary>
        /// Event invoked whenever categories should be updated.
        /// </summary>
        event EventHandler<UpdateCategoriesEventArgs> UpdateCategories;

        /// <summary>
        /// Called whenever a combat loggable event is invoked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnCombatLoggableEvent(object sender, CombatLoggableEventArgs args);

        /// <summary>
        /// Called whenever the command focus is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnCommandFocusChanged(object sender, CommandFocusChangedEventArgs args);

        /// <summary>
        /// Called whenever a key is pressed by the player.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnKeyPressed(object sender, KeyPressedEventArgs args);

        /// <summary>
        /// Prints the UI onto the Console screen.
        /// </summary>
        void PrintUI();

        /// <summary>
        /// Prints the UI onto the Console screen a certain amount of times, waiting between printing each frame.
        /// </summary>
        /// <param name="times">The amount of times to print the UI onto the Console.</param>
        /// <param name="msWaitPerFrame">The amount of time in milliseconds to wait before printing each frame.</param>
        void PrintUI(int times, int msWaitPerFrame);

        /// <summary>
        /// Updates the UI multiple times, updating health values slightly until the amount of frameupdates is reached.
        /// </summary>
        /// <param name="preCharactersChanged">A dictionary containing the ids of each character and the amount of health
        /// before the change.</param>
        /// <param name="postCharactersChanged">A dictionary containing the ids of each character and the amount of health
        /// after the change.</param>
        /// <param name="gradualChanges">A dictionary containing the ids of each character and the amount of health to
        /// update per frame.</param>
        /// <param name="frameUpdates">How many times to update the UI.</param>
        void UpdateHealthGradually(IReadOnlyDictionary<int, int> preCharactersChanged,
                                   IReadOnlyDictionary<int, int> postCharactersChanged,
                                   IReadOnlyDictionary<int, float> gradualChanges,
                                   int frameUpdates);
    }
}