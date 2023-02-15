using UnityEngine;
using System.Collections.Generic;

namespace Vozon.Rendering
{
    public class RenderSystem : MonoBehaviour
    {
        private static RenderSystem instance;
        public static RenderSystem Instance => instance;

        private Dictionary<string, Material> materials;
        private Dictionary<string, ShaderEffect> shaderEffects;
        private RenderSettings settings;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeRenderSystem();
        }

        private void InitializeRenderSystem()
        {
            Debug.Log("Initializing VOZON Render System...");
            materials = new Dictionary<string, Material>();
            shaderEffects = new Dictionary<string, ShaderEffect>();
            settings = new RenderSettings();
            LoadDefaultShaders();
        }

        private void LoadDefaultShaders()
        {
            // Lade Standard-Shader
            AddShaderEffect("standard", new StandardShader());
            AddShaderEffect("pbr", new PBRShader());
            AddShaderEffect("toon", new ToonShader());
        }

        public void AddShaderEffect(string name, ShaderEffect effect)
        {
            if (!shaderEffects.ContainsKey(name))
            {
                shaderEffects.Add(name, effect);
                Debug.Log($"Added shader effect: {name}");
            }
        }

        public void ApplyPostProcessing(Camera camera)
        {
            foreach (var effect in settings.activePostProcessing)
            {
                effect.Apply(camera);
            }
        }

        public Material CreateMaterial(string shaderName)
        {
            if (shaderEffects.TryGetValue(shaderName, out ShaderEffect effect))
            {
                var material = effect.CreateMaterial();
                materials.Add(System.Guid.NewGuid().ToString(), material);
                return material;
            }
            return null;
        }
    }

    public abstract class ShaderEffect
    {
        public abstract Material CreateMaterial();
        public abstract void UpdateProperties();
    }

    public class StandardShader : ShaderEffect
    {
        public override Material CreateMaterial()
        {
            return new Material(Shader.Find("Standard"));
        }

        public override void UpdateProperties()
        {
            // Standard Shader-Eigenschaften aktualisieren
        }
    }

    public class PBRShader : ShaderEffect
    {
        public override Material CreateMaterial()
        {
            return new Material(Shader.Find("HDRP/Lit"));
        }

        public override void UpdateProperties()
        {
            // PBR Shader-Eigenschaften aktualisieren
        }
    }

    public class ToonShader : ShaderEffect
    {
        public override Material CreateMaterial()
        {
            return new Material(Shader.Find("Toon/Lit"));
        }

        public override void UpdateProperties()
        {
            // Toon Shader-Eigenschaften aktualisieren
        }
    }

    public class RenderSettings
    {
        public List<PostProcessingEffect> activePostProcessing = new List<PostProcessingEffect>();
    }

    public abstract class PostProcessingEffect
    {
        public abstract void Apply(Camera camera);
    }
} 