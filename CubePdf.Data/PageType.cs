using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubePdf.Data
{
    /* --------------------------------------------------------------------- */
    ///
    /// PageType
    /// 
    /// <summary>
    /// Page オブジェクトの種類を定義した列挙型です。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public enum PageType
    {
        Pdf,
        Image,
        Unknown = -1
    }
}
