using System;
using System.Collections.ObjectModel;
using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;

namespace co2monitor {
    class Program {
        private static int vendor = 0x04d9;
        private static int productID = 0xa052;
        private static Co2DeviceHandler co2DeviceHandler;
        private static DataProcessor dataProcessor;

        private static void deviceLoop(UsbDevice usbDevice) {
            //the device won't send anything before receiving this packet
            //var report = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };   
            var key = new byte[] { 0xc4, 0xc6, 0xc0, 0x92, 0x40, 0x23, 0xdc, 0x96 };
            byte[] report = key;
            int bytesWritten = co2DeviceHandler.sendReport(usbDevice, report);
            if (bytesWritten != report.Length) {
                Console.WriteLine(value: "Unable to send report");
                return;
            }
            
            while (true) {
                byte[] dataBuffer;

                int bytesLength = co2DeviceHandler.readData(usbDevice, out dataBuffer);
                if (bytesLength == 0) {
                    Console.WriteLine(value: "Unable to read datar");
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
            UsbRegDeviceList allDevices = UsbDevice.AllDevices;
            foreach (UsbRegistry usbRegistry in allDevices) {
                UsbDevice MyUsbDevice;
                if (usbRegistry.Open(out MyUsbDevice)) {
                    Console.WriteLine(MyUsbDevice.Info.ToString());
                    for (int iConfig = 0; iConfig < MyUsbDevice.Configs.Count; iConfig++) {
                        UsbConfigInfo configInfo = MyUsbDevice.Configs[iConfig];
                        Console.WriteLine(configInfo.ToString());

                        ReadOnlyCollection<UsbInterfaceInfo> interfaceList = configInfo.InterfaceInfoList;
                        for (int iInterface = 0; iInterface < interfaceList.Count; iInterface++) {
                            UsbInterfaceInfo interfaceInfo = interfaceList[iInterface];
                            Console.WriteLine(interfaceInfo.ToString());

                            ReadOnlyCollection<UsbEndpointInfo> endpointList = interfaceInfo.EndpointInfoList;
                            for (int iEndpoint = 0; iEndpoint < endpointList.Count; iEndpoint++) {
                                Console.WriteLine(endpointList[iEndpoint].ToString());
                            }
                        }
                    }
                }
            }
            int unused = args.Length;
            co2DeviceHandler = new Co2DeviceHandler();
            dataProcessor = new DataProcessor();

            loop();
            
            co2DeviceHandler.exit();
        }
    }
}
