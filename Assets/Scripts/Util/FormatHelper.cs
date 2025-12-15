using UnityEngine;

public static class FormatHelper
{
    public static string FormatNumber(long num)
    {
        if (num >= 10000)  
            return (num / 1000f).ToString("0.#") + "K";
        
        return num.ToString();
    }
}
