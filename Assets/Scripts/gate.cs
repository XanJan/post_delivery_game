using System.Collections.Generic;
using UnityEngine;

public class gate : MonoBehaviour
{
    [SerializeField] List<button_interactable> _buttons;
    [SerializeField] Transform stick1;
    [SerializeField] Transform stick2;
    private Dictionary<button_interactable,bool> _d = new Dictionary<button_interactable, bool>();
    private float _timer = 0f;

    private bool _openGate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(button_interactable bi in _buttons)
        {
            _d.Add(bi,false);
            bi.GetOBVC().GetObservableBool("isActive").UpdateValue += (name,b) => SetAndCheck(bi,b);
        }
        
    }

    // Update is called once per frame
    void SetAndCheck(button_interactable bi, bool b)
    {
        Debug.Log("SETANDCHECK");
        _d[bi] = b;
        bool res = true;
        foreach(KeyValuePair<button_interactable,bool> kvp in _d)
        {
            if(kvp.Value == false){res = false;}
        }
        if(res)
        {
            OpenGate();
        }
    }

    private void OpenGate()
    {
        _openGate = true;
    }
    void FixedUpdate()
    {
        if(_openGate)
        {
            if(_timer <= 5)
            {
                Debug.Log("OPENING");

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


}
