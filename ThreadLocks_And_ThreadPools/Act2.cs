using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadLocks_And_ThreadPools
{
    public class Act2
    {
        static void ThreadProc(Object stateInfo) {
            Console.WriteLine("ThreadProc method ThreadPool Thread Id : " + Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("ThreadProc method Is Background Thread ?  : " + Thread.CurrentThread.IsBackground);
            Console.WriteLine("Hello from the ThreadPool");
        }
        public static void Main(string[] args)
        {
            //Race condition - Yarış durumu
            //Critical section
            //scheduler, scheduling
            //context switch
            //thread safe
            //lock keyword
            //ThreadPool
            //Background-ForeGround Threads
            /*
                Birden fazla Thread, paylaşılan aynı veriye ulaşmaya çalışıyorsa bu duruma Race condition denir. Yani yarış durumu.
                Yarış durumunda olan en az iki Thread aynı veri üzerinde işlem yapacak, veriyi değiştirecek olabilir.
                Birden fazla Thread'in ulaşmaya çalıştığı alanlara "critical section" adı verilir.

                Aşağıda oluşturduğumuz Account classından örnek verelim, diyelim ki 1 ve 2 numaralı threadlerimiz var.
                1 numaralı Thread if(amount>balance) satırındaki if sorgusunu geçti ve balance-=amount; satırındaki kodu çalıştırmadan önce. 
                işletim sistemimizin scheduler'ı threadleri kendince sıralarken, 1 numaralı threadin işlemini durdurmaya karar verdi ve o anda 
                "context switch" dedigimiz işlemi gerçekleştirip 2 numaralı Threade geçiş yaptı.
                Bu durumda 2 numaralı Thread'in aynı veriyi kullanarak balance-=amount işlemini gerçekleştirdiğini varsayalım.
                Artık 1 numaralı Thread devam ettiğinde, yetkisi olmayan bir işlemi yapabilir hale gelecektir. Elbette bunun olmasını istemiyoruz.         
             

                Thread safe : Birden fazla Thread tarafından güvenli şekilde çalıştırılabilir. 


                lock keyword
                Bir metodu Thread safe yapmanın birden fazla yöntemi vardır.
                Ben lock keywordunu ve kullanımını anlatacağım.

                lock keywordunun kullanıldığı kod blokları anahtar-kilit şeklinde bir yapıya sahiptir. 
                Anahtar nesne olarak herhangi bir nesneyi kullanabilir.
                lock keywordun kullanabileceği nesnenin instance edilmiş ve bellekte yer tutuyor olması gereklidir.
                Bu projedeki Account sınıfında thisLock şeklinde isimlendirilmiş nesne, 
                kilit görevi görecek ve new() diyerek instanceini oluşturduğum bir nesnedir.
                lock keywordunun kullanıldığı kod bloğundaki işlem bitene kadar, 
                o kod bloğunu çalıştıran Thread için kilitler ve başka bir Thread'in kullanımına izin vermez.  
                lock keyworduyle açılan kod bloğundaki işlem bittikten sonra başka bir Threadin kullanımı için anahtarı serbest bırakır. 
                kod bloğunu kullanacak Threadler queue veriyapısıyla işlem için sıraya girer. Bu biraz performans kaybına sebep olabilir.

                ThreadPool
                Thread oluşturmak ve var olan Threadi sonlandırmak maliyetli bir işlemdir. 
                Bu işlemleri çok düşük bir maliyetle yapmamıza yardımcı olan yapı ThreadPool'dur.
                ThreadPool performans açısından stabil ve kullanım açısından daha güvenli bir yapıdır. 
                ThreadPool kaç Thread oluşturacağına karar verirken bilgisayarın CPU ve Core sayısına bakar 
                ve kaç Thread'i gerçek zamanlı ve uyumlu olarak çalıştırabileceğine karar verir.
                
                Aşağıdaki kod örneğinde ThreadPool.QueueUserWorkItem metodu "Delegate" veritipinde bir parametre alır.
                Aldığı parametre ThreadPool'a eklenen bir Thread'dir.

                Background-ForeGround Threads
                Thread'ler foreground ve background şeklinde olmak üzere ikiye ayrılır.
                Bir proses içerisindeki tüm foreground Threadler işlerini bitirip kapanırlarsa,
                o an çalışmakta olan tüm background threadler de sonlandırılır.
                ThreadPool'da çalışan threadler varsayılan olarak background threaddir. Ama kendimiz thread oluşturursak, threadi foregrounda çekebiliriz.  
            
                Foreground Thread'ler çalıştıkları sürece, sadece bir işlemi kalsa bile thread bellekte yer tutmaya devam eder, kapanmaz. 
                Bu background threadler için geçerli değildir, çalışan bir foreground thread kalmadıysa, aktif background thread de kapanır.
                O nedenle , yeni bir foreground t hread oluşturmak çok dikkatlice verilmesi gereken bir karardır.
                Uygulamanın terminate olması gerektiği yerde, çalışmaya devam etmesi gibi durumlarla karşılaşılabilir.
             */

            //Aşağıdaki kod mantıken, synchronized bir kod olsaydı çıktı sırasıyla şöyle olurdu :
            //Hello from the ThreadPool -> Main thread does some work, then sleeps. -> Main thread exists.
            Console.WriteLine("Main Thread Id : " + Thread.CurrentThread.ManagedThreadId);

            Thread t = new Thread(ThreadProc);
            t.IsBackground = false;
            t.Start();
            Console.WriteLine("t is alive? : " + t.IsAlive);
            
            //ThreadPool.QueueUserWorkItem(ThreadProc);
            Console.WriteLine("Main thread does some work, then sleeps.");
            Thread.Sleep(1000);
            Console.WriteLine("Main thread exists.");


        }
    }


class Account
    {
        decimal balance; //hesaptaki para
        private Object thisLock = new Object();
        
        public void Withdraw(decimal amount) //para çekme fonksiyonu, çekilecek para miktarını parametre(amount) olarak alır
        {
            lock(thisLock)
            {
                if (amount > balance) //çekilecek miktar hesaptaki paradan fazla ise
                {
                    throw new Exception("Insufficient funds. - Yetersiz bakiye.");  
                }
                balance -= amount;    
            }
        }
    }
}
