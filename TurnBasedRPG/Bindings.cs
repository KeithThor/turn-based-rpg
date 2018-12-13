using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Controller;
using TurnBasedRPG.Controller.AI;
using TurnBasedRPG.Controller.AI.Interfaces;
using TurnBasedRPG.Controller.Combat;
using TurnBasedRPG.Controller.Combat.Interfaces;
using TurnBasedRPG.Controller.Interfaces;
using TurnBasedRPG.Model.Entities;
using TurnBasedRPG.Model.Repository;
using TurnBasedRPG.Model.Repository.Interfaces;
using TurnBasedRPG.UI;
using TurnBasedRPG.UI.Combat;
using TurnBasedRPG.UI.Combat.Interfaces;
using TurnBasedRPG.UI.Combat.Panels;

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

            // Repositories
            Bind<IRepository<Attack>>().To<AttackRepository>();
            Bind<IRepository<Category>>().To<CategoryRepository>();
            Bind<IRepository<CharacterBase>>().To<CharacterBaseRepository>();
            Bind<IRepository<Equipment>>().To<EquipmentRepository>();
            Bind<IRepository<Item>>().To<ItemRepository>();
            Bind<IRepository<Spell>>().To<SpellRepository>();
            Bind<IRepository<StatusEffect>>().To<StatusEffectRepository>();
            Bind<IRepository<Weapon>>().To<WeaponRepository>();

            // AI
            Bind<ICombatAI>().To<BasicCombatAI>();

            // Panels
            Bind<IActionDetailsPanel>().To<ActionDetailsPanel>();
            Bind<IActionPanel>().To<ActionPanel>();
            Bind<ICategoryDetailsPanel>().To<CategoryDetailsPanel>();
            Bind<ICategoryPanel>().To<CategoryPanel>();
            Bind<ICharacterPanel>().To<CharacterPanel>();
            Bind<ICombatLogPanel>().To<CombatLogPanel>();
            Bind<ICommandPanel>().To<CommandPanel>();
            Bind<IDamageTypesSubPanel>().To<DamageTypesSubPanel>();
            Bind<IFormationPanel>().To<FormationPanel>();
            Bind<IOffensiveSubPanel>().To<OffensiveSubPanel>();
            Bind<IStatsSubPanel>().To<StatsSubPanel>();
            Bind<IStatusEffectsPanel>().To<StatusEffectsPanel>();
            Bind<ITargetPanel>().To<TargetPanel>();
            Bind<ITurnOrderPanel>().To<TurnOrderPanel>();

            Bind<IViewModelController>().To<ViewModelController>();
            Bind<IActionController>().To<ActionController>();
            Bind<IStatusController>().To<StatusController>();

            // Scopings
            Bind<GameUIConstants>().ToSelf().InSingletonScope();
            Bind<IUIStateTracker>().To<UIStateTracker>().InScope(x => ProcessingScope.Current);
            Bind<IUICharacterManager>().To<UICharacterManager>().InScope(x => ProcessingScope.Current);
            Bind<ICombatController>().To<CombatController>().InScope(x => ProcessingScope.Current);
            Bind<DisplayManager, IDisplayManager>().To<DisplayManager>().InScope(x => ProcessingScope.Current);
            Bind<CombatStateHandler, IDisplayCombatState>().To<CombatStateHandler>().InScope(x => ProcessingScope.Current);
            Bind<IUIContainer>().To<UIContainer>().InScope(x => ProcessingScope.Current);
        }
    }
}
