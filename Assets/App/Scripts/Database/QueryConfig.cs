
using System.Collections.Generic;

public class QueryConfig
{
    public bool isCollection { get; set; }
    public bool moreDocuments { get; set; }
    public string KeyOrder { get; set; }
    public string Collection { get; set; }
    public string Document { get; set; }
    public WhereType Where { get; set; }
    public string Value { get; set; }
    public List<string> ValuesList { get; set; }
    public List<object> ValuesObjet { get; set; }
    public string OrderBy { get; set; }
    public int Limit { get; set; }
    public bool isOrderAscending { get; set; }
    public Dictionary<string, object> FieldsToUpdate { get; set; }

    public QueryConfig()
    {
        Where = new WhereType();
    }

    public QueryConfig(string collection, string document)
    {
        Collection = collection;
        Document = document;
    }
}

public class QueryConfigExtend : QueryConfig
{
    public string SubDocument { get; set; }
    public string SubCollection { get; set; }
    public WhereType SecondWhere { get; set; }
    public string SecondValue { get; set; }
    public QueryConfigExtend()
    {
        SecondWhere = new WhereType();
    }

    public QueryConfigExtend(string collection, string document,
        string subCollection, string subDocument)
    {
        Collection = collection;
        Document = document;
        SubCollection = subCollection;
        SubDocument = subDocument;
    }
}

public class WhereType
{
    public string WhereEqual { get; set; }
    public string WhereIn { get; set; }
    public string WhereArray { get; set; }
    public WhereType()
    {

    }
}

