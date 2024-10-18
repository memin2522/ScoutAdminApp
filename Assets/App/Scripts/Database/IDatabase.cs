using System.Collections.Generic;
using System;

public interface IDatabase
{
    public void Initialize();
    public string GenerateIdForNewEntry(QueryConfig databaseQuery);
    public void RetriveDataQuery<T>(QueryConfig databaseQuery, Action<T> callbackResult);
    public void RetriveDataListQuery<T>(QueryConfig databaseQuery, Action<List<T>> callbackResult);
    public void SetDataQuery(object modelData, QueryConfig databaseQuery, Action<bool> callback);
    public void DeleteDataQuery(QueryConfig databaseQuery, Action<bool> callback = null);
    public void ChangeFieldWithDataQuery(QueryConfig databaseQuery, Action<bool> callback = null);

}
