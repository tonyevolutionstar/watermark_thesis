using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    private static bool NextCombination(IList<int> num, int n, int k)
    {
        bool finished;

        var changed = finished = false;

        if (k <= 0) return false;

        for (var i = k - 1; !finished && !changed; i--)
        {
            if (num[i] < n - 1 - (k - 1) + i)
            {
                num[i]++;

                if (i < k - 1)
                    for (var j = i + 1; j < k; j++)
                        num[j] = num[j - 1] + 1;
                changed = true;
            }
            finished = i == 0;
        }

        return changed;
    }

    private static IEnumerable Combinations<T>(IEnumerable<T> elements, int k)
    {
        var elem = elements.ToArray();
        var size = elem.Length;

        if (k > size) yield break;

        var numbers = new int[k];

        for (var i = 0; i < k; i++)
            numbers[i] = i;

        do
        {
            yield return numbers.Select(n => elem[n]);
        } while (NextCombination(numbers, size, k));
    }


    public static void Main()
    {
        List<string> options = new List<string>();
        List<string> options2 = new List<string>();
        const int k = 2;
        var n = new[] {"qrcode1_l", "qrcode1_r", "qrcode1_b", "qrcode2_l", "qrcode2_r", "qrcode2_b",  "qrcode3_l", "qrcode3_r", "qrcode3_b",
                       "qrcode4_l", "qrcode4_r", "qrcode4_b", "qrcode5_l", "qrcode5_r", "qrcode5_b",  "qrcode6_l", "qrcode6_r", "qrcode6_b",
                       "qrcode7_l", "qrcode7_r", "qrcode7_b", "qrcode8_l", "qrcode8_r", "qrcode8_b",  "qrcode9_l", "qrcode9_r", "qrcode9_b"};

        Console.Write("n: ");
        foreach (var item in n)
        {
            Console.Write("{0} ", item);
        }
        Console.WriteLine();
        Console.WriteLine("k: {0}", k);
        Console.WriteLine();

        foreach (IEnumerable<string> i in Combinations(n, k))
        {
            options.Add(string.Join(" ", i));

            Console.WriteLine(string.Join(" ", i));
        }

        for (int i = 0; i < options.Count; i++)
        {
            string[] aux = options[i].Split(' ');
            string[] val0 = aux[0].Split('_');
            string[] val1 = aux[1].Split('_');

            if (!val0[0].Equals(val1[0]))
                options2.Add(options[i]);
        }

        Console.WriteLine("----------------");

        for (int i = 0; i < options2.Count; i++)
        {
            Console.WriteLine(options2[i]);
        }

    }
}