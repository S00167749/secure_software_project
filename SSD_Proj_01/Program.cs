using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Orb.App;

namespace SSD_Proj_01
{
    class Program
    {
        public static List<Person> values = new List<Person>();
        static void Main(string[] args)
        {
            
            LoginUI();
        }

        private static void ReadInData()
        {
            byte[] data = File.ReadAllBytes("ExampleData.txt");
            //string data = File.ReadAllText("ExampleData.txt");
            string keyData = File.ReadAllText("DataKey.txt");

            string[] keyData2 = keyData.Split('?');

            byte[] key = Derive_Key(keyData2[0], ASCIIEncoding.ASCII.GetBytes(keyData2[1]), data.Count());

            byte[] message_byte_array = XOR_ByteArrays(key, data);

            string records = ASCIIEncoding.ASCII.GetString(message_byte_array, 0, message_byte_array.Length);
            //Console.WriteLine(records);
            string[] data2 = records.Split('?');//records

            Lecturer lecturer = Lecturer.FromCsv(data2[0]);
            Student student = Student.FromCsv(data2[1]);
            values.Add(lecturer);
            values.Add(student);
        }
        private static void LoginUI()
        {
            ReadInData();
            string username, password;

            Console.Write("Enter user name : ");
            username = Console.ReadLine();
            Console.Write("Password : ");
            password = Console1.ReadPassword();

            bool isTrue = CheckCredentials(username, password);

            if (isTrue)
            {
                Console.Clear();
                LoggedInUI(username);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Username or Password is incorrect");
                Console.WriteLine();
                LoginUI();
            }

        }
        private static bool CheckCredentials(string userName, string password)
        {

            bool isTrue = false;
            foreach (var item in values)
            {
                if (item.Name == userName)
                {
                    if (AreEqual(password,item.Password,item.Salt))
                        isTrue = true;
                }
            }

            return isTrue;
        }
        private static void LoggedInUI(string username)
        {
            Person person = values.Find(x => x.Name == username);
            if (person.Type == "STUD")
            {
                Student student = (Student)person;
                MainMenu(student);
            }
            else
            {
                Lecturer lecturer = (Lecturer)person;
                MainMenu(lecturer);
            }
           
        }
        private static void MainMenu(Person student)
        {
            string number = "";

            Console.WriteLine("Hello {0}", student.Name);
            Console.WriteLine("You are logged in as a {0}", student.GetType().ToString().Replace("SSD_Proj_01.", ""));
            Console.WriteLine();
            Console.WriteLine("1) Press 1 to change your password");
            Console.WriteLine("2) Press 2 to view your {0} records", student.GetType().ToString().Replace("SSD_Proj_01.", ""));
            Console.WriteLine("3) Press 3 to sign out");
            Console.Write("\nEnter a number: ");
            number = Console.ReadLine();

            switch (number)
            {
                case "1":
                    Console.Clear();
                    ChangePassword(ref student);
                    break;
                case "2":
                    Console.Clear();
                    ViewStudentRecords(student);
                    break;
                case "3":
                    Console.Clear();
                    WriteToFile();
                    LoginUI();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("**Number is incorrect, Please try again**\n");
                    LoggedInUI(student.Name);
                    break;
            }
        }
        private static void ChangePassword(ref Person student)
        {
            string oldPasword, newPassword1, newPassword2;

            Console.Write("Enter old password: ");
            oldPasword = Console1.ReadPassword();
            Console.Write("Enter new password: ");
            newPassword1 = Console1.ReadPassword();
            Console.Write("Enter new password again: ");
            newPassword2 = Console1.ReadPassword();

            if (student.Password != GenerateHash(oldPasword,student.Salt))
            {
                Console.Clear();
                Console.WriteLine("Password is incorrect\n");
                ChangePassword(ref student);
            }
            else if (newPassword1 != newPassword2)
            {
                Console.Clear();
                Console.WriteLine("New password does not match\n");
                ChangePassword(ref student);
            }
            else
            {
                string salt = CreateSalt();
                student.Password = GenerateHash(newPassword1,salt);
                student.Salt = salt;
                Console.WriteLine("\nPassword has been changed\n");
                Console.WriteLine("Enter any key to return to menu");
                Console.ReadKey();
                Console.Clear();
                LoggedInUI(student.Name);
            }

        }
        private static void ViewStudentRecords(Person student)
        {
            string input;
            int modNum = 0;
            Console.WriteLine("Here is the list of your Modules\n");
            foreach (var item in student.Modules)
            {
                Console.WriteLine("{2}) {1}: {0}", item.ModuleName, item.ModuleNumber, modNum);
                modNum++;
            }

            Console.WriteLine("\nEnter BACK to go back to main menu");
            if (student.Type == "STUD")
            {
                Console.WriteLine("Enter UPDATE to update a record: ");
                input = Console.ReadLine();

                if (input.ToUpper() == "UPDATE")
                {
                    UpdateModule(student);
                }
                else if (input.ToUpper() == "BACK")
                {
                    LoggedInUI(student.Name);
                }
                else
                {
                    Console.Clear();
                    ViewStudentRecords(student);
                }
            }
            else
            {
                Console.WriteLine("Enter (ADD, UPDATE OR DELETE) to add,update or delete a record: ");
                input = Console.ReadLine();
                if (input.ToUpper() == "ADD")
                {
                    AddModule(student);
                }
                else if (input.ToUpper() == "UPDATE")
                {
                    UpdateModule(student);
                }
                else if (input.ToUpper() == "DELETE")
                {
                    DeleteModule(student);
                }
                else if (input.ToUpper() == "BACK")
                {
                    LoggedInUI(student.Name);
                }
                else
                {
                    Console.Clear();
                    ViewStudentRecords(student);
                }
            }         
        }
        private static void WriteToFile()
        {
            //before your loop
            string csv = "";

            //in your loop
            foreach (var item in values)
            {
                string module = "";
                foreach (var mod in item.Modules)
                {
                    string modString = string.Format("{0}:{1},", mod.ModuleNumber, mod.ModuleName);
                    module += modString;
                }
                module = module.Remove(module.Length - 1);
                var newLine = string.Format("{0};{1};{2};{3};{4};{5};{6};{7}", item.Type, item.Year, item.Name, item.ID, item.Age,
                    item.Password,
                    module,item.Salt+"?");
                csv += newLine;
            }
            //convert to ascii
             string keyData = File.ReadAllText("DataKey.txt");

             string[] keyData2 = keyData.Split('?');

             byte[] key = Derive_Key(keyData2[0], ASCIIEncoding.ASCII.GetBytes(keyData2[1]), ASCIIEncoding.ASCII.GetByteCount(csv));

             byte[] message_byte_array = ASCIIEncoding.ASCII.GetBytes(csv);

            //ciphertext (base64)
            byte[] ciphertext = XOR_ByteArrays(key, message_byte_array);


            //after your loop
            File.WriteAllBytes("ExampleData.txt", ciphertext);
            //File.WriteAllText("ExampleData.txt", csv);

            // clear ram
            GC.Collect();

        }
        private static void AddModule(Person student)
        {
            string modCode, modName;
            Console.WriteLine("\nAdd a module");
            Console.Write("Enter module code: ");
            modCode = Console.ReadLine();
            Console.Write("Enter module name: ");
            modName = Console.ReadLine();
            Console.WriteLine("Adding module [{0}: {1}]",modCode,modName);
            Module temMode = new Module(modName,modCode);
            student.Modules.Add(temMode);
            Console.ReadKey();
            ViewStudentRecords(student);
        }
        private static void DeleteModule(Person student)
        {
            int modNum = 0;
            Console.WriteLine("\nDelete a module");
            Console.Write("Enter module Number: ");
            modNum = int.Parse(Console.ReadLine());
            Console.WriteLine("Removing module [{0}: {1}]", student.Modules[modNum].ModuleNumber, 
                student.Modules[modNum].ModuleName);
            student.Modules.RemoveAt(modNum);
            Console.ReadKey();
            ViewStudentRecords(student);
        }
        private static void UpdateModule(Person student)
        {
            int modNum = 0;
            string modCode, modName;

            Console.WriteLine("\nUpdate a module");
            Console.Write("Enter module Number: ");
            modNum = int.Parse(Console.ReadLine());

            Console.Write("Enter module code: ");
            modCode = Console.ReadLine();
            Console.Write("Enter module name: ");
            modName = Console.ReadLine();

            Console.WriteLine("Updating module from [{0}: {1}] to [{2} : {3}]", 
                student.Modules[modNum].ModuleNumber,
                student.Modules[modNum].ModuleName, modCode,modName);
            student.Modules[modNum].ModuleName = modName;
            student.Modules[modNum].ModuleNumber = modCode;
            Console.ReadKey();
            ViewStudentRecords(student);
        }
        static byte[] Derive_Key(string password, byte[] salt, int key_length)
        {
            //Convert Password To Byte Array - Assumes That Password Is ASCII Encoded.
            byte[] password_byte_array = ASCIIEncoding.ASCII.GetBytes(password);
            //Generate Key
            Rfc2898DeriveBytes pbkdf = new Rfc2898DeriveBytes(password_byte_array, salt, 10000);//Recommended Minimum Value For Parameter 3 Is 10000.
            byte[] key = pbkdf.GetBytes(key_length);//Length Of Key To Be Derived In Bytes.

            return key;
        }

        static byte[] GenerateKey(int messageLengthInBytes)
        {

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();//Secure Random Generator

            byte[] key_generated = new byte[messageLengthInBytes];
            rng.GetBytes(key_generated);//Fills The key_generated Array With Randomly Generated Bytes

            return key_generated;

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
        //generate salt
        private static string CreateSalt()
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[16];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }
        //2 generate hash
        public static string GenerateHash(string input, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        //check hash
        private static bool AreEqual(string plainTextInput, string hashedInput, string salt)
        {
            string newHashedPin = GenerateHash(plainTextInput, salt);
            return newHashedPin.Equals(hashedInput);
        }
    }

    class Student : Person
    {
        public static Student FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            List<Module> modules = SplitModules(values[6]);

            Student student = new Student
            {
                Name = values[2],
                ID = values[3],
                Year = values[1],
                Type = values[0],
                Password = values[5],
                Salt = values[7]
            };
            student.Age = int.Parse(values[4]);
            student.Modules = modules;
            return student;

        }
    }

    class Lecturer : Person
    {
        public static Lecturer FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            List<Module> modules = SplitModules(values[6]);

            Lecturer lecturer = new Lecturer
            {
                Name = values[2],
                ID = values[3],
                Year = values[1],
                Type = values[0],
                Password = values[5],
                Salt = values[7]
            };
            lecturer.Age = int.Parse(values[4]);
            lecturer.Modules = modules;
            return lecturer;

        }
    }

     class Person
    {
        public string Name { get; set; }
        public string Year { get; set; }
        public string ID { get; set; }
        public string Type { get; set; }
        public int Age { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public List<Module> Modules { get; set; }

      
        public static List<Module> SplitModules(string modules)
        {
            List<Module> modulesList = new List<Module>();
            string[] moduleList = modules.Split(',');
            foreach (var mod in moduleList)
            {
                string[] modList = mod.Split(':');
                modulesList.Add(new Module(modList[0], modList[1]));
            }

            return modulesList;
        }

        public static Person RetrievePersonByName(List<Person> peopleList, string name)
        {
            return peopleList.Find(x => x.Name == name);
        }

        public static Person RetrievePersonByID(List<Person> peopleList, string ID)
        {
            return peopleList.Find(x => x.ID == ID);
        }
    }

    public class Module
    {
        public string ModuleName { get; set; }
        public string ModuleNumber { get; set; }  //code

        public Module(string modName, string modNumber)
        {
            ModuleName = modName;
            ModuleNumber = modNumber;
        }
    }


}
