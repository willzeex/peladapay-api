using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeladaPay.Domain.Constants;
using PeladaPay.Domain.Entities;

namespace PeladaPay.Infrastructure.Data.Configurations;

public sealed class PlanFeatureConfiguration : IEntityTypeConfiguration<PlanFeature>
{
    public void Configure(EntityTypeBuilder<PlanFeature> entity)
    {
        entity.Property(x => x.Description).HasMaxLength(120).IsRequired();

        entity.HasOne(x => x.Plan)
            .WithMany(x => x.Features)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasData(
            new PlanFeature { Id = Guid.Parse("31111111-1111-1111-1111-111111111111"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.FreeId, Description = "Grupos ilimitados", DisplayOrder = 1 },
            new PlanFeature { Id = Guid.Parse("31111111-1111-1111-1111-111111111112"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.FreeId, Description = "Jogadores ilimitados", DisplayOrder = 2 },
            new PlanFeature { Id = Guid.Parse("31111111-1111-1111-1111-111111111113"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.FreeId, Description = "Cobranças automáticas via Pix", DisplayOrder = 3 },
            new PlanFeature { Id = Guid.Parse("31111111-1111-1111-1111-111111111114"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.FreeId, Description = "Extrato público para o grupo", DisplayOrder = 4 },
            new PlanFeature { Id = Guid.Parse("31111111-1111-1111-1111-111111111115"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.FreeId, Description = "Relatórios fiscais", DisplayOrder = 5 },
            new PlanFeature { Id = Guid.Parse("31111111-1111-1111-1111-111111111116"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.FreeId, Description = "Cartão virtual do grupo", DisplayOrder = 6 },
            new PlanFeature { Id = Guid.Parse("31111111-1111-1111-1111-111111111117"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.FreeId, Description = "Taxa de R$ 5,00 por Pix de saque", DisplayOrder = 7 },
            new PlanFeature { Id = Guid.Parse("32222222-2222-2222-2222-222222222221"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.ProId, Description = "Tudo do Plano Grátis", DisplayOrder = 1 },
            new PlanFeature { Id = Guid.Parse("32222222-2222-2222-2222-222222222222"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.ProId, Description = "0% de taxa por recebimento", DisplayOrder = 2 },
            new PlanFeature { Id = Guid.Parse("32222222-2222-2222-2222-222222222223"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.ProId, Description = "1 Pix de saque grátis por mês", DisplayOrder = 3 },
            new PlanFeature { Id = Guid.Parse("32222222-2222-2222-2222-222222222224"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.ProId, Description = "Saques adicionais: R$ 5,00 cada", DisplayOrder = 4 },
            new PlanFeature { Id = Guid.Parse("32222222-2222-2222-2222-222222222225"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.ProId, Description = "Cobranças automáticas via Pix", DisplayOrder = 5 },
            new PlanFeature { Id = Guid.Parse("32222222-2222-2222-2222-222222222226"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.ProId, Description = "Extrato público para o grupo", DisplayOrder = 6 },
            new PlanFeature { Id = Guid.Parse("32222222-2222-2222-2222-222222222227"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.ProId, Description = "Relatórios fiscais", DisplayOrder = 7 },
            new PlanFeature { Id = Guid.Parse("32222222-2222-2222-2222-222222222228"), CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), PlanId = Plans.ProId, Description = "Cartão virtual do grupo", DisplayOrder = 8 });
    }
}
