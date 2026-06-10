namespace PersonalInfoApi.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string IdNumber { get; set; } = string.Empty;  // 身分證字號
        public string Name { get; set; } = string.Empty;      // 姓名
        public string Gender { get; set; } = string.Empty;    // 性別
        public DateTime Birthday { get; set; }                 // 生日
        public string City { get; set; } = string.Empty;      // 縣市
        public string District { get; set; } = string.Empty;  // 鄉鎮市區
        public string Address { get; set; } = string.Empty;   // 地址
        public string Phone { get; set; } = string.Empty;     // 聯絡電話
    }
}