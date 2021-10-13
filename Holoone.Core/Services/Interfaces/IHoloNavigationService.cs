using Caliburn.Micro;
using Holoone.Core.ViewModels;
using System.Threading.Tasks;

namespace Holoone.Core.Services.Interfaces
{
    public interface IHoloNavigationService
    {
        Task GoTo<T>() where T : IScreen;
    }
}
