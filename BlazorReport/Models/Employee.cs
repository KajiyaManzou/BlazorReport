using System.Text.Json.Serialization;

namespace BlazorReport.Models
{
    /// <summary>
    /// 社員情報を表すモデルクラス
    /// </summary>
    public class Employee
    {
        [JsonPropertyName("EmployeeNumber")]
        public string EmployeeNumber { get; set; } = string.Empty;

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("Department")]
        public string Department { get; set; } = string.Empty;

        [JsonPropertyName("Post")]
        public string Post { get; set; } = string.Empty;

        [JsonPropertyName("DateOfJoining")]
        public string DateOfJoining { get; set; } = string.Empty;
    }
}