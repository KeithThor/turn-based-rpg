using System.Collections.Generic;
using System.Linq;
using TurnBasedRPG.Controller.Interfaces;
using TurnBasedRPG.Shared;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Panel responsible for displaying extra information to the player about what each stat does for a character.
    /// </summary>
    public class StatsDetailsPanel : IStatsDetailsPanel
    {
        private readonly IStatDescriptionController _statDescriptionController;

        public int MaxHeight { get; set; }
        public int MaxWidth { get; set; }

        private int characterPanelFocus;
        private int subPanelFocus;
        private bool isSubPanelActive;

        public StatsDetailsPanel(IStatDescriptionController statDescriptionController)
        {
            MaxWidth = 55;
            MaxHeight = 16;
            _statDescriptionController = statDescriptionController;
            characterPanelFocus = 1;
            subPanelFocus = 1;
            isSubPanelActive = false;
        }

        /// <summary>
        /// Renders out the stat details panel with different renders depending on whether and which subpanels are active in the
        /// CharacterPanel.
        /// </summary>
        /// <returns>A ReadOnlyList of string containing the stats details panel.</returns>
        public IReadOnlyList<string> Render()
        {
            var render = new List<string>();

            // Append CharacterPanel and SubPanel focuses if sub panel is active to get id of stat
            string spFocus = (isSubPanelActive) ? $"{subPanelFocus}" : "";
            bool success = int.TryParse($"{characterPanelFocus}{spFocus}", out int id);
            if (!success) throw new System.Exception("StatsDetailsPanel created an invalid id from focus.");

            var stat = _statDescriptionController.GetStatFromId(id);

            render.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            render.Add(RenderName(stat.Item1));
            render.Add("║" + new string('─', MaxWidth - 2) + "║");
            render.AddRange(RenderDescription(stat.Item2));

            // Render empty lines if description doesn't take up the max height
            int emptyLines = MaxHeight - render.Count() - 1;
            for (int i = 0; i < emptyLines; i++)
            {
                render.Add("║" + new string(' ', MaxWidth - 2) + "║");
            }

            render.Add("╚" + new string('═', MaxWidth - 2) + "╝");

            return render;
        }

        /// <summary>
        /// Given a stat name, renders out the stat name enclosed in a box border.
        /// </summary>
        /// <param name="name">The name of the stat to render.</param>
        /// <returns>A string containing the stat name enclosed in a box border.</returns>
        private string RenderName(string name)
        {
            int spaces = MaxWidth - name.Length - 4;
            return "║ " + name + new string(' ', spaces) + " ║";
        }

        /// <summary>
        /// Given a stat description, renders out the descriptions in multiple lines to fit the size of the panel.
        /// </summary>
        /// <param name="description">The description of the stat.</param>
        /// <returns>An Ienumerable of string containing the stat description.</returns>
        private IEnumerable<string> RenderDescription(string description)
        {
            var render = new List<string>();
            var descAsArray = description.GetStringAsList(MaxWidth - 4);

            foreach (var item in descAsArray)
            {
                int spaces = MaxWidth - item.Length - 4;
                render.Add("║ " + item + new string(' ', spaces) + " ║");
            }
            
            return render;
        }

        /// <summary>
        /// Called when the CharacterPanel has its focus changed to coordinate changes in this panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnCharacterPanelFocusChanged(object sender, FocusChangedEventArgs args)
        {
            characterPanelFocus = args.NewFocus;
        }

        /// <summary>
        /// Called whenever a subpanel of the CharacterPanel has its activeness changed to coordinate changes in this panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnSubPanelActivenessChanged(object sender, ActivenessChangedEventArgs args)
        {
            isSubPanelActive = args.IsActive;
        }

        /// <summary>
        /// Called whenever a subpanel of the CharacterPanel has its focus changed to coordinate changes in this panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnSubPanelFocusChanged(object sender, FocusChangedEventArgs args)
        {
            subPanelFocus = args.NewFocus;
        }
    }
}
