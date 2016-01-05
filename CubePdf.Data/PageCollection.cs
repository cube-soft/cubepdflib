using System.Collections.Generic;
using CubePdf.Data;

namespace CubePdf.Data
{
    public class PageCollection : List<PageBase>, IReadOnlyCollection<PageBase>
    {
        #region Constructors

        public PageCollection() : base() { }

        #endregion
    }
}
