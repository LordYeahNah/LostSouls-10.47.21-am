using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _Underline;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_Underline)
            _Underline.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(_Underline)
            _Underline.SetActive(false);
    }
}
