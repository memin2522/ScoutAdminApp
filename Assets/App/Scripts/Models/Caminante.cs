using Firebase.Firestore;
using System;


[FirestoreData]
public class Caminante
{
    [FirestoreProperty]
    public string id { get; set; } 
    
    [FirestoreProperty]
    public string address { get; set; } = "";
    
    [FirestoreProperty]
    public string adelanto { get; set; } = "";
    
    [FirestoreProperty]
    public DateTime date_of_birth { get; set; }

    [FirestoreProperty]
    public string emergency_contact { get; set; }

    [FirestoreProperty]
    public bool is_active { get; set; }

    [FirestoreProperty]
    public bool is_register { get; set; }

    [FirestoreProperty]
    public string name { get; set; }

    [FirestoreProperty]
    public string phone_contact { get; set; }

    [FirestoreProperty]
    public string phone_number { get; set; }

}