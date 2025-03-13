using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class GameManager: MonoBehaviour
{
    public static GameManager instance;
    public GameObject lifesUI;
    public CharacterController player;
    public Text coinText;

    public int coins;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void UpdateCoinCounter()
    {
        coins++;
        coinText.text = coins.ToString();
    }
}