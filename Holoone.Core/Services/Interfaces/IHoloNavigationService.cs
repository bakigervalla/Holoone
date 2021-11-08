using Caliburn.Micro;
using HolooneNavis.ViewModels;
using System.Threading.Tasks;

namespace HolooneNavis.Services.Interfaces
{
    public interface IHoloNavigationService
    {
        Task GoTo<T>() where T : IScreen;
    }
}
