using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller
{
    /// <summary>
    /// Handles charges for in-game consumables.
    /// </summary>
    public class ConsumablesHandler
    {
        /// <summary>
        /// Removes 1 charge from a consumable if the consumable is perishable.
        /// </summary>
        /// <param name="consumable">The consumable being used.</param>
        /// <param name="consumer">The character using the consumable.</param>
        public void UseConsumable(Consumable consumable, Character consumer)
        {
            if (consumable.Charges == -1) return;
            if (!consumer.Inventory.Contains(consumable))
                throw new Exception($"Consuming character has no consumables of type {consumable.Name} in inventory.");

            consumable.Charges--;
            if (consumable.Charges == 0)
                consumer.Inventory.Remove(consumable);
        }
    }
}
