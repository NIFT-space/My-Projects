using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ConsoleApp1
{
	internal class FileName
	{
		public FileName()
		{

		}
		class A
		{
			public int sp;
			public A(int speed) {
				sp = speed = 10;
			}
		}
		class B:A
		{
			public B(int sp):base(sp) { }
		}
		//static void Main(string[] args)
		//{
		//	try
		//	{
		//		Console.WriteLine("Hello");
		//	}
		//	catch (ArgumentNullException)
		//	{
		//		Console.WriteLine("A");
		//	}
		//	catch (Exception)
		//	{
		//		Console.WriteLine("B");
		//	}
		//	finally { Console.WriteLine("C"); }

		//}
		private static string result;
		
		public T swap<T>( T a, T b)
		{
			T temp = a;
			a = b;
			b = temp;
			return temp;
		}

		public static string swapp(string arr,int a, int b)
		{
			string arr2;
			char[] chr = arr.ToCharArray();
			if(arr.Length > a && arr.Length > b )
			{
				char c = arr[a];
				chr[a] = arr[b];
				chr[b] = c;
			}
			arr2 = string.Join("", chr);
			return arr2;
		}

		static void Main(){

			string arr = "BASKETBALL";
			arr= swapp(arr, 0, 1);
			arr= swapp(arr, 2, 3);
			arr= swapp(arr, 10, 30);


            Testblock();
            try
            {
                Method2();
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                Console.ReadKey();
            }


            Singleton s1 = Singleton.GetInstance();
			Singleton s2 = Singleton.GetInstance();


			//T swap
			//swap(2, "abc");


			/////PALINDROME/////
			string st = "Civic2";
			string res = "";
			string[] st2 = new string[st.Length];
			int count = st.Length -1;
			foreach(char c in st.ToLower())
			{
				if (count >= 0)
				{
					st2[count] = c.ToString();
					count--;
				}
			}
			
			res = string.Join("", st2);

			if(res == st.ToLower())
			{
                Console.WriteLine("true");
            }
			else
			{
				Console.WriteLine("false");
			}
			////////////////////////////


            string s = "foo $ boo $ far $";
			string[] str = s.Split(' ');
			int i = str.Length;
			//string t = s.Replace('$', ' ');
			//Console.WriteLine(t);
			//SaySomething();
			//Console.WriteLine(result);
			}


        private static void Method2()
        {
            try
            {
                Method1();
            }
            catch (Exception ex)
            {
                //throw ex resets the stack trace Coming from Method 1 and propogates it to the caller(Main)
                throw ex;
            }
        }

        private static void Method1()
        {
            try
            {
                throw new Exception("Inside Method1");
            }
            catch (Exception)
            {
                throw;
            }
        }



		public static void Testblock()
		{
			var t1 = SaySomething();
			var t2 = GotSomething();

			Task.WaitAll(t1,t2);
			string res = result;
			Method1();
		}





        static async Task<string> SaySomething()
		{
			await Task.Delay(500);
			result = "Hello World!";
			return "Something";
		}

        static async Task<string> GotSomething()
        {
            //await Task.Delay(300);
            result = "Hello got!";
            return "Something";
        }
        public sealed class Singleton
        {
            // The Singleton's constructor should always be private to prevent
            // direct construction calls with the `new` operator.
            private Singleton() { }

            // The Singleton's instance is stored in a static field. There there are
            // multiple ways to initialize this field, all of them have various pros
            // and cons. In this example we'll show the simplest of these ways,
            // which, however, doesn't work really well in multithreaded program.
            private static Singleton _instance;

            // This is the static method that controls the access to the singleton
            // instance. On the first run, it creates a singleton object and places
            // it into the static field. On subsequent runs, it returns the client
            // existing object stored in the static field.
            public static Singleton GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new Singleton();
                }
                return _instance;
            }

            // Finally, any singleton should define some business logic, which can
            // be executed on its instance.
            public void someBusinessLogic()
            {
                // ...
            }
        }
    }
	

}
