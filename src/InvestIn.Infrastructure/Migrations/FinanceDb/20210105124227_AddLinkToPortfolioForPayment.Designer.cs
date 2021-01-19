﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using InvestIn.Infrastructure;

namespace InvestIn.Infrastructure.Migrations.FinanceDb
{
    [DbContext(typeof(FinanceDbContext))]
    [Migration("20210105124227_AddLinkToPortfolioForPayment")]
    partial class AddLinkToPortfolioForPayment
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InvestIn.Core.Entities.Finance.AssetAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("AssetActions");
                });

            modelBuilder.Entity("InvestIn.Core.Entities.Finance.AssetOperation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount");

                    b.Property<int>("AssetActionId");

                    b.Property<int>("AssetTypeId");

                    b.Property<DateTime>("Date");

                    b.Property<int>("PaymentPrice");

                    b.Property<int>("PortfolioId");

                    b.Property<string>("Ticket");

                    b.HasKey("Id");

                    b.HasIndex("AssetActionId");

                    b.HasIndex("AssetTypeId");

                    b.HasIndex("PortfolioId");

                    b.ToTable("AssetOperations");
                });

            modelBuilder.Entity("InvestIn.Core.Entities.Finance.AssetType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("AssetTypes");
                });

            modelBuilder.Entity("InvestIn.Core.Entities.Finance.CurrencyAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("CurrencyActions");
                });

            modelBuilder.Entity("InvestIn.Core.Entities.Finance.CurrencyOperation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CurrencyActionId");

                    b.Property<string>("CurrencyId");

                    b.Property<string>("CurrencyName");

                    b.Property<DateTime>("Date");

                    b.Property<int>("PortfolioId");

                    b.Property<int>("Price");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyActionId");

                    b.HasIndex("PortfolioId");

                    b.ToTable("CurrencyOperations");
                });

            modelBuilder.Entity("InvestIn.Core.Entities.Finance.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount");

                    b.Property<DateTime>("Date");

                    b.Property<int>("PaymentValue");

                    b.Property<int>("PortfolioId");

                    b.Property<string>("Ticket");

                    b.HasKey("Id");

                    b.HasIndex("PortfolioId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("InvestIn.Core.Entities.Finance.Portfolio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Portfolios");
                });

            modelBuilder.Entity("InvestIn.Core.Entities.Finance.AssetOperation", b =>
                {
                    b.HasOne("InvestIn.Core.Entities.Finance.AssetAction", "AssetAction")
                        .WithMany("AssetOperations")
                        .HasForeignKey("AssetActionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InvestIn.Core.Entities.Finance.AssetType", "AssetType")
                        .WithMany("AssetOperations")
                        .HasForeignKey("AssetTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InvestIn.Core.Entities.Finance.Portfolio", "Portfolio")
                        .WithMany("AssetOperations")
                        .HasForeignKey("PortfolioId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InvestIn.Core.Entities.Finance.CurrencyOperation", b =>
                {
                    b.HasOne("InvestIn.Core.Entities.Finance.CurrencyAction", "CurrencyAction")
                        .WithMany("CurrencyOperations")
                        .HasForeignKey("CurrencyActionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("InvestIn.Core.Entities.Finance.Portfolio", "Portfolio")
                        .WithMany()
                        .HasForeignKey("PortfolioId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("InvestIn.Core.Entities.Finance.Payment", b =>
                {
                    b.HasOne("InvestIn.Core.Entities.Finance.Portfolio", "Portfolio")
                        .WithMany("Payments")
                        .HasForeignKey("PortfolioId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
