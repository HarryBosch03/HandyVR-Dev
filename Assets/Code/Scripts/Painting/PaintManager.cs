using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Painting
{
    public static class PaintManager
    {
        private const int TextureSize = 512;
        
        private static readonly CommandBuffer Cmd = new();
        private static Material paintMaterial = new(Shader.Find("Hidden/Paint"));

        private static readonly Dictionary<Renderer, RenderTexture> TextureMap = new();
        
        private static readonly int BPos = Shader.PropertyToID("_BPos");
        private static readonly int BCol = Shader.PropertyToID("_BColor");
        private static readonly int BSize = Shader.PropertyToID("_BSize");
        private static readonly int BHardness = Shader.PropertyToID("_BHardness");
        private static readonly int PaintTex = Shader.PropertyToID("_PaintTex");

        private static readonly int Behind = Shader.PropertyToID("_Behind");

        public static RenderTexture GetTexture(Renderer renderer)
        {
            if (!TextureMap.ContainsKey(renderer))
            {
                var rt = new RenderTexture(TextureSize, TextureSize, GraphicsFormat.R8G8B8A8_SRGB, GraphicsFormat.None);
                TextureMap.Add(renderer, rt);
            }
            return TextureMap[renderer];
        }
        
        public static void Paint(Brush brush, Renderer renderer)
        {
            var texture = GetTexture(renderer);
            
            paintMaterial.SetVector(BPos, brush.position);
            paintMaterial.SetColor(BCol, brush.color);
            paintMaterial.SetFloat(BSize, brush.radius);
            paintMaterial.SetFloat(BHardness, brush.hardness);

            foreach (var material in renderer.sharedMaterials)
            {
                material.SetTexture(PaintTex, texture);
            }
            
            Cmd.GetTemporaryRT(Behind, TextureSize, TextureSize, 0);
            Cmd.SetRenderTarget(Behind);
            Cmd.Blit(texture, Behind);
            Cmd.SetRenderTarget(texture);
            Cmd.DrawRenderer(renderer, paintMaterial);
            
            Graphics.ExecuteCommandBuffer(Cmd);
            Cmd.Clear();
        }
        
        [System.Serializable]
        public class Brush
        {
            public Vector3 position;
            public Color color;
            public float radius;
            [Range(0.0f, 1.0f)]public float hardness;

            public Brush(Vector3 position, Color color, float radius, float hardness)
            {
                this.position = position;
                this.color = color;
                this.radius = radius;
                this.hardness = hardness;
            }
        }
    }
}