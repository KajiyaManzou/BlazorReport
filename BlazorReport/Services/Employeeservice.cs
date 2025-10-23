using System.Net.Http.Json;
using System.Text.Json;
using BlazorReport.Models;

namespace BlazorReport.Services
{
    public class EmployeeService
    {
        private readonly HttpClient _httpClient;
        private List<Employee>? _employees;

        public EmployeeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// 社員データを取得します
        /// </summary>
        /// <returns>社員データのリスト</returns>
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            // キャッシュがあればそれを返す
            if (_employees != null)
            {
                return _employees;
            }

            try
            {
                // JSONファイルから社員データを読み込み
                _employees = await _httpClient.GetFromJsonAsync<List<Employee>>("data/SampleData.json");
                return _employees ?? new List<Employee>();
            }
            catch (Exception ex)
            {
                // エラー時は空のリストを返す
                Console.WriteLine($"Error loading employee data: {ex.Message}");
                return new List<Employee>();
            }
        }

        /// <summary>
        /// キャッシュをクリアしてデータを再読み込みします
        /// </summary>
        public async Task<List<Employee>> ReloadEmployeesAsync()
        {
            _employees = null;
            return await GetEmployeesAsync();
        }
    }
}
