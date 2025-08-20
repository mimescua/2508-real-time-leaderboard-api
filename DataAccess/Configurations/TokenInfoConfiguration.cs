using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeProjectName.Models;

namespace SafeProjectName.DataAccess.Configurations;

public class TokenInfoConfiguration : IEntityTypeConfiguration<TokenInfo>
{
	public void Configure(EntityTypeBuilder<TokenInfo> entity)
	{
		entity.HasKey(g => g.TokenId);

		entity.Property(g => g.Username)
			.IsRequired()
			.HasMaxLength(32);

		entity.Property(g => g.RefreshToken)
			.IsRequired()
			.HasMaxLength(200);

		entity.Property(g => g.ExpiredAt)
			.HasColumnType("datetime")
			.IsRequired();
	}
}
