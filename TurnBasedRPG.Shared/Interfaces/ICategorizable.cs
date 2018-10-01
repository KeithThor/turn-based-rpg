using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Shared.Interfaces
{
    public interface ICategorizable
    {
        string Category { get; set; }
        string CategoryDescription { get; set; }
    }
}
