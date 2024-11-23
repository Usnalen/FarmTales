using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent { }

// Currency change event: when we receive or spend currency
public class CurrencyChangeGameEvent : GameEvent
{
    public int amount;
    public CurrencyType currencyType;

    public CurrencyChangeGameEvent(int amount, CurrencyType currencyType)
    {
        this.amount = amount;
        this.currencyType = currencyType;
    }
}

// Currency change event: when there is not enough currency
public class NotEnoughCurrencyGameEvent : GameEvent
{
    public int amount;
    public CurrencyType currencyType;

    public NotEnoughCurrencyGameEvent(int amount, CurrencyType currencyType)
    {
        this.amount = amount;
        this.currencyType = currencyType;
    }
}

// Currency change event: when there is enough currency
public class EnoughCurrencyGameEvent : GameEvent
{

}