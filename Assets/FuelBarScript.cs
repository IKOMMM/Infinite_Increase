using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FuelBarScript : MonoBehaviour
{
    [SerializeField] PlayerHandler playerHandler;

    private Image gasBar;
    public float currentFuel;
    private float maxFuel;

    // Start is called before the first frame update
    void Start()
    {
        gasBar = GetComponent<Image>();
        playerHandler = FindObjectOfType<PlayerHandler>();
        maxFuel = playerHandler.maxfuelAmount;
    }

    // Update is called once per frame
    void Update()
    {
        currentFuel = playerHandler.fuelAmount;
        gasBar.fillAmount = currentFuel / maxFuel;
    }
}
