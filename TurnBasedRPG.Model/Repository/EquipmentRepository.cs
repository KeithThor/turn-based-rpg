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
    public class EquipmentRepository : IRepository<Equipment>
    {
        private List<Equipment> _equipmentList;
        private List<Equipment> EquipmentList
        {
            get
            {
                if (_equipmentList == null)
                    GetEquipment();
                return _equipmentList;
            }
            set { _equipmentList = value; }
        }

        public EquipmentRepository() { }

        public IReadOnlyList<Equipment> GetAll()
        {
            return EquipmentList;
        }

        private void GetEquipment()
        {
            using (var reader = new StreamReader("Data/equipment.json"))
            {
                EquipmentList = JsonConvert.DeserializeObject<List<Equipment>>(reader.ReadToEnd());
            }
        }
    }
}
