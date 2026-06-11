using CommunityToolkit.Mvvm.Input;
using iteratie3matrix.Models;

namespace iteratie3matrix.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}