// @nuget: HtmlAgilityPack
// @nuget: System.Net.Http
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;

public static class Program
{
    private const string Url = "https://app.haugiang.gov.vn/LichLamViec/Lich/DonVi?MaDonVi=vptu";
    private const string FromDate = "20/8/2024";
    private const string ToDate = ", ngày 21/8/2024";

    public static async Task Main()
    {
        using var client = new HttpClient();

        try
        {
            var pageContent = await GetPageContentAsync(client, Url);
            var pContents = ExtractParagraphContents(pageContent);

            if (pContents.Count == 0)
            {
                Console.WriteLine("Không tìm thấy thẻ <p> nào trên trang.");
                return;
            }

            var fromDateIndex = FindLastIndexContaining(pContents, FromDate);
            if (fromDateIndex < 0)
            {
                Console.WriteLine("Không tìm thấy thẻ <p> chứa fromDate'.");
                return;
            }

            Console.WriteLine("Chương trình làm việc của Thường trực Tỉnh ủy Hậu Giang hôm nay:");

            foreach (var content in pContents[fromDateIndex..])
            {
                var decodedContent = WebUtility.HtmlDecode(content);
                if (decodedContent.Contains(ToDate, StringComparison.Ordinal))
                {
                    break;
                }

                Console.WriteLine(decodedContent);
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Không thể tải dữ liệu từ máy chủ: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Yêu cầu đã hết thời gian chờ: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Đã xảy ra lỗi: {ex.Message}");
        }
    }

    private static async Task<string> GetPageContentAsync(HttpClient client, string url)
    {
        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private static List<string> ExtractParagraphContents(string pageContent)
    {
        var document = new HtmlDocument();
        document.LoadHtml(pageContent);

        var pNodes = document.DocumentNode.SelectNodes("//p");
        if (pNodes is null)
        {
            return [];
        }

        var contents = new List<string>(pNodes.Count);
        foreach (var p in pNodes)
        {
            contents.Add(p.InnerText.Trim());
        }

        return contents;
    }

    private static int FindLastIndexContaining(IReadOnlyList<string> values, string searchText)
    {
        for (var i = values.Count - 1; i >= 0; i--)
        {
            if (values[i].Contains(searchText, StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
    }
}
