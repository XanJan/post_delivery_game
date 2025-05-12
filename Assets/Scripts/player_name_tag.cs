using TMPro;
using UnityEngine;

public class player_name_tag : MonoBehaviour
{
    [SerializeField] private observable_value_collection _obvc;
    [SerializeField] private TextMeshPro _tmp;

    void FixedUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }
    void OnEnable()
    {
        observable_value<string> obname= _obvc.GetObservableString("playerName");
        _tmp.text = obname.Value;
        obname.UpdateValue+=HandleNameUpdate;
    }
    void OnDisable()
    {
        _obvc.GetObservableString("playerName").UpdateValue-=HandleNameUpdate;
    }

    public void HandleNameUpdate(observable_value<string> context)
    {
        _tmp.text = context.Value;
    }
}
