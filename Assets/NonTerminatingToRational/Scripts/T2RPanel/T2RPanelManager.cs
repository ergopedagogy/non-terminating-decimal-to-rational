using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace T2REngine
{
    public class T2RPanelManager : MonoBehaviour
    {
        public static T2RPanelManager INST;

        [SerializeField] private TextMeshProUGUI terminatingDecimalTxt;
        public string terminatingDecimal;


        //---------------------------

        private void Awake()
        {
            INST = this;
            Prepare();
        }

        private void Prepare()
        {
            Reset();
            FetchRandomTerminatingDecimal();
        }

        private void FetchRandomTerminatingDecimal()
        {
            while (!terminatingDecimal.Contains("."))
            {
                int sign = (Random.Range(0, 10) % 2 == 0) ? 1 : -1;

                int powerOf10 = Random.Range(1, 6);
                // int powerOf10 = Random.Range(6, 10);
                double denominator = (int)Mathf.Pow(10, powerOf10);
                Debug.Log($"denominator : {denominator}");

                //int powerOf2 = Random.Range(1, powerOf10);
                //int powerOf5 = Random.Range(1, powerOf10);
                int powerOf2 = Random.Range(1, 6);
                int powerOf5 = Random.Range(1, 6);
                double numerator = (int)Mathf.Pow(2, powerOf2) * (int)Mathf.Pow(5, powerOf5);
                Debug.Log($"numerator1 : {numerator}");

                int nofRandomNumbers = Random.Range(2, 5);
                while (nofRandomNumbers > 0)
                {
                    if (Random.Range(0, 10) % 3 == 0)
                    {
                        numerator *= 5;
                    }
                    else
                    {
                        numerator *= Random.Range(11, 50);
                    }
                    nofRandomNumbers--;
                }
                Debug.Log($"numerator2 : {numerator}");

                double terminatinDecimalDOUBLE = numerator / denominator * sign;
                terminatingDecimal = $"{terminatinDecimalDOUBLE}";
            }
            terminatingDecimalTxt.text = $"Convert {terminatingDecimal} to rational.";
            Debug.Log(terminatingDecimalTxt.text);
        }

        private void Start()
        {
            T2RWorkbook.INST.Initiate();
        }

        private void Reset()
        {
            terminatingDecimal = terminatingDecimalTxt.text = "";
        }
    }
}
