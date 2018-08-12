using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WeiXin.Entity
{
    public class UserInfo
    {
        public DateTime AddTime { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]//不自增
        public string Telphone { get; set; }

        public string QQ { get; set; }
        public string BirthDay { get; set; }
        public string Address { get; set; }             
        /// <summary>
        /// 年级
        /// </summary>
        public string Grade { get; set; }
        public string WX { get; set; }
        /// <summary>
        /// 学院
        /// </summary>
        public string College { get; set; }
        public string Height { get; set; }
        /// <summary>
        /// 自我介绍
        /// </summary>
        public string Introduce { get; set; }
        /// <summary>
        /// 对另一半的期望
        /// </summary>
        public string Expect { get; set; }
    }
}
