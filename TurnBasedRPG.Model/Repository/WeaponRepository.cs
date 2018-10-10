using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class WeaponRepository : IRepository<Weapon>
    {
        private IRepository<Attack> _attackRepo;
        private List<Weapon> _weaponList;
        private List<Weapon> WeaponList
        {
            get
            {
                if (_weaponList == null)
                    GetWeapons();
                return _weaponList;
            }
            set { _weaponList = value; }
        }

        public WeaponRepository(IRepository<Attack> attackRepo)
        {
            _attackRepo = attackRepo;
        }

        public IReadOnlyList<Weapon> GetAll()
        {
            return WeaponList;
        }

        private void GetWeapons()
        {
            using (var reader = new StreamReader("Data/weapons.json"))
            {
                WeaponList = new List<Weapon>();
                JContainer weaponsAsList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var weaponObject in weaponsAsList)
                {
                    var weapon = weaponObject.ToObject<Weapon>();
                    // Gives each weapon a reference to their respective attack objects
                    var attackList = weaponObject["attackIds"].ToObject<List<int>>();
                    if (attackList != null)
                        weapon.AttackList = _attackRepo.GetAll().Where(attack => attackList.Contains(attack.Id)).ToList();
                    else
                        weapon.AttackList = new List<Attack>();
                }
            }
        }
    }
}
