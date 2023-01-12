namespace Threadss
{
    public static class Program
    {
        static void Main(string[] args)
        {
            //Thread
            /*
             * Thread a = new Thread();
                Bu tür bir Thread kullanımı artık eski moda sayılır, 
                Thread tipinde veri türü kullanmaktansa artık Task türü yaygın olarak kullanılıyor. 
                Bu projenin Threading uygulamasındaki Act1 classında örnekleriyle anlattım.
             */
            //Thread a = new Thread(() => 
            //{
            //    Thread.Sleep(500);
            //    Console.WriteLine("Hello World!");
            //    Console.WriteLine("a.IsBackground ?  :  " + Thread.CurrentThread.IsBackground);
            //});
            Thread a = new Thread(DoSomething);
            a.Start();
            a.Join(); //Start komutuyla başlattığımız a threadi Start() ile başladıktan sonra, işini bitirene dek main threadi durdurur.
            Console.WriteLine("Merhaba Dünya!");
            Console.WriteLine("Is 'a' thread alive? " + a.IsAlive);
        }
        public static void DoSomething()
        {
            Thread.Sleep(1000);
            Console.WriteLine("Thread is doing something.");        
        }
    }
}

