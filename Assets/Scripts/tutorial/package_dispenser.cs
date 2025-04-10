using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class package_dispenser : MonoBehaviour
{
    [SerializeField] private List<GameObject> _packages;
    [SerializeField] private Transform _dispensePoint;
    [SerializeField] private float _secondsBetweenDispenses;
    private float _secondsSinceLast;
    private bool _dispense;
    public package_dispenser()
    {
        _packages = new List<GameObject>();
    }

    void Awake()
    {
        BeginDispensingPackages();
    }

    void FixedUpdate()
    {
        if(_dispense)
        {
            if(_packages.Count > 0)
            {
                _secondsSinceLast += Time.deltaTime;
                if(_secondsSinceLast >= _secondsBetweenDispenses)
                {
                    Instantiate(_packages.Last(),_dispensePoint);
                    _packages.Remove(_packages.Last());
                    _secondsSinceLast = 0;
                }
                
            }
        }
    }

    public void BeginDispensingPackages()
    {
        _dispense = true;
    }
    public void StopDispensingPackages()
    {
        _dispense = false;
    }
}
