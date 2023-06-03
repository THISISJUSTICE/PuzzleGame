using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSetting : MonoBehaviour
{
    public GameObject background;

    private void OnEnable()
    {
        background.SetActive(true);
    }

    private void OnDisable()
    {
        background.SetActive(false);
    }
}
