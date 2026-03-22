using MediatR;
using PeladaPay.Application.DTOs;
using PeladaPay.Domain.Interfaces;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Application.Features.Plans.Queries;

public sealed record GetPlansQuery : IRequest<IReadOnlyCollection<PlanDto>>;

public sealed class GetPlansQueryHandler(
    IRepository<Plan> planRepository,
    IRepository<PlanFeature> planFeatureRepository) : IRequestHandler<GetPlansQuery, IReadOnlyCollection<PlanDto>>
{
    public async Task<IReadOnlyCollection<PlanDto>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await planRepository.GetAsync(_ => true, cancellationToken);
        var orderedPlans = plans.OrderBy(x => x.DisplayOrder).ToList();

        var features = await planFeatureRepository.GetAsync(_ => true, cancellationToken);

        var featuresByPlan = features
            .GroupBy(x => x.PlanId)
            .ToDictionary(
                x => x.Key,
                x => (IReadOnlyCollection<string>)x.OrderBy(y => y.DisplayOrder).Select(y => y.Description).ToList());

        return orderedPlans
            .Select(plan => new PlanDto(
                plan.Id,
                plan.Name,
                plan.Slug,
                plan.MonthlyPrice,
                plan.PixFeePercentage,
                plan.FreeWithdrawalsPerMonth,
                plan.AdditionalWithdrawalFee,
                plan.IsPopular,
                featuresByPlan.TryGetValue(plan.Id, out var planFeatures) ? planFeatures : Array.Empty<string>()))
            .ToList();
    }
}
