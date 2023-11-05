using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parameter_ML.Models
{
    public class Models
    {
        /// 註冊參數
        /// </summary>
        public class DoRegisterIn
        {
            public string UserID { get; set; }
            public string UserPwd { get; set; }
            public string UserName { get; set; }
            public string UserEmail { get; set; }

            [Key]
            public int Index { get; set; }
        }

        /// <summary>
        /// 註冊回傳
        /// </summary>
        public class DoRegisterOut
        {
            public string ErrMsg { get; set; }
            public string ResultMsg { get; set; }
        }
    }
}