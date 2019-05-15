using System;
using LibUsbDotNet;

namespace co2monitor {
    class Program {
        private static int vendor = 0x1687; //0x04d9;
        private static int productID = 0x6213; //0xa052;
        private static Co2DeviceHandler co2DeviceHandler;
        private static DataProcessor dataProcessor;

        private static void deviceLoop(UsbDevice usbDevice) {
            //the device won't send anything before receiving this packet 
            //var key = new byte[] { 0xc4, 0xc6, 0xc0, 0x92, 0x40, 0x23, 0xdc, 0x96 };
           
            var key = new byte[] { 0xc4, 0xc6, 0xc0, 0x92, 0x40, 0x23, 0xdc, 0x96 };
            
            int bytesWritten = co2DeviceHandler.sendReport(usbDevice, key);
            if (bytesWritten != key.Length) {
                Console.WriteLine(value: "Unable to send report: expected bytes amount = " + key.Length + ", written bytes amount = " + bytesWritten);
                return;
            }
            
            while (true) {
                byte[] dataBuffer;

                int bytesLength = co2DeviceHandler.readData(usbDevice, out dataBuffer);
                if (bytesLength == 0) {
                    Console.WriteLine(value: "Unable to read data");
                    break;
                }
                if (bytesLength != 8) {
                    Console.WriteLine(value: "transferred amount of bytes != expected bytes amount");
                    break;
                }
                
                int[] data = dataProcessor.decryptData(key, ref dataBuffer);

                if (!dataProcessor.checkEndOfMessage(ref data)) {
                    Console.WriteLine(value: "Unexpected data from device");
                    continue;
                }

                if (!dataProcessor.checkCheckSum(ref data)) {
                    Console.WriteLine(value: "checksum error");
                    continue;
                }
                
                dataProcessor.dataProcessing(ref data);
            }
        }

        private static void loop() {
            while (true) {
                UsbDevice usbDevice = co2DeviceHandler.openDevice(vendor, productID);
                if (usbDevice == null) {
                    Console.WriteLine(value: "Unable to connect CO2 monitor");
                    break;
                }
                co2DeviceHandler.setIfWhole(usbDevice);
                deviceLoop(usbDevice);
                co2DeviceHandler.closeDevice(usbDevice);            
            }
        }

        static void Main(string[] args) {          
            int unused = args.Length;
            co2DeviceHandler = new Co2DeviceHandler();
            dataProcessor = new DataProcessor();

            loop();
            
            co2DeviceHandler.exit();
        }
    }
}
