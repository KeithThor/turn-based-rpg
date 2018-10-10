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
    public class StatusEffectRepository : IRepository<StatusEffect>
    {
        private List<StatusEffect> _statusList;
        private List<StatusEffect> StatusList
        {
            get
            {
                if (_statusList == null)
                    GetList();
                return _statusList;
            }
            set { _statusList = value; }
        }

        public IReadOnlyList<StatusEffect> GetAll()
        {
            return StatusList;
        }

        private void GetList()
        {
            using (var reader = new StreamReader("Data/statuseffects.json"))
            {
                StatusList = JsonConvert.DeserializeObject<List<StatusEffect>>(reader.ReadToEnd());
            }
        }
    }
}
