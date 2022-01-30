using Flurl.Http;
using Holoone.Api.Models;
using Holoone.Api.Services.MicrosoftGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Services.Interfaces
{
    public interface ILoginService
    {
        Task<IFlurlResponse> LoginSphereAsync(LoginCredentials loginCredentials);

        Task<MicrosoftGraphResponse> LoginWithMicrosoftAsync(LoginCredentials loginCredentials);

        Task<IFlurlResponse> UpdateMicrosoftGraphTokenAsync(string graphToken, DateTimeOffset expiryOn);

        Task<string> LCPLoginGenerateAuthCode(string regionUrl, string deviceId);

        Task<LCPLogin> LCPLoginPolling(string regionUrl, string deviceId, string authCode);
    }
}
