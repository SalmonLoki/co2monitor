using System;
using LibUsbDotNet;

namespace co2monitor {
    class Program {
        private static int vendor;
        private static int productID;
        private static Co2DeviceHandler co2DeviceHandler;
        private static DataProcessor dataProcessor;

        private static void deviceLoop(UsbDevice usbDevice) {
            while (true) {
                byte[] dataBuffer;
                byte[] data;
                
                int bytesLength = co2DeviceHandler.readData(usbDevice, out dataBuffer);
                if (bytesLength == 0) {
                    Console.WriteLine(value: "Unable to connect CO2 monitor");
                    break;
                }

                if (!dataProcessor.decryptData(ref dataBuffer, out data)) {
                    Console.WriteLine(value: "Decryption failed");
                    continue;
                }

                if (!dataProcessor.checkEndOfMessage(ref data, bytesLength)) {
                    Console.WriteLine(value: "Unexpected data from device");
                    continue;
                }

                if (!dataProcessor.checkCheckSum(ref data, bytesLength)) {
                    Console.WriteLine(value: "checksum error");
                    continue;
                }
                
                dataProcessor.dataProcessing(ref data, bytesLength);
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
