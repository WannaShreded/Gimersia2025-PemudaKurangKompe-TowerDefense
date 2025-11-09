using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI currencyUI;
    [SerializeField] Animator anim;

    private bool isOpenShop = true;

    public void ToggleMenu()
    {
        if (Time.timeScale == 0f) return;
        isOpenShop = !isOpenShop;
        anim.SetBool("OpenShop", isOpenShop);
    }

    private void OnGUI()
    {
        currencyUI.text = LevelManager.main.currency.ToString();
    }

    public void SetSelected()
    {
        
    }
}
