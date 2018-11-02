using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;

namespace TurnBasedRPG.Controller.AI.Interfaces
{
    public struct AIDecision
    {

    }

    public interface ICombatAI
    {
        IReadOnlyList<Character> MyCharacters { get; set; }
        IReadOnlyList<Character> PlayerCharacters { get; set; }

        AIDecision GetAIDecision(Character character);
    }
}
