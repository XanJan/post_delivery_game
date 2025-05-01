using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "value_names", menuName = "Scriptable Objects/value_names")]
public class value_names : ScriptableObject
{
    [SerializeField] private List<string> _intNames;
    [SerializeField] private List<string> _floatNames;
    [SerializeField] private List<string> _boolNames;
    [SerializeField] private List<string> _vector2Names;
    [SerializeField] private List<string> _stringNames;

    public List<string> IntNames{get{return new List<string>(_intNames);}}
    public string[] IntNamesArray{get{return _intNames.ToArray();}}

    public List<string> FloatNames{get{return new List<string>(_floatNames);}}
    public string[] FloatNamesArray{get{return _floatNames.ToArray();}}

    public List<string> BoolNames{get{return new List<string>(_boolNames);}}
    public string[] BoolNamesArray{get{return _boolNames.ToArray();}}

    public List<string> Vector2Names{get{return new List<string>(_vector2Names);}}
    public string[] Vector2NamesArray{get{return _vector2Names.ToArray();}}

    public List<string> StringNames{get{return new List<string>(_stringNames);}}
    public string[] StringNamesArray{get{return _stringNames.ToArray();}}
}
