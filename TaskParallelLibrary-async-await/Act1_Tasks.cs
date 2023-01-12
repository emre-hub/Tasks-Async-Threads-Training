namespace Threading
{
    public class Act1_Tasks
    {
        public static void Main(string[] args)
        {
            //Metotlar: 

            //GetGoogleSourceCode thread'i, scope'u içerisinde çalıştırdığı t thread'ini sonuç olarak döndürür.
            Task<string> GetGoogleSourceCodeAsyncI()
            {
                var httpClient = new HttpClient();
                var t = Task.Run(() =>
                {
                    /*
                     * Bu Thread kendisi bir CPU-Bound operation iken, 
                     * içerisinde çalıştırdığı httpClient.GetStringAsync metodu I/O-Bound operation'dur.

                       Taskler Non-Blocking operation olarak geçiyordu, biz bir I/O-bound operation çalıştırıp,
                       ardından .Result yazarak operasyonun sonucunu istediğimiz için, operasyonun Non-Blocking özelliğini sonlandırmış olduk.
                       İşlem, içinde bulunduğu Thread'i, Result'ı bekleyerek bitirir ve bloke etmiş olur.
                       Artık bu aşamada program aldığı Result'ı bir sonraki kod satırında kullanabilir.
                       Örneğimizde bloke olan thread, var t olarak tanımlayıp Task.Run şeklinde çalıştırdığımız Thread'dir.
                    */
                    return httpClient.GetStringAsync("http://www.google.com").Result;
                });
                return t;
            }

            /*
            GetGoogleSourceCodeAsyncI metodu ile aynı işlemi yapar
            fakat bu Task metodu async keywordu ile tanımlıdır.
            async ve await keywordleri her zaman bir arada kullanılır. 
            async keywordunun tek özelliği, await'i kullanabilmemizi sağlamasıdır.

            await beklemek demektir, GetGoogleSourceCodeAsyncII metodundaki kullanımında await keywordu,
            httpClient.GetStringAsync metodunu bekliyor demektir.

            Buradaki bekleme işlemi, metodu bloklama anlamına gelmez. 
            return etmek için metodun çalışmasının bitmesini beklemediği anlamına gelir.
            Metot, çalışmaya devam ederken return edilir.
            */
            async Task<string> GetGoogleSourceCodeAsyncII()
            {
                var httpClient = new HttpClient();

                //kodlar main metotta yukarıdan aşağı akmaya-çalışmaya devam edecek
                //ve GetGoogleSourceCodeAsyncII taskinde await işlemindeki metot işlemini
                //bitirdiğinde result değişkenine değerini yazacak demiş oluyoruz... 
                //elbette tüm bunlar olurken await aşamasındaki metodun işini bitirmesini main metot beklemeyecek. async.

                //ANCAK, Console.WriteLine(result); kodu elbette result değeri dönmeden çalışmaz.
                //await işleminden sonra aynı Thread içerisindeki metotlar, sırasını bekler. ContinueWith... şeklinde çalışır.
                var result = await httpClient.GetStringAsync("http://www.google.com"); 
                Console.WriteLine(result);
                return result;
            }


            //Açıklamalar.
            /*               
            Kod :                           
            Task.Run(() => Console.WriteLine("Merhaba Dünya."));
            Console.WriteLine("Merhaba Mars");
            ---------------------------------------------------------------
            Yukarıdaki kodun çıktısı sabit değildir. 
            Bazen Merhaba Dünya -> Merhaba Mars sırası şeklinde çıktı verirken
            bazen de önce Merhaba Mars 'ı  sonra Merhaba Dünya 'yı ekrana basar.
            
            Yani bu kodlar Asynchronous çalışır, 
            bir önceki satırda yazılan kod, kendinden sonraki satırda yazılmış kodu bekletmez. 
            Non-Blocking Operation olarak da adlandırılır.Threadler'e benzer.

            Task.Run şeklinde çalıştırıp kullandığımız metot, ThreadPool'dan alınıp oluşturulan bir Thread'dir 
            ve kendisine verilen işi kendi zamanında ayrıca yapmakla görevlidir.

            Task.Run şeklinde çalıştırdığımız metot, Thread'lerden farklı olarak bize Task türünde bir nesne döndürür.
            Yukarıdaki Task kodunu eğer şu şekilde yazarsak : 
                Task t = Task.Run(() => Console.WriteLine("Merhaba Dünya."));
            Burada tanımladığımız t nesnesinin üzerinden, Task'a ait olan IsCanceled, IsCompleted vb. metotlara erişebiliriz.
            Task library, Task objesi vasıtası ile devam etmekte olan işlemler üzerinde kontrol yapabilmemize olanak sağlar.
                    



            CPU-Bound Operations - I/O-BoundOperations

            CPU-Bound Operations : 
                CPU üzerindeki bir iş yükü. 
                Bir rakamın faktöriyelini hesaplamak vs. gibi işlemciyi kullanan her işlem CPU-Bound operation'dur.
                Zaman zaman bu operasyonların iş yükü çok büyük olabilir.
            
            I/O Bound Operations : 
                Bir Network veya Database yanıtı, bir File'dan dosya okumak, 
                bir Folder'dan dosyaları getirmek vb. gibi işlemler I/O-Bound operationlara örnektir.
                Bir I/O aracından yanıt alındığı için bir latency/bekleme süresi söz konusudur.
                
            Bazı işlemleri async olarak yapmak gerekir. 
            Örneğin internetten bir dosya indirdiğinizde bilgisayarda aynı anda başka işlemleri de yapabilmek istersiniz.
            Bir GUI application hazırladınız, iyi bir kullanıcı deneyimi için uygulama bir işlem yaparken, 
            kullanıcının uygulama üzerinde hareket edebilmeye devam etmesi için 
            örneğin pencereyi büyütüp küçültebilmesi veya çalışan bir işlem varken butona basıp durdurabilmesi önemlidir.

            İşlemlerin tamamı main threadde senkron olarak, 
            kodların yukarıdan aşağı akışına göre gerçekleşirse, 
            uygulama pek çok kısımda unresponsive(yanıtsız) kalacaktır.

            Tüm bu işlemlerin gerçekleşebilmesi için uygulamaların asenkron biçimde yazılabilmesi gerekmekte.




            EXTRA NOTLAR

            Task tipi içinde IsCompleted gibi property'lerin yanında Status diye bir property var.
            Bu property sayesinde daha detaylı durum bilgisi alabilirsiniz. 
            Mesela Running, Created, RanToCompletion, etc. Her task mutlaka çalışıyor degildir.
            
            Eğer task.Start() methodu çağırılmamış ise, yada Task.Run() veya 
            Task.Factory.StartNew() ile başlatılmamışsa, 
            o zaman elinizdeki Task'in çalışmıyor olmasi muhtemeldir. 

            Dolayısıyla, Task.Result diye sonuç almaya çalıştığınızda bitmeyen bir bekleme sürecine gireceğinizden
            kodunuz sorun oluşturacaktır. Onun için Task.State == TaskStatus.Running ile check etmek sureti ile
            durumu hakkında bilgi alabilir, ve gerekirse Task.Start() demek suretiyle çalışmayan bir Task'i başlatabilirsiniz.
            */

            //Örnek 1 : 
            /*
            
                Task t = Task.Run(() => 
                {
                    Console.WriteLine("Merhaba Dünya.");
                    Task.Delay(1000).Wait();
                    Console.WriteLine("Merhaba Venüs.");
                });
                Console.WriteLine("I : t taskı tamamlandı mı ? : " + t.IsCompleted.ToString());
                Console.WriteLine("Merhaba Mars");
                Thread.Sleep(1000); //Task.Delay(1000).Wait(); //Mars çıktısından sonra main metot 1 saniye bekler.
                Console.WriteLine("II : t taskı tamamlandı mı ? : " + t.IsCompleted.ToString());
            */

            //Örnek 2 : 
            /*
               //Aşağıdaki kodun anlamı şu :
               //GetGoogleSourceCode taskinde indirme işlemini yap
               //ve indirme tamamlanana kadar bekle, indirdiğin sonucu kullan :

                var t = GetGoogleSourceCodeAsyncI();
                Console.WriteLine(t.Result);
                Console.WriteLine("\n\n\n\n-----------------------------Finish-----------------------------");
            */

            //Örnek 3 : 
            var t = GetGoogleSourceCodeAsyncII();
            Console.WriteLine("\n\n\n\n-----------------------------Finish-----------------------------");
            Console.ReadLine(); //önce finishi ekrana basarsa console uygulamasını kapatmaması için ReadLine ekledim



        }
    }
}