using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace co2monitor {
	public class CO2device : ICO2device {
		public static UsbDevice usbDevice;

		public int init() {
			UsbRegDeviceList allDevices = UsbDevice.AllDevices;
			throw new System.NotImplementedException();
		}

		public void exit() {
			throw new System.NotImplementedException();
		}

		UsbDevice ICO2device.openDevice() {
			return openDevice();
		}

		UsbDevice ICO2device.openPathDevice(string path) {
			return openPathDevice(path);
		}

		public void closeDevice(UsbDevice device) {
			throw new System.NotImplementedException();
		}

		public UsbDevice openDevice() {
			throw new System.NotImplementedException();
		}

		public UsbDevice openPathDevice(string path) {
			throw new System.NotImplementedException();
		}

		public void closeDevice(UsbDevice device) {
			throw new System.NotImplementedException();
		}

		public int readData() {
			throw new System.NotImplementedException();
		}
	}
}