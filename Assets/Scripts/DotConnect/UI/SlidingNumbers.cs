using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BizzyBeeGames.DotConnect
{

    public class SlidingNumbers : SingletonComponent<SlidingNumbers>
    {
        public TextMeshProUGUI numberText;
        public float animationTime;
        public int desiredNumber;
        public int initialNumber;
        public int currentNumber;
        int Multiple = 100;

        public void SetNumber(int Value)
        {
            initialNumber = currentNumber;
            desiredNumber = Value;
        }

        public void AddToNumber(int Value,int max)
        {
            Debug.Log(" in "+this.name);
            if (Value.ToString().Length >= 4)
            {
                Multiple = 1;
            }
            else if (Value.ToString().Length == 3)
            {
                Multiple = 10;
            }
            else if (Value.ToString().Length == 2)
            {
                Multiple = 50;
            }
            else
            {
                Multiple = 100;
            }
            initialNumber = currentNumber;
            desiredNumber += Value;
            if (desiredNumber >= max)
            {
                desiredNumber = max;
            }
        }

        public void SubToNumber(int Value)
        {
            Debug.Log("Val length is " + Value.ToString().Length);
            if (Value.ToString().Length >= 4)
            {
                Multiple = 1;
            }
            else if (Value.ToString().Length == 3)
            {
                Multiple = 10;
            }
            else if (Value.ToString().Length == 2)
            {
                Multiple = 50;
            }
            else
            {
                Multiple = 100;
            }
            initialNumber = currentNumber;
            desiredNumber -= Value;
        }

        void Update()
        {
            if(currentNumber != desiredNumber)
            {
                if (initialNumber < desiredNumber)
                {
                    //currentNumber += (animationTime * Time.deltaTime) * (desiredNumber - initialNumber);
                    currentNumber += (int)((animationTime * Time.deltaTime) * (desiredNumber - initialNumber) * Multiple);
                    if (currentNumber >= desiredNumber)
                        currentNumber  = desiredNumber;
                }
                else
                {
                    currentNumber -= (int)((animationTime * Time.deltaTime) * (initialNumber - desiredNumber) * Multiple);
                    if(currentNumber <= desiredNumber)
                        currentNumber  = desiredNumber;
                }
                numberText.text = currentNumber.ToString();
            }
        }
    }
}