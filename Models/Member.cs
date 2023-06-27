//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace 浪愛有家.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Member
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Member()
        {
            this.Adoption = new HashSet<Adoption>();
            this.Missing = new HashSet<Missing>();
            this.Rescue = new HashSet<Rescue>();
        }

        [DisplayName("會員帳號")]
        [Required(ErrorMessage = "請填寫會員帳號")]
        [StringLength(10, ErrorMessage = "不可超過10個字")]
        [RegularExpression("[A-Za-z0-9]{6,10}", ErrorMessage = "帳號格式錯誤")]
        public string Account { get; set; }

        string password;
        [DisplayName("會員密碼")]
        [Required(ErrorMessage = "請填寫會員密碼")]
        [RegularExpression("[A-Za-z0-9]{6,}", ErrorMessage = "密碼格式錯誤")]
        [DataType(DataType.Password)]
        public string Password
        {
            get
            {
                return password;
            }
            set
            {

                password = BR.getHashPassword(value);
            }
        }

        [DisplayName("會員名字")]
        [Required(ErrorMessage = "請填寫會員名字")]
        [StringLength(30, ErrorMessage = "不可超過30個字")]
        public string Name { get; set; }

        [DisplayName("手機號碼")]
        [Required(ErrorMessage = "請填寫手機號碼")]
        [RegularExpression("[0][9][0-9]{8}", ErrorMessage = "手機格式錯誤")]
        public string Phone { get; set; }

        [DisplayName("圖像")]
        //[Required(ErrorMessage = "請選擇頭像")]
        public byte[] PhotoFile { get; set; }

        [DisplayName("相片類型")]
        public string ImageType { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Adoption> Adoption { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Missing> Missing { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rescue> Rescue { get; set; }
    }
}