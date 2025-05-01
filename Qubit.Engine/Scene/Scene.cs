
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qubit.Engine.Core;
using Qubit.Engine.Graphics;
using Silk.NET.Core.Native;

namespace Qubit.Engine.Scene
{
    public class Scene
    {
        private Dictionary<String, Mesh> meshList;
        private String sceneName;
        private bool isInitialized = false;
        private Camera camera = new();

        public Camera Camera => camera;

        public Dictionary<String, Mesh> MeshList
        {
            get { return meshList; }
            set { meshList = value; }
        }

        public Scene(string name)
        {
            sceneName = name;
            meshList = new();
        }

        public void AddMesh(String meshId, Mesh mesh)
        {
            meshList.Add(meshId, mesh);
        }

        public void Render(Render render)
        {
            render.Camera = camera;

            // Set up common rendering state  
            render.SetViewport(EngineWindow._window.FramebufferSize);
            render.SetRenderTargetView();
            render.SetDefaultRasterizerState();

            // Render each mesh  
            foreach (var mesh in meshList)
            {
                render.Assemble(mesh.Value, 3U * sizeof(float), 0U, D3DPrimitiveTopology.D3D11PrimitiveTopologyTrianglelist);
                render.BindShader();
                render.DrawQuad(mesh.Value.Indices.Length);
            }
        }
    }
}
