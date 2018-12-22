using System;
using System.Linq;
using TurnBasedRPG.Controller.Interfaces;
using TurnBasedRPG.Model.Repository;

namespace TurnBasedRPG.Controller
{
    /// <summary>
    /// Controller responsible for retrieving and returning stat descriptions to the UI.
    /// </summary>
    public class StatDescriptionController : IStatDescriptionController
    {
        private readonly StatDescriptionRepository _statDescriptionRepo;

        public StatDescriptionController(StatDescriptionRepository statDescriptionRepo)
        {
            _statDescriptionRepo = statDescriptionRepo;
        }

        /// <summary>
        /// Gets a stat name and stat description from a provided Id.
        /// <para>Returns an empty name and description if no stats are found.</para>
        /// </summary>
        /// <param name="id">The id of the stat to retrieve.</param>
        /// <returns>A tuple containing the stat name and the stat description.</returns>
        public Tuple<string, string> GetStatFromId(int id)
        {
            var statDescription = _statDescriptionRepo.GetAll().FirstOrDefault(item => item.Id == id);
            if (statDescription == null)
            {
                return new Tuple<string, string>("", "");
            }
            else
            {
                return new Tuple<string, string>(statDescription.Name, statDescription.Description);
            }
        }

        /// <summary>
        /// Gets a stat name and stat description from a provided stat name.
        /// <para>Returns an empty name and description if no stats are found.</para>
        /// </summary>
        /// <param name="name">The name of the stat to retrieve.</param>
        /// <returns>A tuple containing the stat name and the stat description.</returns>
        public Tuple<string, string> GetStatFromName(string name)
        {
            var statDescription = _statDescriptionRepo.GetAll().FirstOrDefault(item => item.Name.Equals(name));
            if (statDescription == null)
            {
                return new Tuple<string, string>("", "");
            }
            else
            {
                return new Tuple<string, string>(statDescription.Name, statDescription.Description);
            }
        }
    }
}
