using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ErgoEngine.Utils
{
    public static class MathHelper
    {
        public static bool AreCoPrime(int firstNumber, int secondNumber)
        {
            bool areNumbersCoPrime = false;
            int numberA = firstNumber, numberB = secondNumber;

            while (numberA != 0 && numberB != 0)
            {
                if (numberA > numberB)
                {
                    numberA %= numberB;
                }
                else
                {
                    numberB %= numberA;
                }
            }
            areNumbersCoPrime = (Mathf.Max(numberA, numberB) == 1);
            return areNumbersCoPrime;
        }
    }
}