﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSearch.Components
{
    public class SearchCriteria
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string Value { get; set; }
    }
}


// Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;