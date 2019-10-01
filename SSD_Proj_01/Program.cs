using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orb.App;

namespace SSD_Proj_01
{
    class Program
    {
        static void Main(string[] args)
        {
            ProgramUi();
            
        }

        private static void ReadInData()
        {
            List<Student> values = File.ReadAllLines("ExampleData.csv")
                                           .Skip(1)
                                           .Select(v => Student.FromCsv(v))
                                           .ToList();
            PrintData(values);
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
                //Show something.
            }
            else
            {
                Console.Clear();
                ProgramUi();
            }

            ReadInData();
            Console.ReadKey();


        }
        private static void LoginUI()
        {
            string username, password;

            Console.Write("Enter user name : ");
            username = Console.ReadLine();
            Console.Write("Password : ");
            password = Console1.ReadPassword();

            Console.WriteLine("Sorry - I just can't keep a secret!");
            Console.WriteLine("Your password was:\n<Password>{0}</Password>", password);

            Console.ReadLine();
            Console.WriteLine("Password : ");

        }
        private static void PrintData(List<Student> students)
        {
            foreach (Student student in students)
            {
                Console.WriteLine("**************************************************");
                Console.WriteLine(student.Name +"/t"+student.ID +"/t"+student.Course);
            }
        }
        private static string WelcomeDisplayUI()
        {
            string login;

            Console.WriteLine("***************************************************************");
            Console.WriteLine("*******************Welcome to this application*****************");
            Console.Write("Would you like to sign in or browser as a guest (Y/N) : ");
            login = Console.ReadLine();

            return login;
        }
    }

    class Student : People
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
                Course = values[1]
            };

            return student;
        }

        public void SearchStudentByName()
        { }

        public void SearchStudentByID()
        {

        }
    }

    class Lectures : People {

    }

    abstract class People
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public int Age { get; set; }
    }


}
