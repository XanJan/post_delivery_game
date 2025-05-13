using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class tutorial_text : MonoBehaviour
{
    [SerializeField] private observable_value_collection _wagonObvc;
    [SerializeField] private observable_value_collection _buttonObvc;
    [SerializeField] private observable_value_collection _playerInstanceManagerObvc;
    [SerializeField] private GameObject _textBubble;
    [SerializeField] private GameObject _arrowWagon;
    [SerializeField] private GameObject _arrowButton;
    [SerializeField] private GameObject _arrowExit;
    [SerializeField] private float _defaultWriteSpeed;
    [SerializeField] TextMeshPro _tmp;
    
    [SerializeField] private string _preEnter;
    [SerializeField] private string _postEnter;

    [SerializeField] private string _missionDescription1;
    [SerializeField] private string _missionDescription2;
    [SerializeField] private string _nameChangeDirections;
    [SerializeField] private string _pickupDirections;
    [SerializeField] private string _dropOffDirections;
    [SerializeField] private string _wagonFull;
    [SerializeField] private string _steerWagonDirection1;
    [SerializeField] private string _steerWagonDirection2;
    [SerializeField] private string _buttonDirection;
    [SerializeField] private string _exitDirection;
    private List<string> _queue = new List<string>();
    private StringBuilder _buffer = new StringBuilder();
    private StringBuilder _displayString = new StringBuilder();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _writeSpeed = _defaultWriteSpeed;
        _wagonObvc.GetObservableBool("isFull").UpdateValue += HandleWagonFull;
        _playerInstanceManagerObvc.GetObservableInt("numberOfPlayers").UpdateValue += HandlePlayerJoin;
        _buttonObvc.GetObservableBool("isActive").UpdateValue += HandleButtonActive;
        _textBubble.SetActive(false);
        _arrowButton.SetActive(false);
        _arrowWagon.SetActive(false);
        _arrowExit.SetActive(false);
        QueueString(_preEnter);
        
    }

    // Update is called once per frame
    private float _bufferTimer;
    private float _queueTimer;
    private float _endTimer;
    private float _queueTimeout=3f;
    private float _writeSpeed = 1;
    private delegate void OnQueueEmptyDelegate();
    private event OnQueueEmptyDelegate OnQueueEmptyEvent = default;

    void FixedUpdate()
    {
        if(_buffer.Length!=0 )
        {
            if((_bufferTimer += Time.fixedDeltaTime) >= 1/_writeSpeed){
                if(_endTimer>0) _endTimer = 0;
                NextChar();
                _bufferTimer = 0;
            }
        } 
        else if(_queue.Count!=0)
        {
            if((_queueTimer+=Time.fixedDeltaTime) >= _queueTimeout)
            {
                _displayString.Clear();
                _buffer.Append(_queue[0]);
                _queue.RemoveAt(0);
                if(_queue.Count == 0) OnQueueEmptyEvent?.Invoke();
                _queueTimer = 0;
            }
        } 
        else if(_endTimer < 12f && (_endTimer+=Time.fixedDeltaTime) >= 12f)
        {
            _textBubble.SetActive(false);
            _tmp.text = "";
        }
    }

    private void NextChar()
    {
        _textBubble.SetActive(true);
        string first = _buffer.ToString(0,1);
        _buffer.Remove(0,1);
        _displayString.Append(first);
        _tmp.text = _displayString.ToString();
    }
    public void QueueString(string s)
    {
        _queue.Add(s);
    }

    public void HandleWagonFull(observable_value<bool> context)
    {
        if(context.Value) 
        {
            FastForward();
            if(_queue.Count!=0){OnQueueEmptyEvent+=ResetSpeed;OnQueueEmptyEvent+=DoWagonFullSequence;}
            else{DoWagonFullSequence();}
            
            _arrowWagon.SetActive(false);
        }
        
    }
    private void DoWagonFullSequence()
    {
        _queue.Add(_wagonFull);
        _queue.Add(_steerWagonDirection1);
        _queue.Add(_steerWagonDirection2);
        _queue.Add(_buttonDirection);
        _wagonObvc.GetObservableBool("isFull").UpdateValue -= HandleWagonFull;
        OnQueueEmptyEvent += DisplayArrowButton;
        OnQueueEmptyEvent-=DoWagonFullSequence;
    }
    public void HandleButtonActive(observable_value<bool> context)
    {
        if(context.Value) 
        {
            FastForward();
            if(_queue.Count>0){OnQueueEmptyEvent+=DoButtonActiveSequence;}
            else{DoButtonActiveSequence();}
        }
    }
    private void DoButtonActiveSequence()
    {
        _queue.Add(_exitDirection);
        _arrowButton.SetActive(false);
        _arrowExit.SetActive(true);
        _buttonObvc.GetObservableBool("isActive").UpdateValue -= HandleButtonActive;
        OnQueueEmptyEvent-=DoButtonActiveSequence;
    }

    public void HandlePlayerJoin(observable_value<int> context)
    {
        if(context.Value==1)
        {
            QueueString(_postEnter);
            QueueString(_missionDescription1);
            QueueString(_missionDescription2);
            QueueString(_pickupDirections);
            QueueString(_dropOffDirections);
            OnQueueEmptyEvent+=DisplayArrowWagon;
            OnQueueEmptyEvent+=DisableArrowWagonMaybe;
        }
    }
    private void DisableArrowWagonMaybe()
    {
        if(_wagonObvc.GetObservableBool("isFull").Value){_arrowWagon.SetActive(false);}
        OnQueueEmptyEvent-=DisableArrowWagonMaybe;
    }
    private void FastForward()
    {
        if(_queue.Count>0){_writeSpeed=70;_queueTimeout=0f;OnQueueEmptyEvent+=ResetSpeed;}
    }

    private void ResetSpeed()
    {
        _writeSpeed=_defaultWriteSpeed;
        _queueTimeout =3f;
        OnQueueEmptyEvent-=ResetSpeed;
    }
    public void DisplayArrowButton()
    {
        _arrowButton.SetActive(true);
        OnQueueEmptyEvent-=DisplayArrowButton;
    }
    public void DisplayArrowWagon()
    {
        _arrowWagon.SetActive(true);
        OnQueueEmptyEvent-=DisplayArrowWagon;
    }    
}
