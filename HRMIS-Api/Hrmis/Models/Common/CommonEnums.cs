using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phfmc.Models.Common
{
    public enum ResultType
    {
        Success,
        Exception,
        Error,
        DbError,
        Duplicate,
        DB
    }
}