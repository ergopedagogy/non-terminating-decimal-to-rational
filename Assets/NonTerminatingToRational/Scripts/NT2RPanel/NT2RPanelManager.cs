using System;
using System.Collections;
using System.Collections.Generic;
using T2REngine;
using TMPro;
using UnityEngine;

public class NT2RPanelManager : MonoBehaviour
{
    public static NT2RPanelManager INST;

    [SerializeField] private TextMeshProUGUI nonTerminatingDecimalTxt, repeatingBarTxt, mn1Txt, m1Txt, mn2Txt, m2Txt;
    [SerializeField] private TMP_InputField pTif, qTif, sTif;
    public string nonTerminatingDecimal, numerator, denominator;


    //---------------------------

    private void Awake()
    {
        INST = this;
        Prepare();
    }

    private void Prepare()
    {
        Reset();
        FetchRandomNonTerminatingDecimal();
    }

    private void FetchRandomNonTerminatingDecimal()
    {
        string integer = $"{UnityEngine.Random.Range(0, 100)}";
        int m = UnityEngine.Random.RandomRange(2, 4);
        int n = UnityEngine.Random.RandomRange(2, 5);
        string nonRepeatingDecimal = "";
        string repeatingDecimal = "";
        int i = m;
        while (i > 0)
        {
            nonRepeatingDecimal = $"{nonRepeatingDecimal}{UnityEngine.Random.RandomRange(0, 10)}";
            i--;
        }

        i = n;
        while (i > 0)
        {
            repeatingDecimal = $"{repeatingDecimal}{UnityEngine.Random.RandomRange(0, 10)}";
            i--;
        }
        Debug.Log($"repeatingDecimal : {repeatingDecimal}");

        nonTerminatingDecimal = nonTerminatingDecimalTxt.text = $"{integer}.{nonRepeatingDecimal}{repeatingDecimal}";

        string bar = " ";
        i = integer.Length + m;
        while (i > 0)
        {
            bar = $"{bar} ";
            i--;
        }

        i = n;
        while (i > 0)
        {
            bar = $"{bar}_";
            i--;
        }

        repeatingBarTxt.text = bar;
        try
        {
            m1Txt.text = m2Txt.text = $"{m}";
            mn1Txt.text = mn2Txt.text = $"{m}+{n}";
            pTif.text = $"{integer}{nonRepeatingDecimal}{repeatingDecimal}.{repeatingDecimal}";
            qTif.text = $"{integer}{nonRepeatingDecimal}.{repeatingDecimal}";
            sTif.text = $"{(int)(decimal.Parse(pTif.text) - decimal.Parse(qTif.text))}";
        }
        catch (Exception e)
        {
            Debug.Log("Overflowed! re-evaluating...");
            FetchRandomNonTerminatingDecimal();
        }

        numerator = sTif.text;
        denominator = $"{Mathf.Pow(10, (m + n)) - Mathf.Pow(10, m)}";

        Debug.Log($"{numerator} : {denominator}");
    }

    private void Start()
    {
        T2RWorkbook.INST.Initiate();
    }

    private void Reset()
    {
        nonTerminatingDecimal = nonTerminatingDecimalTxt.text = repeatingBarTxt.text = numerator = denominator = "";
    }
}
