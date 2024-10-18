using System;
using TMPro;
using UnityEngine;

public class RowCaminante : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI edadText;

    private Caminante infoCaminante;

    public void FillRowInfo(Caminante caminante)
    {
        infoCaminante = caminante;
        nameText.text = infoCaminante.name;
        edadText.text = CalculateAge(infoCaminante.date_of_birth).ToString();
    }

    public int CalculateAge(DateTime date)
    {
        DateTime todayDate = DateTime.Now;

        int years = todayDate.Year - date.Year;

        // Si no ha llegado el aniversario este año, resta un año
        if (todayDate.Month < date.Month || (todayDate.Month == date.Month && todayDate.Day < date.Day))
        {
            years--;
        }

        return years;
    }
}
