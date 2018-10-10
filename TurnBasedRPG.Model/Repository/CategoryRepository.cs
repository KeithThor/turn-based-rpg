using Newtonsoft.Json;
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
    public class CategoryRepository : IRepository<Category>
    {
        private List<Category> _categoryList;
        private List<Category> CategoryList
        {
            get
            {
                if (_categoryList == null)
                    GetList();
                return _categoryList;
            }
            set { _categoryList = value; }
        }

        public IReadOnlyList<Category> GetAll()
        {
            return CategoryList;
        }

        private void GetList()
        {
            using (var reader = new StreamReader("Data/category.json"))
            {
                CategoryList = JsonConvert.DeserializeObject<List<Category>>(reader.ReadToEnd());
            }
        }
    }
}
