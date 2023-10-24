using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DialogController : MonoBehaviour
{
    [SerializeField] private TMP_Text _DialogTextElement;
    [SerializeField] private List<DialogMessage> _Dialog = new List<DialogMessage>();
    [SerializeField] private GameObject _InteractionContainer;
    [SerializeField] private GameObject _MessageContainer;

    public bool IsDialogActive = false;
    
    private int _CurrentlyAt = 0;

    public void NextAction()
    {
        if (IsDialogActive)
        {
            _Dialog[_CurrentlyAt].CompleteMessage();
        }
    }
    

    private void Start()
    {
        if(_Dialog.Count > 0)
            SetText(_Dialog[0].Message);
    }

    public void MoveToNextMessage()
    {
        _CurrentlyAt += 1;
        if (_CurrentlyAt < _Dialog.Count)
        {
           SetText(_Dialog[_CurrentlyAt].Message);
        }
    }

    public void FinishInteraction()
    {
        _CurrentlyAt = 0;
        SetText(_Dialog[0].Message);
        if(_InteractionContainer)
            _InteractionContainer.SetActive(true);

        if (_MessageContainer)
            _MessageContainer.SetActive(true);
        
        IsDialogActive = false;
    }

    private void SetText(string message)
    {
        if (_DialogTextElement)
            _DialogTextElement.text = message;
    }
}

[System.Serializable]
public class DialogMessage
{
    [SerializeField] private string _Message;
    public string Message => _Message;
    [SerializeField] private UnityEvent _DialogCompleteEvent;

    public void CompleteMessage()
    {
        _DialogCompleteEvent?.Invoke();
    }
}
