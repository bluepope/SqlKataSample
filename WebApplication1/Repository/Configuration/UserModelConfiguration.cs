using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Repository.Entities;

namespace WebApplication1.Repository.Configuration
{
    public class UserModelConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> entity)
        {
            entity.ToTable("user", (t) =>
            {
                t.HasComment("유저 테이블");
            });

            entity.HasKey(e => e.id);

            entity.Property(e => e.id).HasColumnName("id").IsRequired();
            entity.Property(e => e.name).HasColumnName("name");
        }
    }
}
