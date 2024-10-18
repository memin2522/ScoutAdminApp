using System.Collections.Generic;
using UnityEngine;

public class ScreenCaminantes : MonoBehaviour
{

    private List<Caminante> caminantes = new List<Caminante>();
    private List<GameObject> instanceRows = new List<GameObject>();
    private FirebaseImplementation db;

    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private Transform parent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        db = AppFlow.instance.databaseImplementation;
        CleanScreen();
    }

    void CleanScreen()
    {
        foreach (GameObject go in instanceRows)
        {
            Destroy(go);
        }
        instanceRows.Clear();
        caminantes.Clear();

        RecoverCaminantesFromDB();
    }

    void RecoverCaminantesFromDB()
    {
        QueryConfig query = new QueryConfig
        {
            Collection = "caminantes"
        };
        db.RetriveDataListQuery<Caminante>(query, result =>
        {
            foreach(Caminante caminante in result)
            {
                if (caminante.is_active)
                {
                    caminantes.Add(caminante);
                    GameObject go = Instantiate(rowPrefab, parent);
                    instanceRows.Add(go);
                    go.GetComponent<RowCaminante>().FillRowInfo(caminante);
                }                
            }
        });
    }
}
