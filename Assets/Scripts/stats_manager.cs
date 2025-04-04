using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class stats_manager : singleton_persistent<stats_manager>
{
    [SerializeField] private observable_value_collection _obvc;

    private stats_manager(){}

    public void IncIntStat(string statName, int i){
        try{_obvc.InvokeInt(statName,i + _obvc.GetObservableInt(statName).Value);} 
        catch(Exception){_obvc.AddObservableInt(statName);_obvc.InvokeInt(statName,0);}
        
    }
    public void IncFloatStat(string statName, float f )
    {
        try{_obvc.InvokeFloat(statName,f + _obvc.GetObservableFloat(statName).Value);}
        catch(Exception){_obvc.AddObservableFloat(statName);_obvc.InvokeFloat(statName,1f);}
    }
    public void SetBoolStat(string statName, bool b)
    {
        try{_obvc.InvokeBool(statName,b);}
        catch(Exception){_obvc.AddObservableBool(statName);_obvc.InvokeBool(statName,b);}
    }

    void OnDisable()
    {
        GenerateLogFiles();
    }

    public void GenerateLogFiles()
    {
        if(_obvc!=null)
        {
            string dataPath = Application.dataPath;
            if(!Directory.EnumerateFiles(dataPath).Contains<string>("Stats"))
            {
                Directory.CreateDirectory(dataPath + "/" + "Stats");
            }
            string statsPath = dataPath + "/" + "Stats";
            string fileName = "playerStats.txt";
            int cnt = 0;
            while(Directory.GetFiles(statsPath).Contains<string>(statsPath + "\\" +fileName))
            {
                cnt += 1;
                fileName = "playerStats" + cnt.ToString() + ".txt";
            }
            using (StreamWriter sw = File.CreateText(statsPath + "/"+ fileName))
            {
                sw.WriteLine("Logged at : " + DateTime.Now.ToString("f"));
                foreach(observable_value<int> i in _obvc.GetObservableIntArray())
                {
                    string valueName = i.Name;
                    int value = i.Value;
                    sw.WriteLine(valueName + " : " + value);
                }
                foreach(observable_value<float> f in _obvc.GetObservableFloatArray())
                {
                    string valueName = f.Name;
                    float value = f.Value;
                    sw.WriteLine(valueName + " : " + value);
                }
                foreach(observable_value<bool> b in _obvc.GetObservableBoolArray())
                {
                    string valueName = b.Name;
                    bool value = b.Value;
                    sw.WriteLine(valueName + " : " + value);
                }
            } Debug.Log("Created gameplay log file at : " + statsPath + "/" + fileName);
        } else {Debug.Log("Warning: Observable Value Collection is null on stats manager. Stats were not generated.");}
    }
}
