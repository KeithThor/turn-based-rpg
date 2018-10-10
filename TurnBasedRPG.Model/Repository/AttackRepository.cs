using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Model.Repository.Interfaces;

namespace TurnBasedRPG.Model.Repository
{
    public class AttackRepository : IRepository<Attack>
    {
        private List<Attack> _attackList;
        private List<Attack> AttackList
        {
            get
            {
                if (_attackList == null)
                    GetAttacks();
                return _attackList;
            }
            set { _attackList = value; }
        }

        public AttackRepository() { }

        public IReadOnlyList<Attack> GetAll()
        {
            return AttackList;
        }

        private void GetAttacks()
        {
            using (var reader = new StreamReader("Data/attacks.json"))
            {
                AttackList = JsonConvert.DeserializeObject<List<Attack>>(reader.ReadToEnd());
            }
        }
    }
}
