using System.Collections.Generic;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Class that keeps track of the state of the UI.
    /// </summary>
    public interface IUIStateTracker
    {
        /// <summary>
        /// Contains the names and descriptions of categories that an action belongs to.
        /// </summary>
        IReadOnlyList<string[]> ActionCategories { get; set; }

        /// <summary>
        /// The one-based index of the focus of the player in the action panel.
        /// </summary>
        int ActionFocusNumber { get; set; }

        /// <summary>
        /// The number of actions in the action panel.
        /// </summary>
        int ActionPanelItemCount { get; set; }

        /// <summary>
        /// The number of lines from the top of the action panel the player is currently displaced.
        /// </summary>
        int ActionPanelLineOffset { get; set; }

        /// <summary>
        /// A list of actions contained in the currently active category.
        /// </summary>
        IReadOnlyList<IDisplayAction> ActionPanelList { get; set; }

        /// <summary>
        /// Contains data about the currently selected action.
        /// </summary>
        UIStateTracker.ActionStore ActiveAction { get; set; }

        /// <summary>
        /// The name of the currently active category.
        /// </summary>
        string ActiveCategory { get; }

        /// <summary>
        /// The id of the currently active character.
        /// </summary>
        int ActiveCharacterId { get; set; }

        /// <summary>
        /// The one-based index of the focus of the player in the category panel.
        /// </summary>
        int CategoryFocusNumber { get; set; }

        /// <summary>
        /// The number of items in the category panel.
        /// </summary>
        int CategoryItemCount { get; }

        /// <summary>
        /// The number of lines from the top of the category panel the player is currently displaced.
        /// </summary>
        int CategoryLineOffset { get; set; }

        /// <summary>
        /// The one-based index of the focus of the player in the command panel.
        /// </summary>
        int CommandFocusNumber { get; set; }

        /// <summary>
        /// The current center target position of the player for the current action.
        /// </summary>
        int CurrentTargetPosition { get; set; }

        /// <summary>
        /// The multi-target positions of the player for the current action.
        /// </summary>
        IReadOnlyList<int> CurrentTargetPositions { get; set; }

        /// <summary>
        /// Gets whether or not it is the player's turn.
        /// </summary>
        bool IsPlayerTurn { get; }

        /// <summary>
        /// Whether or not the player is in the action panel.
        /// </summary>
        bool IsInActionPanel { get; set; }

        /// <summary>
        /// Whether or not the player is in the category panel.
        /// </summary>
        bool IsInCategoryPanel { get; set; }

        /// <summary>
        /// Whether or not the player is in the character panel.
        /// </summary>
        bool IsInCharacterPanel { get; set; }

        /// <summary>
        /// Whether or not the player is in the command panel.
        /// </summary>
        bool IsInCommandPanel { get; set; }

        /// <summary>
        /// Whether or not the player is in the formation panel.
        /// </summary>
        bool IsInFormationPanel { get; set; }

        /// <summary>
        /// Whether or not the player is currently using the status command.
        /// </summary>
        bool IsInStatusCommand { get; set; }
    }
}