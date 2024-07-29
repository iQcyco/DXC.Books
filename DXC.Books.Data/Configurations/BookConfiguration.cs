using DXC.Books.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace DXC.Books.Data.Configurations;

internal class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property<Guid>("Id")
            .ValueGeneratedOnAdd()
            .HasValueGenerator(typeof(SequentialGuidValueGenerator));

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(500)
            .IsUnicode();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<EnumToStringConverter<Status>>();

        builder.Property(x => x.Author)
            .IsUnicode()
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Isbn)
            .HasMaxLength(13)
            .IsRequired();
        
        builder.HasKey("Id");
        
        builder.HasIndex(x => x.Isbn)
            .IsUnique();
    }
}