using System.Text.Json.Serialization;

namespace BlazorReport.Models
{
    public class Employee
    {
        [JsonPropertyName("社員番号")]
        public string EmployeeNumber { get; set; } = string.Empty;

        [JsonPropertyName("氏名")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("所属")]
        public string Department { get; set; } = string.Empty;

        [JsonPropertyName("役職")]
        public string Post { get; set; } = string.Empty;

        [JsonPropertyName("入社年月日")]
        public string DateOfJoining { get; set; } = string.Empty;
    }
}