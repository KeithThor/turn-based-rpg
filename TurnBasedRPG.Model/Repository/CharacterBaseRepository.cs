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
    /// <summary>
    /// Repository responsible for loading and retrieving character information from a database.
    /// </summary>
    public class CharacterBaseRepository : IRepository<CharacterBase>
    {
        private IRepository<Spell> _spellRepository;
        private List<CharacterBase> _characterList;
        private List<CharacterBase> CharacterList
        {
            get
            {
                if (_characterList == null)
                    GetCharacters();
                return _characterList;
            }
            set { _characterList = value; }
        }

        public CharacterBaseRepository(IRepository<Spell> spellRepository)
        {
            _spellRepository = spellRepository;
        }

        public IReadOnlyList<CharacterBase> GetAll()
        {
            GetCharacters();
            return CharacterList;
        }

        private void GetCharacters()
        {
            using (var reader = new StreamReader("Data/characters.json"))
            {
                CharacterList = JsonConvert.DeserializeObject<List<CharacterBase>>(reader.ReadToEnd());
                //CharacterList = new List<CharacterBase>();
                //JContainer charAsList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                //foreach (var charObject in charAsList)
                //{
                //    Console.WriteLine(charObject.ToString());
                //    var character = charObject.ToObject<CharacterBase>();
                //    var spellsId = charObject["spellsId"].ToObject<List<int>>();
                //    Gives each character a reference to their respective spell objects
                //    if (spellsId != null)
                //        character.SpellIdList = spellsId;
                //    CharacterList.Add(character);
                //}
            }
        }
    }
}
