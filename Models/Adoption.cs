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

    public partial class Adoption
    {
        [DisplayName("送養文章編號")]
        public string AdoptionID { get; set; }

        [DisplayName("會員帳號")]
        public string Account { get; set; }

        [DisplayName("動物名稱")]
        [StringLength(10, ErrorMessage = "不可超過10個字")]
        public string PetName { get; set; }

        [DisplayName("種類")]
        [Required(ErrorMessage = "請選擇動物種類")]
        public string AnimalKind { get; set; }

        [DisplayName("品種")]
        [StringLength(10, ErrorMessage = "不可超過10個字")]
        public string AnimalVariety { get; set; }

        [DisplayName("性別")]
        [Required(ErrorMessage = "請選擇動物性別")]
        public string AnimalSex { get; set; }

        [DisplayName("年齡")]
        [StringLength(10, ErrorMessage = "不可超過10個字")]
        public string Age { get; set; }

        [DisplayName("體型")]
        [Required(ErrorMessage = "請選擇動物體型")]
        public string BodyType { get; set; }

        [DisplayName("特徵")]
        [StringLength(100, ErrorMessage = "不可超過100個字")]
        public string Feature { get; set; }

        [DisplayName("是否結紮")]
        [Required(ErrorMessage = "請選擇動物是否結紮")]
        public string Neutered { get; set; }

        [DisplayName("所在縣市")]
        public byte CityID { get; set; }

        [DisplayName("所在鄉鎮區")]
        public string CountryID { get; set; }

        [DisplayName("刊登日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public System.DateTime PublicationDate { get; set; }

        [DisplayName("補充說明")]
        [StringLength(500, ErrorMessage = "不可超過500個字")]
        public string Caption { get; set; }

        [DisplayName("動物相片")]
        public byte[] PhotoFile { get; set; }

        [DisplayName("相片類型")]
        public string ImageType { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual City City { get; set; }
        public virtual Country Country { get; set; }
    }
}