using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
    public GameObject UnitPrefab;
    public Text PriceText;

    public Barack Barack;

    private void Start() {
        PriceText.text = UnitPrefab.GetComponent<Unit>().Price.ToString();
    }

    public void TryBuy() {

        int price = UnitPrefab.GetComponent<Unit>().Price;

        if (FindObjectOfType<Resources>().Money >= price) {

            FindObjectOfType<Resources>().Money -= price;

            Barack.CreateUnit(UnitPrefab);
        } else {
            Debug.Log("���� �����");
        }

    }
}
