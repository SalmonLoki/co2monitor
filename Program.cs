using System;
using LibUsbDotNet;

namespace co2monitor {
    class Program {
        private static int vendor = 0x04d9;
        private static int productID = 0xa052;
        private static Co2DeviceHandler co2DeviceHandler;
        private static DataProcessor dataProcessor;

        private static void deviceLoop(UsbDevice usbDevice) {
            //the device won't send anything before receiving this packet
            var report = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };           
            int bytesWritten = co2DeviceHandler.sendReport(usbDevice, report);
            if (bytesWritten != report.Length) {
                Console.WriteLine(value: "Unable to send first report");
                return;
            }
            
            while (true) {
                byte[] dataBuffer;
                byte[] data;
                
                int bytesLength = co2DeviceHandler.readData(usbDevice, out dataBuffer);
                if (bytesLength == 0) {
                    Console.WriteLine(value: "Unable to connect CO2 monitor");
                    break;
                }
                if (bytesLength != 8) {
                    Console.WriteLine(value: "transferred amount of bytes != expected bytes amount");
                    break;
                }

                if (!dataProcessor.decryptData(ref dataBuffer, out data)) {
                    Console.WriteLine(value: "Decryption failed");
                    continue;
                }

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
