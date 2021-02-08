namespace Storage.MongoDb
{
    public static class MongoExtensions
    {
        //public static SortDefinition<T> ApplyOrderBy<T>(this SortDefinition<T> sort, string orderBy) 
        //    where T : ContentBase
        //{
        //    if (orderBy == null)
        //    {
        //        return sort;
        //    }


        //    var first = true;
        //    var orderByClauses = orderBy.Split(',');
        //    foreach (var orderByClause in orderByClauses.Select(c => c.ToLower()))
        //    {
        //        string clause = orderByClause.ToLower();
        //        string direction = "asc";

        //        var clauses = orderByClause.Split(' ');
        //        if (clauses.Length > 1)
        //        {
        //            clause = clauses.First().ToLower();
        //            direction = clauses.Last().Trim().ToLower();
        //        }
        //        var isAsc = direction.StartsWith("asc");

        //        switch (clause)
        //        {
        //            case "created":
        //                sort =  (isAsc ?
        //                    sort.Ascending(o => o.Created ) :
        //                    sort.Descending(o => o.Created));
        //                break;
        //            case "modified":
        //                sort = isAsc ?
        //                    sort.OrderBy(o => o.Modified) :
        //                    sort.OrderByDescending(o => o.Modified);
        //                break;
        //            case "ownerid":
        //                sort = isAsc ?
        //                    sort.OrderBy(o => o.OwnerId) :
        //                    sort.OrderByDescending(o => o.OwnerId);
        //                break;
        //            case "requestid":
        //                sort = isAsc ?
        //                    sort.OrderBy(o => o.RequestId) :
        //                    sort.OrderByDescending(o => o.RequestId);
        //                break;
        //            case "index":
        //                sort = isAsc ?
        //                    sort.OrderBy(o => o.Index) :
        //                    sort.OrderByDescending(o => o.Index);
        //                break;
        //            case "aspect":
        //                sort = isAsc ?
        //                    sort.OrderBy(o => o.Aspect) :
        //                    sort.OrderByDescending(o => o.Aspect);
        //                break;
        //        }
        //        first = false;
        //    }
        //    return sort;
        //}
    }
}