using System;
using System.Security.Cryptography;
using System.Text;

public class ZobowiazanieBitowe
{
	
    public static byte[] HashString(string inputString)
    {
        HashAlgorithm algorithm = SHA256.Create();  //or use SHA1.Create();
        return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in HashString(inputString))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }

    public static byte[] LosowanieCiagow()
    {
        Random rand = new Random();
        byte[] R = new byte[120];
        for(int i=0; i<120; i++)
        {
            if (rand.Next(0, 101) % 2 == 0)
                R[i] = 0;
            else
                R[i] = 1;
        }
        return R;
    }

    public static string PodjecieDecyzjiSend(string R1, string S)
    {
        return R1 + ";" + S;
    }

    public static string PodjecieDecyzjiGetR1(string inputstring)
    {
        string R1 = "";
        char[] arr;
        arr = inputstring.ToCharArray(0, inputstring.Length);
        int i = 0;
        while(arr[i] != ';')
        {
            R1 = R1 + arr[i];
            i++;
        }
        return R1;
    }

    public static string PodjecieDecyzjiGetS(string inputstring)
    {
        string S = "";
        char[] arr;
        arr = inputstring.ToCharArray(0, inputstring.Length);
        int i = 0;
        while (arr[i] != ';')
            i++;
        i++;
        for (int j = i; j < inputstring.Length; j++)
            S = S + arr[j];
        return S;
    }

    public static string OdkryciaDecyzjiSend(string R1, string R2, string ba)
    {
        return R1 + ";" + R2 + ";" + ba;
    }

    public static string OdkrycieDecyzjiGetR1(string inputstring)
    {
        string R1 = "";
        char[] arr;
        arr = inputstring.ToCharArray(0, inputstring.Length);
        int i = 0;
        while (arr[i] != ';')
        {
            R1 = R1 + arr[i];
            i++;
        }
        return R1;
    }

    public static string OdkrycieDecyzjiGetR2(string inputstring)
    {
        string R2 = "";
        char[] arr;
        arr = inputstring.ToCharArray(0, inputstring.Length);
        int i = 0;
        while (arr[i] != ';')
            i++;
        i++;
        while (arr[i] != ';')
        {
            R2 = R2 + arr[i];
            i++;
        }
        return R2;
    }

    public static string OdkrycieDecyzjiGetba(string inputstring)
    {
        string ba = "";
        char[] arr;
        arr = inputstring.ToCharArray(0, inputstring.Length);
        int i = 0;
        while (arr[i] != ';')
            i++;
        i++;
        while (arr[i] != ';')
            i++;
        i++;
        for (int j = i; j < inputstring.Length; j++)
            ba = ba + arr[j];
        return ba;
    }

    public static string OdkryciaDecyzji(string inputstring, string R1F, string SF)
    {
        string R1A = OdkrycieDecyzjiGetR1(inputstring);
        string R2 = OdkrycieDecyzjiGetR2(inputstring);
        string ba = OdkrycieDecyzjiGetba(inputstring);
        if (R1F == R1A)
        {
            string SA = GetHashString(R1A + R2 + ba);
            if (SF == SA)
                return "accept\n";
            else
                return "Różne S\n";
        }
        else
            return "Różne R1";
    }

}
