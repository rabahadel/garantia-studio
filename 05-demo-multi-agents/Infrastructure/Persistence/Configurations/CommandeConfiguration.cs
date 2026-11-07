using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModernApi.Domain;

namespace ModernApi.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration EF Core Code First explicite pour l'entité Commande.
/// </summary>
public sealed class CommandeConfiguration : IEntityTypeConfiguration<Commande>
{
    public void Configure(EntityTypeBuilder<Commande> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.ClientId);

        builder.Property(c => c.CodePromo)
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(c => c.CommentaireInterne)
            .HasMaxLength(500);

        builder.Property(c => c.MontantTotal)
            .HasPrecision(18, 2);

        builder.HasMany(c => c.Lignes)
            .WithOne()
            .HasForeignKey(l => l.CommandeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
