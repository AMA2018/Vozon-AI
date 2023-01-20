using UnityEngine;
using System.Collections.Generic;

namespace Vozon.Input
{
    public class InputSystem : MonoBehaviour
    {
        private static InputSystem instance;
        public static InputSystem Instance => instance;

        private Dictionary<string, InputAction> registeredActions;
        private Dictionary<string, InputBinding> customBindings;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeInputSystem();
        }

        private void InitializeInputSystem()
        {
            Debug.Log("Initializing VOZON Input System...");
            registeredActions = new Dictionary<string, InputAction>();
            customBindings = new Dictionary<string, InputBinding>();
            SetupDefaultBindings();
        }

        private void SetupDefaultBindings()
        {
            RegisterAction("move", new InputAction(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D));
            RegisterAction("jump", new InputAction(KeyCode.Space));
            RegisterAction("interact", new InputAction(KeyCode.E));
            RegisterAction("pause", new InputAction(KeyCode.Escape));
        }

        public void RegisterAction(string actionName, InputAction action)
        {
            if (!registeredActions.ContainsKey(actionName))
            {
                registeredActions.Add(actionName, action);
                Debug.Log($"Registered input action: {actionName}");
            }
        }

        public void Update()
        {
            foreach (var action in registeredActions.Values)
            {
                action.Update();
            }
        }

        public bool GetButton(string actionName)
        {
            return registeredActions.TryGetValue(actionName, out var action) && action.IsPressed;
        }

        public bool GetButtonDown(string actionName)
        {
            return registeredActions.TryGetValue(actionName, out var action) && action.WasPressedThisFrame;
        }

        public Vector2 GetMovementInput()
        {
            Vector2 movement = Vector2.zero;
            if (GetButton("move"))
            {
                movement.x = Input.GetAxis("Horizontal");
                movement.y = Input.GetAxis("Vertical");
            }
            return movement;
        }
    }

    public class InputAction
    {
        private KeyCode[] keyCodes;
        private bool wasPressed;
        private bool isPressed;

        public bool IsPressed => isPressed;
        public bool WasPressedThisFrame => isPressed && !wasPressed;

        public InputAction(params KeyCode[] keys)
        {
            keyCodes = keys;
        }

        public void Update()
        {
            wasPressed = isPressed;
            isPressed = false;

            foreach (var key in keyCodes)
            {
                if (Input.GetKey(key))
                {
                    isPressed = true;
                    break;
                }
            }
        }
    }

    public class InputBinding
    {
        public string ActionName { get; private set; }
        public KeyCode PrimaryKey { get; set; }
        public KeyCode SecondaryKey { get; set; }

        public InputBinding(string actionName, KeyCode primary, KeyCode secondary = KeyCode.None)
        {
            ActionName = actionName;
            PrimaryKey = primary;
            SecondaryKey = secondary;
        }
    }
} 