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
    public class SpellRepository : IRepository<Spell>
    {
        private IRepository<Category> _categoryRepo;
        private IRepository<StatusEffect> _statusEffectRepo;
        private List<Spell> _spellList;
        private List<Spell> SpellList
        {
            get
            {
                if (_spellList == null)
                    GetSpells();
                return _spellList;
            }
            set { _spellList = value; }
        }

        public SpellRepository(IRepository<Category> categoryRepo,
                                IRepository<StatusEffect> statusEffectRepo)
        {
            _categoryRepo = categoryRepo;
            _statusEffectRepo = statusEffectRepo;
        }

        public IReadOnlyList<Spell> GetAll()
        {
            return SpellList;
        }

        private void GetSpells()
        {
            using (var reader = new StreamReader("Data/spells.json"))
            {
                SpellList = new List<Spell>();
                JContainer spellsAsList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var spellObject in spellsAsList)
                {
                    var spell = spellObject.ToObject<Spell>();
                    // Gives each spell a reference to their respective category objects
                    if (spellObject["categoryId"] != null)
                        spell.SpellCategory = _categoryRepo.GetAll().First(category => category.Id == spellObject["categoryId"].ToObject<int?>());
                    if (spellObject["statusIds"] != null)
                        spell.BuffsToApply = _statusEffectRepo.GetAll()
                                                .Where(status => spellObject["statusIds"].ToObject<int[]>().Contains(status.Id))
                                                .ToList();
                    SpellList.Add(spell);
                }
            }
        }
    }
}
