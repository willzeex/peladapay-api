namespace PeladaPay.Application.DTOs;

public sealed record OnboardingStepResponseDto(Guid SessionId, int CurrentStep, int TotalSteps, string NextStep);
