using Coyote.Console.ViewModels;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Coyote.Console.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
    public interface ICompetitionService
    {
        Task<CompetitionResponseViewModel> Insert(CompetitionRequestViewModel viewModel, int userId);
        Task<CompetitionResponseViewModel> GetCompetitionById(long id);
        Task<PagedOutputModel<List<CompetitionResponseViewModel>>> GetCompetitions(PagedInputModel inputModel);
        Task<bool> DeleteCompetition(long id, int userId);
        Task<bool> DeleteCompetitionTriggerItem(long id, long triggerId, int userId);
        Task<bool> DeleteCompetitionRewardItem(long id, long rewardId, int userId);
        Task<CompetitionResponseViewModel> Update(CompetitionRequestViewModel viewModel, long id, int userId, string imagePath = null);
        Task<PagedOutputModel<List<CompetitionResponseViewModel>>> GetActiveCompetitions(PagedInputModel inputModel, SecurityViewModel securityViewModel);
    }
}
