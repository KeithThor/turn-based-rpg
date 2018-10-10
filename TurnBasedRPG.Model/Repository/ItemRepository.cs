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
    /// Responsible for retrieving all items from multiple files.
    /// </summary>
    public class ItemRepository : IRepository<Item>
    {
        private IRepository<Equipment> _equipmentRepo;
        private IRepository<Weapon> _weaponRepo;
        private IRepository<Spell> _spellRepo;
        private IRepository<Category> _categoryRepo;
        private List<Item> _itemList;
        private List<Item> ItemList
        {
            get
            {
                if (_itemList == null)
                    GetItems();
                return _itemList;
            }
            set { _itemList = value; }
        }

        public ItemRepository(IRepository<Equipment> equipmentRepo, 
                                IRepository<Weapon> weaponRepo,
                                IRepository<Spell> spellRepo,
                                IRepository<Category> categoryRepo)
        {
            _equipmentRepo = equipmentRepo;
            _weaponRepo = weaponRepo;
            _spellRepo = spellRepo;
            _categoryRepo = categoryRepo;
        }

        public IReadOnlyList<Item> GetAll()
        {
            return ItemList;
        }

        private void GetItems()
        {
            using (var reader = new StreamReader("Data/items.json"))
            {
                ItemList = new List<Item>();
                JContainer itemsAsList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var itemObject in itemsAsList)
                {
                    Item item;
                    // If the item is a consumable, create and fill in data for consumables
                    if (itemObject["isConsumable"] != null && itemObject["isConsumable"].ToObject<bool>())
                    {
                        item = itemObject.ToObject<Consumable>();
                        if (itemObject["spellId"] != null)
                            ((Consumable)item).ItemSpell = _spellRepo.GetAll()
                                                            .First(spell => spell.Id == itemObject["spellId"].ToObject<int>());
                        if (itemObject["categoryId"] != null)
                            ((Consumable)item).ConsumableCategory = _categoryRepo.GetAll()
                                                                    .First(category => category.Id == itemObject["categoryId"].ToObject<int>());
                    }
                    else
                        item = itemObject.ToObject<Item>();
                    ItemList.Add(item);
                }
            }
            ItemList.AddRange(_equipmentRepo.GetAll());
            ItemList.AddRange(_weaponRepo.GetAll());
        }
    }
}
