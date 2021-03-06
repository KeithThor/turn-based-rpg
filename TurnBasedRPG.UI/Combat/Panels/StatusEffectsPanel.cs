﻿using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedRPG.Controller.Interfaces;
using TurnBasedRPG.Shared;
using TurnBasedRPG.Shared.Enums;
using TurnBasedRPG.Shared.Viewmodel;
using TurnBasedRPG.UI.Combat.EventArgs;
using TurnBasedRPG.UI.Combat.Interfaces;

namespace TurnBasedRPG.UI.Combat.Panels
{
    /// <summary>
    /// Panel responsible for rendering details for a status effect
    /// </summary>
    public class StatusEffectsPanel : IStatusEffectsPanel
    {
        private readonly IViewModelController _viewModelController;
        private readonly IUIStateTracker _defaultsHandler;

        public int MaxHeight { get; set; }
        public int MaxWidth { get; set; }
        public bool IsActive { get; set; }
        public int FocusNumber { get; set; }
        private int _maxFocusNumber;

        public StatusEffectsPanel(IViewModelController viewModelController,
                                  IUIStateTracker defaultsHandler)
        {
            MaxWidth = 55;
            MaxHeight = 16;
            _viewModelController = viewModelController;
            _defaultsHandler = defaultsHandler;
            _maxFocusNumber = 1;
            FocusNumber = 1;
        }

        /// <summary>
        /// Constructs a rendering of a status effects panel using data provided by view model data.
        /// </summary>
        /// <param name="data">The status data to use to fill the status effects panel.</param>
        /// <returns>A list of string containing the rendering of the status effects panel.</returns>
        public IReadOnlyList<string> Render()
        {
            var action = _viewModelController.GetActionViewData((Commands)_defaultsHandler.CommandFocusNumber,
                                                                 _defaultsHandler.ActiveCategory,
                                                                 _defaultsHandler.ActionFocusNumber - 1);
            _maxFocusNumber = action.StatusEffects.Count();

            var data = _viewModelController.GetStatusViewData((Commands)_defaultsHandler.CommandFocusNumber,
                                                                  _defaultsHandler.ActiveCategory,
                                                                  _defaultsHandler.ActionFocusNumber - 1,
                                                                  FocusNumber - 1);

            var render = new List<string>();

            string navTriangle = "Tab ► ";
            int spaces = MaxWidth - data.Name.Length - navTriangle.Length - 3;

            render.Add("╔" + new string('═', MaxWidth - 2) + "╗");
            render.Add("║ " + data.Name + new string(' ', spaces) + navTriangle + "║");
            render.Add("║" + new string('─', MaxWidth - 2) + "║");

            int maxDataWidth = (MaxWidth - 7) / 2;

            render.AddRange(RenderDescription(data));

            var damageData = RenderDamageData(data, maxDataWidth);
            var bulkData = RenderBulkData(data, maxDataWidth);
            int maxCount = (damageData.Count() > bulkData.Count()) ? damageData.Count() : bulkData.Count();
            string blankLine = new string(' ', maxDataWidth);

            for (int i = 0; i < maxCount; i++)
            {
                if (bulkData.Count() - 1 < i)
                {
                    render.Add("║ " + damageData[i] + " │ " + blankLine + " ║");
                }
                else if (damageData.Count() - 1 < i)
                {
                    render.Add("║ " + blankLine + " │ " + bulkData[i] + " ║");
                }
                else
                {
                    render.Add("║ " + damageData[i] + " │ " + bulkData[i] + " ║");
                }
            }

            // Fill empty spaces
            int size = render.Count();
            int emptySpaces = (MaxWidth - 3) / 2;
            for (int i = 0; i < MaxHeight - size - 1; i++)
            {
                render.Add("║" + new string(' ', emptySpaces) + "│" + new string(' ', emptySpaces) + "║");
            }

            render.Add("╚" + new string('═', MaxWidth - 2) + "╝");

            return render;
        }

        /// <summary>
        /// Renders the damage and healing data for the status effect viewmodel.
        /// </summary>
        /// <param name="data">The viewmodel data to render.</param>
        /// <returns>A list of string containing the viewmodel data.</returns>
        private List<string> RenderDamageData(StatusData data, int maxWidth)
        {
            var render = new List<string>();

            foreach (var pair in data.DamageData)
            {
                if (pair.Key.Equals("Crit Chance") || pair.Key.Equals("Crit Damage"))
                {
                    string description = $"{pair.Value}% {pair.Key}";
                    int spaces = maxWidth - description.Length;
                    render.Add(description + new string(' ', spaces));
                }
                else
                {
                    int spaces = maxWidth - pair.Key.Length - pair.Value.ToString().Length - 1;
                    render.Add($"{pair.Value} {pair.Key}" + new string(' ', spaces));
                }
            }

            return render;
        }

        /// <summary>
        /// Renders the bulk of the status data and returns it as a list of string.
        /// </summary>
        /// <param name="data">The status data view model to use.</param>
        /// <returns>A list of string containing a partial panel filled with status data.</returns>
        private List<string> RenderBulkData(StatusData data, int maxWidth)
        {
            var render = new List<string>();

            foreach (var pair in data.StatusEffectsData)
            {
                if (pair.Key.Equals("Duration"))
                {
                    string description = $"Lasts {pair.Value} turns";
                    int spaces = maxWidth - description.Length;
                    render.Add(description + new string(' ', spaces));
                }
                else if (pair.Key.Equals("% Crit Chance") || pair.Key.Equals("% Crit Damage"))
                {
                    string description = $"+{pair.Value} {pair.Key}";
                    int spaces = maxWidth - description.Length;
                    render.Add(description + new string(' ', spaces));
                }
                else
                {
                    int spaces = maxWidth - pair.Key.Length - pair.Value.ToString().Length - 1;
                    render.Add($"{pair.Value} {pair.Key}" + new string(' ', spaces));
                }
            }

            return render;
        }

        /// <summary>
        /// Renders the description of the status effect and returns it as a list of string.
        /// </summary>
        /// <param name="data">The status effect data to render the description of.</param>
        /// <returns>A list of string containing the description of the status effect.</returns>
        private List<string> RenderDescription(StatusData data)
        {
            var render = new List<string>();

            var description = data.Description.GetStringAsList(MaxWidth - 4);
            for (int i = 0; i < description.Count(); i++)
            {
                int offset = MaxWidth - description[i].Length - 4;
                render.Add("║ " + description[i] + new string(' ', offset) + " ║");
            }
            render.Add("║" + new string(' ', MaxWidth - 2) + "║");

            return render;
        }

        /// <summary>
        /// Handles key press events if the panel is active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            if (IsActive && !args.Handled)
            {
                switch(args.PressedKey.Key)
                {
                    case ConsoleKey.Tab:
                        SwitchFocus();
                        args.Handled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Handles switching focus number.
        /// </summary>
        private void SwitchFocus()
        {
            FocusNumber++;
            if (FocusNumber > _maxFocusNumber)
            {
                FocusNumber = 1;
                IsActive = false;
            }
        }
    }
}
