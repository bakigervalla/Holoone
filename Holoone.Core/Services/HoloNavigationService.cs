using Caliburn.Micro;
using HolooneNavis.Services.Interfaces;
using System.Threading.Tasks;

namespace HolooneNavis.Services
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
