using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SocketClient
{
    class Program
    {


        private static byte[] GetContent(string fileName)//Метод для перевода содержимого файла в байты тут все просто
        {
            byte[] Content = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                Content = new byte[fs.Length];
                fs.Read(Content, 0, (int)fs.Length);
            }
            return Content;
        }

        // адрес и порт сервера, к которому будем подключаться
        static int port = 8005; // порт сервера
        static string address = "127.0.0.1"; // адрес сервера
        static void Main(string[] args)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                string path; //Путь для указания где лежит файл
                Console.Write("Введите адрес файла: ");
                path = $@"{Console.ReadLine()}";//Тут пользователь вводит путь


                MemoryStream ms = new MemoryStream();//Делаем поток для записи в него имя файла и содержимого
                var bytesContent = GetContent(path);//Тут получаем содержимое файла в байтах
                var bytesFileName = new byte[100];//Задаем переменную для хранения имени файла
                byte[] dataSend = null;//Массив байт для отправки файла и его названия
                var bFIleName = Encoding.UTF8.GetBytes(Path.GetFileName(path));//Тут получаем имя файла в байтах
                ms.Write(bFIleName, 0, bFIleName.Length);//Записываем в поток имя файла
                ms.Position = bytesFileName.Length;//Смещаем положение в потоке на длину названия файла
                ms.Write(bytesContent, 0, bytesContent.Length);//Записываем в поток содержимае файла
                dataSend = ms.ToArray();//записываем все в массив для отпрвавки

                socket.Send(dataSend);//отправляем все на сервер
                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Console.WriteLine("Файл отправлен поток закрыт");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}