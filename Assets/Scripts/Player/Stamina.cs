using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina
{
    public float _stamina = 100.0f;
    public float _maxStamina = 100.0f;
    public float runCostPerSecond = 20f;

    public Stamina(float maxStamina)
    {
        maxStamina = _maxStamina;
        _stamina = _maxStamina;
    }

    public float GetStamina()
    {
        return _stamina;
    }
    public void SetStamina(float value)
    {
        _stamina = value;
    }

    public void StaminaDecrease(float cost)
    {
        if (_stamina > 0f)
        {
            _stamina -= cost * Time.deltaTime;
        }
    }

    public void StaminaIncrease(float value)
    {
        if (_stamina < _maxStamina)
        {
            _stamina += value * Time.deltaTime;
        }
    }

    public void StaminaIncrease()
    {
        if (_stamina < _maxStamina)
        {
            _stamina += 20 * Time.deltaTime;
        }
    }
}
