using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Linq;

public class FirebaseImplementation : MonoBehaviour, IDatabase
{
    private FirebaseFirestore db;
    private string _lastId = "0";

    public void Initialize()
    {
        db = FirebaseFirestore.DefaultInstance;
    }
    public string GenerateIdForNewEntry(QueryConfig databaseQuery)
    {
        DocumentReference docRef;
        if (databaseQuery is QueryConfigExtend)
        {
            var queryConfig = databaseQuery as QueryConfigExtend;
            docRef = db.Collection(queryConfig.Collection).Document(queryConfig.Document).Collection(queryConfig.SubCollection).Document();
        }
        else
        {
            var queryConfig = databaseQuery;
            docRef = db.Collection(queryConfig.Collection).Document();
        }
        return docRef.Id;
    }

    public void RetriveDataQuery<T>(QueryConfig databaseQuery, Action<T> callbackResult)
    {
        DocumentReference docRef = GetDocumentReferenceFromQueryConfig(databaseQuery);
        var dataTask = docRef.GetSnapshotAsync().ContinueWithOnMainThread(dataTask =>
        {
            if (dataTask.IsCompleted)
            {
                DocumentSnapshot snapshot = dataTask.Result;
                T data = snapshot.ConvertTo<T>();
                callbackResult(data);
            }
            else
            {
                Debug.LogWarning($"Failed to data");
            }
        });
    }

    public void RetriveDataListQuery<T>(QueryConfig databaseQuery, Action<List<T>> callbackResult)
    {
        QueryConfig queryConfig = databaseQuery as QueryConfigExtend;
        if (queryConfig == null) { queryConfig = databaseQuery; }
        Query query = GetDocumentsQueryFromQueryConfig(databaseQuery);

        var queryTask = query.GetSnapshotAsync().ContinueWithOnMainThread(queryTask =>
        {
            if (queryTask.IsCompleted)
            {
                Debug.Log(queryTask.Result.Count);
                List<T> data = new List<T>();
                try
                {
                    var coll = queryTask.Result.Documents.ToList();
                    if (coll != null)
                    {
                        if (coll.Count > 0)
                        {
                            Debug.Log(coll.Count);
                            foreach (DocumentSnapshot document in coll)
                            {
                                if (!string.IsNullOrEmpty(queryConfig.KeyOrder))
                                {
                                    _lastId = document.GetValue<string>(queryConfig.KeyOrder);
                                }
                                data.Add(document.ConvertTo<T>());
                            }
                        }
                        else
                        {
                            // no hay Elementos 
                        }
                    }
                    Debug.Log("Carlos");
                    callbackResult(data);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    callbackResult(data);
                }
            }
            else
            {
                Debug.LogWarning($"Failed to retrieve NanoData from FB: {queryTask.Exception}");
            }
        });
    }

    public void SetDataQuery(object modelData, QueryConfig databaseQuery, Action<bool> callback = null)
    {
        DocumentReference docRef = GetDocumentReferenceFromQueryConfig(databaseQuery);
        var dataTask = docRef.SetAsync(modelData).ContinueWithOnMainThread(dataTask =>
        {
            if (dataTask.IsCompleted)
            {
                Debug.Log("Complete Save");
                if (callback != null) callback.Invoke(true);
            }
            else
            {
                Debug.Log("Error saving: " + dataTask.Exception);
                if (callback != null) callback.Invoke(false);
            }
        });
    }

    public void DeleteDataQuery(QueryConfig databaseQuery, Action<bool> callback = null)
    {
        DocumentReference docRef = GetDocumentReferenceFromQueryConfig(databaseQuery);
        var dataTask = docRef.DeleteAsync().ContinueWithOnMainThread(dataTask =>
        {
            if (dataTask.IsCompleted)
            {
                Debug.Log("Completado el borrado del documento");
                if (callback != null) callback.Invoke(true);
            }
            else
            {
                Debug.LogError("Errores en borrado del documento");
                if (callback != null) callback.Invoke(false);
            }
        });
    }

    public void ChangeFieldWithDataQuery(QueryConfig databaseQuery, Action<bool> callback = null)
    {
        DocumentReference docRef = GetDocumentReferenceFromQueryConfig(databaseQuery);
        QueryConfig queryConfig = databaseQuery;

        var dataTask = docRef.UpdateAsync(databaseQuery.FieldsToUpdate).
            ContinueWithOnMainThread(dataTask =>
            {
                if (dataTask.IsCompleted)
                {
                    Debug.Log("Completado el borrado del documento");
                    if (callback != null) callback.Invoke(true);
                }
                else
                {
                    Debug.LogError("Errores en borrado del documento");
                    if (callback != null) callback.Invoke(false);
                }
            });
    }

    #region --- Query Functions and Internal Utilities ---
    private DocumentReference GetDocumentReferenceFromQueryConfig(QueryConfig databaseQuery)
    {
        DocumentReference docRef;
        if (databaseQuery is QueryConfigExtend)
        {
            docRef = GetDocumentReference(databaseQuery as QueryConfigExtend);
        }
        else
        {
            docRef = GetDocumentReference(databaseQuery);
        }

        return docRef;
    }

    public DocumentReference GetDocumentReference(QueryConfig _config)
    {
        DocumentReference docRef;
        docRef = db.Collection(_config.Collection).Document(_config.Document);
        return docRef;
    }

    public DocumentReference GetDocumentReference(QueryConfigExtend _config)
    {
        DocumentReference docRef;
        if (!string.IsNullOrEmpty(_config.SubDocument))
        {
            docRef = db.Collection(_config.Collection).Document(_config.Document)
                .Collection(_config.SubCollection).Document(_config.SubDocument);
        }
        else
        {
            docRef = db.Collection(_config.Collection).Document(_config.Document);
        }
        return docRef;
    }

    private Query GetDocumentsQueryFromQueryConfig(QueryConfig databaseQuery)
    {
        Query query;
        if (databaseQuery is QueryConfigExtend)
        {
            QueryConfigExtend queryConfig = databaseQuery as QueryConfigExtend;
            query = GetQuery(queryConfig);
        }
        else
        {
            QueryConfig queryConfig = databaseQuery;
            query = GetQuery(queryConfig);
        }

        return query;
    }

    public Query GetQuery(QueryConfigExtend _config)
    {
        CollectionReference collRef;
        if (!string.IsNullOrEmpty(_config.Document) && !string.IsNullOrEmpty(_config.SubCollection))
        {
            collRef = db.Collection(_config.Collection).Document(_config.Document)
                .Collection(_config.SubCollection);
        }
        else
        {
            collRef = db.Collection(_config.Collection);
        }

        return Limit(_config, SecondWhere(_config, FirtsWhere(_config, OrderBy(collRef, _config))));
    }

    public Query GetQuery(QueryConfig _config)
    {
        CollectionReference collRef;
        collRef = db.Collection(_config.Collection);
        return Limit(_config, FirtsWhere(_config, OrderBy(collRef, _config)));
    }

    public Query OrderBy(CollectionReference collRef, QueryConfig _config)
    {
        if (_config.moreDocuments)
        {
            if (!string.IsNullOrEmpty(_config.KeyOrder))
            {
                if (_config.isOrderAscending)
                    return collRef.OrderBy(_config.OrderBy).StartAfter(_lastId);
                else
                    return collRef.OrderByDescending(_config.OrderBy).StartAfter(_lastId);
            }
            else
                return collRef;
        }
        else
        {
            if (!string.IsNullOrEmpty(_config.OrderBy))
            {
                if (_config.isOrderAscending)
                    return collRef.OrderBy(_config.OrderBy);
                else
                    return collRef.OrderByDescending(_config.OrderBy);
            }
            else
                return collRef;
        }
    }

    public Query FirtsWhere(QueryConfig _config, Query query)
    {
        if (!string.IsNullOrEmpty(_config.Where.WhereEqual))
            return query.WhereEqualTo(_config.Where.WhereEqual, _config.Value);
        else if (!string.IsNullOrEmpty(_config.Where.WhereIn))
            return query.WhereIn(_config.Where.WhereIn, _config.ValuesList);
        else if (!string.IsNullOrEmpty(_config.Where.WhereArray))
            return query.WhereArrayContains(_config.Where.WhereArray, _config.Value);
        else
            return query;
    }

    public Query SecondWhere(QueryConfigExtend _config, Query query)
    {
        if (_config.SecondWhere != null)
        {
            if (!string.IsNullOrEmpty(_config.SecondWhere.WhereEqual))
            {
                return query.WhereEqualTo(_config.SecondWhere.WhereEqual, _config.SecondValue);
            }
            else if (!string.IsNullOrEmpty(_config.SecondWhere.WhereIn))
            {
                return query.WhereIn(_config.SecondWhere.WhereIn, _config.ValuesList);
            }
            else if (!string.IsNullOrEmpty(_config.SecondWhere.WhereArray))
            {
                return query.WhereArrayContains(_config.SecondWhere.WhereArray, _config.SecondValue);
            }
            else
                return query;
        }
        else
            return query;
    }

    public Query Limit(QueryConfig _config, Query query)
    {
        if (_config.Limit > 0)
            return query.Limit(_config.Limit);
        else
            return query;
    }
    #endregion
}
