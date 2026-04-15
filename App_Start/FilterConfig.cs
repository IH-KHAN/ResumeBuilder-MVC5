using System.Web;
using System.Web.Mvc;

namespace ResumeBuilder_1291763
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
