using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public EnumOrbType OrbType;

    void OnTriggerEnter(Collider other)
    {
        GameController.Instance.CollectOrb(OrbType);
        transform.parent = other.transform;
        transform.localPosition = Vector3.up * 1f;
    }
}
