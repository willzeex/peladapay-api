using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Moq;
using PeladaPay.Application.Features.Users.Commands;
using PeladaPay.Application.Interfaces;
using PeladaPay.Domain.Entities;
using PeladaPay.Domain.Enums;
using PeladaPay.Domain.Exceptions;
using PeladaPay.Domain.Interfaces;
using Xunit;

namespace PeladaPay.Application.Tests.Features.Users.Commands;

public class UpdateUserOnboardingSettingsCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithJsonPayload_UpdatesDraftAndReturnsResponse()
    {
        const string payload = """
        {
            "onboardingGroupName": "Pelada de Terca",
            "onboardingFrequency": "Semanal",
            "onboardingVenue": "Arena Sao Cristovao"
        }
        """;

        var command = JsonSerializer.Deserialize<UpdateUserOnboardingSettingsCommand>(
            payload,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });

        Assert.NotNull(command);

        var user = new ApplicationUser
        {
            Id = "user-123"
        };

        var drafts = new Dictionary<string, OnboardingGroupDraft>
        {
            [user.Id] = new()
            {
                UserId = user.Id,
                Name = "Grupo antigo",
                Frequency = GroupFrequency.Mensal,
                Venue = "Quadra antiga"
            }
        };

        var userManagerMock = CreateUserManagerMock();
        userManagerMock
            .Setup(x => x.FindByIdAsync(user.Id))
            .ReturnsAsync(user);

        var handler = new UpdateUserOnboardingSettingsCommandHandler(
            userManagerMock.Object,
            new StubOnboardingGroupDraftRepository(drafts),
            new StubPlanRepository(),
            new StubCurrentUserService(user.Id),
            new StubUnitOfWork());

        var response = await handler.Handle(command!, CancellationToken.None);

        Assert.Equal("Pelada de Terca", response.OnboardingGroupName);
        Assert.Equal(GroupFrequency.Semanal, response.OnboardingFrequency);
        Assert.Equal("Arena Sao Cristovao", response.OnboardingVenue);
        Assert.Null(response.OnboardingCrestUrl);
        Assert.Null(response.PlanId);

        Assert.Equal("Pelada de Terca", drafts[user.Id].Name);
        Assert.Equal(GroupFrequency.Semanal, drafts[user.Id].Frequency);
        Assert.Equal("Arena Sao Cristovao", drafts[user.Id].Venue);

        userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPlanDoesNotExist_ThrowsNotFoundException()
    {
        var user = new ApplicationUser
        {
            Id = "user-456"
        };

        var userManagerMock = CreateUserManagerMock();
        userManagerMock
            .Setup(x => x.FindByIdAsync(user.Id))
            .ReturnsAsync(user);

        var command = new UpdateUserOnboardingSettingsCommand(
            "Pelada Premium",
            GroupFrequency.Quinzenal,
            "Arena Central",
            null,
            Guid.NewGuid());

        var handler = new UpdateUserOnboardingSettingsCommandHandler(
            userManagerMock.Object,
            new StubOnboardingGroupDraftRepository(),
            new StubPlanRepository(),
            new StubCurrentUserService(user.Id),
            new StubUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));

        userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestHasNoChanges_ReturnsCurrentStateWithoutUpdatingUser()
    {
        var user = new ApplicationUser
        {
            Id = "user-789"
        };

        var drafts = new Dictionary<string, OnboardingGroupDraft>
        {
            [user.Id] = new()
            {
                UserId = user.Id,
                Name = "Pelada da Quinta",
                Frequency = GroupFrequency.Mensal,
                Venue = "Arena Norte",
                CrestUrl = "crest.png"
            }
        };

        var userManagerMock = CreateUserManagerMock();
        userManagerMock
            .Setup(x => x.FindByIdAsync(user.Id))
            .ReturnsAsync(user);

        var handler = new UpdateUserOnboardingSettingsCommandHandler(
            userManagerMock.Object,
            new StubOnboardingGroupDraftRepository(drafts),
            new StubPlanRepository(),
            new StubCurrentUserService(user.Id),
            new StubUnitOfWork());

        var response = await handler.Handle(
            new UpdateUserOnboardingSettingsCommand(null, null, null, null, null),
            CancellationToken.None);

        Assert.Equal("Pelada da Quinta", response.OnboardingGroupName);
        Assert.Equal(GroupFrequency.Mensal, response.OnboardingFrequency);
        Assert.Equal("Arena Norte", response.OnboardingVenue);
        Assert.Equal("crest.png", response.OnboardingCrestUrl);

        userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();

        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    private sealed class StubCurrentUserService(string? userId) : ICurrentUserService
    {
        public string? UserId { get; } = userId;
    }

    private sealed class StubUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(1);
    }

    private sealed class StubPlanRepository(Dictionary<Guid, Plan>? plans = null) : IRepository<Plan>
    {
        private readonly Dictionary<Guid, Plan> _plans = plans ?? [];

        public Task<Plan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_plans.TryGetValue(id, out var plan) ? plan : null);

        public Task<IReadOnlyCollection<Plan>> GetAsync(System.Linq.Expressions.Expression<Func<Plan, bool>> predicate, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task AddAsync(Plan entity, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public void Update(Plan entity)
            => throw new NotSupportedException();
    }

    private sealed class StubOnboardingGroupDraftRepository(Dictionary<string, OnboardingGroupDraft>? drafts = null) : IRepository<OnboardingGroupDraft>
    {
        private readonly Dictionary<string, OnboardingGroupDraft> _drafts = drafts ?? [];

        public Task<OnboardingGroupDraft?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_drafts.Values.SingleOrDefault(x => x.Id == id));

        public Task<IReadOnlyCollection<OnboardingGroupDraft>> GetAsync(System.Linq.Expressions.Expression<Func<OnboardingGroupDraft, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var compiled = predicate.Compile();
            IReadOnlyCollection<OnboardingGroupDraft> result = _drafts.Values.Where(compiled).ToArray();
            return Task.FromResult(result);
        }

        public Task AddAsync(OnboardingGroupDraft entity, CancellationToken cancellationToken = default)
        {
            _drafts[entity.UserId] = entity;
            return Task.CompletedTask;
        }

        public void Update(OnboardingGroupDraft entity)
            => _drafts[entity.UserId] = entity;
    }
}
