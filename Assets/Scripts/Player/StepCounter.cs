using UnityEngine;
using TMPro;

public class StepCounter : MonoBehaviour
{
    [SerializeField] private int Steps = 10000;
    [SerializeField] private TMP_Text stepCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stepCount.text = "Steps: "+Steps;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SubtractSteps(int StepCost)
    {
        Steps-=StepCost;
        stepCount.text = "Steps: "+Steps;
    }
}
