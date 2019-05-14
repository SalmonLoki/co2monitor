using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace co2monitor {
	public class Co2DeviceHandler : ICo2DeviceHandler {	
		//null or UsbDevice
		public UsbDevice openDevice(int vendor, int productID) {
			var usbFinder = new UsbDeviceFinder(vendor, productID);
			UsbDevice newUsbDevice = UsbDevice.OpenUsbDevice(usbFinder);
			return newUsbDevice;
		}

		public void setIfWhole(UsbDevice usbDevice) {
			//should setup whole usb-devices (LibUsbDevice, MonoUsbDevice,...)
			//Partial or interfaces of devices are ok, driver is handling device configuration (WinUsbDevice,...)
			var wholeUsbDevice = usbDevice as IUsbDevice;
			wholeUsbDevice?.SetConfiguration(config: 1);
			wholeUsbDevice?.ClaimInterface(interfaceID: 0);
		}

		//0 or more bytes read, readBuffer changed
		public int readData(UsbDevice usbDevice, out byte[] readBuffer) {
			using (UsbEndpointReader reader = usbDevice.OpenEndpointReader(ReadEndpointID.Ep01)) {
				readBuffer = new byte[8];
				reader.Read(readBuffer, timeout: 5000, out int bytesLength);					
				return bytesLength;
			}							
		}
		
		public void closeDevice(UsbDevice usbDevice) {
			usbDevice.Close();
		}

		public void exit() {
			UsbDevice.Exit();
		}
	}
}