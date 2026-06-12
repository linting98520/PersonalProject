using System.ComponentModel.DataAnnotations;

namespace PersonalInfoApi.Models
{
    public class CreatePersonDto
    {
        [Required(ErrorMessage = "身分證字號是必填")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "身分證字號必須是10碼")]
        public string IdNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "姓名是必填")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "性別是必填")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "生日是必填")]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "縣市是必填")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "鄉鎮市區是必填")]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "地址是必填")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "電話是必填")]
        [RegularExpression(@"^09\d{8}$", ErrorMessage = "手機號碼格式錯誤")]
        public string Phone { get; set; } = string.Empty;
    }
}