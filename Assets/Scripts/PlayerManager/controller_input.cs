using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// A controller input is attached to a controller gameobject to separate the input
/// from the player gameobject. A controller controlls a player, rather than being
/// a player. This script simply forwards the controller inputs to any controllable 
/// gameobject via the observable_value_collection on that gameobject. 
/// </summary>
public class controller_input : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private string _interactButtonKeyboard = "E";
    [SerializeField] private string _interactButtonGamepad = "X";
    private observable_value_collection _targetObvc;
    private readonly string started = "Started";
    private readonly string performed = "Performed";
    private readonly string canceled = "Canceled";
    // Dictionary used to remove all active callbacks on disable and when switching scenes.
    private Dictionary<string,Action<InputAction.CallbackContext>> _activeCallbacks;
    private string _playerName;
    private void Awake()
    {
        _activeCallbacks = new Dictionary<string, Action<InputAction.CallbackContext>>();
        // Dontdestroyonload to keep the controller between scenes.
        DontDestroyOnLoad(this);   
    }
    void Start()
    {
        Setup();
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded+=OnSceneLoad;
        SceneManager.activeSceneChanged+=OnSceneUnLoad;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded-=OnSceneLoad;
        SceneManager.activeSceneChanged-=OnSceneUnLoad;
    }
    void OnDestroy()
    {
        UnsubscribePlayerInputCallbacks();
    }
    /// <summary>
    /// Instantiate player prefab, subscribe to events.
    /// </summary>
    private void Setup()
    {
        if(controller_instance_manager.Instance.TryGetControllerSpawnPos(this,out var spawnPos))
        {
            GameObject player = Instantiate (_playerPrefab ,  spawnPos ,  Quaternion.identity);
            if  (player.TryGetComponent<observable_value_collection>(out var playerObvc))
            {
                _targetObvc = playerObvc;
                // for each action, subscribe to equivalent event in obvc...
                foreach(InputAction action in _playerInput.actions)
                {
                    switch(action.type)
                    {
                        // Button actions are propigated as bools, though their values should not be used.
                        case InputActionType.Button:
                        AddButtonCallback(action);
                        break;
                        case InputActionType.Value:
                        switch(action.expectedControlType)
                        {
                            case "Vector2":
                            AddVector2Callback(action);
                            break;
                        }
                        break;
                        case InputActionType.PassThrough:
                        switch(action.expectedControlType)
                        {
                            case "Vector2":
                            AddVector2Callback(action);
                            break;
                        }
                        break;
                    }
                }
                // Determine interact button and initialize player with correct value.
                string s;
                switch(_playerInput.currentControlScheme.ToString())
                {
                    case "Gamepad":s = _interactButtonGamepad; // If gamepad
                    break;
                    default:s = _interactButtonKeyboard; // Default: set to keyboard value
                    break;
                }
                _targetObvc.AddObservableString("interactButton");
                _targetObvc.InvokeString("interactButton", s);
                _targetObvc.AddObservableString("playerName");
                _targetObvc.GetObservableString("playerName").UpdateValue+=OnPlayerNameUpdate;
                if(_playerName!=null)
                {
                    if(player.TryGetComponent<player_initializer>(out var initializer)){initializer.PlayerName = _playerName;}
                }
            } else{Debug.Log("Warning: Player observable_value_collection not present for player \""+_playerInput.gameObject.name+"\". Input may not be registered correctly." );}
        }        
    }
    /// <summary>
    /// Unsubscribe all Actions in activeCallbacks from PlayerInput action events.
    /// </summary>
    private void UnsubscribePlayerInputCallbacks()
    {
        
        foreach(KeyValuePair<string,Action<InputAction.CallbackContext>> kvp in _activeCallbacks)
        {
            if(kvp.Key.EndsWith(started)){_playerInput.actions[kvp.Key.Replace(started,"")].started -= kvp.Value;}
            else if(kvp.Key.EndsWith(performed)){_playerInput.actions[kvp.Key.Replace(performed,"")].performed -= kvp.Value;}
            else if(kvp.Key.EndsWith(canceled)){_playerInput.actions[kvp.Key.Replace(canceled,"")].canceled -= kvp.Value;}
            else{return;}
            
        }

         _activeCallbacks.Clear();
        
    }
    /// <summary>
    /// Event handler for scene load event.
    /// </summary>
    private void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        Setup(); // Setup again in new scene.
    }
    /// <summary>
    /// Event handler for scene unload event.
    /// </summary>
    private void OnSceneUnLoad(Scene s,Scene a)
    {
        UnsubscribePlayerInputCallbacks();// Remove old callbacks from last scene.
    }
    private void OnPlayerNameUpdate(observable_value<string> context)
    {
        _playerName = context.Value;
    }

    /// <summary>
    /// Initialize a callback that propagates a vector2 InputAction to three 
    /// observable Vector2s in the target observable value collection, representing
    /// the started, performed and canceled events.
    /// </summary>
    /// <param name="action">The InputAction to propagate.</param>
    private void AddVector2Callback(InputAction action)
    {
         // Add observable values for started,performed and canceled to obvc...
        _targetObvc.AddObservableVector2(action.name+started);
        _targetObvc.AddObservableVector2(action.name+performed);
        _targetObvc.AddObservableVector2(action.name+canceled);
        // Create event handlers.
        Action<InputAction.CallbackContext> onActionStarted = context => _targetObvc.InvokeVector2(action.name+started,context.ReadValue<Vector2>());
        Action<InputAction.CallbackContext> onActionPerformed = context => _targetObvc.InvokeVector2(action.name+performed,context.ReadValue<Vector2>());
        Action<InputAction.CallbackContext> onActionCanceled = context => _targetObvc.InvokeVector2(action.name+canceled,context.ReadValue<Vector2>());
        // Save event handlers to dictionary.
        _activeCallbacks.Add(action.name+started, onActionStarted);
        _activeCallbacks.Add(action.name+performed, onActionPerformed);
        _activeCallbacks.Add(action.name+canceled, onActionCanceled);
        // Subscribe to playerinput events with the event handlers.
        action.started += onActionStarted;
        action.performed += onActionPerformed;
        action.canceled += onActionCanceled;
    }
    /// <summary>
    /// Initialize a callback that propagates a button InputAction to three 
    /// observable bools in the target observable value collection, representing
    /// the started, performed and canceled events. The boolean value invoked is a 
    /// dummy value used for compatibility with the observable value collection and 
    /// will always be true.
    /// </summary>
    /// <param name="action">The InputAction to propagate.</param>
    private void AddButtonCallback(InputAction action)
    {
         // Add observable values for started,performed and canceled to obvc...
        _targetObvc.AddObservableBool(action.name+started);
        _targetObvc.AddObservableBool(action.name+performed);
        _targetObvc.AddObservableBool(action.name+canceled);
        // Create event handlers. Invokes dummy value true.
        Action<InputAction.CallbackContext> onActionStarted = context => _targetObvc.InvokeBool(action.name+started,true);
        Action<InputAction.CallbackContext> onActionPerformed = context => _targetObvc.InvokeBool(action.name+performed,true);
        Action<InputAction.CallbackContext> onActionCanceled = context => _targetObvc.InvokeBool(action.name+canceled,true);
        // Save event handlers to dictionary.
        _activeCallbacks.Add(action.name+started, onActionStarted);
        _activeCallbacks.Add(action.name+performed, onActionPerformed);
        _activeCallbacks.Add(action.name+canceled, onActionCanceled);
        // Subscribe to playerinput events with the event handlers.
        action.started += onActionStarted;
        action.performed += onActionPerformed;
        action.canceled += onActionCanceled;
    }
}