using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

class Program
{

    static byte[] Derive_Key(string password, byte[] salt, int key_length)
    {
        //Convert Password To Byte Array - Assumes That Password Is ASCII Encoded.
        byte[] password_byte_array = ASCIIEncoding.ASCII.GetBytes(password);
        //Generate Key
        Rfc2898DeriveBytes pbkdf = new Rfc2898DeriveBytes(password_byte_array, salt, 10000);//Recommended Minimum Value For Parameter 3 Is 10000.
        byte[] key = pbkdf.GetBytes(key_length);//Length Of Key To Be Derived In Bytes.

        return key;
    }

    static byte[] Generate_Salt()
    {

        //Generate Salt
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();//Secure Random Generator.
        byte[] salt = new byte[8];//Recommended Minimum Salt Size Is 64 Bits/8 Bytes.
        rng.GetBytes(salt);

        return salt;

    }

    static byte[] XOR_ByteArrays(byte[] array1, byte[] array2)//Same Method Used For Encryption And Decryption
    {

        //Assumes Both Arrays Are Of The Same Length - Not Safe If Invalid Data Entered.

        byte[] array3 = new byte[array1.Length];//New Array To Hold Result Of XOR Operations

        for (int i = 0; i < array3.Length; i++)
        {

            int resultOfXOR = array1[i] ^ array2[i];//Hat Symbol Denotes XOR - Result Of Operation Is Int Data Type.
            array3[i] = (byte)resultOfXOR;//Casting The Above To Byte Data Type And Storing In Returned Array.

        }

        return array3;

    }

    static void Main(string[] args)
    {

        string chosenOption;

        do
        {

            Console.WriteLine("MENU: ");
            Console.WriteLine("1. Encrypt Message: ");
            Console.WriteLine("2. Decrypt Message: ");
            Console.WriteLine("3. Quit");
            Console.WriteLine("Enter Option: ");
            chosenOption = Console.ReadLine();

            switch (chosenOption)
            {

                case "1":
                    Console.Write("Enter Message To Encrypt (ASCII): ");
                    string message = Console.ReadLine();//Data-Entry Purposes Only
                    Console.Write("Enter Encryption Password (ASCII): ");
                    string password = Console.ReadLine();

                    byte[] salt = Generate_Salt();
                    Console.WriteLine("SALT Generated (Base64): " + Convert.ToBase64String(salt));//Display Purposes Only.

                    byte[] key = Derive_Key(password, salt, ASCIIEncoding.ASCII.GetByteCount(message));
                    Console.WriteLine("Derived Key (Base64): " + Convert.ToBase64String(key));//Display Purposes Only.

                    byte[] message_byte_array = ASCIIEncoding.ASCII.GetBytes(message);

                    byte[] ciphertext = XOR_ByteArrays(key, message_byte_array);
                    Console.WriteLine("Ciphertext (Base64): " + Convert.ToBase64String(ciphertext));//Display Purposes Only.
                    break;

                case "2":
                    Console.Write("Enter Message To Decrypt (Base64): ");
                    string ciphertext_string = Console.ReadLine();//Data-Entry Purposes Only
                    ciphertext = Convert.FromBase64String(ciphertext_string);

                    Console.Write("Enter Decryption Password (ASCII): ");
                    password = Console.ReadLine();

                    Console.Write("Enter SALT (Base64): ");//Data-Entry Purposes Only
                    string salt_string = Console.ReadLine();
                    salt = Convert.FromBase64String(salt_string);

                    key = Derive_Key(password, salt, ciphertext.Length);
                    Console.WriteLine("Derived Key (Base64): " + Convert.ToBase64String(key));//Display Purposes Only.

                    message_byte_array = XOR_ByteArrays(key, ciphertext);
                    Console.WriteLine("Message (ASCII): " + ASCIIEncoding.ASCII.GetString(message_byte_array, 0, message_byte_array.Length));//Display Purposes Only.
                    break;

                case "3": break;

                default:
                    Console.WriteLine("Inavlid Option Chosen.");
                    break;

            }


        } while (chosenOption != "3");

    }
}