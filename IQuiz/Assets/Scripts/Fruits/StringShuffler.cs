using System;

// Class definition: Blueprint for creating objects (instances)
public class StringShuffler
{
    // Function to shuffle the characters in a string
    public string ShuffleString(string input)
    {
        // Convert the string to a character array
        char[] charArray = input.ToCharArray();

        // Use Fisher-Yates shuffle algorithm to shuffle the characters
        Random rng = new Random();
        int n = charArray.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            char value = charArray[k];
            charArray[k] = charArray[n];
            charArray[n] = value;
        }

        // Convert the character array back to a string
        return new string(charArray);
    }
}
