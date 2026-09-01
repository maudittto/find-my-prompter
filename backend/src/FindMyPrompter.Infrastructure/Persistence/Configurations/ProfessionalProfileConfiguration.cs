using FindMyPrompter.Domain.Professionals;
using FindMyPrompter.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FindMyPrompter.Infrastructure.Persistence.Configurations;

public sealed class ProfessionalProfileConfiguration : IEntityTypeConfiguration<ProfessionalProfile>
{
    public void Configure(EntityTypeBuilder<ProfessionalProfile> builder)
    {
        builder.HasKey(profile => profile.Id);

        builder.HasIndex(profile => profile.UserId).IsUnique();
        builder.HasIndex(profile => profile.Username).IsUnique();

        builder.Property(profile => profile.Username)
            .HasMaxLength(ProfessionalProfile.UsernameMaxLength)
            .IsRequired();

        builder.Property(profile => profile.DisplayName).HasMaxLength(80).IsRequired();
        builder.Property(profile => profile.Headline).HasMaxLength(160);
        builder.Property(profile => profile.About).HasMaxLength(4000);
        builder.Property(profile => profile.Location).HasMaxLength(120);

        // Enums como texto: legível no banco e estável se a ordem do enum mudar.
        builder.Property(profile => profile.Seniority).HasConversion<string>().HasMaxLength(20);
        builder.Property(profile => profile.WorkMode).HasConversion<string>().HasMaxLength(20);

        // ponytail: skills/modelos como text[] do Postgres. Trocar por taxonomia
        // própria se a busca do M6 exigir sinônimos ou validação de vocabulário.
        builder.Property(profile => profile.Skills).HasColumnType("text[]").IsRequired();
        builder.Property(profile => profile.AiModels).HasColumnType("text[]").IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<ProfessionalProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(profile => profile.Experiences, experience =>
        {
            experience.ToTable("ProfessionalExperiences");
            experience.WithOwner().HasForeignKey("ProfessionalProfileId");
            experience.HasKey(item => item.Id);

            experience.Property(item => item.Company).HasMaxLength(120).IsRequired();
            experience.Property(item => item.Position).HasMaxLength(120).IsRequired();
            experience.Property(item => item.Description).HasMaxLength(2000);
            experience.Property(item => item.Location).HasMaxLength(120);

            experience.Ignore(item => item.CurrentlyWorking);
        });

        builder.Navigation(profile => profile.Experiences)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
