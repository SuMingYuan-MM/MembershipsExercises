using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memberships.Models;

namespace Memberships.Comparer
{
    public class ThumbnailEqualityComparer:IEqualityComparer<ThumbnailModel>
    {
        public bool Equals(ThumbnailModel thumbnail1, ThumbnailModel thumbnail2)
        {
            return thumbnail1.ProductId.Equals(thumbnail2.ProductId);
        }

        public int GetHashCode(ThumbnailModel thumbnail)
        {
            return thumbnail.ProductId;
        }
    }
}
