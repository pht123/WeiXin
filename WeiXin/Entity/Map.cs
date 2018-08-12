using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WeiXin.Entity
{
    public class Map
    {
        [Key]
        public int ManTelphone { get; set; }
        [Key]
        public int WomenTelphone{get;set;}
    }
}
