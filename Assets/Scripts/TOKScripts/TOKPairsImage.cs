using UnityEngine;
using System;

namespace TwoOfAKindGame
{
    /// <summary>
    /// This script displays a list of text pairs, each pair contains a First Text and a Second Text
    /// </summary>
    public class TOKPairsImage : MonoBehaviour
    {
        public Sprite[] pairsImage;

        void Awake()
        {
            // If we have a game controller, assign the list of text pairs to it
            if (GetComponent<TOKGameController>()) GetComponent<TOKGameController>().pairsImage = pairsImage;
        }
	}
}