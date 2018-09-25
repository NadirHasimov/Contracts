using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdyContracts.DomainModels
{
    public class MenuModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuParentPath { get; set; }
    }
}