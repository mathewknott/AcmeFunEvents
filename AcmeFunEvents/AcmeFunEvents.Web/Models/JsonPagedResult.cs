namespace AcmeFunEvents.Web.Models
{
    /// <summary>
    /// A generic object that reurns Json mainly used for paging with bootstrap tables
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonPagedResult<T>
    {
        public int Total { get; set; }
        public T Rows { get; set; }
    }
}