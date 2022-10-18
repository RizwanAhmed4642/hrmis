using Hrmis.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.User
{
    public class UserViewModelCustom
    {

    }
    public class UserActivityFilters : Paginator
    {
        public string Query { get; set; }
    }
}