using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace SyncServer
{
    public class SynchronousSocketListener
    {

        public static int TAM = 1024;

        // Incoming data from the client.  
        public static string data = null;


        public static void StartListening()
        {
            //Aux Data
            int amountOfZeros = 0;
            int amountOfones = 0;
            int amountOftwos = 0;

            // Data buffer for incoming data.  
            byte[] bytes = new Byte[TAM];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = getLocalIpAddress();//MAC OS
            IPEndPoint localEndPoint1 = new IPEndPoint(ipAddress, 11000);
            IPEndPoint localEndPoint2 = new IPEndPoint(ipAddress, 11001);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

            Socket listener2 = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                listener.Bind(localEndPoint1);
                listener.Listen(10);

                
                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    while(true){
                        data = null;
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        // An incoming connection needs to be processed.  
                        while (bytesRec == TAM)
                        {
                            bytesRec = handler.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        }

                        // Show the data on the console.  
                        Console.WriteLine("Text received : {0}", data);

                        // Echo the data back to the client.  
                        byte[] msg = Encoding.ASCII.GetBytes(data);

                        handler.Send(msg);

                        
                        if(data != "0" && data != "1" && data != "2"){
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            break;
                        }

                        switch(data){
                            case "0":
                                amountOfZeros++;
                                break;
                            case "1":
                                amountOfones++;
                                break;
                            case "2":
                                amountOftwos++;
                                break;
                        }
                    }
                    Console.WriteLine("[0]:"+ amountOfZeros +"\t[1]:"+ amountOfones +"\t[2]:"+amountOftwos);
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nProgram ended. Press ENTER to continue...");
            Console.Read();

        }

        static IPAddress getLocalIpAddress()
        {
            IPAddress ipAddress = null;
            try
            {
                foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ipAddress = addrInfo.Address;
                            }
                        }
                    }
                }
                if (ipAddress == null)
                {
                    IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
                    ipAddress = ipHostInfo.AddressList[0];
                }
            }
            catch (Exception) { }
            return ipAddress;
        }

        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }
    }
}
