using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace T2REngine
{
    public class T2RWorkbook : MonoBehaviour
    {
        public static T2RWorkbook INST;

        [SerializeField] private FractionItem fractionItemPrefab;
        [SerializeField] private Camera camera;
        [SerializeField] private Transform fractionsHolder;
        [SerializeField] private ScrollRect scrollRect;

        private List<FractionItem> allFractionItems;
        private FractionItem lastFractionItem;
        private Transform root;

        //---------------------------

        private void Awake()
        {
            INST = this;
            allFractionItems = new List<FractionItem>();
            Prepare();
        }

        private void Prepare()
        {
            Reset();
        }

        public void Initiate()
        {
            GenerateFractionItem();
        }

        private void GenerateFractionItem(string numerator = null, string denominator = null)
        {
            FractionItem newFractionItem = Instantiate(fractionItemPrefab, fractionsHolder);
            if (allFractionItems.Count < 1)
            {
                newFractionItem.MakeItTheRoot();
                root = newFractionItem.transform;
            }
            else
            {
                lastFractionItem.AddTail(newFractionItem);
                Vector3 rootPosition = root.position;
                rootPosition.x -= 275f;
                //root.position = rootPosition;
                //newFractionItem.transform.SetParent(lastFractionItem.transform);
                lastFractionItem.ShowArrows();
            }
            allFractionItems.Add(newFractionItem);
            newFractionItem.name = $"FI{allFractionItems.Count - 1}";
            if (!string.IsNullOrEmpty(numerator) && !string.IsNullOrEmpty(denominator))
            {
                newFractionItem.SetDefaultValues(numerator, denominator);
            }
            lastFractionItem = newFractionItem;
        }

        private void Update()
        {
            scrollRect.enabled = !Input.GetKey(KeyCode.LeftControl);
        }

        private void Reset()
        {
            foreach (FractionItem fractionItem in allFractionItems)
            {
                Destroy(fractionItem.gameObject);
            }
            allFractionItems.Clear();
            lastFractionItem = null;
        }

        private void OnEnable()
        {
            FractionItem.Event_OnSuccessfulDivision += Event_OnSuccessfulDivision_Handler;
        }

        private void OnDisable()
        {
            FractionItem.Event_OnSuccessfulDivision -= Event_OnSuccessfulDivision_Handler;
        }

        private void Event_OnSuccessfulDivision_Handler(string numerator, string denominator)
        {
            GenerateFractionItem(numerator, denominator);
        }

        public Vector3 GetWorldPosition(Vector3 position)
        {
            return camera.ScreenToWorldPoint(position);
        }
    }
}
