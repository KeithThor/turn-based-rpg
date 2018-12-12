using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller.AI;
using TurnBasedRPG.Controller.AI.Interfaces;
using TurnBasedRPG.Controller.Combat;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Model.Repository;
using TurnBasedRPG.Model.Repository.Interfaces;
using TurnBasedRPG.UI;
using TurnBasedRPG.UI.Combat;

namespace TurnBasedRPG
{
    public class Bindings : NinjectModule
    {
        public class ScopeObject { }

        public static class ProcessingScope
        {
            public static ScopeObject Current { get; set; }
        }

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
            Bind<DefaultsHandler>().ToSelf().InScope(x => ProcessingScope.Current);
            Bind<UICharacterManager>().ToSelf().InScope(x => ProcessingScope.Current);
            Bind<CombatController>().ToSelf().InScope(x => ProcessingScope.Current);
            Bind<DisplayManager>().ToSelf().InScope(x => ProcessingScope.Current);
            Bind<CombatStateHandler>().ToSelf().InScope(x => ProcessingScope.Current);
            Bind<UIContainer>().ToSelf().InScope(x => ProcessingScope.Current);
        }
    }
}
