using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class LevelManager : MonoBehaviour {

    public static LevelManager main;

    [SerializeField] public TMP_Text warningText;

    public Transform startPoint;
    public Transform[] path;

    public int currency;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Warning Text not assigned in LevelManager. Assign a TMP_Text in the inspector.");
        }
    }

    public void IncreaseCurrency(int amount)
    {
        currency += amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount <= currency)
        {
            currency -= amount;
            return true;
        }
        else
        {
            StartCoroutine(ShowWarning($"Not enough gold! Need {amount} gold."));
            return false;
        }
    }

    public IEnumerator ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            warningText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            warningText.gameObject.SetActive(false);
        }
    }

    // Helper method to show warning without directly dealing with coroutines
    public void ShowWarningMessage(string message)
    {
        StartCoroutine(ShowWarning(message));
    }
}
