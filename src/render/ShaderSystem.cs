using System;
using System.Collections.Generic;

namespace VozonAI
{
    public enum ShaderQuality
    {
        Low,
        Medium,
        High,
        Ultra
    }

    public class ShaderConfig
    {
        public ShaderQuality Quality { get; set; } = ShaderQuality.Medium;
        public bool EnableShadows { get; set; } = true;
        public bool EnableReflections { get; set; } = true;
        public bool EnableRayTracing { get; set; } = false;
        public float RenderScale { get; set; } = 1.0f;
    }

    public class ShaderSystem
    {
        private static ShaderSystem _instance;
        public static ShaderSystem Instance => _instance ?? (_instance = new ShaderSystem());

        private ShaderConfig _config;
        private Dictionary<string, Shader> _shaders = new Dictionary<string, Shader>();
        private bool _isInitialized = false;

        private ShaderSystem()
        {
            _config = new ShaderConfig();
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            Console.WriteLine("Initializing Shader System...");
            
            // Register default shaders
            RegisterBuiltinShaders();
            
            _isInitialized = true;
            Console.WriteLine($"Shader System initialized with quality: {_config.Quality}");
        }

        public void Configure(ShaderConfig config)
        {
            _config = config;
            Console.WriteLine($"Shader System configured with quality: {config.Quality}");
            
            if (config.EnableRayTracing)
            {
                Console.WriteLine("Ray tracing enabled - this may impact performance");
            }
        }

        private void RegisterBuiltinShaders()
        {
            // Register standard shaders
            RegisterShader(new Shader("Standard", "assets/shaders/standard.shader"));
            RegisterShader(new Shader("PBR", "assets/shaders/pbr.shader"));
            RegisterShader(new Shader("Toon", "assets/shaders/toon.shader"));
            RegisterShader(new Shader("Water", "assets/shaders/water.shader"));
            RegisterShader(new Shader("Skybox", "assets/shaders/skybox.shader"));
            
            // Register ray-tracing shaders if enabled
            if (_config.EnableRayTracing)
            {
                RegisterShader(new Shader("RayTracedShadows", "assets/shaders/rt_shadows.shader"));
                RegisterShader(new Shader("RayTracedGI", "assets/shaders/rt_gi.shader"));
                RegisterShader(new Shader("RayTracedReflections", "assets/shaders/rt_reflections.shader"));
            }
        }

        public void RegisterShader(Shader shader)
        {
            if (!_shaders.ContainsKey(shader.Name))
            {
                _shaders.Add(shader.Name, shader);
                Console.WriteLine($"Registered shader: {shader.Name}");
            }
        }

        public Shader GetShader(string name)
        {
            if (_shaders.TryGetValue(name, out Shader shader))
            {
                return shader;
            }
            
            Console.WriteLine($"Shader {name} not found. Returning Standard shader.");
            return _shaders["Standard"];
        }

        public void ReloadShaders()
        {
            Console.WriteLine("Reloading all shaders...");
            foreach (var shader in _shaders.Values)
            {
                shader.Reload();
            }
            Console.WriteLine("All shaders reloaded successfully.");
        }
    }

    public class Shader
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public bool IsCompiled { get; private set; }

        public Shader(string name, string path)
        {
            Name = name;
            Path = path;
            IsCompiled = Compile();
        }

        private bool Compile()
        {
            // Simulate shader compilation
            Console.WriteLine($"Compiling shader: {Name} from {Path}");
            return true;
        }

        public void Reload()
        {
            Console.WriteLine($"Reloading shader: {Name}");
            IsCompiled = Compile();
        }
    }
} 