using System;
using System.Threading.Tasks;
using ThaiNationalIDCard.NET;
using ThaiNationalIDCard.NET.Models;

namespace ProjectManagement.Infrastructure
{
    public class SmartCardService
    {
        public async Task<SmartCardData?> GetCardDataAsync()
        {
            return await Task.Run(() => GetCardData());
        }

        public SmartCardData? GetCardData()
        {
            try
            {
                Console.WriteLine("Starting smart card read...");
                
                var cardReader = new ThaiNationalIDCardReader();
                var personalPhoto = cardReader.GetPersonalPhoto();
                
                if (personalPhoto != null)
                {
                    Console.WriteLine("Smart card read successful");
                    Console.WriteLine($"Citizen ID: {personalPhoto.CitizenID}");
                    
                    // Build full address from components
                    string fullAddress = BuildFullAddress(personalPhoto.AddressInfo);
                    
                    return new SmartCardData
                    {
                        CitizenId = personalPhoto.CitizenID ?? string.Empty,
                        TitleNameTh = personalPhoto.ThaiPersonalInfo?.Prefix ?? string.Empty,
                        FirstNameTh = personalPhoto.ThaiPersonalInfo?.FirstName ?? string.Empty,
                        LastNameTh = personalPhoto.ThaiPersonalInfo?.LastName ?? string.Empty,
                        TitleNameEn = personalPhoto.EnglishPersonalInfo?.Prefix ?? string.Empty,
                        FirstNameEn = personalPhoto.EnglishPersonalInfo?.FirstName ?? string.Empty,
                        LastNameEn = personalPhoto.EnglishPersonalInfo?.LastName ?? string.Empty,
                        BirthDate = personalPhoto.DateOfBirth.ToString("yyyy-MM-dd"),
                        Address = fullAddress,
                        IssueDate = personalPhoto.IssueDate.ToString("yyyy-MM-dd"),
                        ExpireDate = personalPhoto.ExpireDate.ToString("yyyy-MM-dd"),
                        Gender = personalPhoto.Sex ?? string.Empty,
                        Issuer = personalPhoto.Issuer ?? string.Empty,
                        PhotoBase64 = personalPhoto.Photo ?? string.Empty
                    };
                }
                
                Console.WriteLine("No card found or card cannot be read");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading smart card: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Don't return mock data in production - let the error bubble up
                throw new InvalidOperationException($"Smart card read failed: {ex.Message}", ex);
            }
        }

        private string BuildFullAddress(dynamic? addressInfo)
        {
            if (addressInfo == null) return string.Empty;
            
            try
            {
                var parts = new List<string>();
                
                if (!string.IsNullOrEmpty(addressInfo.HouseNumber?.ToString()))
                    parts.Add($"บ้านเลขที่ {addressInfo.HouseNumber}");
                    
                if (!string.IsNullOrEmpty(addressInfo.VillageNumber?.ToString()))
                    parts.Add($"หมู่ {addressInfo.VillageNumber}");
                    
                if (!string.IsNullOrEmpty(addressInfo.Lane?.ToString()))
                    parts.Add($"ตรอก/ซอย {addressInfo.Lane}");
                    
                if (!string.IsNullOrEmpty(addressInfo.Road?.ToString()))
                    parts.Add($"ถนน {addressInfo.Road}");
                    
                if (!string.IsNullOrEmpty(addressInfo.SubDistrict?.ToString()))
                    parts.Add($"ตำบล {addressInfo.SubDistrict}");
                    
                if (!string.IsNullOrEmpty(addressInfo.District?.ToString()))
                    parts.Add($"อำเภอ {addressInfo.District}");
                    
                if (!string.IsNullOrEmpty(addressInfo.Province?.ToString()))
                    parts.Add($"จังหวัด {addressInfo.Province}");
                    
                if (!string.IsNullOrEmpty(addressInfo.PostalCode?.ToString()))
                    parts.Add(addressInfo.PostalCode.ToString());
                
                return string.Join(" ", parts);
            }
            catch
            {
                return addressInfo.ToString() ?? string.Empty;
            }
        }

    }

    public class SmartCardData
    {
        public string CitizenId { get; set; } = string.Empty;
        public string TitleNameTh { get; set; } = string.Empty;
        public string FirstNameTh { get; set; } = string.Empty;
        public string LastNameTh { get; set; } = string.Empty;
        public string TitleNameEn { get; set; } = string.Empty;
        public string FirstNameEn { get; set; } = string.Empty;
        public string LastNameEn { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string IssueDate { get; set; } = string.Empty;
        public string ExpireDate { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string PhotoBase64 { get; set; } = string.Empty;
    }
}
