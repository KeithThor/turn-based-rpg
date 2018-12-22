using TurnBasedRPG.UI.Combat.EventArgs;

namespace TurnBasedRPG.UI.Combat.Interfaces
{
    /// <summary>
    /// Responsible for rendering the details of stats onto the UI for the player.
    /// </summary>
    public interface IStatsDetailsPanel : IPanel
    {
        /// <summary>
        /// Called when the CharacterPanel has its focus changed to coordinate changes in this panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnCharacterPanelFocusChanged(object sender, FocusChangedEventArgs args);

        /// <summary>
        /// Called whenever a subpanel of the CharacterPanel has its activeness changed to coordinate changes in this panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnSubPanelActivenessChanged(object sender, ActivenessChangedEventArgs args);

        /// <summary>
        /// Called whenever a subpanel of the CharacterPanel has its focus changed to coordinate changes in this panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnSubPanelFocusChanged(object sender, FocusChangedEventArgs args);
    }
}