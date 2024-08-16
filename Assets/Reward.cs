using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
    public ShipController parentShip;

    public List<MonoBehaviour> behaviours = new List<MonoBehaviour>();

    public void SetPartActive(bool partActive) {
        foreach (MonoBehaviour behaviour in behaviours) {
            behaviour.enabled = partActive;
        }
    }

}
