using System;
using System.Collections.Generic;

namespace VozonAI
{
    public enum PathfindingType
    {
        Basic,
        Advanced,
        NavMesh
    }

    public class AIConfig
    {
        public PathfindingType PathfindingAlgorithm { get; set; } = PathfindingType.Basic;
        public float DecisionUpdateRate { get; set; } = 0.5f;
        public bool UseThreading { get; set; } = true;
        public int MaxAgentsPerUpdate { get; set; } = 50;
    }

    public class AISystem
    {
        private static AISystem _instance;
        public static AISystem Instance => _instance ?? (_instance = new AISystem());

        private AIConfig _config;
        private Dictionary<int, AIAgent> _agents = new Dictionary<int, AIAgent>();
        private List<BehaviorTree> _behaviorTrees = new List<BehaviorTree>();
        private PathfindingManager _pathfindingManager;

        private AISystem()
        {
            _pathfindingManager = new PathfindingManager();
        }

        public static void Configure(AIConfig config)
        {
            Instance._config = config;
            Instance._pathfindingManager.Initialize(config.PathfindingAlgorithm);
            Console.WriteLine($"AI System configured with {config.PathfindingAlgorithm} pathfinding");
        }

        public void RegisterAgent(AIAgent agent)
        {
            if (!_agents.ContainsKey(agent.ID))
            {
                _agents.Add(agent.ID, agent);
                Console.WriteLine($"Registered AI agent {agent.ID}");
            }
        }

        public void RegisterBehaviorTree(BehaviorTree tree)
        {
            _behaviorTrees.Add(tree);
            Console.WriteLine($"Registered behavior tree {tree.Name}");
        }

        public void Update(float deltaTime)
        {
            // Update agents with decision making
            int agentsUpdated = 0;
            foreach (var agent in _agents.Values)
            {
                if (agentsUpdated >= _config.MaxAgentsPerUpdate)
                    break;

                agent.Update(deltaTime);
                agentsUpdated++;
            }

            // Update pathfinding requests
            _pathfindingManager.Update();
        }

        public List<Vector3> RequestPath(Vector3 start, Vector3 end)
        {
            return _pathfindingManager.FindPath(start, end);
        }
    }

    public class AIAgent
    {
        public int ID { get; private set; }
        public string Name { get; set; }
        public BehaviorTree BehaviorTree { get; set; }
        private float _updateTimer = 0;

        public AIAgent(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public void Update(float deltaTime)
        {
            _updateTimer += deltaTime;
            if (_updateTimer >= AISystem.Instance._config.DecisionUpdateRate)
            {
                _updateTimer = 0;
                if (BehaviorTree != null)
                {
                    BehaviorTree.Execute(this);
                }
            }
        }
    }

    public class BehaviorTree
    {
        public string Name { get; set; }
        private BehaviorNode _rootNode;

        public BehaviorTree(string name, BehaviorNode rootNode)
        {
            Name = name;
            _rootNode = rootNode;
        }

        public bool Execute(AIAgent agent)
        {
            return _rootNode.Execute(agent);
        }
    }

    public abstract class BehaviorNode
    {
        public abstract bool Execute(AIAgent agent);
    }

    public class PathfindingManager
    {
        private PathfindingType _algorithm;

        public void Initialize(PathfindingType algorithm)
        {
            _algorithm = algorithm;
        }

        public List<Vector3> FindPath(Vector3 start, Vector3 end)
        {
            // Simplified implementation
            List<Vector3> path = new List<Vector3>();
            path.Add(start);
            
            // Add waypoints based on algorithm complexity
            if (_algorithm == PathfindingType.Advanced || _algorithm == PathfindingType.NavMesh)
            {
                path.Add(new Vector3((start.X + end.X) * 0.33f, (start.Y + end.Y) * 0.33f, (start.Z + end.Z) * 0.33f));
                path.Add(new Vector3((start.X + end.X) * 0.66f, (start.Y + end.Y) * 0.66f, (start.Z + end.Z) * 0.66f));
            }
            
            path.Add(end);
            return path;
        }

        public void Update()
        {
            // Process pending pathfinding requests
        }
    }

    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
} 