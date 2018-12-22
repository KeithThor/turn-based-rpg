using System;

namespace TurnBasedRPG.Controller.Interfaces
{
    public interface IStatDescriptionController
    {
        Tuple<string, string> GetStatFromId(int id);
        Tuple<string, string> GetStatFromName(string name);
    }
}