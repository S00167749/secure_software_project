using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Orb.App;

namespace SSD_Proj_01
{
    class Program
    {
        public static List<Student> values;
        static void Main(string[] args)
        {
            ReadInData();
            ProgramUi();
            
       }

        private static void ReadInData()
        {
            values = File.ReadAllLines("ExampleData.csv")
                                           .Select(v => Student.FromCsv(v))
                                           .ToList();
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
                    ViewStudentRecords();
                    break;
                case "3":
                    Console.Clear();
                    values.Add(student);
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
        private static void ViewStudentRecords() { }
        private static void WriteToFile()
        {
            using (var writer = new StreamWriter("ExampleData.csv"))
            {
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteRecords(values);
                }
            }
        }
    }

    class Student : Person
    {
        public string Course { get; set; }
        public string Year { get; set; }

        public static Student FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            Student student = new Student
            {
                Name = values[0],
                ID = values[2],
                Year = values[4],
                Age = int.Parse(values[3]),
                Course = values[1],
                Password = values[5]
            };

            return student;
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

    abstract class Person
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public int Age { get; set; }
        public string Password { get; set; }
    }


}
