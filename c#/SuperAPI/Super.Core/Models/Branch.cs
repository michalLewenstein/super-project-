﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Super.Core.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int ShippingCost { get; set; }
        //אנחנו צריכות להוסיף רשימה מסוג מוצר סניף עם קשרי גומלין
        [JsonIgnore]
        public List<BranchProduct> BranchProducts { get; set; }
    }
}
