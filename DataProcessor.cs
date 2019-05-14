namespace co2monitor {
	public class DataProcessor : IDataProcessor {
		public bool decryptData(ref byte[] dataBuffer, out byte[] data) {
			
		}
		
		public bool checkEndOfMessage(ref byte[] data, int bytesLength) {
			return data[4] == 0x0d;
		}
		
		public bool checkCheckSum(ref byte[] data, int bytesLength) {
			return data[0] + data[1] + data[2] == data[3];
		}

		public void dataProcessing(ref byte[] data, int bytesLength) {
			switch ((int)data[0]) {
				case 0x_50d:

					break;	
				
				case 0x_42d:
					
					break;
			}
		}		
	}
}