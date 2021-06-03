using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace sharatachary.ergo.ui
{
    public class UITabNavigator : MonoBehaviour
    {
        public static UITabNavigator INST;

        [SerializeField] private List<GameObject> uiControls;
        [SerializeField] private bool shouldActivateFirstControl;
        private EventSystem eventSystem;
        private int selectedIndex;

        //---------------------------


        private void Awake()
        {
            INST = this;
            Prepare();
        }

        private void Prepare()
        {
            eventSystem = EventSystem.current;

            Reset();
        }

        private void Update()
        {
            if (uiControls.Count < 1)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (eventSystem.currentSelectedGameObject != null)
                {
                    SetSelectionIndex(uiControls.IndexOf(eventSystem.currentSelectedGameObject.gameObject));
                }
                else
                {
                    SetSelectionIndex(-1);
                }
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyUp(KeyCode.Tab))
                {
                    SetSelectionIndex(selectedIndex - 1);
                }
                return;
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                SetSelectionIndex(selectedIndex + 1);
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
                {
                    Button button = uiControls[selectedIndex].GetComponent<Button>();
                    if (button != null)
                    {
                        button.OnPointerClick(new PointerEventData(eventSystem));
                    }
                    else
                    {
                        SetSelectionIndex(selectedIndex + 1);
                    }
                }
            }
        }

        private void SelectControl()
        {
            // if (selectedIndex < 0)
            // {
            //     //eventSystem.SetSelectedGameObject(null);
            //     selectedIndex = uiControls.Count - 1;
            // }
            // else
            // {
            eventSystem.SetSelectedGameObject(uiControls[selectedIndex], new BaseEventData(eventSystem));
            // }
        }

        private void SetSelectionIndex(int newSelectionIndex)
        {
            selectedIndex = newSelectionIndex >= uiControls.Count ? 0 : (newSelectionIndex < 0 ? uiControls.Count - 1 : newSelectionIndex);
            SelectControl();
        }

        private void Reset()
        {
            if (shouldActivateFirstControl)
            {
                if (uiControls.Count > 0)
                {
                    SetSelectionIndex(0);
                }
            }
        }

        public void UpdateUIControls(GameObject[] uiControlList)
        {
            uiControls.Clear();
            uiControls.AddRange(uiControlList);
            Prepare();
        }
    }
}
