using MediaRentals.DTOs;
using System;
using System.IO;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using System.Linq;

namespace ProtoBufDeserializeTest
{
    class Program
    {
        static void Main(string[] args)
        {


            DvdRental msg1 = new DvdRental() { DvdTitle = "Mission impossible", DvdGenre = DvdRental.Types.MovieGenre.Action, RentalDateTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow) };
            BookRental msg2 = new BookRental() { BookAuthor = "Alexandre Dumas", BookTitle = "The Three Musketeers", ISBN13 = "1234-45645546", PrintLength = 500, RentalDateTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow - TimeSpan.FromDays(2)) };
            DvdRental msg3 = new DvdRental() { DvdTitle = "Avengers", DvdGenre = DvdRental.Types.MovieGenre.Adventure, RentalDateTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow - TimeSpan.FromDays(3)) };



            using (MemoryStream stream = new MemoryStream())
            {

                //writing messages to stream
                WriteToStream(stream, msg1);
                WriteToStream(stream, msg2);
                WriteToStream(stream, msg3);

                //position stream to beginning
                stream.Seek(0, SeekOrigin.Begin);

                Console.WriteLine();

                //reading messages to stream
                while (stream.Position < stream.Length)
                {
                    var msg = ReadFromStream(stream);
                    Console.WriteLine($"{msg.ToString()}");
                }
            }

        }


        public static void WriteToStream(Stream outputStream, IMessage message)
        {
            MessageDescriptor stateMsgDescriptor = message.Descriptor;

            using (CodedOutputStream codedOutStr = new CodedOutputStream(outputStream, true))
            {
                //Console.WriteLine($"Initial position is {codedOutStr.Position}");
                //Console.WriteLine($"Writing name of type: {stateMsgDescriptor.FullName}");
                codedOutStr.WriteString(stateMsgDescriptor.FullName);
                //Console.WriteLine($"Position is now {codedOutStr.Position}");

                int size = message.CalculateSize();
                //Console.WriteLine($"Writing size of message: {size}");
                codedOutStr.WriteLength(size);
                //Console.WriteLine($"Position is now {codedOutStr.Position}");

                Console.WriteLine($"###Writing message: {message.ToString()}");
                message.WriteTo(codedOutStr);
                Console.WriteLine($"Position is now {codedOutStr.Position}");
                
            }
        }

        public static IMessage ReadFromStream(Stream inputStream)
        {

            

            using (CodedInputStream codedInStream = new CodedInputStream(inputStream, true))
            {
                Console.WriteLine($"Initial position is {codedInStream.Position}");

                Console.WriteLine($"Reading name of type");
                string s_fullName = codedInStream.ReadString();
                Console.WriteLine($"Position is now {codedInStream.Position}");
                Console.WriteLine($"Name of type just red = {s_fullName}");

                MessageDescriptor  matchingMsgDesc = TestProtoReflection.Descriptor.MessageTypes.SingleOrDefault(md => md.FullName == s_fullName);

                //reading size of message
                int sz = codedInStream.ReadLength();


                SubStream str1 = new SubStream(inputStream, codedInStream.Position, sz, SeekOrigin.Begin);
                IMessage msg = matchingMsgDesc.Parser.ParseFrom(str1);


                ////Another way to do it:
                //##################################
                //stupid hack!!! As the position in initial stream is lost once the CodedInputStream starts to read
                //inputStream.Position = codedInStream.Position;

                //////Going to read message at position
                //byte[] buffer = new byte[sz];
                //int ss = inputStream.Read(buffer, (int)codedInStream.Position, sz);
                //IMessage msg = matchingMsgDesc.Parser.ParseFrom(buffer);

                return msg;
            }

        }



        //public static void WriteToStream(Stream outputStream, IMessage message)
        //{
        //    MessageDescriptor stateMsgDescriptor = message.Descriptor;

        //    using (CodedOutputStream codedOutStr = new CodedOutputStream(outputStream, true))
        //    {
        //        codedOutStr.WriteString(stateMsgDescriptor.FullName);

        //    }
        //    message.WriteDelimitedTo(outputStream);
        //}

        //public static IMessage ReadFromStream(Stream inputStream)
        //{
        //    //ses hack below
        //    long lastPos = inputStream.Position;


        //    using (CodedInputStream codedInStream = new CodedInputStream(inputStream, true))
        //    {
        //        string s_fullName = codedInStream.ReadString();

        //        int sz = codedInStream.ReadInt32();

        //        MessageDescriptor matchingMsgDesc = TestProtoReflection.Descriptor.MessageTypes.SingleOrDefault(md => md.FullName == s_fullName);

        //        //this is just a hack as the stream is being repositionned when codedInStream is closed!!
        //        inputStream.Position = codedInStream.Position;

        //        IMessage msg = matchingMsgDesc.Parser.ParseDelimitedFrom(inputStream);

        //        return msg;
        //    }
        //}






    }
}
