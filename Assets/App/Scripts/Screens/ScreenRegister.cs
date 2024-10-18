using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenRegister : MonoBehaviour
{

    [Header("Button")]
    [SerializeField] private Button registerButton;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private TMP_InputField inputBirth;
    [SerializeField] private TMP_InputField inputPhone;
    [SerializeField] private TMP_InputField inputContact;
    [SerializeField] private TMP_InputField inputEmergency;
    [SerializeField] private TMP_InputField inputAddress;
    [SerializeField] private TMP_InputField inputRegister;
    [SerializeField] private TMP_InputField inputAdelanto;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI feedbackText;

    private FirebaseImplementation db;
    void Start()
    {
        db = AppFlow.instance.databaseImplementation;
        CleanInputs();
    }

    void CleanInputs()
    {
        inputName.text = "";
        inputBirth.text = "";
        inputPhone.text = "";
        inputContact.text = "";
        inputEmergency.text = "";
        inputAddress.text = "";
        inputRegister.text = "";
        inputAdelanto.text = "";
    }

    public void RegisterUsers()
    {
        QueryConfig query = new QueryConfig
        {
            Collection = "caminantes"
        };

        string id = db.GenerateIdForNewEntry(query);

        Caminante caminante = new Caminante
        {
            name = inputName.text,
            date_of_birth = DateTime.Parse(inputBirth.text),
            phone_number = inputPhone.text,
            emergency_contact = inputEmergency.text,
            phone_contact = inputContact.text,
            address = inputAddress.text,
            is_register = CheckBool(inputRegister.text),
            id = id,
            adelanto = inputAdelanto.text,
            is_active = true
        };

        QueryConfig querySet = new QueryConfig
        {
            Collection = "caminantes",
            Document = id
        };

        db.SetDataQuery(caminante, querySet, result =>
        {
            if (result)
            {
                feedbackText.text = "Succesfully Added";
                CleanInputs();
            }
            else
            {
                feedbackText.text = "Error";
            }
        });
    }

    bool CheckBool(string text)
    {
        if (text == "Si") { return true; }
        else { return false; }
    }

    public void CheckInputs()
    {
        if (inputName.text != "" &&
            inputBirth.text != "" &&
            inputPhone.text != "" &&
            inputContact.text != "" &&
            inputEmergency.text != "" &&
            inputAddress.text != "" &&
            inputRegister.text != "" &&
            inputAdelanto.text != "") 
        {
            registerButton.interactable = true;
        }
        else
        {
            registerButton.interactable = false;
        }
    }


}
