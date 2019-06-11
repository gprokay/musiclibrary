using GP.Tools.DriveService;
using Microsoft.IdentityModel.Tokens;
using MusicLibrary.DataAccess.Context;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibrary.Services
{
    public class TrackStreamService
    {
        private const string TrackIdClaimType = "TrackId";
        private const string RequestorUserIdClaimType = "UserId";
        private const string ClientAddressClaimType = "ClientAddress";

        private readonly MusicLibraryContext context;
        private readonly IDriveServiceFactory driveServiceFactory;
        private readonly string tokenSecret;

        public TrackStreamService(MusicLibraryContext context, IDriveServiceFactory driveServiceFactory, string tokenSecret)
        {
            this.context = context;
            this.driveServiceFactory = driveServiceFactory;
            this.tokenSecret = tokenSecret;
        }

        public async Task<Stream> GetTrackAsStream(string token, string clientAddress)
        {
            var principal = GetPrincipalFromToken(token);
            var trackId = int.Parse(principal.FindFirst(TrackIdClaimType).Value);
            var tokenClientAddress = principal.FindFirst(ClientAddressClaimType).Value;

            if (!string.Equals(tokenClientAddress, clientAddress))
            {
                throw new InvalidOperationException("Invalid client address");
            }

            var files = (await context.MusicFileRepository.GetByTrackId(trackId)).ToList();
            var azureFile = files.FirstOrDefault(f => f.DriveType == "azure");
            if (azureFile == null)
            {
                return null;
            }

            var driveService = driveServiceFactory.GetService(azureFile.DriveType, azureFile.Root);
            var file = new DriveFile(azureFile.FileId, azureFile.Name, azureFile.Root, azureFile.DriveType, azureFile.Length);
            return await driveService.GetFileContentStreamAsync(file);
        }

        public string GetStreamingToken(int trackId, IPrincipal principal, string clientAddress)
        {
            var userId = principal.GetUserId();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(tokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(RequestorUserIdClaimType, userId.ToString("D")),
                    new Claim(TrackIdClaimType, trackId.ToString("D")),
                    new Claim(ClientAddressClaimType, clientAddress)
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.Now.AddDays(1)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(tokenSecret);
            var validationParameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
    }
}
