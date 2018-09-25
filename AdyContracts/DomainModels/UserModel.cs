using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AdyContracts.DomainModels
{
    public class UserModel
    {
        public int Id { get; set; }
        public bool confirmationStatus { get; set; }

        public string  firstName { get; set; }
        public string  lastName { get; set; }
        public string gender { get; set; }
        public DateTime birthdate { get; set; }
        public int departId { get; set; }
        public int? roleId { get; set; }
        public string  email { get; set; }
        public string  department { get; set; }

        public string  password { get; set; }
        public string username { get; set; }
    }
}