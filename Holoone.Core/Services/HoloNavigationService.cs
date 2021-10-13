using Caliburn.Micro;
using Holoone.Core.Services.Interfaces;
using System.Threading.Tasks;

namespace Holoone.Core.Services
{
    public class HoloNavigationService : Conductor<IScreen>, IHoloNavigationService
    {
        public async Task GoTo<T>() where T : IScreen
        {
            var viewModel = IoC.Get<T>();
            await ActivateItemAsync(viewModel);
        }
    }
}
