using System;
using System.Collections;
using System.Collections.Generic;
using ErgoEngine.Utils;
using sharatachary.ergo.ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace T2REngine
{
    public class FractionItem : MonoBehaviour
    {
        public static event Action<string, string> Event_OnSuccessfulDivision;
        public static FractionItem root;

        [SerializeField] private TMP_InputField numeratorTif, denominatorTif, subNumeratorTif, subDenominatorTif;
        [SerializeField] private Image numeratorBgImg, denominatorBgImg, subNumeratorBgImg, subDenominatorBgImg;
        [SerializeField] private GameObject numeratorStrikeGob, denominatorStrikeGob, subNumeratorArrowGob, subDenominatorArrowGob;
        [SerializeField] private Color defaultBgColor, correctBgColor, wrongBgColor, solvedColor, defaultSubBgColor;
        [SerializeField] private LineRenderer lineRenderer;

        public FractionItem previous, next;
        private bool isRoot, canDrawStrikeLine, canValidate;
        private GameObject tartgetToStrike;
        private StrikeObserver numeratorStrikeObserver, denominatorStrikeObserver;
        private Vector3 strikeStart, strikeEnd;
        private bool canStrike
        {
            get { return (numeratorStrikeObserver != null) || (denominatorStrikeObserver != null); }
        }
        int divisorNumerator, divisorDenominator;


        //---------------------------

        private void Awake()
        {
            numeratorTif.onValueChanged.AddListener(OnValueChangedForNumerator);
            denominatorTif.onValueChanged.AddListener(OnValueChangedForDenominator);
            Prepare();
        }

        private void Prepare()
        {
            Reset();
        }

        private void OnValidateSub()
        {
            bool isValid = false;
            isValid = !string.IsNullOrEmpty(subNumeratorTif.text.Trim()) && !string.IsNullOrEmpty(subDenominatorTif.text.Trim());
            isValid = isValid && int.TryParse(subNumeratorTif.text.Trim(), out divisorNumerator);
            isValid = isValid && int.TryParse(subDenominatorTif.text.Trim(), out divisorDenominator);

            if (isValid)
            {
                divisorNumerator = (int)(int.Parse(numeratorTif.text) / divisorNumerator);
                divisorDenominator = (int)(int.Parse(denominatorTif.text) / divisorDenominator);
                isValid = (divisorNumerator == divisorDenominator);
            }

            if (isValid)
            {
                Event_OnSuccessfulDivision?.Invoke(subNumeratorTif.text, subDenominatorTif.text);
                Destroy(this, 1f);
            }
            else
            {
                FlashWrongAnswer();
            }
        }

        private void FlashWrongAnswer()
        {
            StopCoroutine("FlashWrongAnswerCo");
            StartCoroutine("FlashWrongAnswerCo");
        }

        private IEnumerator FlashWrongAnswerCo()
        {
            subNumeratorBgImg.color = subDenominatorBgImg.color = wrongBgColor;
            float value = 0;
            float speed = Time.deltaTime * 10f;
            WaitForSeconds wait = new WaitForSeconds(speed);
            while (value < 0.99f)
            {
                value = Mathf.Lerp(value, 1, speed);
                subNumeratorBgImg.color = Color.Lerp(subNumeratorBgImg.color, defaultSubBgColor, speed);
                subDenominatorBgImg.color = subNumeratorBgImg.color;
                yield return wait;
            }

            yield return null;
        }

        private void OnStrikeNumerator()
        {
            Destroy(numeratorTif.gameObject.GetComponent<StrikeObserver>());
            numeratorStrikeGob.SetActive(true);
            subNumeratorTif.gameObject.SetActive(true);
            CheckIfValidateButtonCouldBeShown();
        }

        private void OnStrikeDenominator()
        {
            Destroy(denominatorTif.gameObject.GetComponent<StrikeObserver>());
            denominatorStrikeGob.SetActive(true);
            subDenominatorTif.gameObject.SetActive(true);
            CheckIfValidateButtonCouldBeShown();
        }

        private void CheckIfValidateButtonCouldBeShown()
        {
            canValidate = (numeratorStrikeGob.activeSelf && denominatorStrikeGob.activeSelf);
            UpdateUITabNavigator();
        }

        public void ShowArrows()
        {
            subNumeratorArrowGob.SetActive(true);
            subDenominatorArrowGob.SetActive(true);
            subNumeratorTif.interactable = false;
            subDenominatorTif.interactable = false;
        }

        private void OnValueChangedForNumerator(string numeratorValue)
        {
            /*int rhs;
            bool isValid = int.TryParse(numeratorValue, out rhs);
            if (isValid)
            {
                int lhs = int.Parse(T2RPanelManager.INST.terminatingDecimal.Replace(".", ""));
                isValid = (lhs == rhs);
            }*/

            bool isValid = (numeratorValue == NT2RPanelManager.INST.numerator);
            numeratorBgImg.color = isValid ? correctBgColor : wrongBgColor;
            EnableNumberStrikeIfValid();
        }

        private void OnValueChangedForDenominator(string denominatorValue)
        {
            /*int rhs;
            bool isValid = int.TryParse(denominatorValue, out rhs);
            if (isValid)
            {
                int tenthPower = T2RPanelManager.INST.terminatingDecimal.Split("."[0])[1].Length;
                isValid = Mathf.Pow(10, tenthPower).ToString() == denominatorTif.text;
            }*/

            bool isValid = (denominatorValue == NT2RPanelManager.INST.denominator);
            denominatorBgImg.color = isValid ? correctBgColor : wrongBgColor;
            EnableNumberStrikeIfValid();
        }

        private void EnableNumberStrikeIfValid()
        {
            bool isValid = (numeratorBgImg.color == correctBgColor && denominatorBgImg.color == correctBgColor);
            numeratorTif.interactable = denominatorTif.interactable = !isValid;
            if (isValid)
            {
                numeratorStrikeObserver = numeratorTif.gameObject.AddComponent<StrikeObserver>();
                denominatorStrikeObserver = denominatorTif.gameObject.AddComponent<StrikeObserver>();
                UpdateUITabNavigator();
            }
        }

        public void MakeItTheRoot()
        {
            root = this;
            isRoot = true;
        }

        public void AddTail(FractionItem tail)
        {
            next = tail;
            tail.previous = this;
        }

        public void SetDefaultValues(string numerator, string denominator)
        {
            numeratorTif.text = numerator;
            denominatorTif.text = denominator;
            numeratorBgImg.color = correctBgColor;
            denominatorBgImg.color = correctBgColor;

            int numeratorInt = Mathf.Abs(int.Parse(numerator));
            int denominatorInt = int.Parse(denominator);
            if (MathHelper.AreCoPrime(numeratorInt, denominatorInt))
            {
                ConvertedToRationalNumber();
            }
            else
            {
                EnableNumberStrikeIfValid();
            }
        }

        private void ConvertedToRationalNumber()
        {
            numeratorBgImg.color = denominatorBgImg.color = solvedColor;
            numeratorTif.interactable = denominatorTif.interactable = false;
        }

        public void SelectForStrike(GameObject targetToStrike)
        {
            if (this.tartgetToStrike == null)
            {
                this.tartgetToStrike = targetToStrike;
            }
        }

        private void Update()
        {
            if (canValidate)
            {
                ProcessEnterKeyUp(Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter));
            }
            else
            if (canStrike)
            {
                if (!Input.GetKey(KeyCode.LeftControl)) return;
                ProcessMouseDown(Input.GetMouseButtonDown(0));
                ProcessMouse(Input.GetMouseButton(0));
                ProcessMouseUp(Input.GetMouseButtonUp(0));
            }
        }

        private void ProcessEnterKeyUp(bool canProcess)
        {
            if (canProcess)
            {
                OnValidateSub();
            }
        }

        private void ProcessMouseDown(bool canProcess)
        {
            if (canProcess)
            {
                canDrawStrikeLine = true;
                strikeStart = Input.mousePosition;
                strikeStart.z -= 0.01f;
            }
        }

        private void ProcessMouse(bool canProcess)
        {
            if (canProcess && canDrawStrikeLine)
            {
                strikeEnd = Input.mousePosition;
                strikeEnd.z -= 0.01f;

                DrawStrikeLine();
            }
        }

        private void ProcessMouseUp(bool canProcess)
        {
            if (canProcess)
            {
                if (tartgetToStrike == numeratorTif.gameObject)
                {
                    OnStrikeNumerator();
                }
                else
                if (tartgetToStrike == denominatorTif.gameObject)
                {
                    OnStrikeDenominator();
                }
                tartgetToStrike = null;
                canDrawStrikeLine = false;
                lineRenderer.positionCount = 0;
            }
        }

        private void DrawStrikeLine()
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, strikeStart);
            lineRenderer.SetPosition(1, strikeEnd);
        }

        private void UpdateUITabNavigator()
        {
            List<GameObject> activeUIControls = new List<GameObject>();

            if (numeratorTif.interactable) { activeUIControls.Add(numeratorTif.gameObject); }
            if (denominatorTif.interactable) { activeUIControls.Add(denominatorTif.gameObject); }
            if (subNumeratorTif.gameObject.activeSelf && subNumeratorTif.interactable) { activeUIControls.Add(subNumeratorTif.gameObject); }
            if (subDenominatorTif.gameObject.activeSelf && subDenominatorTif.interactable) { activeUIControls.Add(subDenominatorTif.gameObject); }

            UITabNavigator.INST.UpdateUIControls(activeUIControls.ToArray());
        }

        private void Reset()
        {
            numeratorTif.text = denominatorTif.text = subNumeratorTif.text = subDenominatorTif.text = "";
            isRoot = false;
            numeratorBgImg.color = denominatorBgImg.color = defaultBgColor;
            numeratorStrikeGob.SetActive(false);
            denominatorStrikeGob.SetActive(false);
            subNumeratorArrowGob.SetActive(false);
            subDenominatorArrowGob.SetActive(false);
            subNumeratorTif.gameObject.SetActive(false);
            subDenominatorTif.gameObject.SetActive(false);
            tartgetToStrike = null;
            canDrawStrikeLine = false;
            lineRenderer.startWidth = lineRenderer.endWidth = 2f;
            canValidate = false;

            UpdateUITabNavigator();
        }
    }
}
