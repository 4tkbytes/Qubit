using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Maths;

namespace Qubit.Engine.Graphics
{
    public class Entity
    {
        public string Name { get; set; }
        public Mesh Mesh { get; set; }
        public Transform Transform { get; set; } = new();

        public Entity(string name, Mesh mesh)
        {
            Name = name;
            Mesh = mesh;
        }
    }

}
