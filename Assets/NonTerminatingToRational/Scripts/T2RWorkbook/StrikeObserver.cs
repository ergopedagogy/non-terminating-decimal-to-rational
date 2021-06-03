using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace T2REngine
{
    public class StrikeObserver : MonoBehaviour, IPointerEnterHandler
    {

        private Selectable selectable;
        private FractionItem fractionItem;

        //---------------------------

        private void Awake()
        {
            Prepare();
        }

        private void Prepare()
        {
            Reset();

            selectable = GetComponent<Selectable>();
            fractionItem = GetComponentInParent<FractionItem>();
        }

        private void Reset()
        {
            selectable = null;
            fractionItem = null;
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            if (Input.GetKey(KeyCode.LeftControl) && !selectable.interactable)
            {
                fractionItem.SelectForStrike(gameObject);
            }
        }

        private void OnDestroy()
        {
            Reset();
        }
    }
}
