using UnityEngine;
using System.Collections.Generic;

namespace Vozon.Physics
{
    public class PhysicsSystem : MonoBehaviour
    {
        private static PhysicsSystem instance;
        public static PhysicsSystem Instance => instance;

        private List<IPhysicsObject> physicsObjects = new List<IPhysicsObject>();
        private PhysicsSettings settings;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePhysicsSystem();
        }

        private void InitializePhysicsSystem()
        {
            Debug.Log("Initializing VOZON Physics System...");
            settings = new PhysicsSettings();
            LoadDefaultSettings();
        }

        private void LoadDefaultSettings()
        {
            settings.gravity = new Vector3(0, -9.81f, 0);
            settings.timeStep = 0.02f;
            settings.iterations = 10;
        }

        public void AddPhysicsObject(IPhysicsObject obj)
        {
            if (!physicsObjects.Contains(obj))
            {
                physicsObjects.Add(obj);
            }
        }

        private void FixedUpdate()
        {
            UpdatePhysics();
        }

        private void UpdatePhysics()
        {
            foreach (var obj in physicsObjects)
            {
                ApplyGravity(obj);
                ResolveCollisions(obj);
            }
        }

        private void ApplyGravity(IPhysicsObject obj)
        {
            if (obj.UseGravity)
            {
                obj.AddForce(settings.gravity * obj.Mass);
            }
        }

        private void ResolveCollisions(IPhysicsObject obj)
        {
            // Implementierung der Kollisionserkennung und -auflösung
            Debug.Log($"Resolving collisions for object: {obj}");
        }
    }

    public interface IPhysicsObject
    {
        Vector3 Position { get; set; }
        Vector3 Velocity { get; set; }
        float Mass { get; }
        bool UseGravity { get; }
        void AddForce(Vector3 force);
    }

    public class PhysicsSettings
    {
        public Vector3 gravity;
        public float timeStep;
        public int iterations;
    }
} 