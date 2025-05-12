using System.Collections.Generic;
using UnityEngine;

public class gate : MonoBehaviour
{
    [SerializeField] List<button_interactable> _buttons;
    [SerializeField] Transform stick1;
    [SerializeField] Transform stick2;
    [SerializeField] private int neighborhoodId; // ID of the neighborhood this gate belongs to
    
    // Debug let gate be openable
    [SerializeField] private bool debugAlwaysAllowOpen = false;
    
    private Dictionary<button_interactable,bool> _d = new Dictionary<button_interactable, bool>();
    private float _timer = 0f;
    private bool _openGate;
    private neighborhood_manager _neighborhoodManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Try find the neighborhood manager
        _neighborhoodManager = GetComponentInParent<neighborhood_manager>();
        if (_neighborhoodManager == null)
        {
            
            neighborhood_manager[] managers = FindObjectsOfType<neighborhood_manager>();
            
            Debug.Log($"Gate {gameObject.name} is looking for neighborhood ID {neighborhoodId}");
            Debug.Log($"Found {managers.Length} neighborhood managers in the scene:");
            foreach (neighborhood_manager manager in managers)
            {
                Debug.Log($"  - Manager with ID {manager.GetNeighborhoodId()} in GameObject {manager.gameObject.name}");
                if (manager.GetNeighborhoodId() == neighborhoodId)
                {
                    _neighborhoodManager = manager;
                    Debug.Log($"    > This manager matches our gate's ID");
                }
            }
        }
        else
        {
            Debug.Log($"Gate {gameObject.name} found parent neighborhood manager with ID {_neighborhoodManager.GetNeighborhoodId()}");
        }
        
        // If we found a neighborhood manager, use its ID
        if (_neighborhoodManager != null)
        {
            // Only override if the gate has default ID (0)
            if (neighborhoodId == 0)
            {
                neighborhoodId = _neighborhoodManager.GetNeighborhoodId();
                Debug.Log($"Gate {gameObject.name} updated ID from 0 to {neighborhoodId} from parent manager");
            }
            else if (neighborhoodId != _neighborhoodManager.GetNeighborhoodId())
            {
                Debug.LogWarning($"Gate {gameObject.name} has ID {neighborhoodId} but parent manager has ID {_neighborhoodManager.GetNeighborhoodId()} - this might cause issues");
            }
        }
        
        if (neighborhood_delivery_tracker.Instance != null)
        {
            Debug.Log($"Associating gate {gameObject.name} with neighborhood {neighborhoodId}");
            Debug.Log($"Initial neighborhood {neighborhoodId} completion status: {neighborhood_delivery_tracker.Instance.IsNeighborhoodComplete(neighborhoodId)}");
        }
        
        // Set up button callbacks
        foreach(button_interactable bi in _buttons)
        {
            _d.Add(bi,false);
            bi.GetOBVC().GetObservableBool("isActive").UpdateValue += cxt => SetAndCheck(bi,cxt.Value);
        }
    }
    
    void OnEnable()
    {
        // Recheck tracker status whenever the gate becomes enabled
        if (neighborhood_delivery_tracker.Instance != null)
        {
            Debug.Log($"Gate {gameObject.name} (neighborhoodId: {neighborhoodId}) enabled, completion status: {neighborhood_delivery_tracker.Instance.IsNeighborhoodComplete(neighborhoodId)}");
        }
    }

    // Update is called once per frame
    void SetAndCheck(button_interactable bi, bool b)
    {
        _d[bi] = b;
        bool allButtonsPressed = true;
        foreach(KeyValuePair<button_interactable,bool> kvp in _d)
        {
            if(kvp.Value == false){
                allButtonsPressed = false;
                break;
            }
        }
        
        if(allButtonsPressed)
        {
            Debug.Log($"All buttons pressed on gate for neighborhood {neighborhoodId}");
            
            bool trackerSaysComplete = neighborhood_delivery_tracker.Instance.IsNeighborhoodComplete(neighborhoodId);
            Debug.Log($"Neighborhood {neighborhoodId} completion status: {trackerSaysComplete}");
            
            neighborhood_delivery_tracker.Instance.LogAllCompletionStatus();
            
            // Check if the gate's neighborhood is complete
            bool shouldOpen = trackerSaysComplete || debugAlwaysAllowOpen;
            
            if (shouldOpen)
            {
                OpenGate();
            }
            else
            {
                Debug.Log($"Gate buttons pressed but neighborhood {neighborhoodId} is not complete yet!");
            }
        }
    }

    private void OpenGate()
    {
        _openGate = true;
        Debug.Log($"Opening gate for neighborhood {neighborhoodId}");
    }
    
    void FixedUpdate()
    {
        if(_openGate)
        {
            if(_timer <= 5)
            {
                stick1.transform.eulerAngles = Vector3.Lerp(stick1.transform.eulerAngles, 75f * Vector3.forward, 1 * Time.deltaTime);
                stick2.transform.eulerAngles = Vector3.Lerp(stick2.transform.eulerAngles, Vector3.zero, 1 * Time.deltaTime);
                _timer += Time.fixedDeltaTime;
            } else
            {
                _openGate = false;
                _timer = 0;
            }
        }
    }

    public void SetNeighborhoodId(int id)
    {
        neighborhoodId = id;
    }
}
