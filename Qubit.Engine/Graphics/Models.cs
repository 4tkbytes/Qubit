using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qubit.Engine.Graphics
{
    public class Models
    {
        public class Model
        {
            public string Name { get; set; }
            public List<Entity> Entities { get; set; } = new();

            public Model(string name)
            {
                Name = name;
            }

            public void AddEntity(Entity entity)
            {
                Entities.Add(entity);
            }
        }

    }
}
