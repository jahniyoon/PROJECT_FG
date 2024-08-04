using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class MainUIController : MonoBehaviour
    {
        [Header("Main UI")]
        [SerializeField] private SliderUI m_healthUI;
        [SerializeField] private SliderUI m_hungerUI;
        [SerializeField] private FoodPowerUI m_foodPowerUI;

        public SliderUI HealthUI => m_healthUI;
        public SliderUI HungerUI => m_hungerUI;
        public FoodPowerUI FoodPowerUI => m_foodPowerUI;

    }

}