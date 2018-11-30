using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller.AI;
using TurnBasedRPG.Controller.AI.Interfaces;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Model.Repository;
using TurnBasedRPG.Model.Repository.Interfaces;
using TurnBasedRPG.UI;

namespace TurnBasedRPG
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            // Interfaces
            Bind<IRepository<Attack>>().To<AttackRepository>();
            Bind<IRepository<Category>>().To<CategoryRepository>();
            Bind<IRepository<CharacterBase>>().To<CharacterBaseRepository>();
            Bind<IRepository<Equipment>>().To<EquipmentRepository>();
            Bind<IRepository<Item>>().To<ItemRepository>();
            Bind<IRepository<Spell>>().To<SpellRepository>();
            Bind<IRepository<StatusEffect>>().To<StatusEffectRepository>();
            Bind<IRepository<Weapon>>().To<WeaponRepository>();
            Bind<ICombatAI>().To<BasicCombatAI>();

            // Scopings
            Bind<GameUIConstants>().ToSelf().InSingletonScope();
        }
    }
}
