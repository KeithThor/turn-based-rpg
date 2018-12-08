using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TurnBasedRPG.Core;
using TurnBasedRPG.Model.Repository;

namespace TurnBasedRPG
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            Game game = kernel.Get<Game>();

            game.Start();
        }
    }
}
