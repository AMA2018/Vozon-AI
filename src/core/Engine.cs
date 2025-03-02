using System;
using System.Collections.Generic;

namespace VozonAI
{
    public class VozonEngine
    {
        private static VozonEngine _instance;
        public static VozonEngine Instance => _instance ?? (_instance = new VozonEngine());

        private bool _isInitialized = false;
        private Dictionary<Type, object> _systems = new Dictionary<Type, object>();

        private VozonEngine() { }

        public static void Initialize()
        {
            if (Instance._isInitialized)
            {
                Console.WriteLine("Engine already initialized.");
                return;
            }

            Console.WriteLine("Initializing VOZON AI Engine...");
            
            // Initialize core systems
            Instance.RegisterSystem<AISystem>(AISystem.Instance);
            
            Instance._isInitialized = true;
            Console.WriteLine("VOZON AI Engine initialized successfully.");
        }

        public void RegisterSystem<T>(T system) where T : class
        {
            if (!_systems.ContainsKey(typeof(T)))
            {
                _systems.Add(typeof(T), system);
                Console.WriteLine($"Registered system: {typeof(T).Name}");
            }
        }

        public T GetSystem<T>() where T : class
        {
            if (_systems.TryGetValue(typeof(T), out object system))
            {
                return system as T;
            }
            
            Console.WriteLine($"System {typeof(T).Name} not found.");
            return null;
        }

        public void Update(float deltaTime)
        {
            if (!_isInitialized)
            {
                Console.WriteLine("Engine not initialized. Call Initialize() first.");
                return;
            }

            // Update all systems
            if (_systems.TryGetValue(typeof(AISystem), out object aiSystem))
            {
                (aiSystem as AISystem).Update(deltaTime);
            }
            
            // Update other systems as they get implemented
        }
    }
} 