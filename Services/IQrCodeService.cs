namespace StudentAdminPortal.Services;

public interface IQrCodeService
{
    string GenerateQrCodeBase64(string payload);
}
