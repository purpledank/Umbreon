﻿// <auto-generated />
using Espeon.Databases.GuildStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Collections.Generic;

namespace Espeon.Migrations.GuildStoreMigrations
{
    [DbContext(typeof(GuildStore))]
    [Migration("20190508165009_auto-quotes")]
    partial class autoquotes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "3.0.0-preview5.19227.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Espeon.Databases.CustomCommand", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("GuildId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("CustomCommand");
                });

            modelBuilder.Entity("Espeon.Databases.Guild", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Admins");

                    b.Property<bool>("AutoQuotes")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<decimal>("DefaultRoleId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<bool>("EmotesEnabled")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<string>("Moderators");

                    b.Property<decimal>("NoReactions")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<List<string>>("Prefixes");

                    b.Property<string>("RestrictedChannels");

                    b.Property<string>("RestrictedUsers");

                    b.Property<string>("SelfAssigningRoles");

                    b.Property<int>("StarLimit")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(3);

                    b.Property<decimal>("StarboardChannelId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<int>("WarningLimit")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(3);

                    b.Property<decimal>("WelcomeChannelId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("WelcomeMessage");

                    b.HasKey("Id");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("Espeon.Databases.StarredMessage", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal>("AuthorId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal>("ChannelId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Content");

                    b.Property<decimal>("GuildId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("ImageUrl");

                    b.Property<string>("ReactionUsers");

                    b.Property<decimal>("StarboardMessageId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("StarredMessage");
                });

            modelBuilder.Entity("Espeon.Databases.Warning", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("GuildId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<long>("IssuedOn")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1557334208982L);

                    b.Property<decimal>("Issuer")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Reason");

                    b.Property<decimal>("TargetUser")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("Warning");
                });

            modelBuilder.Entity("Espeon.Databases.CustomCommand", b =>
                {
                    b.HasOne("Espeon.Databases.Guild", "Guild")
                        .WithMany("Commands")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Espeon.Databases.StarredMessage", b =>
                {
                    b.HasOne("Espeon.Databases.Guild", "Guild")
                        .WithMany("StarredMessages")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Espeon.Databases.Warning", b =>
                {
                    b.HasOne("Espeon.Databases.Guild", "Guild")
                        .WithMany("Warnings")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
