using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedRPG.Model.Repository.Interfaces
{
    public interface IRepository<T>
    {
        IReadOnlyList<T> GetAll();
    }
}
