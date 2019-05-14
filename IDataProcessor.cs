namespace co2monitor {
	public interface IDataProcessor {
		bool decryptData(ref byte[] dataBuffer, out byte[] data);
		
		bool checkEndOfMessage(ref byte[] data, int bytesLength);

		bool checkCheckSum(ref byte[] data, int bytesLength);

		void dataProcessing(ref byte[] data, int bytesLength);
	}
}