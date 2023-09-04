using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wallet {

    public int money;

    public int GetMoney() {
        return money;
    }

    public void AddMoney(int addedAmt) {
        money += addedAmt;
    }

    public void RemoveMoney(int removedAmt) {
        money -= removedAmt;
        if (money < 0) {
            money = 0;
        }
    }

    public bool CanPay(int amt) {
        return money - amt >= 0;
    }

}
