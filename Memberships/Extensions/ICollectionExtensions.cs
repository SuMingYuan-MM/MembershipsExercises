using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Memberships.Extensions
{
    public static class ICollectionExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this ICollection<T> items, int selectedValue)
        {
            //Code goes here
            return from item in items
                select new SelectListItem
                {
                    Text = item.GetPropertyValue("Title"),
                    Value = item.GetPropertyValue("Id"),
                    Selected = item.GetPropertyValue("Id")
                        .Equals(selectedValue.ToString())
                };
        }
    }
}