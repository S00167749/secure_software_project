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
        public static List<Student> values = new List<Student>();
        static void Main(string[] args)
        {
            ReadInData();
            ProgramUi();
       }

        private static void ReadInData()
        {
            var records = File.ReadAllLines("ExampleData.csv");
            //.Select(v => Student.FromCsv(v))                                        
            //.ToList();
            //records = records.Skip(1).ToArray();
            foreach (var item in records)
            {
                values.Add(Student.FromCsv(item));
            }

            // PrintData(values);
        }
        private static void ProgramUi()
        {
            string login;

            login = WelcomeDisplayUI();

            if (login.ToUpper() == "Y")
            {
                //Login UI
                Console.Clear();
                LoginUI();
            }
            else if (login.ToUpper() == "N")
            {
                Console.Clear();
                GuestUI();
            }
            else
            {
                Console.Clear();
                ProgramUi();
            }
            Console.ReadKey();

        }
        private static void LoginUI()
        {
            string username, password;
            
            Console.Write("Enter user name : ");
            username = Console.ReadLine();
            Console.Write("Password : ");
            password = Console1.ReadPassword();

            bool isTrue = CheckCredentials(username,password);

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
        private static void PrintData(List<Student> students)
        {
            foreach (Student student in students)
            {
                Console.WriteLine("Name: "+student.Name +"/tID: "+student.ID +"/tCourse: "+student.Course +"Password:  "+ student.Password);
            }
        }
        private static string WelcomeDisplayUI()
        {
            string login;
            Console.WriteLine("****************Welcome to this application**************");
            Console.Write("Would you like to sign in or browser as a guest (Y/N) : ");
            login = Console.ReadLine();

            return login;
        }
        private static void GuestUI()
        {
            Console.WriteLine("there is nothing to see here");
        }
        private static bool CheckCredentials(string userName, string password)
        {
            bool isTrue = false;
            Student student = Student.RetrieveStudentByName(values, userName);

            if (student.Password == password)
                isTrue = true;

            return isTrue;
        }
        private static void LoggedInUI(string username)
        {
            Student student = Student.RetrieveStudentByName(values,username);
            string number = "";

            Console.WriteLine("Hello {0}",student.Name);
            Console.WriteLine("You are logged in as a {0}",student.GetType().ToString().Replace("SSD_Proj_01.",""));
            Console.WriteLine();
            Console.WriteLine("1) Press 1 to change your password");
            Console.WriteLine("2) Press 2 to view your {0} records",student.GetType().ToString().Replace("SSD_Proj_01.", ""));
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
                    //values.Add(student);
                    WriteToFile();
                    ProgramUi();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("**Number is incorrect, Please try again**\n");
                    LoggedInUI(username);
                    break;
            }
        }
        private static void ChangePassword(ref Student student)
        {
            string oldPasword, newPassword1,newPassword2;

            Console.Write("Enter old password: ");
            oldPasword = Console1.ReadPassword();
            Console.Write("Enter new password: ");
            newPassword1 = Console1.ReadPassword();
            Console.Write("Enter new password again: ");
            newPassword2 = Console1.ReadPassword();

            if (student.Password != oldPasword)
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
                student.Password = newPassword1;
                Console.WriteLine("\nPassword has been changed\n");
                Console.WriteLine("Enter any key to return to menu");
                Console.ReadKey();
                Console.Clear();
                LoggedInUI(student.Name);
            }

        }
        private static void ViewStudentRecords(Student student)
        {
            string input;
            int modNum =0;
            Console.WriteLine("Here is the list of your Modules\n");
            foreach (var item in student.Modules)
            {
                Console.WriteLine("{2}) {1}: {0}",item.ModuleName,item.ModuleNumber,modNum);
                modNum++;
            }

            Console.WriteLine("\nEnter BACK to go back to main menu");
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
        }
        private static void WriteToFile()
        {
            //before your loop
            var csv = new StringBuilder();

            //in your loop
            foreach (var item in values)
            {
                string module = "";
                foreach (var mod in item.Modules)
                {
                    string modString = string.Format("{0}:{1},", mod.ModuleNumber,mod.ModuleName);
                    module += modString;
                }
                module = module.Remove(module.Length - 1);
                var newLine = string.Format("{0};{1};{2};{3};{4};{5};{6}", item.Course, item.Year, item.Name,item.ID,item.Age,
                    item.Password,
                    module);
                csv.AppendLine(newLine);
            }
            //after your loop
           File.WriteAllText("ExampleData.csv", csv.ToString());
            //using (var writer = new StreamWriter("ExampleData.csv"))
            //{
            //    using (var csv1 = new CsvWriter(writer))
            //    {
            //        csv1.WriteRecords(values);
            //    }
            //}
        }
        private static void AddModule(Student student)
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
        private static void DeleteModule(Student student)
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
        private static void UpdateModule(Student student)
        { }
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
    }

    class Student : Person
    {
        public string Course { get; set; }
        public string Year { get; set; }

        public static Student FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            List<Module> modules = SplitModules(values[6]);

            Student student = new Student
            {
                Name = values[2],
                ID = values[3],
                Year = values[1],
                Course = values[0],
                Password = values[5]
            };
            student.Age = int.Parse(values[4]);
            student.Modules = modules;
            return student;
            
        }

        public static List<Module> SplitModules(string modules)
        {
            List<Module> modulesList = new List<Module>();
            string[] moduleList = modules.Split(',');
            foreach (var mod in moduleList)
            {
                string[] modList = mod.Split(':');
                modulesList.Add(new Module(modList[0],modList[1]));
            }

            return modulesList;
        }

        public static Student RetrieveStudentByName(List<Student> students, string name)
        {
            return students.Find(x => x.Name == name);
        }

        public static Student RetrieveStudentByID(List<Student> students, string ID)
        {
            return students.Find(x => x.ID == ID);
        }
    }

    class Lectures : Person {

    }

    public class Person
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public int Age { get; set; }
        public string Password { get; set; }
        public List<Module> Modules { get; set; }
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
