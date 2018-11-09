using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Shared.Interfaces;

namespace TurnBasedRPG.Shared
{
    /// <summary>
    /// Represents the UI layer representation of a game character.
    /// </summary>
    public class DisplayCharacter : IDisplayCharacter
    {
        public int Id { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public string Name { get; set; }
        public char Symbol { get; set; }
        public int Position { get; set; }

        public int HealthChange { get; set; }

        public int GetCurrenthealth()
        {
            return CurrentHealth;
        }

        public int GetId()
        {
            return Id;
        }

        public int GetMaxHealth()
        {
            return MaxHealth;
        }

        public string GetName()
        {
            return Name;
        }

        public int GetPosition()
        {
            return Position;
        }

        public char GetSymbol()
        {
            return Symbol;
        }
    }
}
