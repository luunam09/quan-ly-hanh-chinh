using QRCoder;

namespace StudentAdminPortal.Services;

public class QrCodeService : IQrCodeService
{
    public string GenerateQrCodeBase64(string payload)
    {
        try
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
        }
        catch
        {
            return string.Empty;
        }
    }
}
