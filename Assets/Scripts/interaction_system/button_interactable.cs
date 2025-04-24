using UnityEngine;

public class button_interactable : interactable_object
{
    [SerializeField] private GameObject _off;
    [SerializeField] private GameObject _on;
    [SerializeField] private observable_value_collection _obvc;
    [SerializeField] private float _activeDuration = 1f;
    private bool _count=false;
    private float _timer;
    public void Test(interactor c){Debug.Log("test success");OnInteractStart(c);}
    public void OnInteractTrigger(interactor context)
    {
        _obvc.InvokeBool("isActive",true);
        _off.SetActive(false);
        _on.SetActive(true);
        _count=true;
        _timer = 0;
    }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _off.SetActive(true);
        _on.SetActive(false);
        _obvc.AddObservableBool("isActive");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_count)
        {
            _timer+=Time.fixedDeltaTime;
            if(_timer >= _activeDuration)
            {
                _obvc.InvokeBool("isActive",false);
                _off.SetActive(true);
                _on.SetActive(false);
                _timer = 0;
                _count = false;
            }
        }
    }

    public observable_value_collection GetOBVC(){return _obvc;}
}
