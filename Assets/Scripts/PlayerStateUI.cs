using EmergencyRoulette;
using TMPro;
using UnityEngine;

public class PlayerStateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI techText;
    [SerializeField] private TextMeshProUGUI dataText;

    public void RefreshUI()
    {
        var ps = GameManager.Instance.PlayerState;

        energyText.text = ps.Energy.ToString();
        foodText.text = ps.Food.ToString();
        techText.text = ps.Technology.ToString();
        dataText.text = ps.Data.ToString();
    }
}
