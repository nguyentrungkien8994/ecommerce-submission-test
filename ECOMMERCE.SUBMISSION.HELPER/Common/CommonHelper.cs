using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.HELPER;

public class CommonHelper
{
    public static string RandomVerifyCodeNumber()
    {
        Random rand = new Random();
        int number = rand.Next(0, 999999);
        return number.ToString("D6");       // format luôn 6 chữ số, thêm số 0 ở đầu nếu thiếu
    }

    public static bool ValidateRegex(string pattern, string input)
    {
        if (string.IsNullOrEmpty(pattern) || input == null)
            return false;

        return Regex.IsMatch(input, pattern);
    }

    /// <summary>
    /// Format giá trị 
    /// </summary>
    /// <param name="amount">Giá trị cần format</param>
    /// <returns>Giá trị đã format</returns>
    public static decimal? RoundCurrency(decimal? amount)
    {
        if (!amount.HasValue) return null;
        return Math.Round(amount.Value, 0, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Format giá trị phần trăm - nhân 100 và lấy 2 số sau dấu chấm
    /// </summary>
    /// <param name="percentage">Giá trị cần format (dạng decimal 0-1)</param>
    /// <returns>Giá trị đã format (dạng phần trăm)</returns>
    public static decimal? RoundPercentage(decimal? percentage)
    {
        if (!percentage.HasValue) return null;
        return Math.Round(percentage.Value * 100, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal? RoundNumber(decimal? percentage)
    {
        if (!percentage.HasValue) return null;
        return Math.Round(percentage.Value, 2, MidpointRounding.AwayFromZero);
    }
}
