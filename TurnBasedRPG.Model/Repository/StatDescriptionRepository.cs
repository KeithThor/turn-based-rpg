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
    public class StatDescriptionRepository : IRepository<StatDescription>
    {
        private List<StatDescription> _statList;
        private List<StatDescription> StatList
        {
            get
            {
                if (_statList == null)
                    GetStatDescriptions();
                return _statList;
            }
            set { _statList = value; }
        }

        public IReadOnlyList<StatDescription> GetAll()
        {
            return StatList;
        }

        private void GetStatDescriptions()
        {
            using (var reader = new StreamReader("Data/statdescriptions.json"))
            {
                StatList = JsonConvert.DeserializeObject<List<StatDescription>>(reader.ReadToEnd());
            }
        }
    }
}
