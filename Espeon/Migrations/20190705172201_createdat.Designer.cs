﻿// <auto-generated />
using Espeon.Databases.UserStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Espeon.Migrations
{
    [DbContext(typeof(UserStore))]
    [Migration("20190705172201_createdat")]
    partial class createdat
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Espeon.Databases.Reminder", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("ChannelId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<long>("CreatedAt");

                    b.Property<decimal>("GuildId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal>("InvokeId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("JumpUrl");

                    b.Property<int>("ReminderId");

                    b.Property<string>("TheReminder");

                    b.Property<decimal>("UserId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<long>("WhenToRemove");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Reminders");
                });

            modelBuilder.Entity("Espeon.Databases.User", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<bool>("BoughtEmotes")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<int>("CandyAmount")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(10);

                    b.Property<int>("HighestCandies")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(10);

                    b.Property<long>("LastClaimedCandies");

                    b.Property<int>("ResponsePack")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<int[]>("ResponsePacks")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(new[] { 0 });

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Espeon.Databases.Reminder", b =>
                {
                    b.HasOne("Espeon.Databases.User")
                        .WithMany("Reminders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
