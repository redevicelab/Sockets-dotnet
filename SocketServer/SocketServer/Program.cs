using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;

namespace SocketServer
{
    class Program
    {
        static int port = 8005; // порт для приема входящих запросов
        static void Main(string[] args)
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);
                Console.Write("Введите папке для получания файла:");
                string path = $"{Console.ReadLine()}";
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных
                    MemoryStream ms = new MemoryStream();//Поток для получение имени файла и содержимого
                    ms.Position = 0;//Позиция откуда начать запись
                    do
                    {//В цикле читаем данные пока они есть и записываем в поток
                        bytes = handler.Receive(data);
                        ms.Write(data, 0, data.Length);
                    }
                    while (handler.Available > 0);
                    ms.Seek(0, SeekOrigin.Begin);//Тут указываем позицию откуда будем читать
                    var fileNameByte = new byte[100];//Переменная дла имени файла в байтах
                    ms.Read(fileNameByte, 0, fileNameByte.Length-1);//чттаем имя файла
                    string fileNameString = Encoding.ASCII.GetString(fileNameByte).Replace("\0", string.Empty);//переводим имя файла из байт в строку
                    ms.Position = 100;//указывем позицию откуда будем читать содержимое файла
                    byte[] content = new byte[ms.Length-fileNameByte.Length];//Делаем буфер для хранения содержимого файла
                    ms.Read(content, 0, (int)ms.Length - fileNameByte.Length);//Читаем в буфер содержимое файла
                    File.WriteAllBytes(path + "\\" +fileNameString, content);//Тут уже сохраняем файл в папку 
                    Console.WriteLine("Файл получен и сохранен!!!");
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}