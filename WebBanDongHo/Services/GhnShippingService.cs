using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WebBanDongHo.Services
{
    public class GhnShippingService
    {
        private readonly HttpClient _http;
        private readonly int _shopId = 216452; // ID shop test GHN[cite: 7]
        private readonly int _fromDistrictId = 1450; // Mã quận/huyện gửi mặc định[cite: 7]

        public GhnShippingService(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://dev-online-gateway.ghn.vn/shiip/public-api/"); // Môi trường test GHN[cite: 7]
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _http.DefaultRequestHeaders.Add("Token", "fd9143c6-a73d-11f1-a973-aee5264794df");
            _http.DefaultRequestHeaders.Add("ShopId", _shopId.ToString());
        }

        public async Task<string> GetProvincesAsync()
        {
            try
            {
                var res = await _http.GetAsync("master-data/province"); // Lấy danh sách Tỉnh/Thành[cite: 12]
                return await res.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { code = 500, message = ex.Message, data = Array.Empty<object>() });
            }
        }

        public async Task<string> GetDistrictsAsync(int provinceId)
        {
            try
            {
                var res = await _http.GetAsync($"master-data/district?province_id={provinceId}"); // Lấy Quận/Huyện[cite: 12]
                return await res.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { code = 500, message = ex.Message, data = Array.Empty<object>() });
            }
        }

        public async Task<string> GetWardsAsync(int districtId)
        {
            try
            {
                var res = await _http.GetAsync($"master-data/ward?district_id={districtId}"); // Lấy Phường/Xã[cite: 12]
                return await res.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { code = 500, message = ex.Message, data = Array.Empty<object>() });
            }
        }

        public async Task<int> CalculateFeeAsync(int toDistrictId, string toWardCode, int weight = 300)
        {
            try
            {
                var body = new
                {
                    shop_id = _shopId,
                    service_type_id = 2,
                    from_district_id = _fromDistrictId,
                    to_district_id = toDistrictId,
                    to_ward_code = toWardCode,
                    weight = weight > 0 ? weight : 300,
                    length = 15,
                    width = 15,
                    height = 10
                };

                var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                var res = await _http.PostAsync("v2/shipping-order/fee", content); // Tính phí giao hàng[cite: 12]
                if (!res.IsSuccessStatusCode) return 0;

                using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
                if (doc.RootElement.TryGetProperty("code", out var code) && code.GetInt32() == 200 &&
                    doc.RootElement.TryGetProperty("data", out var data) && data.TryGetProperty("total", out var total))
                {
                    return total.GetInt32();
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}