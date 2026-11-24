using UnityEngine;
using TMPro;

public class StepCounter : MonoBehaviour
{
    [SerializeField] private int Steps = 10000;
    [SerializeField] private TMP_Text stepCount;

    void Start()
    {
        stepCount.text = "Steps: "+Steps;
    }
    public void SubtractSteps(int StepCost)
    {
        Steps-=StepCost;
        stepCount.text = "Steps: "+Steps;
    }
}
