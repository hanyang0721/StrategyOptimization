using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;

namespace optimization
{
    class indiObject
    {
        public string indicName { get; set; }
        public int numbegin { get; set; }
        public int numend { get; set; }
        public int gap { get; set; }
    }

    class IndiObjects : IEnumerable
    {
        ArrayList mylist = new ArrayList();

        public indiObject this[int index]
        {
            get { return (indiObject)mylist[index]; }
            set { mylist.Insert(index, value); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mylist.GetEnumerator();
        }
    }

    public static class TwoDArrayExtensions
    {
        public static void Populate<T>(this T[,] arr, T value)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(0); j++)
                {
                    arr[i, j] = value;
                }
            }
        }
    }

    class Program
    {
        public static List<Task> tasks;
        private const string pythonpath = @"C:\Python\Scripts\python.exe";
        private const string scriptpath = @"C:\TradeSoft\Morning_1min.py";
        private const string connectionstr = @"SERVER =localhost;DATABASE=Stock;Trusted_Connection=True";
        private static int MultithreadThrottle = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings.Get("MultithreadThrottle"));
        //public delegate void strategy(Dictionary<string, string> dict);
        public static int processcount = 0;
        //public static Task[] tasksArray;
        static void Main(string[] args)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            tasks = new List<Task>();

            int countC = 0;
            const int conk = 1;//This is the combination, 2-> {A,B}, {B,A}, you need to put two objects in myObjects, not myObjects2
            const int conr = 1;//1->{A}

            IndiObjects myObjects = new IndiObjects();
            myObjects[0] = new indiObject() { indicName = "sma_vol5", numbegin = 1, numend = 1, gap = 1 };
            //myObjects[1] = new indiObject() { indicName = "sma_vol20", numbegin = 20, numend = 60, gap = 10 };
            //myObjects[2] = new indiObject() { indicName = "0", numbegin = 1, numend = 1, gap = 1 };

            IndiObjects myObjects2 = new IndiObjects();
            //set one object only if not using second group
            myObjects2[0] = new indiObject() { indicName = "1", numbegin = 1, numend = 1, gap = 1 };
            //myObjects2[1] = new indiObject() { indicName = "sma_vol5", numbegin = 5, numend = 30, gap = 5 };
            //myObjects2[2] = new indiObject() { indicName = "1", numbegin = 1, numend = 1, gap = 1 };

            var var1 = new[] { myObjects[0].indicName};//add the same number of indiobject, if you put 2 objects and set conk=2, you can get {A,B}, {B,A} 
            var var2 = new[] { myObjects2[0].indicName };

            foreach (IEnumerable<string> i in GetPermutations(var1, conk))
            {
                int[,] loopcount0 = new int[3, 3];
                loopcount0.Populate(1);
                int count0 = 0;

                foreach (string s in i)
                {
                    //Console.Write(s + " ");
                    if (s == myObjects[0].indicName)
                    {
                        loopcount0[count0, 0] = myObjects[0].numbegin;
                        loopcount0[count0, 1] = myObjects[0].numend;
                        loopcount0[count0, 2] = myObjects[0].gap;
                    }
                    else if (s == myObjects[1].indicName)
                    {
                        loopcount0[count0, 0] = myObjects[1].numbegin;
                        loopcount0[count0, 1] = myObjects[1].numend;
                        loopcount0[count0, 2] = myObjects[1].gap;
                    }
                    else
                    {
                        loopcount0[count0, 0] = myObjects[2].numbegin;
                        loopcount0[count0, 1] = myObjects[2].numend;
                        loopcount0[count0, 2] = myObjects[2].gap;
                    }
                    count0++;
                }
                //Console.WriteLine();
                for (int f1 = loopcount0[0, 0]; f1 <= loopcount0[0, 1]; f1 += loopcount0[0, 2])
                    for (int f2 = loopcount0[1, 0]; f2 <= loopcount0[1, 1]; f2 += loopcount0[1, 2])
                        for (int f3 = loopcount0[2, 0]; f3 <= loopcount0[2, 1]; f3 += loopcount0[2, 2])
                            foreach (IEnumerable<string> i2 in GetPermutations(var2, conr))
                            {
                                int[,] loopcount = new int[3, 3];
                                loopcount.Populate(1);
                                int count = 0;
                                //Console.WriteLine("sma_day5 " + f1 + "sma_day20" + f2 + "sma_day50" + f3);
                                foreach (string s in i2)
                                {
                                    //Console.WriteLine(s);
                                    if (s == myObjects2[0].indicName)
                                    {
                                        loopcount[count, 0] = myObjects2[0].numbegin;
                                        loopcount[count, 1] = myObjects2[0].numend;
                                        loopcount[count, 2] = myObjects2[0].gap;
                                    }
                                    else if (s == myObjects2[1].indicName)
                                    {
                                        loopcount[count, 0] = myObjects2[1].numbegin;
                                        loopcount[count, 1] = myObjects2[1].numend;
                                        loopcount[count, 2] = myObjects2[1].gap;
                                    }
                                    else
                                    {
                                        loopcount[count, 0] = myObjects2[2].numbegin;
                                        loopcount[count, 1] = myObjects2[2].numend;
                                        loopcount[count, 2] = myObjects2[2].gap;
                                    }
                                    count++;
                                }
                                for (int f4 = loopcount[0, 0]; f4 <= loopcount[0, 1]; f4 += loopcount[0, 2])
                                    for (int f5 = loopcount[1, 0]; f5 <= loopcount[1, 1]; f5 += loopcount[1, 2])
                                        for (int f6 = loopcount[2, 0]; f6 <= loopcount[2, 1]; f6 += loopcount[2, 2])
                                            for (int takeprofit = 80; takeprofit <= 160; takeprofit += 40)//5
                                                for (int exitloss = 60; exitloss <= 120; exitloss += 30)//4
                                                    for (int vcdp = 90; vcdp <= 144; vcdp += 9)//4
                                                        for (int vtime = 8; vtime <= 10; vtime += 1)//4
                                                        {
                                                            try
                                                            {
                                                                //add para loop below f6, like for (int jump = 100; jump <= 400; jump += 50) to add another loop
                                                                dict.Clear();
                                                                dict.Add("--takeprofit", takeprofit.ToString());
                                                                dict.Add("--exitloss", exitloss.ToString());
                                                                dict.Add("--vcdp", vcdp.ToString());
                                                                dict.Add("--vtime", vtime.ToString());

                                                                //Use the following para to skip process that has done
                                                                //if (countC >= 60)
                                                                tasks.Add(Task.Factory.StartNew(() => RunBacktrader(dict), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default));

                                                                //uncomment to show parameters 
                                                                //string str = "";
                                                                //foreach (KeyValuePair<string, string> kvp in dict)
                                                                //{
                                                                //    str += kvp.Key + "=" + kvp.Value;
                                                                //}
                                                                //Console.WriteLine(str + " " + countC++);

                                                                while (tasks.Where(t => t.Status == TaskStatus.Created || t.Status == TaskStatus.Running || t.Status == TaskStatus.WaitingToRun).Count() >= MultithreadThrottle)
                                                                {
                                                                    //Console.WriteLine("Task pool count {0}", tasks.Count);
                                                                    var whenAllTask = Task.WhenAll(tasks);//when all tasks finish, it return a task called whenAllTask
                                                                    whenAllTask.ContinueWith(t => {
                                                                        tasks.Remove(t);
                                                                        //Console.WriteLine(t.Id + " finished");
                                                                    },
                                                                    TaskContinuationOptions.OnlyOnRanToCompletion);
                                                                    ConfigurationManager.RefreshSection("appSettings");
                                                                    if (MultithreadThrottle != Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings.Get("MultithreadThrottle")))
                                                                    {
                                                                        MultithreadThrottle = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings.Get("MultithreadThrottle"));
                                                                        Console.WriteLine("Number of thread run in sync: " + MultithreadThrottle);
                                                                    }
                                                                }

                                                                //Console.WriteLine("Process ID " + countC++);
                                                                /*
                                                                tasksArray = tasks.Where(t => t != null).ToArray();
                                                                if (tasksArray.Length >= 10)
                                                                {
                                                                    Task.WaitAll(tasksArray);
                                                                    foreach (var t in tasksArray.ToList())
                                                                    {
                                                                        t.ContinueWith(completed =>
                                                                        {
                                                                            switch (completed.Status)
                                                                            {
                                                                                case TaskStatus.RanToCompletion:
                                                                                        tasks.Remove(t);//Index was out of range. Must be non-negative and less than the size of the collection.
                                                                                        break;
                                                                            }
                                                                        }, TaskScheduler.Default);
                                                                    }
                                                                    Console.WriteLine("All tasks finished " + DateTime.Now.ToString());
                                                                }
                                                                */
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                RecordLog(ex.Message);
                                                            }


                                                        }

                            }
                //Console.WriteLine(string.Join(" ", i));
            }
            Console.WriteLine("Optimization Compeleted");
            Console.Read();
        }

        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        private static void RunBacktrader(Dictionary<string, string> dict)
        {
            try
            {
                string paras = "";
                foreach (KeyValuePair<string, string> pp in dict)
                {
                    paras += pp.Key + "=" + pp.Value + " ";
                }

                Console.WriteLine("Tasks count " + processcount++);
                string scriptName = scriptpath + " " + paras;

                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(@pythonpath, @scriptName)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                RecordLog("5 " + ex.Message);
            }

        }

        private static void RecordLog(string message)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstr))
                {
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Parameters.Add(new SqlParameter("message", message));
                    connection.Open();
                    sqlcmd.Connection = connection;
                    sqlcmd.CommandText = "INSERT INTO [dbo].[optmizationLog] (ExecTime, Steps) VALUES (GETDATE(), CAST(@message as varchar(512)) )";
                    sqlcmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
