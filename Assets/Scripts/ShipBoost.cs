using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class ShipBoost : NetworkBehaviour
{
    public bool isBoosting = false;
    public bool canBoost = false;
    [Header("Amounts")]

    //[SyncVar]
    public float boostAmount = 100;

    [SyncVar]
    public float syncBoostAmount;

    public int roundedBoostAmount = 100;
    public float boostDecrease;

    [Header("UI")]
    public TMP_Text amountText;
    public Slider amountBar;
    // Start is called before the first frame update
    void Start()
    {
        amountText = GameObject.Find("Boost Value Text").GetComponent<TMP_Text>();
        amountBar = GameObject.Find("Boost Slider").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!base.IsOwner)
        {
            return;
        }

        if (isBoosting)
        {
            boostAmount = Mathf.Clamp(boostAmount - (boostDecrease * Time.deltaTime), 0, 100);
        }

        if (roundedBoostAmount > 0)
        {
            canBoost = true;
        }
        else
        {
            canBoost = false;
        }

        roundedBoostAmount = (int)Mathf.Round(boostAmount);
        amountBar.value = boostAmount / 100;
        amountText.text = roundedBoostAmount.ToString();

        if (IsOwner)
        {
            syncBoostAmount = boostAmount;
        }
        else
        {
            boostAmount = syncBoostAmount;
        }
    }
    public void addBoost(float amount)
    {
        boostAmount = Mathf.Clamp(boostAmount + amount, 0, 100);
    }

    public void subtractBoost(float amount)
    {
        boostAmount = Mathf.Clamp(boostAmount - amount, 0, 100);
    }

    public void zeroBoost()
    {
        boostAmount = 0;
        
    }

}
