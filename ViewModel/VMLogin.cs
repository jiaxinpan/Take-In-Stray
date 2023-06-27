using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace 浪愛有家.ViewModel
{
    public class VMLogin
    {
        [DisplayName("帳號")]
        [Required(ErrorMessage = "請填寫管理員帳號")]
        [StringLength(10, ErrorMessage = "帳號不可超過10字")]
        [RegularExpression("[A-Za-z0-9]{6,10}", ErrorMessage = "帳號格式錯誤")]
        public string Account { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "請填寫管理員密碼")]
        [RegularExpression("[A-Za-z0-9]{6,}", ErrorMessage = "密碼格式錯誤")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}