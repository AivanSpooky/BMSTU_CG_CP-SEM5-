using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class Scene
    {
        public List<Mesh> Meshes { get; private set; }

        public Scene()
        {
            Meshes = new List<Mesh>();
        }

        public void AddObject(Mesh mesh)
        {
            Meshes.Add(mesh);
        }

        public void RemoveObjectByName(string name)
        {
            Mesh meshToRemove = Meshes.Find(m => m.Name == name);
            if (meshToRemove != null)
            {
                Meshes.Remove(meshToRemove);
            }
        }
    }
}
